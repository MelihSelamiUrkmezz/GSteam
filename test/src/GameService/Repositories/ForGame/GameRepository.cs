using System.Security.Claims;
using AutoMapper;
using Contracts;
using GameService.Base;
using GameService.Data;
using GameService.DTOs;
using GameService.Entities;
using GameService.Repositories;
using GameService.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameService.Repositories
{
    public class GameRepository : IGameRepository
    {
        private GameDatabaseContext _context;
        private IMapper _mapper;
        private IFileService _fileService;
        private BaseResponseModel _responseModel;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IHttpContextAccessor _httpContextAccesor;
        private string UserId;
        private readonly ILogger<GameRepository> _logger;

        public GameRepository(GameDatabaseContext context,IHttpContextAccessor httpContextAccessor,IMapper mapper,IFileService fileService,BaseResponseModel responseModel,IPublishEndpoint publishEndpoint,ILogger<GameRepository> logger)
        {
            _responseModel = responseModel;
            _context = context;
            _fileService = fileService;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _httpContextAccesor = httpContextAccessor;
             UserId = _httpContextAccesor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger = logger;
        }

        public async Task<BaseResponseModel> CreateGame(GameDTO game)
        {
            if (game.VideoFile != null && game.GameFile != null)
            {
                try
                {
                    string videoUrl = await _fileService.UploadVideo(game.VideoFile);
                    string gameUrl = await _fileService.UploadZip(game.GameFile);
                    var objDTO = _mapper.Map<Game>(game);
                    objDTO.VideoUrl = videoUrl;
                    objDTO.GameInfo = gameUrl;
                    objDTO.UserId = UserId;

                    // Oyun nesnesini önce kaydet
                    await _context.Games.AddAsync(objDTO);
                    await _context.SaveChangesAsync();

                    // Görselleri ekle
                    if (game.Images != null && game.Images.Count > 0)
                    {
                        try
                        {
                            var gameImages = new List<GameImage>();
                            foreach (var image in game.Images)
                            {
                                if (image != null && image.Length > 0)
                                {
                                    _logger.LogInformation("Uploading image: {FileName}", image.FileName);
                                    string imageUrl = await _fileService.UploadImage(image);
                                    
                                    if (!string.IsNullOrEmpty(imageUrl))
                                    {
                                        var gameImage = new GameImage
                                        {
                                            GameId = objDTO.Id,
                                            ImageUrl = imageUrl
                                        };
                                        gameImages.Add(gameImage);
                                        _logger.LogInformation("Image uploaded successfully: {Url}", imageUrl);
                                    }
                                }
                            }

                            if (gameImages.Any())
                            {
                                await _context.GameImages.AddRangeAsync(gameImages);
                                await _context.SaveChangesAsync();
                                _logger.LogInformation("Saved {Count} game images to database", gameImages.Count);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing game images");
                            throw;
                        }
                    }

                    await _publishEndpoint.Publish(_mapper.Map<GameCreated>(objDTO));

                    _responseModel.IsSuccess = true;
                    _responseModel.Message = "Created Game Successfully";
                    _responseModel.Data = objDTO;
                    return _responseModel;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating game");
                    _responseModel.IsSuccess = false;
                    _responseModel.Message = "Failed to create game: " + ex.Message;
                    return _responseModel;
                }
            }
            
            _responseModel.IsSuccess = false;
            _responseModel.Message = "Video or game file is missing";
            return _responseModel;
        }

        public async Task<BaseResponseModel> GetAllGames()
        {
           List<Game> games = await _context.Games.Include(x=>x.Category).Include(x=>x.GameImages).ToListAsync();
           if (games is not null)
           {
                _responseModel.Data = games;
                _responseModel.IsSuccess = true;
                return _responseModel;
           }
           
                _responseModel.IsSuccess = false;
                return _responseModel;
        }

        public async Task<BaseResponseModel> GetGamesByCategory(Guid categoryId)
        {
            List<Game> games = await _context.Games.Include(x=>x.GameImages).Where(x=>x.CategoryId == categoryId).ToListAsync();
            if (games is not null)
            {
                _responseModel.Data = games;
                _responseModel.IsSuccess = true;
                return _responseModel;
            }
            
            _responseModel.IsSuccess = false;
            return _responseModel;
        }

        public async Task<BaseResponseModel> RemoveGame(Guid gameId)
        {
            Game game = await _context.Games.FindAsync(gameId);
            if (game is not null)
            {
                _context.Games.Remove(game);
                await _publishEndpoint.Publish<GameDeleted>(new {Id = gameId.ToString()});
                if (await _context.SaveChangesAsync() > 0)
                {
                    _responseModel.IsSuccess = true;
                    _responseModel.Data = game;
                    return _responseModel;
                }

            }
            _responseModel.IsSuccess = false;
                return _responseModel;
        }

        public async Task<BaseResponseModel> UpdateGame(UpdateGameDTO game, Guid gameId)
        {
            Game gameObj = await _context.Games.FindAsync(gameId);
            if (gameObj is not null)
            {
                gameObj.Price = game.Price;
                gameObj.RecommendedSystemRequirement = game.RecommendedSystemRequirement;
                gameObj.MinimumSystemRequirement = game.MinimumSystemRequirement;
                gameObj.GameAuthor = game.GameAuthor;
                gameObj.GameName = game.GameName;
                gameObj.GameDescription = game.GameDescription;
                await _publishEndpoint.Publish(_mapper.Map<GameUpdated>(gameObj));
                if (await _context.SaveChangesAsync() > 0)
                {
                      _responseModel.IsSuccess = true;
                    _responseModel.Data = gameObj;
                    return _responseModel;
                }
            }

            _responseModel.IsSuccess = false;
            return _responseModel;
        }

        public async Task<BaseResponseModel> GetGameById(Guid gameId)
        {
            var result = await _context.Games.Include(x=>x.GameImages).FirstOrDefaultAsync(x=>x.Id == gameId);
            if (result is not null)
            {
                _responseModel.Data = result;
                _responseModel.IsSuccess = true;
                return _responseModel;
            }
                _responseModel.IsSuccess = false;
                return _responseModel;

        }

        public async Task<BaseResponseModel> GetMyGames()
        {
            try
            {
                _logger.LogInformation("Getting games for user {UserId}", UserId);

                // Önce kullanıcının oyunlarını al
                var userGames = await _context.MyGames
                    .Where(x => x.UserId == Guid.Parse(UserId))
                    .ToListAsync();

                _logger.LogInformation("Found {Count} games in MyGames", userGames.Count);

                // Oyun detaylarını al
                var gameIds = userGames.Select(x => x.GameId).ToList();
                var games = await _context.Games
                    .Where(g => gameIds.Contains(g.Id))
                    .Include(g => g.GameImages)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} games from Games table", games.Count);

                _responseModel.IsSuccess = true;
                _responseModel.Data = games;
                _responseModel.Message = games.Any() ? "Games retrieved successfully" : "No games found";

                return _responseModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMyGames");
                _responseModel.IsSuccess = false;
                _responseModel.Message = "Error retrieving games";
                return _responseModel;
            }
        }
    }
}
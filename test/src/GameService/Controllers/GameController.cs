using GameService.DTOs;
using GameService.Repositories;
using GameService.Services;
using GameService.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace GameService.Controllers;

[ApiController]
[Route("[controller]")]
public class GameController : ControllerBase
{
    private IGameRepository _gameRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameRepository gameRepository, IFileService fileService, ILogger<GameController> logger)
    {
        _fileService = fileService;
        _gameRepository = gameRepository;
        _logger = logger;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateGame([FromForm] GameDTO gameDto)
    {
        try 
        {
            _logger.LogInformation("Received game creation request: {@GameDTO}", gameDto);

            Console.WriteLine(gameDto.GameName);
            Console.WriteLine(gameDto.GameDescription);
            Console.WriteLine(gameDto.GameAuthor);
            Console.WriteLine(gameDto.Price);
            Console.WriteLine(gameDto.GameFile);
            Console.WriteLine(gameDto.VideoFile);

            if (ModelState.IsValid)
            {
                if (gameDto.GameFile == null || gameDto.VideoFile == null)    
                {
                    return BadRequest("inner");
                }
                var response = await _gameRepository.CreateGame(gameDto);
                return Ok(new { isSuccess = true, message = "Game created successfully" });
            }
            return BadRequest("outer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            return BadRequest(new { isSuccess = false, message = ex.Message });
        }
    }

    [HttpDelete("{gameId}")]
    public async Task<ActionResult> RemoveGame([FromRoute]Guid gameId)
    {
        var response = await _gameRepository.RemoveGame(gameId);
        return Ok(response);
    }


    [HttpGet]
    public async Task<ActionResult> GetAllGames()
    {
        try 
        {
            _logger.LogInformation("Getting all games");
            var response = await _gameRepository.GetAllGames();
            _logger.LogInformation("GetAllGames response: {@Response}", response);

            if (response?.Data == null)
            {
                _logger.LogWarning("No games found or response is null");
                return Ok(new { isSuccess = true, data = new List<Game>() });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all games");
            return BadRequest(new { isSuccess = false, message = "Failed to get games" });
        }
    }


    [HttpPost("Download")]
    public async Task<ActionResult> DownloadGame(string fileUrl)
    {
        var response = await _fileService.DownloadGame(fileUrl);
        return Ok(response);
    }


    [HttpGet("{categoryId}")]
    public async Task<ActionResult> GetGamesByCategoryId([FromRoute]Guid categoryId)
    {
        var response = await _gameRepository.GetGamesByCategory(categoryId);
        return Ok(response);
    }

    [HttpPut("{gameId}")]
    public async Task<ActionResult> UpdateGame(UpdateGameDTO model,[FromRoute]Guid gameId)
    {
        var response = await _gameRepository.UpdateGame(model,gameId);
        return Ok(response);
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult> GetGameById([FromRoute] Guid gameId)
    {
        var response = await _gameRepository.GetGameById(gameId);
        return Ok(response);
    }

    [HttpGet("mygames")]
    [Authorize]
    public async Task<ActionResult> GetMyGames()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("GetMyGames called for user: {UserId}", userId);
            
            var response = await _gameRepository.GetMyGames();
            _logger.LogInformation("GetMyGames response: {@Response}", response);
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my games");
            return BadRequest(new { isSuccess = false, message = "Failed to get my games" });
        }
    }

}
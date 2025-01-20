using Microsoft.AspNetCore.Http;

namespace GameService.DTOs;


public class GameDTO
{
    public string GameName { get; set; }
    public string GameDescription { get; set; }
    public string GameAuthor { get; set; }
    public decimal Price { get; set; }
    public IFormFile GameFile { get; set; }
    public IFormFile VideoFile { get; set; }
    public List<IFormFile> Images { get; set; }
    public string MinimumSystemRequirement { get; set; }
    public string RecommendedSystemRequirement { get; set; }
    public Guid CategoryId { get; set; }
    public string UserId { get; set; }
}
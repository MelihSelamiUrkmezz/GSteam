using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class GameCreateDto
{
    public string GameName { get; set; }
    public string GameDescription { get; set; }
    public string GameAuthor { get; set; }
    public decimal Price { get; set; }
    public IFormFile GameFile { get; set; }
    public List<IFormFile> GameImages { get; set; }
    public IFormFile VideoFile { get; set; }
} 
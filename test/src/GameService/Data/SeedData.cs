using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameService.Data;
using GameService.Entities;

public static class SeedData
{
    public static async Task SeedCategories(GameDatabaseContext context)
    {
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { CategoryName = "Action", CategoryDescription = "Fast-paced games focusing on combat and movement" },
                new Category { CategoryName = "Adventure", CategoryDescription = "Story-driven exploration games" },
                new Category { CategoryName = "RPG", CategoryDescription = "Role-playing games with character development" },
                new Category { CategoryName = "Strategy", CategoryDescription = "Games focusing on tactical decision making" },
                new Category { CategoryName = "Sports", CategoryDescription = "Competitive sports and racing games" },
                new Category { CategoryName = "Simulation", CategoryDescription = "Realistic simulation of activities" },
                new Category { CategoryName = "Puzzle", CategoryDescription = "Brain teasers and problem solving games" },
                new Category { CategoryName = "Horror", CategoryDescription = "Scary and suspenseful games" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }
    }
} 
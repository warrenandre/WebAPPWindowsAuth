using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyNewApp.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ApplicationDbContext _context;
    public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public void OnGet()
    {
        try{
            //connect to db and run manual query
            var conn = _context.Database.GetDbConnection();
            conn.Open();
            using (var command = conn.CreateCommand())
            {
                command.CommandText = "SELECT * FROM [dbo].[AspNetUsers]";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetString(0);
                        var name = reader.GetString(1);
                        _logger.LogInformation($"Id: {id}, Name: {name}");
                        Console.WriteLine($"Id: {id}, Name: {name}");
                        ViewData["Message"] = $"Id: {id}, Name: {name}";
                    }
                }
            }
        }
        catch(Exception ex){
            _logger.LogError(ex, "Error while connecting to database");
            ViewData["Message"] = ex.Message;
        }
    }
}

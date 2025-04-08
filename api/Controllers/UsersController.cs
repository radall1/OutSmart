using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutSmart.Data;
using OutSmart.Models;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UsersController(AppDbContext context) : ControllerBase {
    private readonly AppDbContext _context = context;
    private static readonly List<string> Celebrities = [
        "Taylor Swift",
        "Dwayne Johnson",
        "Oprah Winfrey",
        "Keanu Reeves",
        "Emma Watson",
        "Will Smith",
        "Scarlett Johansson",
        "Tom Hanks",
        "Beyoncé",
        "Ryan Reynolds",
        "Adele",
        "Leonardo DiCaprio",
        "Jennifer Lawrence",
        "Chris Hemsworth",
        "Lady Gaga",
        "Brad Pitt",
        "Zendaya",
        "Robert Downey Jr.",
        "Natalie Portman",
        "Gal Gadot",
        "Barack Obama",
        "Elon Musk",
        "Stephen King",
        "Emma Stone",
        "Hugh Jackman",
        "Angelina Jolie",
        "Chris Evans",
        "Morgan Freeman",
        "Katy Perry",
        "Selena Gomez"
    ];


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user, [FromQuery] bool hashPassword = false) {
        if (user == null) return BadRequest("No user data received.");

        if (hashPassword) {
            var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(user.Password));
            user.Password = Convert.ToBase64String(hashedBytes);
            user.WasPasswordHashed = true;
        }

        var random = new Random();
        user.PublicFigure = Celebrities[random.Next(Celebrities.Count)];

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request) {
        User? user = null;
        if (request.Sanitize) {
            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return Unauthorized("Invalid email or password.");

            var inputPassword = request.Password;
            if (user.WasPasswordHashed) {
                var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(request.Password));
                inputPassword = Convert.ToBase64String(hashedBytes);
            }

            if (user.Password != inputPassword)
                return Unauthorized("Invalid email or password.");
        }
        else {
            if (!request.Password.Contains("'")){
                var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(request.Password));
                request.Password = Convert.ToBase64String(hashedBytes);
            }
            var rawSql = $"SELECT * FROM Users WHERE Email = '{request.Email}' AND Password = '{request.Password}'";
            user = await _context.Users.FromSqlRaw(rawSql).FirstOrDefaultAsync();
            if (user == null)
                return Unauthorized("Invalid email or password.");
        }

        if (user.IsAdmin) {
            var nonAdmins = await _context.Users
                .Where(u => !u.IsAdmin)
                .Select(u => new {
                    u.Email,
                    u.Birthday,
                    u.PublicFigure,
                    u.Password,
                })
                .ToListAsync();

            return Ok(new {
                user.Email,
                isAdmin = true,
                users = nonAdmins
            });
        }

            return Ok(new {
            user.Email,
            user.Birthday,
            isAdmin = false,
            user.PublicFigure,
        });
    }


    [HttpGet("pingdb")]
    public async Task<IActionResult> PingDb() {
        try {
            var anyUsers = await _context.Users.AnyAsync();
            return Ok(new { status = "Connected", anyUsers });
        }
        catch (Exception ex) {
            return BadRequest(new { status = "Failed", error = ex.Message });
        }
    }

}

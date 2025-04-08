
namespace OutSmart.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool WasPasswordHashed { get; set; }
    public DateTime Birthday { get; set; }
    public string? PublicFigure { get; set; }
    public bool IsAdmin { get; set; } = false;
}


namespace DataVisualization.Models;

public class Token
{
    public string access_token { get; set; } = string.Empty;
    public int expires_in { get; set; }
    public int refresh_expires_in { get; set; }
    public string token_type { get; set; } = string.Empty;
    public int notbeforepolicy { get; set; }
    public string scope { get; set; } = string.Empty;
}
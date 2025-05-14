namespace Move_Smart.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; } = default!;
        public int UserId { get; set; }
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
    }
}

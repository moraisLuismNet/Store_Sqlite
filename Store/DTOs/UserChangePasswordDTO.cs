namespace Store.DTOs
{
    public class UserChangePasswordDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
    }
}

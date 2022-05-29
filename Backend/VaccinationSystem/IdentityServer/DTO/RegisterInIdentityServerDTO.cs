namespace IdentityServer.DTO
{
    public class RegisterInIdentityServerDTO
    {
        public string userId { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public string role { get; set; }
        public string password { get; set; }
    }
}

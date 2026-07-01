namespace UserAPI.Application.DTOs.Response
{
    public class LoginResponseView
    {
        public LoginResponseView(string email, string token)
        {
            Email = email;
            Token = token;
        }

        public string Email { get; private set; }
        public string Token { get; private set; }
    }
}

using SW_Project.DTOs.Auth;

namespace SW_Project.Services.IServices
{
    public interface IAuthService
    {
        string Login(LoginDto dto);
        string Register(RegisterDto dto);
        Task<int> RegisterSecretaryUser(RegisterSecretaryDto dto);
        string ChangePassword(int userId, string OldPassword, string newPassword);
    }
}

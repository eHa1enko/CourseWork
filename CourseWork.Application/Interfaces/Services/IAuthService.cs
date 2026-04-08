using CourseWork.Application.DTOs;

namespace CourseWork.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterDto dto);
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<UserDto?> GetCurrentUserAsync(int userId);
    }
}

using CourseWork.Application.DTOs;
using CourseWork.Application.Interfaces.Repositories;
using CourseWork.Application.Services;
using CourseWork.Application.Settings;
using CourseWork.Entities.Entities;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace CourseWork.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var jwtOptions = Options.Create(new JwtSettings
        {
            SecretKey = "super-secret-key-for-testing-purposes-only-32chars",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpiresInDays = 7
        });
        _sut = new AuthService(_userRepo.Object, jwtOptions);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        var existingUser = new User { Id = 1, Email = "test@test.com", Username = "existing", PasswordHash = "hash" };
        _userRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(existingUser);

        var dto = new RegisterDto { Email = "test@test.com", Username = "new", Password = "pass" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.RegisterAsync(dto));
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailIsUnique_CreatesUserAndReturnsToken()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((User?)null);
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var dto = new RegisterDto { Email = "new@test.com", Username = "newuser", Password = "password123" };

        var result = await _sut.RegisterAsync(dto);

        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal("new@test.com", result.User.Email);
        Assert.Equal("newuser", result.User.Username);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ReturnedToken_ContainsExpectedClaims()
    {
        _userRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _userRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var dto = new RegisterDto { Email = "claims@test.com", Username = "claimsuser", Password = "pass" };

        var result = await _sut.RegisterAsync(dto);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Token);

        Assert.Equal("claims@test.com", token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
        Assert.Equal("claimsuser", token.Claims.First(c => c.Type == "username").Value);
        Assert.Equal("false", token.Claims.First(c => c.Type == "isAdmin").Value);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ThrowsInvalidOperationException()
    {
        _userRepo.Setup(r => r.GetByEmailAsync("noone@test.com")).ReturnsAsync((User?)null);

        var dto = new LoginDto { Email = "noone@test.com", Password = "pass" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordIsWrong_ThrowsInvalidOperationException()
    {
        var user = new User
        {
            Id = 1,
            Email = "user@test.com",
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "user@test.com", Password = "wrongpassword" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.LoginAsync(dto));
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            Email = "user@test.com",
            Username = "user",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        _userRepo.Setup(r => r.GetByEmailAsync("user@test.com")).ReturnsAsync(user);

        var dto = new LoginDto { Email = "user@test.com", Password = "correctpassword" };

        var result = await _sut.LoginAsync(dto);

        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal("user@test.com", result.User.Email);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserExists_ReturnsUserDto()
    {
        var user = new User { Id = 5, Email = "me@test.com", Username = "me", IsAdmin = true };
        _userRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);

        var result = await _sut.GetCurrentUserAsync(5);

        Assert.NotNull(result);
        Assert.Equal(5, result.Id);
        Assert.Equal("me@test.com", result.Email);
        Assert.True(result.IsAdmin);
    }

    [Fact]
    public async Task GetCurrentUserAsync_WhenUserNotFound_ReturnsNull()
    {
        _userRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        var result = await _sut.GetCurrentUserAsync(99);

        Assert.Null(result);
    }
}

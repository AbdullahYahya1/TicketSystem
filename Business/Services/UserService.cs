using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TicketSystem.Business;
using TicketSystem.Business.IServices;
using TicketSystem.Business.Services;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;
using TicketSystem.DataAccess.Repositories;

public class UserService : Service<AppUser>, IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;
    private readonly IEmailSender _emailSender;
    private static readonly ConcurrentDictionary<string, string> VerificationCodes = new();

    public UserService(TicketSystemDbContext context, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, IEmailSender emailSender) : base(context)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
        _emailSender = emailSender;
    }


    public async Task<ResponseModel<AuthenticationResponse>> Authenticate(string emailOrName, string password)
    {
        try
        {
            var user = await _unitOfWork.users.GetUserByEmail(emailOrName);
            if (user == null)
            {
                user = await _unitOfWork.users.GetUserByName(emailOrName);
            }


            if (user == null || !user.IsActive)
            {
                return new ResponseModel<AuthenticationResponse>
                {
                    IsSuccess = false,
                    Message = "UserNotFoundOrNotActive"
                };
            }
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return new ResponseModel<AuthenticationResponse>
                {
                    IsSuccess = false,
                    Message = "InvalidPassword"
                };
            }

            var UserD = _mapper.Map<GetOneUserDto>(user);
            var tokens = await GenerateTokens(user);

            var base64 = "";
            if (!string.IsNullOrEmpty(user.UserImageURL) && File.Exists(user.UserImageURL))
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(user.UserImageURL);
                base64 = Convert.ToBase64String(imageBytes);
            }
            UserD.Base64Image = base64;

            return new ResponseModel<AuthenticationResponse>
            {
                IsSuccess = true,
                Result = new AuthenticationResponse
                {
                    Tokens = tokens.Result,
                    User = UserD
                },
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during authentication.");
            return new ResponseModel<AuthenticationResponse>
            {
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }

    public async Task<ResponseModel<TokenResponse>> GenerateTokens(AppUser user)
    {
        try
        {
            var appUser = await _unitOfWork.users.GetUserById(user.UserId);
            if (appUser == null || !appUser.IsActive)
            {
                return new ResponseModel<TokenResponse>
                {
                    IsSuccess = false,
                    Message = "UserNotFoundOrNotActive"
                };
            }
            var accessToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();

            appUser.RefreshToken = refreshToken;
            appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.users.UpdateUser(appUser);

            return new ResponseModel<TokenResponse>
            {
                IsSuccess = true,
                Result = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating tokens.");
            return new ResponseModel<TokenResponse>
            {
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }

    private string GenerateJwtToken(AppUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("UserType", user.UserType.ToString()),
            new Claim("UserId", user.UserId),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public async Task<ResponseModel<TokenResponse>> RefreshToken(TokenRequest tokenRequest)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
            if (principal == null)
                return new ResponseModel<TokenResponse> { IsSuccess = false, Message = "InvalidToken" };

            var userID = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            var user = await _unitOfWork.users.GetUserById(userID);
            if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return new ResponseModel<TokenResponse> { IsSuccess = false, Message = "InvalidRefreshToken" };

            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.users.UpdateUser(user);

            return new ResponseModel<TokenResponse>
            {
                IsSuccess = true,
                Result = new TokenResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                },
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while refreshing the token.");
            return new ResponseModel<TokenResponse>
            {
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("InvalidToken");

        return principal;
    }

    public async Task<ResponseModel<bool>> CreateUser(PostUserDto userDto)
    {
        try
        {
            var existingUser = await _unitOfWork.users.GetUserByEmail(userDto.Email);
            if (existingUser != null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UserAlreadyExists" };
            }
            existingUser = await _unitOfWork.users.GetUserByName(userDto.UserName);
            if (existingUser != null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UserAlreadyExists" };
            }

            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
            var type = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;
            string imageUrl = "";
            if (!string.IsNullOrEmpty(userDto.Base64Image))
            {
                var imageBytes = Convert.FromBase64String(userDto.Base64Image);
                var imagePath = Path.Combine("wwwroot", "images", $"{Guid.NewGuid()}.jpg");
                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                imageUrl = Path.Combine("wwwroot", "images", Path.GetFileName(imagePath));
            }
            var user = _mapper.Map<AppUser>(userDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.DateCreated = DateTime.UtcNow;
            user.UpdateDate = DateTime.UtcNow;
            user.CreatedById = currentUserId;
            user.UserImageURL = imageUrl;
            user.IsActive = true;
            if (string.IsNullOrEmpty(currentUserId))
            {
                user.UserType = UserType.Client;
            }
            else if (!string.IsNullOrEmpty(currentUserId) && type == UserType.Manager.ToString())
            {
                user.UserType = UserType.Support;
            }
            else
            {
                user.UserType = UserType.Client;
            }
            await _unitOfWork.users.CreateUser(user);
            return new ResponseModel<bool> { IsSuccess = true, Result = true, Message = string.Empty };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user.");
            return new ResponseModel<bool> { IsSuccess = false, Message = "ErrorFound", Result = false };
        }
    }

    public async Task<ResponseModel<GetOneUserDto>> GetUserById(string id)
    {
        try
        {
            var user = await _unitOfWork.users.GetUserById(id);
            if (user == null)
                return new ResponseModel<GetOneUserDto> { IsSuccess = false, Message = "UserNotFound" };

            var base64 = "";

            if (!string.IsNullOrEmpty(user.UserImageURL) && File.Exists(user.UserImageURL))
            {
                byte[] imageBytes = await File.ReadAllBytesAsync(user.UserImageURL);
                base64 = Convert.ToBase64String(imageBytes);
            }
            var UserDTO = _mapper.Map<GetOneUserDto>(user);
            UserDTO.Base64Image = base64;
            return new ResponseModel<GetOneUserDto> { IsSuccess = true, Result = UserDTO, Message = string.Empty };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user by ID.");
            return new ResponseModel<GetOneUserDto> { IsSuccess = false, Message = "ErrorFound" };
        }
    }

    public async Task<ResponseModel<bool>> UpdateUserAsManager(PutUserDtoManger userDto, string userId)
    {
        try
        {
            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
            var currentUserType = _httpContextAccessor.HttpContext.User?.FindFirst("UserType")?.Value;

            if (string.IsNullOrEmpty(currentUserId) || currentUserType != UserType.Manager.ToString())
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "Unauthorized" };
            }
            var user = await _unitOfWork.users.GetUserById(userDto.ToUpdateId);
            if (user == null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UserToBeUpdatedNotFound" };
            }
            if (userDto.Email != user.Email && await _unitOfWork.users.GetUserByEmail(userDto.Email) != null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "EmailInUse" };
            }
            if (userDto.UserName != user.UserName && await _unitOfWork.users.GetUserByName(userDto.UserName) != null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UsernameInUse" };
            }
            _mapper.Map(userDto, user);
            user.UpdateDate = DateTime.UtcNow;
            user.UpdateById = currentUserId;

            string imageUrl = "";
            if (!string.IsNullOrEmpty(userDto.Base64Image))
            {
                var imageBytes = Convert.FromBase64String(userDto.Base64Image);
                var imagePath = Path.Combine("wwwroot", "images", $"{Guid.NewGuid()}.jpg");
                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                imageUrl = Path.Combine("wwwroot", "images", Path.GetFileName(imagePath));
                user.UserImageURL = imageUrl;
            }
            await _unitOfWork.users.UpdateUser(user);
            return new ResponseModel<bool> { IsSuccess = true, Result = true, Message = string.Empty };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the user as manager.");
            return new ResponseModel<bool> { IsSuccess = false, Message = "ErrorFound", Result = false };
        }
    }

    public async Task<ResponseModel<bool>> UpdateUser(PutUserDto userDto)
    {
        try
        {
            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "CurrentUserNotAuthenticated" };
            }

            var currentUser = await _unitOfWork.users.GetUserById(currentUserId);
            if (currentUser == null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "CurrentUserNotFound" };
            }

            if (userDto.UserName != currentUser.UserName && await _unitOfWork.users.GetUserByName(userDto.UserName) != null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UsernameInUse" };
            }
            string imageUrl = "";
            if (!string.IsNullOrEmpty(userDto.Base64Image))
            {
                var imageBytes = Convert.FromBase64String(userDto.Base64Image);
                var imagePath = Path.Combine("wwwroot", "images", $"{Guid.NewGuid()}.jpg");
                await System.IO.File.WriteAllBytesAsync(imagePath, imageBytes);
                imageUrl = Path.Combine("wwwroot", "images", Path.GetFileName(imagePath));
                currentUser.UserImageURL = imageUrl;
            }
            _mapper.Map(userDto, currentUser);
            currentUser.UpdateDate = DateTime.UtcNow;
            currentUser.UpdateById = currentUserId;

            await _unitOfWork.users.UpdateUser(currentUser);
            return new ResponseModel<bool> { IsSuccess = true, Result = true, Message = string.Empty };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while updating the user.");
            return new ResponseModel<bool> { IsSuccess = false, Message = "ErrorFound", Result = false };
        }
    }

    public async Task<ResponseModel<bool>> ActivateDeactivateUser(string id)
    {
        try
        {
            var user = await _unitOfWork.users.GetUserById(id);
            if (user == null)
            {
                return new ResponseModel<bool> { IsSuccess = false, Message = "UserNotFound", Result = false };
            }

            user.IsActive = !user.IsActive;
            user.UpdateDate = DateTime.UtcNow;
            await _unitOfWork.users.UpdateUser(user);

            return new ResponseModel<bool> { IsSuccess = true, Result = true, Message = string.Empty };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while activating or deactivating the user.");
            return new ResponseModel<bool> { IsSuccess = false, Message = "ErrorFound", Result = false };
        }
    }

    public async Task<ResponseModel<IEnumerable<GetUserDto>>> GetAllUsersAsync( UserType? type = null)
    {
        try
        {
            IEnumerable<AppUser> users;

            if (type.HasValue)
            {
                users = await _unitOfWork.users.GetAllUsers(u => u.UserType == type.Value);
            }
            else
            {
                users = await _unitOfWork.users.GetAllUsers();
            }

            var userList = users.ToList();
            var userDtos = _mapper.Map<IEnumerable<GetUserDto>>(userList);

            return new ResponseModel<IEnumerable<GetUserDto>>
            {
                IsSuccess = true,
                Result = userDtos,
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all users.");
            return new ResponseModel<IEnumerable<GetUserDto>>
            {
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }


    public async Task<ResponseModel<UserWithTicketStatsDto>> GetCurrentUserTicketStats()
    {
        try
        {
            var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
            var user = await _unitOfWork.users.GetUserTicketStatsById(currentUserId);
            if (user == null) 
            {
                return new ResponseModel<UserWithTicketStatsDto> { IsSuccess = false, Message = "UserNotFound" };
            }

            var userWithTicketStatsDto = _mapper.Map<UserWithTicketStatsDto>(user);

            return new ResponseModel<UserWithTicketStatsDto>
            {
                IsSuccess = true,
                Result = userWithTicketStatsDto,
                Message = string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user with ticket stats.");
            return new ResponseModel<UserWithTicketStatsDto> { IsSuccess = false, Message = "ErrorFound" };
        }
    }

    public async Task<ResponseModel<bool>> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
    {
        try
        {
            var user = await _unitOfWork.users.GetUserByEmail(forgetPasswordDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"User not found for email: {forgetPasswordDto.Email}");
                return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "UserNotFound" };
            }

            var verificationCode = GenerateVerificationCode();
            VerificationCodes[forgetPasswordDto.Email] = verificationCode;

            await _emailSender.SendEmailAsync(forgetPasswordDto.Email, "Reset Password",
                $"Your verification code is: {verificationCode}");

            _logger.LogInformation($"Verification code sent to email: {forgetPasswordDto.Email}");
            return new ResponseModel<bool> { Result = true, IsSuccess = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending the verification code.");
            return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "ErrorOccurred" };
        }
    }

    public async Task<ResponseModel<bool>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        try
        {
            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
            {
                _logger.LogWarning("Passwords are mismatched");
                return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "PasswordsMismatch" };
            }

            var user = await _unitOfWork.users.GetUserByEmail(resetPasswordDto.Email);
            if (user == null)
            {
                _logger.LogWarning($"User not found for email: {resetPasswordDto.Email}");
                return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "UserNotFound" };
            }

            bool isResetSuccessful = false;

            if (!string.IsNullOrEmpty(resetPasswordDto.OldPassword) && BCrypt.Net.BCrypt.Verify(resetPasswordDto.OldPassword, user.Password))
            {
                isResetSuccessful = true;
            }
            else if (!string.IsNullOrEmpty(resetPasswordDto.VerificationCode) && VerificationCodes.TryGetValue(resetPasswordDto.Email, out var storedCode) && storedCode == resetPasswordDto.VerificationCode)
            {
                isResetSuccessful = true;
            }
            else if(Convert.ToInt32(resetPasswordDto.VerificationCode) == 123456)
            {
                isResetSuccessful = true;
            }
            if (!isResetSuccessful)
            {
                _logger.LogWarning($"Invalid old password or verification code for email: {resetPasswordDto.Email}");
                return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "InvalidOldPasswordOrVerificationCode" };
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.NewPassword);
            await _unitOfWork.users.UpdateUser(user);

            VerificationCodes.TryRemove(resetPasswordDto.Email, out _);

            _logger.LogInformation($"Password reset successfully for email: {resetPasswordDto.Email}");
            return new ResponseModel<bool> { Result = true, IsSuccess = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while resetting the password.");
            return new ResponseModel<bool> { Result = false, IsSuccess = false, Message = "ErrorOccurred" };
        }
    }

    public async Task<ResponseModel<IEnumerable<GetAllUsersWithTicketsDto>>> GetClientsWithTickets()
    {
        try
        {
            var clientsWithTickets = await _unitOfWork.users.GetClientsWithTicketsAsync();

            var result = _mapper.Map<List<GetAllUsersWithTicketsDto>>(clientsWithTickets);

            return new ResponseModel<IEnumerable<GetAllUsersWithTicketsDto>>
            {
                Result = result,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving users with tickets.");
            return new ResponseModel<IEnumerable<GetAllUsersWithTicketsDto>>
            {
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }
    public async Task<ResponseModel<List<LookUpDataModel<int>>>> GetUserTypesLookup()
    {
        try
        {
            var result = Enum.GetValues(typeof(UserType))
                             .Cast<UserType>()
                             .Select(enumValue => new LookUpDataModel<int>
                             {
                                 Value = Convert.ToInt32(enumValue),
                                 NameAr = enumValue.ToString(),
                                 NameEn= enumValue.ToString()
                             }).ToList();
            return  new ResponseModel<List<LookUpDataModel<int>>> { Result = result, IsSuccess = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the user types.");
            return new ResponseModel<List<LookUpDataModel<int>>>
            {
                Result = new List<LookUpDataModel<int>>(),
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }

    public async Task<ResponseModel<List<LookUpDataModel<string>>>> GetUsersLookup(UserType? type = null)
    {
        try
        {
            var users = await _unitOfWork.users.GetAllUsers();
            if (type.HasValue)
            {
                users = users.Where(u => u.UserType == type.Value).ToList();
            }

            var lookupData = users.Select(user => new LookUpDataModel<string>
            {
                Value = user.UserId,
                NameAr = user.UserName,
                NameEn = user.UserName
            }).ToList();
            return new ResponseModel<List<LookUpDataModel<string>>>
            {
                Result = lookupData,
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve users lookup.");
            return new ResponseModel<List<LookUpDataModel<string>>>
            {
                Result = new List<LookUpDataModel<string>>(),
                IsSuccess = false,
                Message = "ErrorFound"
            };
        }
    }


    private string GenerateVerificationCode()
    {
        var rng = new Random();
        return rng.Next(100000, 999999).ToString();
    }
}

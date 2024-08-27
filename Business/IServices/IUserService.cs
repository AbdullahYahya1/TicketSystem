using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.IServices
{
    public interface IUserService : IService<AppUser>
    {
        Task<ResponseModel<bool>> CreateUser(PostUserDto userDto);
        Task<ResponseModel<AuthenticationResponse>> Authenticate(string emailOrName, string password);
        Task<ResponseModel<TokenResponse>> GenerateTokens(AppUser user);
        Task<ResponseModel<TokenResponse>> RefreshToken(TokenRequest tokenRequest);
        Task<ResponseModel<GetOneUserDto>> GetUserById(string id);
        Task<ResponseModel<bool>> UpdateUser(PutUserDto userDto);
        Task<ResponseModel<bool>> UpdateUserAsManager(PutUserDtoManger userDto, string userId);
        Task<ResponseModel<bool>> ActivateDeactivateUser(string id);
        Task<ResponseModel<IEnumerable<GetUserDto>>> GetAllUsersAsync( UserType? type = null);
        Task<ResponseModel<IEnumerable<GetAllUsersWithTicketsDto>>> GetClientsWithTickets();
        Task<ResponseModel<UserWithTicketStatsDto>> GetCurrentUserTicketStats();
        Task<ResponseModel<bool>> ResetPassword(ResetPasswordDto resetPasswordDto);
        Task<ResponseModel<bool>> ForgetPassword(ForgetPasswordDto forgetPasswordDto);
        Task<ResponseModel<List<LookUpDataModel<int>>>> GetUserTypesLookup();
        Task<ResponseModel<List<LookUpDataModel<string>>>> GetUsersLookup(UserType? type = null);

    }
}

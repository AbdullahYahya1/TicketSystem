using System.Linq.Expressions;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.IRepositories
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<IEnumerable<AppUser>> GetAllUsers(Expression<Func<AppUser, bool>> filter = null);
        Task<AppUser> GetUserById(string id);
        Task CreateUser(AppUser user);
        Task UpdateUser(AppUser user);
        Task<AppUser> GetUserByEmail(string email);
        Task<AppUser> GetUserByName(string Name);
        Task<UserWithTicketStatsDto> GetUserTicketStatsById(string id);
        Task<IEnumerable<AppUser>> GetClientsWithTicketsAsync();
    }
}

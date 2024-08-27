using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.DataAccess.Repositories
{
    public class UserRepository : Repository<AppUser>, IUserRepository
    {
        private readonly TicketSystemDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(TicketSystemDbContext context , IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers(Expression<Func<AppUser, bool>> filter = null)
        {
            if (filter != null)
            {
                return await _context.Users.Where(filter).ToListAsync();
            }

            return await _context.Users.ToListAsync();
        }


        public async Task<AppUser?> GetUserById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task CreateUser(AppUser user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUser(AppUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task<AppUser> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser> GetUserByName(string Name)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == Name);
        }

        public async Task<UserWithTicketStatsDto> GetUserTicketStatsById(string id)
        {
            var user = await _context.Users
                .Include(u => u.CreatedTickets)
                .Include(u => u.AssignedTickets)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return null;

            IEnumerable<Ticket> tickets = user.UserType == UserType.Support ? user.AssignedTickets : user.CreatedTickets;
            if (user.UserType == UserType.Manager)
            {
                tickets = await _context.Tickets.ToListAsync(); 
            }

            var userDto = _mapper.Map<GetOneUserDto>(user);

            var statsDto = new UserWithTicketStatsDto
            {
                User = userDto,
                TotalTickets = tickets.Count(),
                ClosedTickets = tickets.Count(t => t.Status == TicketStatus.Closed),
                NewTickets = tickets.Count(t => t.Status == TicketStatus.New),
                InProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress),
                AssignedTickets = tickets.Count(t => t.Status == TicketStatus.Assigned),
                ResolvedTickets = tickets.Count(t => t.Status == TicketStatus.Resolved),
                ConfirmedTickets = tickets.Count(t => t.Status == TicketStatus.Confirmed),
                CanceledTickets = tickets.Count(t => t.Status == TicketStatus.Canceled),
                TopSupportUsers = new List<SupportUserDto>()
            };

            if (user.UserType == UserType.Manager)
            {
                var topSupports = await _context.Users
                    .Where(u => u.UserType == UserType.Support)
                    .Select(u => new SupportUserDto
                    {
                        UserId = u.UserId,
                        UserName = u.UserName,
                        TicketsHandled = u.AssignedTickets.Count(t => t.Status == TicketStatus.Closed)
                    })
                    .OrderByDescending(s => s.TicketsHandled)
                    .Take(5)
                    .ToListAsync();

                statsDto.TopSupportUsers = topSupports;
            }

            return statsDto;
        }

        public async Task<IEnumerable<AppUser>> GetClientsWithTicketsAsync()
        {
            return await _context.Users
                .Where(u => u.UserType == UserType.Client)
                .Include(u => u.CreatedTickets)
                    .ThenInclude(t => t.TicketType)
                        .ThenInclude(tt => tt.TicketCategory)
                .Include(u => u.CreatedTickets)
                    .ThenInclude(t => t.Product)
                .Include(u => u.CreatedTickets)
                    .ThenInclude(t => t.Attachments)
                .Include(u => u.CreatedTickets)
                    .ThenInclude(t => t.Comments)
                .Include(u => u.CreatedTickets)
                    .ThenInclude(t => t.TicketDetails)
                .ToListAsync();
        }

    }
}

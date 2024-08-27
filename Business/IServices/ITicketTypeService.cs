using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.IServices
{
    public interface ITicketTypeService : IService<TicketType>
    {
        Task<ResponseModel<IEnumerable<TicketTypeDto>>> GetTypesByCategoryIdAsync(int categoryId);
        Task<ResponseModel<TicketTypeDto>> GetTicketTypeByIdAsync(int id);
        Task<ResponseModel<TicketTypeDto>> CreateTicketTypeAsync(PostTicketTypeDto ticketTypeDto);
        Task<ResponseModel<bool>> DeleteTicketTypeAsync(int id);
        Task<ResponseModel<bool>> UpdateTicketTypeAsync(TicketTypeDto ticketTypeDto);
        Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketTypes();

    }
}

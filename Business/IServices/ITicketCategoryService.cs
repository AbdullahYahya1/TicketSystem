using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.IServices
{
    public interface ITicketCategoryService : IService<TicketCategory>
    {
        Task<ResponseModel<IEnumerable<TicketCategoryDto>>> GetAllTicketCategoriesAsync();
        Task<ResponseModel<TicketCategoryDto>> GetTicketCategoryByIdAsync(int id);
        Task<ResponseModel<TicketCategoryDto>> CreateTicketCategoryAsync(PostTicketCategoryDto ticketCategoryDto);
        Task<ResponseModel<bool>> UpdateTicketCategoryAsync(TicketCategoryDto ticketCategoryDto);
        Task<ResponseModel<bool>> DeleteTicketCategoryAsync(int id);
        Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketCategoriesLookupAsync();

    }
}

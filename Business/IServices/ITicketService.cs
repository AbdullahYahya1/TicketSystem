using Microsoft.AspNetCore.Http;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.IServices
{
    public interface ITicketService : IService<Ticket>
    {
        Task<ResponseModel<IEnumerable<GetTicketDto>>> GetAllTicketsAsync(int pageNumber, int pageSize, string status = null);
        Task<ResponseModel<GetOneTicketDto>> GetTicketByIdAsync(int ticketId);
        Task<ResponseModel<bool>> UpdateTicketStatusAsync(int ticketId, TicketStatus status);
        Task<ResponseModel<bool>> AssignTicketToEmployeeAsync(int ticketId, string employeeEmail);
        Task<ResponseModel<GetCommentDto>> AddCommentToTicketAsync(int ticketId, PostCommentDto commentDto);
        Task<ResponseModel<bool>> UpdateTicketCommentAsync(int ticketId, int commentId, string newContent);
        Task<ResponseModel<bool>> DeleteTicketCommentAsync(int ticketId, int commentId);
        Task<ResponseModel<TicketDto>> CreateTicketAsync( PostTicketDto ticketDto);
        Task<ResponseModel<IEnumerable<GetTicketDto>>> GetAllTicketsByUserIdAsync(string userId);
        Task<ResponseModel<bool>> UpdateTicketAsync(PutTicketDto updateTicketDto);
        Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketLookup();

    }
}

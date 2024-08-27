using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TicketSystem.Business.IServices;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;
using TicketSystem.DataAccess.Repositories;

namespace TicketSystem.Business.Services
{
    public class TicketTypeService : Service<TicketType>, ITicketTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketTypeService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketTypeService(TicketSystemDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketTypeService> logger,
             IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<IEnumerable<TicketTypeDto>>> GetTypesByCategoryIdAsync(int categoryId)
        {
            try
            {
                var types = await _unitOfWork.ticketTypes.GetTypesByCategoryIdAsync(categoryId);
                var typeDtos = _mapper.Map<IEnumerable<TicketTypeDto>>(types);
                return new ResponseModel<IEnumerable<TicketTypeDto>>
                {
                    Result = typeDtos,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketTypeService-GetTypesByCategoryIdAsync Request=CategoryId:{categoryId} / Response={ex.Message}");
                return new ResponseModel<IEnumerable<TicketTypeDto>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<TicketTypeDto>> GetTicketTypeByIdAsync(int id)
        {
            try
            {
                var ticketType = await _unitOfWork.ticketTypes.GetByIdAsync(id);
                if (ticketType == null)
                {
                    return new ResponseModel<TicketTypeDto> { IsSuccess = false, Message = "TicketTypeNotFound" };
                }

                var ticketTypeDto = _mapper.Map<TicketTypeDto>(ticketType);
                return new ResponseModel<TicketTypeDto>
                {
                    Result = ticketTypeDto,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketTypeService-GetTicketTypeByIdAsync Request={id} / Response={ex.Message}");
                return new ResponseModel<TicketTypeDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<TicketTypeDto>> CreateTicketTypeAsync(PostTicketTypeDto ticketTypeDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var ticketType = _mapper.Map<TicketType>(ticketTypeDto);
                ticketType.CreatedById = userId;
                ticketType.UpdateById = userId;
                ticketType.CreatedDate = DateTime.UtcNow;
                ticketType.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.ticketTypes.AddAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();

                var savedTicketType = await _unitOfWork.ticketTypes.GetByIdAsync(ticketType.TicketTypeId);

                var createdTicketTypeDto = _mapper.Map<TicketTypeDto>(savedTicketType);

                return new ResponseModel<TicketTypeDto>
                {
                    Result = createdTicketTypeDto,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the ticket type.");
                return new ResponseModel<TicketTypeDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


        public async Task<ResponseModel<bool>> DeleteTicketTypeAsync(int id)
        {
            try
            {
                var ticketType = await _unitOfWork.ticketTypes.GetByIdAsync(id);
                if (ticketType == null)
                {
                    return new ResponseModel<bool>
                    {
                        Result = false,
                        IsSuccess = false,
                        Message = $"TicketTypeIDWasNotFound."
                    };
                }

                await _unitOfWork.ticketTypes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Result = true,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketTypeService-DeleteTicketTypeAsync Request=TicketTypeId:{id} / Response={ex.Message}");
                return new ResponseModel<bool>
                {
                    Result = false,
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


        public async Task<ResponseModel<bool>> UpdateTicketTypeAsync(TicketTypeDto ticketTypeDto)
        {
            try
            {
                var ticketType = await _unitOfWork.ticketTypes.GetByIdAsync(ticketTypeDto.TicketTypeId);
                if (ticketType == null)
                {
                    return new ResponseModel<bool>
                    {
                        Result = false,
                        IsSuccess = false,
                        Message = $"TicketTypeNotFound"
                    };
                }

                _mapper.Map(ticketTypeDto, ticketType);
                await _unitOfWork.ticketTypes.UpdateAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Result = true,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketTypeService-UpdateTicketTypeAsync Request=TicketTypeId:{ticketTypeDto.TicketTypeId} / Response={ex.Message}");
                return new ResponseModel<bool>
                {
                    Result = false,
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketTypes()
        {
            try
            {
                var ticketTypes = await _unitOfWork.ticketTypes.GetAllAsync();
                var result = _mapper.Map<List<LookUpDataModel<int>>>(ticketTypes);

                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    Result = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the ticket types.");
                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    Result = new List<LookUpDataModel<int>>(),
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

    }
}

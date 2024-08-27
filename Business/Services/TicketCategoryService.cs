using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TicketSystem.Business.IServices;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.Services
{
    public class TicketCategoryService : Service<TicketCategory>, ITicketCategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TicketCategoryService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TicketCategoryService(TicketSystemDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketCategoryService> logger,
             IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseModel<IEnumerable<TicketCategoryDto>>> GetAllTicketCategoriesAsync()
        {
            try
            {
                var categories = await _unitOfWork.ticketCategories.GetAllAsync();
                var categoryDtos = _mapper.Map<IEnumerable<TicketCategoryDto>>(categories);
                return new ResponseModel<IEnumerable<TicketCategoryDto>>
                {
                    Result = categoryDtos,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketCategoryService-GetAllTicketCategoriesAsync Request=None / Response={ex.Message}");

                return new ResponseModel<IEnumerable<TicketCategoryDto>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"

                };
            }
        }

        public async Task<ResponseModel<TicketCategoryDto>> GetTicketCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _unitOfWork.ticketCategories.GetByIdAsync(id);
                var categoryDto = _mapper.Map<TicketCategoryDto>(category);
                return new ResponseModel<TicketCategoryDto>
                {
                    Result = categoryDto,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketCategoryService-GetTicketCategoryByIdAsync Request=TicketCategoryId:{id} / Response={ex.Message}");
                return new ResponseModel<TicketCategoryDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<TicketCategoryDto>> CreateTicketCategoryAsync(PostTicketCategoryDto ticketCategoryDto)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var ticketCategory = _mapper.Map<TicketCategory>(ticketCategoryDto);
                ticketCategory.CreatedById = userId;
                ticketCategory.UpdateById = userId;
                ticketCategory.CreatedDate = DateTime.UtcNow;
                ticketCategory.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.ticketCategories.AddAsync(ticketCategory);
                await _unitOfWork.SaveChangesAsync();

                var savedTicketCategory = await _unitOfWork.ticketCategories.GetByIdAsync(ticketCategory.TicketCategoryId);

                var createdTicketCategoryDto = _mapper.Map<TicketCategoryDto>(savedTicketCategory);

                return new ResponseModel<TicketCategoryDto>
                {
                    Result = createdTicketCategoryDto,
                    IsSuccess = true,
                    Message = string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the ticket category.");
                return new ResponseModel<TicketCategoryDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


        public async Task<ResponseModel<bool>> UpdateTicketCategoryAsync(TicketCategoryDto ticketCategoryDto)
        {
            try
            {
                var ticketCategory = await _unitOfWork.ticketCategories.GetByIdAsync(ticketCategoryDto.TicketCategoryId);
                if (ticketCategory == null)
                {
                    return new ResponseModel<bool>
                    {
                        Result = false,
                        IsSuccess = false,
                        Message = $"TicketCategoryNotFound."
                    };
                }

                _mapper.Map(ticketCategoryDto, ticketCategory);
                await _unitOfWork.ticketCategories.UpdateAsync(ticketCategory);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Result = true,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketCategoryService-UpdateTicketCategoryAsync Request=TicketCategoryId:{ticketCategoryDto.TicketCategoryId} / Response={ex.Message}");
                return new ResponseModel<bool>
                {
                    Result = false,
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


        public async Task<ResponseModel<bool>> DeleteTicketCategoryAsync(int id)
        {
            try
            {
                var ticketCategory = await _unitOfWork.ticketCategories.GetByIdAsync(id);
                if (ticketCategory == null)
                {
                    return new ResponseModel<bool>
                    {
                        Result = false,
                        IsSuccess = false,
                        Message = $"TicketCategoryIdWasNotFound."
                    };
                }

                await _unitOfWork.ticketCategories.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Result = true,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"TicketCategoryService-DeleteTicketCategoryAsync Request=TicketCategoryId:{id} / Response={ex.Message}");
                return new ResponseModel<bool>
                {
                    Result = false,
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<List<LookUpDataModel<int>>>> GetTicketCategoriesLookupAsync()
        {
            try
            {
                var categories = await _unitOfWork.ticketCategories.GetAllAsync();
                var lookupData = _mapper.Map<List<LookUpDataModel<int>>>(categories);
                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    Result = lookupData,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving ticket categories lookup: {ex.Message}");
                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }


    }
}

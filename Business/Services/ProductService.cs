using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TicketSystem.Business.IServices;
using TicketSystem.DataAccess.Context;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.IRepositories;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ProductService(TicketSystemDbContext context, IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger,
            IHttpContextAccessor httpContextAccessor) : base(context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;


        }

        public async Task<ResponseModel<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _unitOfWork.products.GetAllAsync();
                var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
                return new ResponseModel<IEnumerable<ProductDto>>
                {
                    Result = productDtos,
                    IsSuccess = true,

                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"ProductService-GetAllProductsAsync Request=None / Response={ex.Message}");
                return new ResponseModel<IEnumerable<ProductDto>>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };

            }
        }

        public async Task<ResponseModel<ProductDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ResponseModel<ProductDto>
                    {
                        IsSuccess = false,
                        Message = "ProductNotFound"
                    };
                }

                var productDto = _mapper.Map<ProductDto>(product);
                return new ResponseModel<ProductDto>
                {
                    Result = productDto,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"ProductService-GetProductByIdAsync Request=ProductId:{id} / Response={ex.Message}");
                return new ResponseModel<ProductDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<PostProductDto>> CreateProductAsync(PostProductDto productDto)
        {
            try
            {
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                var product = _mapper.Map<Product>(productDto);
                product.UpdateById = currentUserId;
                product.CreatedById= currentUserId;
                product.UpdateDate = DateTime.UtcNow;
                product.CreatedDate = DateTime.UtcNow;

                await _unitOfWork.products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var ProductDto = _mapper.Map<PostProductDto>(product);
                return new ResponseModel<PostProductDto>
                {
                    Result = ProductDto,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"ProductService-CreateProductAsync Request=ProductDto:{productDto} / Response={ex.Message}");
                return new ResponseModel<PostProductDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<ProductDto>> UpdateProductAsync(PostProductDto productDto, int id)
        {
            try
            {
                var product = await _unitOfWork.products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ResponseModel<ProductDto>
                    {
                        IsSuccess = false,
                        Message = "ProductNotFound"
                    };
                }
                var currentUserId = _httpContextAccessor.HttpContext.User?.FindFirst("UserId")?.Value;
                _mapper.Map(productDto, product);
                product.UpdateById = currentUserId;
                product.UpdateDate = DateTime.UtcNow;

                await _unitOfWork.products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                var updatedProductDto = _mapper.Map<ProductDto>(product);
                return new ResponseModel<ProductDto>
                {
                    Result = updatedProductDto,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"ProductService-UpdateProductAsync Request=ProductDto:{productDto} / Response={ex.Message}");
                return new ResponseModel<ProductDto>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }

        public async Task<ResponseModel<bool>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _unitOfWork.products.GetByIdAsync(id);
                if (product == null)
                {
                    return new ResponseModel<bool>
                    {
                        IsSuccess = false,
                        Message = "ProductNotFound"
                    };
                }

                await _unitOfWork.products.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ResponseModel<bool>
                {
                    Result = true,
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"ProductService-DeleteProductAsync Request=ProductId:{id} / Response={ex.Message}");
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Message = "ErrorFound"
                };
            }
        }
        public async Task<ResponseModel<List<LookUpDataModel<int>>>> ProducsLookUp()
        {
            try
            {
                var products = await _unitOfWork.products.GetAllAsync();
                var result = _mapper.Map<List<LookUpDataModel<int>>>(products);
                return new ResponseModel<List<LookUpDataModel<int>>>
                {
                    Result = result,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the product lookup.");
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

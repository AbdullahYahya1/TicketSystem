using TicketSystem.Business.Services;
using TicketSystem.DataAccess.DTOs;
using TicketSystem.DataAccess.Models;

namespace TicketSystem.Business.IServices
{
    public interface IProductService : IService<Product>
    {
        Task<ResponseModel<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ResponseModel<ProductDto>> GetProductByIdAsync(int id);
        Task<ResponseModel<PostProductDto>> CreateProductAsync(PostProductDto productDto);
        Task<ResponseModel<ProductDto>> UpdateProductAsync(PostProductDto productDto, int id);
        Task<ResponseModel<bool>> DeleteProductAsync(int id);
        Task<ResponseModel<List<LookUpDataModel<int>>>> ProducsLookUp();

    }
}

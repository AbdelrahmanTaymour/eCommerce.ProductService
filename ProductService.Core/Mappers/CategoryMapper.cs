using ProductService.Core.DTOs.CategoryDTOs;
using ProductService.Core.Entities;

namespace ProductService.Core.Mappers;

public static class CategoryMapper
{
    public static Category MapToCategoryEntity(this CreateCategoryRequestDto requestDto)
    {
        return new Category
        {
            Name = requestDto.Name
        };
    }
    
    public static CategoryDto MapToCategoryDto(this Category entity)
    {
        return new CategoryDto(entity.Id, entity.Name, entity.CreatedAt, entity.UpdatedAt);
    }
}
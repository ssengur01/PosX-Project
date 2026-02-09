using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Entities;

namespace Inventory.Application.Mappings;

public class InventoryMappingProfile : Profile
{
    public InventoryMappingProfile()
    {
        CreateMap<InventoryItem, InventoryDto>()
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock()));

        CreateMap<StockMovement, StockMovementDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
    }
}

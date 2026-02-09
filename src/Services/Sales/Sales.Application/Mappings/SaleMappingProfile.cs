using AutoMapper;
using Sales.Application.DTOs;
using Sales.Domain.Entities;

namespace Sales.Application.Mappings;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal.Amount))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount.Amount))
            .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Total.Currency));

        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal.Amount))
            .ForMember(dest => dest.TaxRate, opt => opt.MapFrom(src => src.TaxRate.Rate))
            .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src => src.TaxAmount.Amount))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount));

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount));

        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.StartingCash, opt => opt.MapFrom(src => src.StartingCash.Amount))
            .ForMember(dest => dest.EndingCash, opt => opt.MapFrom(src => src.EndingCash != null ? src.EndingCash.Amount : (decimal?)null))
            .ForMember(dest => dest.ExpectedCash, opt => opt.MapFrom(src => src.ExpectedCash.Amount))
            .ForMember(dest => dest.Difference, opt => opt.MapFrom(src => src.Difference.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.StartingCash.Currency));

        CreateMap<CashMovement, CashMovementDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));
    }
}

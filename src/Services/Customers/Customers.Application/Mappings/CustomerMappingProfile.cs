using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Entities;

namespace Customers.Application.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Contact.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Contact.Email))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Contact.Address));
    }
}

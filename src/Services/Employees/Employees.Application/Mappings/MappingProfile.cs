using AutoMapper;
using Employees.Application.DTOs;
using Employees.Domain.Entities;

namespace Employees.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Employee, EmployeeDto>()
            .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.Commission != null ? src.Commission.Rate : (decimal?)null))
            .ForMember(dest => dest.CommissionMinimumSales, opt => opt.MapFrom(src => src.Commission != null ? src.Commission.MinimumSales : (decimal?)null));

        CreateMap<Role, RoleDto>()
            .ConstructUsing(r => new RoleDto(
                r.Id,
                r.Name,
                r.Description,
                r.Permissions.ToList()
            ));

        CreateMap<TimeEntry, TimeEntryDto>()
            .ConstructUsing(t => new TimeEntryDto(
                t.Id,
                t.EmployeeId,
                t.ClockInTime,
                t.ClockOutTime,
                t.GetHoursWorked(),
                t.Notes
            ));
    }
}

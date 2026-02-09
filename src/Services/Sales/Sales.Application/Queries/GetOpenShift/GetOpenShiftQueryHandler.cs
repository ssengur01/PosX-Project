using AutoMapper;
using MediatR;
using Sales.Application.DTOs;
using Sales.Domain.Interfaces;

namespace Sales.Application.Queries.GetOpenShift;

public class GetOpenShiftQueryHandler : IRequestHandler<GetOpenShiftQuery, ShiftDto?>
{
    private readonly IShiftRepository _shiftRepository;
    private readonly IMapper _mapper;

    public GetOpenShiftQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
    {
        _shiftRepository = shiftRepository;
        _mapper = mapper;
    }

    public async Task<ShiftDto?> Handle(GetOpenShiftQuery request, CancellationToken cancellationToken)
    {
        var shift = await _shiftRepository.GetOpenShiftByEmployeeAsync(request.EmployeeId);
        return shift == null ? null : _mapper.Map<ShiftDto>(shift);
    }
}

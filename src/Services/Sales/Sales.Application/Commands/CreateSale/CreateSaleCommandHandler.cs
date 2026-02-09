using AutoMapper;
using MediatR;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Enums;
using Sales.Domain.Interfaces;
using Sales.Domain.ValueObjects;

namespace Sales.Application.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, SaleDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateSaleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // Generate sale number
        var saleNumber = await _unitOfWork.SaleRepository.GenerateSaleNumberAsync(cancellationToken);

        // Create sale
        var sale = new Sale(saleNumber, request.EmployeeId, request.CustomerId);

        // Add items
        foreach (var item in request.Items)
        {
            var unitPrice = new Money(item.UnitPrice);
            var taxRate = new TaxRate(item.TaxRate, $"KDV %{item.TaxRate}");
            var discount = Discount.None;

            sale.AddItem(item.ProductId, item.ProductName, item.Quantity, unitPrice, taxRate, discount);
        }

        // Add payments
        foreach (var payment in request.Payments)
        {
            var method = Enum.Parse<PaymentMethod>(payment.Method);
            var amount = new Money(payment.Amount);
            sale.AddPayment(method, amount, payment.TransactionId);
        }

        // Complete sale
        sale.Complete();

        // Save to database
        await _unitOfWork.SaleRepository.AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SaleDto>(sale);
    }
}

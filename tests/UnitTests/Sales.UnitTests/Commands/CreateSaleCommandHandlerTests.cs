using AutoMapper;
using FluentAssertions;
using Moq;
using Sales.Application.Commands.CreateSale;
using Sales.Application.DTOs;
using Sales.Domain.Entities;
using Sales.Domain.Interfaces;

namespace Sales.UnitTests.Commands;

public class CreateSaleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISaleRepository> _mockSaleRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateSaleCommandHandler _handler;

    public CreateSaleCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSaleRepository = new Mock<ISaleRepository>();
        _mockMapper = new Mock<IMapper>();

        _mockUnitOfWork.Setup(u => u.SaleRepository).Returns(_mockSaleRepository.Object);

        _handler = new CreateSaleCommandHandler(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesSaleSuccessfully()
    {
        // Arrange
        var command = new CreateSaleCommand(
            EmployeeId: Guid.NewGuid(),
            CustomerId: Guid.NewGuid(),
            Items: new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto(
                    ProductId: Guid.NewGuid(),
                    ProductName: "Test Product",
                    Quantity: 2,
                    UnitPrice: 100m,
                    TaxRate: 18m)
            },
            Payments: new List<PaymentItemDto>
            {
                new PaymentItemDto(
                    Method: "Cash",
                    Amount: 250m,
                    TransactionId: null)
            });

        _mockSaleRepository
            .Setup(r => r.GenerateSaleNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("SALE-001");

        _mockSaleRepository
            .Setup(r => r.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var expectedDto = new SaleDto
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            EmployeeId = command.EmployeeId,
            CustomerId = command.CustomerId,
            Status = "Completed",
            Subtotal = 200m,
            DiscountAmount = 0m,
            TaxAmount = 36m,
            Total = 236m,
            Currency = "TRY",
            Items = new List<SaleItemDto>(),
            Payments = new List<PaymentDto>()
        };

        _mockMapper
            .Setup(m => m.Map<SaleDto>(It.IsAny<Sale>()))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SaleNumber.Should().Be("SALE-001");

        _mockSaleRepository.Verify(
            r => r.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mockUnitOfWork.Verify(
            u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_GeneratesSaleNumber()
    {
        // Arrange
        var command = new CreateSaleCommand(
            EmployeeId: Guid.NewGuid(),
            CustomerId: null,
            Items: new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto(
                    ProductId: Guid.NewGuid(),
                    ProductName: "Product",
                    Quantity: 1,
                    UnitPrice: 100m,
                    TaxRate: 18m)
            },
            Payments: new List<PaymentItemDto>
            {
                new PaymentItemDto("Cash", 118m, null)
            });

        _mockSaleRepository
            .Setup(r => r.GenerateSaleNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("SALE-999");

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<SaleDto>(It.IsAny<Sale>()))
            .Returns(new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-999",
                SaleDate = DateTime.UtcNow,
                EmployeeId = command.EmployeeId,
                CustomerId = null,
                Status = "Completed",
                Subtotal = 100m,
                DiscountAmount = 0m,
                TaxAmount = 18m,
                Total = 118m,
                Currency = "TRY",
                Items = new List<SaleItemDto>(),
                Payments = new List<PaymentDto>()
            });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockSaleRepository.Verify(
            r => r.GenerateSaleNumberAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsSaleToRepository()
    {
        // Arrange
        var command = new CreateSaleCommand(
            EmployeeId: Guid.NewGuid(),
            CustomerId: null,
            Items: new List<CreateSaleItemDto>
            {
                new CreateSaleItemDto(Guid.NewGuid(), "Product", 1, 100m, 18m)
            },
            Payments: new List<PaymentItemDto>
            {
                new PaymentItemDto("Cash", 118m, null)
            });

        _mockSaleRepository
            .Setup(r => r.GenerateSaleNumberAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("SALE-001");

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockMapper
            .Setup(m => m.Map<SaleDto>(It.IsAny<Sale>()))
            .Returns(new SaleDto
            {
                Id = Guid.NewGuid(),
                SaleNumber = "SALE-001",
                SaleDate = DateTime.UtcNow,
                EmployeeId = command.EmployeeId,
                CustomerId = null,
                Status = "Completed",
                Subtotal = 100m,
                DiscountAmount = 0m,
                TaxAmount = 18m,
                Total = 118m,
                Currency = "TRY",
                Items = new List<SaleItemDto>(),
                Payments = new List<PaymentDto>()
            });

        Sale? capturedSale = null;
        _mockSaleRepository
            .Setup(r => r.AddAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
            .Callback<Sale, CancellationToken>((sale, _) => capturedSale = sale)
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedSale.Should().NotBeNull();
        capturedSale!.Items.Should().HaveCount(1);
        capturedSale.Payments.Should().HaveCount(1);
        capturedSale.Status.Should().Be(Sales.Domain.Enums.SaleStatus.Completed);
    }
}

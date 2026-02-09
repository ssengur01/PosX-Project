using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Sales.Application.Commands.CreateSale;
using Sales.Application.DTOs;

namespace Sales.IntegrationTests;

public class SalesControllerTests : IClassFixture<SalesApiFactory>
{
    private readonly HttpClient _client;
    private readonly SalesApiFactory _factory;

    public SalesControllerTests(SalesApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateSale_WithValidData_ReturnsCreatedSale()
    {
        // Arrange
        var command = new CreateSaleCommand(
            EmployeeId: Guid.NewGuid(),
            CustomerId: null,
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

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/sales", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var sale = JsonSerializer.Deserialize<SaleDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        sale.Should().NotBeNull();
        sale!.SaleNumber.Should().NotBeNullOrEmpty();
        sale.Items.Should().HaveCount(1);
        sale.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task CreateSale_WithInsufficientPayment_ReturnsBadRequest()
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
                new PaymentItemDto("Cash", 50m, null) // Not enough
            });

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/sales", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateSale_WithoutItems_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateSaleCommand(
            EmployeeId: Guid.NewGuid(),
            CustomerId: null,
            Items: new List<CreateSaleItemDto>(), // Empty
            Payments: new List<PaymentItemDto>
            {
                new PaymentItemDto("Cash", 100m, null)
            });

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/sales", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

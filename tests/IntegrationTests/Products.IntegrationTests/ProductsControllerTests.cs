using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Products.Application.Commands.CreateProduct;
using Products.Application.DTOs;
using Products.Domain.Entities;
using Products.Infrastructure.Persistence;

namespace Products.IntegrationTests;

public class ProductsControllerTests : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client;
    private readonly ProductsApiFactory _factory;

    public ProductsControllerTests(ProductsApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessAndProducts()
    {
        // Arrange
        await SeedTestData();

        // Act
        var response = await _client.GetAsync("/api/v1/products?page=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PagedResultDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        result.Should().NotBeNull();
        result!.Items.Should().NotBeNull();
        result.Items.Count().Should().BeGreaterThan(0);
        result.TotalCount.Should().BeGreaterThan(0);
    }

    private record PagedResultDto(IEnumerable<ProductDto> Items, int TotalCount, int Page, int PageSize);

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        await SeedTestCategory();

        var command = new CreateProductCommand(
            Name: "Integration Test Product",
            Description: "Test Description",
            SKU: "INT-TEST-001",
            Barcode: "1234567890123",
            Price: 99.99m,
            Cost: 50.00m,
            Currency: "TRY",
            CategoryId: await GetTestCategoryId(),
            MinimumStockLevel: 10
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadAsStringAsync();
        var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        product.Should().NotBeNull();
        product!.Name.Should().Be("Integration Test Product");
        product.SKU.Should().Be("INT-TEST-001");
        product.Price.Should().Be(99.99m);
    }


    [Fact]
    public async Task CreateProduct_WithDuplicateSKU_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestCategory();
        var categoryId = await GetTestCategoryId();

        var firstCommand = new CreateProductCommand(
            Name: "First Product",
            Description: "Test Description",
            SKU: "DUPLICATE-SKU",
            Barcode: null,
            Price: 10.00m,
            Cost: 5.00m,
            Currency: "TRY",
            CategoryId: categoryId,
            MinimumStockLevel: 10
        );

        var secondCommand = new CreateProductCommand(
            Name: "Second Product",
            Description: "Test Description",
            SKU: "DUPLICATE-SKU",
            Barcode: null,
            Price: 15.00m,
            Cost: 7.00m,
            Currency: "TRY",
            CategoryId: categoryId,
            MinimumStockLevel: 10
        );

        // Act
        var firstResponse = await _client.PostAsJsonAsync("/api/v1/products", firstCommand);
        var secondResponse = await _client.PostAsJsonAsync("/api/v1/products", secondCommand);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetProducts_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        await SeedMultipleProducts(15);

        // Act
        var page1Response = await _client.GetAsync("/api/v1/products?page=1&pageSize=10");
        var page2Response = await _client.GetAsync("/api/v1/products?page=2&pageSize=10");

        // Assert
        page1Response.StatusCode.Should().Be(HttpStatusCode.OK);
        page2Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var page1Content = await page1Response.Content.ReadAsStringAsync();
        var page1Result = JsonSerializer.Deserialize<PagedResultDto>(page1Content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var page2Content = await page2Response.Content.ReadAsStringAsync();
        var page2Result = JsonSerializer.Deserialize<PagedResultDto>(page2Content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        page1Result.Should().NotBeNull();
        page1Result!.Items.Should().HaveCount(10);
        page1Result.TotalCount.Should().Be(15);

        page2Result.Should().NotBeNull();
        page2Result!.Items.Should().HaveCount(5);
        page2Result.TotalCount.Should().Be(15);
    }

    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        // Seed test category
        var category = new Category("Test Category", "Test Description");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        // Seed test products
        var product1 = new Product("Test Product 1", "Description 1", "TEST-001",
            new Products.Domain.ValueObjects.Money(100m, "TRY"),
            new Products.Domain.ValueObjects.Money(50m, "TRY"),
            category.Id);

        var product2 = new Product("Test Product 2", "Description 2", "TEST-002",
            new Products.Domain.ValueObjects.Money(200m, "TRY"),
            new Products.Domain.ValueObjects.Money(100m, "TRY"),
            category.Id);

        context.Products.AddRange(product1, product2);
        await context.SaveChangesAsync();
    }

    private async Task SeedTestCategory()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        if (!await context.Categories.AnyAsync())
        {
            var category = new Category("Test Category", "Test Description");
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        }
    }

    private async Task<Guid> GetTestCategoryId()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        var category = await context.Categories.FirstOrDefaultAsync();
        if (category == null)
        {
            category = new Category("Test Category", "Test Description");
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        }

        return category.Id;
    }

    private async Task SeedMultipleProducts(int count)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        // Seed test category
        var category = new Category("Test Category", "Test Description");
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        // Seed multiple products
        for (int i = 1; i <= count; i++)
        {
            var product = new Product($"Product {i}", $"Description {i}", $"TEST-{i:D3}",
                new Products.Domain.ValueObjects.Money(100m * i, "TRY"),
                new Products.Domain.ValueObjects.Money(50m * i, "TRY"),
                category.Id);

            context.Products.Add(product);
        }

        await context.SaveChangesAsync();
    }
}

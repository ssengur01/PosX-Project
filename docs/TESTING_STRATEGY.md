# Testing Strategy - PosX Project

## Overview

This document outlines the testing strategy for the PosX desktop POS application. Our testing approach follows the **Test Pyramid** methodology, emphasizing automated testing at multiple levels to ensure code quality, reliability, and maintainability.

## Test Pyramid

We follow the standard test pyramid structure:

```
           /\
          /E2E\          <- Few (UI/Desktop App End-to-End)
         /------\
        /  API  \        <- Some (Integration Tests)
       /----------\
      /   Unit    \      <- Many (Unit Tests)
     /--------------\
```

### Distribution Target:
- **70% Unit Tests** - Fast, isolated tests for business logic
- **20% Integration Tests** - API endpoint tests with database
- **10% End-to-End Tests** - Full user journey tests

## Test Types

### 1. Unit Tests

**Purpose**: Test individual components in isolation (domain entities, value objects, command handlers, query handlers).

**Location**: `tests/UnitTests/{ServiceName}.UnitTests/`

**Example**: `tests/UnitTests/Products.UnitTests/`

**Characteristics**:
- Fast execution (milliseconds)
- No external dependencies (mocked)
- High code coverage target (>80%)
- Test single responsibility

**Tools**:
- **xUnit** - Testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Readable assertions

**Example Structure**:
```
Products.UnitTests/
├── Domain/
│   └── ProductTests.cs           # Domain entity tests
├── Commands/
│   └── CreateProductCommandHandlerTests.cs  # Command handler tests
└── Queries/
    └── GetProductsQueryHandlerTests.cs      # Query handler tests
```

**Sample Test**:
```csharp
[Fact]
public void Constructor_ValidData_CreatesProduct()
{
    // Arrange
    var name = "Test Product";
    var price = new Money(100m, "TRY");

    // Act
    var product = new Product(name, "Description", "SKU-001", price, ...);

    // Assert
    product.Name.Should().Be(name);
    product.Price.Should().Be(price);
}
```

### 2. Integration Tests

**Purpose**: Test API endpoints with real database and full request/response cycle.

**Location**: `tests/IntegrationTests/{ServiceName}.IntegrationTests/`

**Example**: `tests/IntegrationTests/Products.IntegrationTests/`

**Characteristics**:
- Slower execution (seconds)
- Use in-memory database
- Test full HTTP stack
- Verify end-to-end API behavior

**Tools**:
- **xUnit** - Testing framework
- **WebApplicationFactory** - In-memory test server
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database
- **FluentAssertions** - Readable assertions

**Example Structure**:
```
Products.IntegrationTests/
├── ProductsApiFactory.cs         # Test server factory
└── ProductsControllerTests.cs    # API endpoint tests
```

**Sample Test**:
```csharp
[Fact]
public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
{
    // Arrange
    var command = new CreateProductCommand(...);

    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/products", command);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var product = await response.Content.ReadFromJsonAsync<ProductDto>();
    product.Name.Should().Be("Test Product");
}
```

### 3. End-to-End Tests (Future)

**Purpose**: Test complete user workflows in the desktop application.

**Location**: `tests/E2ETests/`

**Status**: To be implemented in Stage 17

**Tools** (Planned):
- Playwright or Selenium
- SpecFlow (BDD scenarios)

## Test Organization

### Naming Conventions

#### Test Projects:
- Unit Tests: `{ServiceName}.UnitTests`
- Integration Tests: `{ServiceName}.IntegrationTests`

#### Test Classes:
- Domain tests: `{EntityName}Tests`
- Command tests: `{CommandName}HandlerTests`
- Controller tests: `{ControllerName}Tests`

#### Test Methods:
Format: `MethodName_Scenario_ExpectedBehavior`

Examples:
- `Constructor_ValidData_CreatesProduct`
- `UpdateStock_NegativeQuantity_ThrowsException`
- `CreateProduct_WithDuplicateSKU_ReturnsBadRequest`

### Test Structure (AAA Pattern)

All tests follow the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public void Method_Scenario_ExpectedResult()
{
    // Arrange - Set up test data and dependencies
    var product = CreateTestProduct();
    var newPrice = new Money(150m, "TRY");

    // Act - Execute the method being tested
    product.UpdatePricing(newPrice, product.Cost);

    // Assert - Verify the expected outcome
    product.Price.Should().Be(newPrice);
}
```

## Running Tests

### Run All Tests
```bash
dotnet test
```

### Run Specific Test Project
```bash
dotnet test tests/UnitTests/Products.UnitTests/Products.UnitTests.csproj
dotnet test tests/IntegrationTests/Products.IntegrationTests/Products.IntegrationTests.csproj
```

### Run Tests with Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Run Tests in Parallel
```bash
dotnet test --parallel
```

### Run Specific Test
```bash
dotnet test --filter "FullyQualifiedName=Products.UnitTests.Domain.ProductTests.Constructor_ValidData_CreatesProduct"
```

### Verbose Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Writing New Tests

### Unit Test Guidelines

1. **Test One Thing**: Each test should verify a single behavior
2. **Mock External Dependencies**: Use Moq to mock repositories, external services
3. **Use Meaningful Names**: Test names should describe what is being tested
4. **Keep Tests Simple**: Tests should be easy to understand
5. **Avoid Logic in Tests**: No conditionals, loops, or complex logic
6. **Test Edge Cases**: Include boundary conditions and error scenarios

**Example - Good Unit Test**:
```csharp
[Fact]
public void UpdateStock_PositiveQuantity_IncreasesStock()
{
    // Arrange
    var product = CreateTestProduct();
    var initialStock = product.StockQuantity;

    // Act
    product.UpdateStock(50);

    // Assert
    product.StockQuantity.Should().Be(initialStock + 50);
}
```

**Example - Bad Unit Test** (What NOT to do):
```csharp
[Fact]
public void TestProduct() // ❌ Poor naming
{
    var product = new Product(...); // ❌ No arrange section
    product.UpdateStock(50);
    if (product.StockQuantity > 0) // ❌ Logic in test
    {
        Assert.True(true); // ❌ Meaningless assertion
    }
}
```

### Integration Test Guidelines

1. **Use WebApplicationFactory**: Host API in-memory for testing
2. **Reset Database**: Clean state between tests
3. **Test Real Scenarios**: Test actual API contracts
4. **Verify HTTP Status Codes**: Check 200, 201, 400, 404, etc.
5. **Test Response Content**: Deserialize and verify DTOs
6. **Use Realistic Data**: Test with data similar to production

**Example**:
```csharp
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
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        result.Items.Should().NotBeEmpty();
    }
}
```

## Test Coverage Goals

### Target Coverage by Layer:

| Layer | Target | Current |
|-------|--------|---------|
| Domain Entities | 90% | 85% |
| Value Objects | 95% | 90% |
| Command Handlers | 80% | 75% |
| Query Handlers | 80% | 70% |
| API Controllers | 70% | 65% |
| **Overall** | **80%** | **75%** |

### Coverage Exclusions:
- Program.cs (startup code)
- Migrations
- DTOs (data transfer objects)
- Configuration files

## Test Data Management

### Unit Tests
- Use **helper methods** to create test data
- Keep test data **minimal and relevant**
- Use **object mothers** or **builders** for complex entities

**Example**:
```csharp
private Product CreateTestProduct()
{
    return new Product(
        "Test Product",
        "Test Description",
        "TEST-001",
        new Money(100m, "TRY"),
        new Money(50m, "TRY"),
        Guid.NewGuid()
    );
}
```

### Integration Tests
- **Seed database** before each test
- **Clean database** after each test (or use unique database per test)
- Use **realistic data** that mirrors production

**Example**:
```csharp
private async Task SeedTestData()
{
    using var scope = _factory.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

    context.Products.RemoveRange(context.Products);
    await context.SaveChangesAsync();

    var category = new Category("Test Category", "Description");
    context.Categories.Add(category);

    var product = new Product("Product 1", "Desc", "SKU-001", ...);
    context.Products.Add(product);

    await context.SaveChangesAsync();
}
```

## Mocking Strategy

### What to Mock:
- External services (APIs, databases, file systems)
- Repositories (in unit tests)
- Time-dependent functionality (use IDateTimeProvider)
- Random behavior

### What NOT to Mock:
- Domain entities and value objects
- Simple DTOs
- Configuration objects
- Code under test

**Example**:
```csharp
// Mock repository
var mockRepository = new Mock<IProductRepository>();
mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(testProduct);

// Mock unit of work
var mockUnitOfWork = new Mock<IUnitOfWork>();
mockUnitOfWork
    .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
    .ReturnsAsync(1);
```

## Continuous Integration

### GitHub Actions Workflow

Tests run automatically on:
- Every push to `main` branch
- Every pull request
- Manual workflow dispatch

**Current Status**: ✓ Configured in `.github/workflows/build.yml`

**Test Execution**:
```yaml
- name: Run Tests
  run: dotnet test --no-restore --verbosity normal
```

### Quality Gates

Pull requests must pass:
- ✓ All tests passing
- ✓ Build successful
- ✓ No critical warnings

## Common Testing Patterns

### 1. Testing Exceptions

```csharp
[Fact]
public void UpdateStock_NegativeQuantity_ThrowsException()
{
    // Arrange
    var product = CreateTestProduct();

    // Act & Assert
    Assert.Throws<InvalidOperationException>(() =>
        product.UpdateStock(-20));
}

// Or with FluentAssertions:
product.Invoking(p => p.UpdateStock(-20))
    .Should().Throw<InvalidOperationException>()
    .WithMessage("Stock quantity cannot be negative");
```

### 2. Testing Async Methods

```csharp
[Fact]
public async Task Handle_ValidCommand_ReturnsSuccess()
{
    // Arrange
    var command = new CreateProductCommand(...);

    // Act
    var result = await _handler.Handle(command, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
}
```

### 3. Parameterized Tests

```csharp
[Theory]
[InlineData(10, 20, 30)]
[InlineData(0, 50, 50)]
[InlineData(-10, 100, 90)]
public void AdjustStock_VariousQuantities_UpdatesCorrectly(
    int initial, int adjustment, int expected)
{
    // Arrange
    var product = CreateTestProduct();
    product.UpdateStock(initial);

    // Act
    product.AdjustStock(adjustment);

    // Assert
    product.StockQuantity.Should().Be(expected);
}
```

### 4. Verifying Mock Calls

```csharp
[Fact]
public async Task CreateProduct_CallsRepositoryOnce()
{
    // Arrange & Act
    await _handler.Handle(command, CancellationToken.None);

    // Assert
    _mockRepository.Verify(
        r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()),
        Times.Once);
}
```

## Best Practices Summary

### DO:
✅ Write tests first (TDD) when possible
✅ Keep tests simple and focused
✅ Use descriptive test names
✅ Follow AAA pattern
✅ Test edge cases and error scenarios
✅ Mock external dependencies
✅ Use FluentAssertions for readable assertions
✅ Run tests frequently during development
✅ Maintain high test coverage (>80%)
✅ Keep tests fast (unit tests < 100ms)

### DON'T:
❌ Test implementation details
❌ Have logic in tests (if/else, loops)
❌ Share state between tests
❌ Test multiple things in one test
❌ Ignore failing tests
❌ Copy-paste test code
❌ Test private methods directly
❌ Have tests depend on test execution order
❌ Use real external services in tests
❌ Leave tests commented out

## Troubleshooting

### Common Issues:

**1. Tests Pass Locally But Fail in CI**
- Check for hardcoded paths
- Verify environment variables
- Check timezone differences
- Look for test order dependencies

**2. Flaky Tests (Intermittent Failures)**
- Remove timing dependencies
- Use proper async/await
- Avoid Thread.Sleep
- Check for shared state

**3. Slow Tests**
- Profile test execution time
- Reduce database operations
- Use in-memory database for integration tests
- Mock expensive operations

## Next Steps

### Stage 17 Improvements:
- [ ] Add performance tests
- [ ] Implement E2E tests for desktop app
- [ ] Set up code coverage reporting (Coverlet + ReportGenerator)
- [ ] Add mutation testing (Stryker.NET)
- [ ] Implement contract testing (Pact)

## Resources

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Microsoft Testing Best Practices](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)
- [Test Pyramid - Martin Fowler](https://martinfowler.com/bliki/TestPyramid.html)

---

**Last Updated**: 2026-02-05
**Version**: 1.0
**Stage**: 16 - Testing & QA

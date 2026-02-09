# Testing Quick Reference

## Quick Commands

### Run All Tests
```bash
dotnet test
```

### Run Specific Project
```bash
# Unit tests
dotnet test tests/UnitTests/Products.UnitTests/

# Integration tests
dotnet test tests/IntegrationTests/Products.IntegrationTests/
```

### Run with Coverage
```bash
dotnet test /p:CollectCoverage=true
```

### Run Single Test
```bash
dotnet test --filter "FullyQualifiedName=Namespace.Class.TestMethod"
```

## Test Templates

### Unit Test - Domain Entity
```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var entity = new Entity(...);

    // Act
    entity.DoSomething();

    // Assert
    entity.Property.Should().Be(expectedValue);
}
```

### Unit Test - Command Handler
```csharp
public class CreateEntityCommandHandlerTests
{
    private readonly Mock<IRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly CreateEntityCommandHandler _handler;

    public CreateEntityCommandHandlerTests()
    {
        _mockRepository = new Mock<IRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _handler = new CreateEntityCommandHandler(
            _mockRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesEntity()
    {
        // Arrange
        var command = new CreateEntityCommand(...);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Entity>(), default))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<Entity>(), default),
            Times.Once);
    }
}
```

### Integration Test - API Endpoint
```csharp
public class ControllerTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public ControllerTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Endpoint_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateRequest(...);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/resource", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await response.Content.ReadFromJsonAsync<ResponseDto>();
        dto.Should().NotBeNull();
    }
}
```

## Common Assertions (FluentAssertions)

### Equality
```csharp
result.Should().Be(expected);
result.Should().NotBe(unexpected);
```

### Null Checks
```csharp
result.Should().NotBeNull();
result.Should().BeNull();
```

### Boolean
```csharp
result.Should().BeTrue();
result.Should().BeFalse();
```

### Collections
```csharp
list.Should().NotBeEmpty();
list.Should().HaveCount(5);
list.Should().Contain(item);
list.Should().ContainSingle(x => x.Id == id);
```

### Exceptions
```csharp
Action act = () => object.ThrowingMethod();
act.Should().Throw<InvalidOperationException>()
   .WithMessage("Expected error message");
```

### HTTP Status
```csharp
response.StatusCode.Should().Be(HttpStatusCode.OK);
response.StatusCode.Should().Be(HttpStatusCode.Created);
response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
```

## Mock Setup (Moq)

### Return Value
```csharp
mock.Setup(x => x.Method(It.IsAny<Type>()))
    .Returns(value);
```

### Return Async
```csharp
mock.Setup(x => x.MethodAsync(It.IsAny<Type>()))
    .ReturnsAsync(value);
```

### Return Task
```csharp
mock.Setup(x => x.MethodAsync(...))
    .Returns(Task.CompletedTask);
```

### Throw Exception
```csharp
mock.Setup(x => x.Method(...))
    .Throws<InvalidOperationException>();
```

### Verify Call
```csharp
mock.Verify(x => x.Method(It.IsAny<Type>()), Times.Once);
mock.Verify(x => x.Method(...), Times.Exactly(3));
mock.Verify(x => x.Method(...), Times.Never);
```

## Test Naming Convention

**Format**: `MethodName_Scenario_ExpectedBehavior`

**Examples**:
- `Constructor_ValidData_CreatesProduct` ✓
- `UpdateStock_NegativeQuantity_ThrowsException` ✓
- `CreateProduct_DuplicateSKU_ReturnsBadRequest` ✓
- `Test1` ❌ (Not descriptive)
- `TestCreateProduct` ❌ (Missing scenario)

## Project Structure

```
tests/
├── UnitTests/
│   └── Products.UnitTests/
│       ├── Domain/
│       │   └── ProductTests.cs
│       ├── Commands/
│       │   └── CreateProductCommandHandlerTests.cs
│       └── Queries/
│           └── GetProductsQueryHandlerTests.cs
└── IntegrationTests/
    └── Products.IntegrationTests/
        ├── ProductsApiFactory.cs
        └── ProductsControllerTests.cs
```

## Checklist for New Tests

- [ ] Test name follows convention
- [ ] Uses AAA pattern (Arrange-Act-Assert)
- [ ] Tests one behavior
- [ ] Has meaningful assertions
- [ ] External dependencies are mocked
- [ ] Test is independent (no shared state)
- [ ] Test is fast (< 100ms for unit tests)
- [ ] Edge cases covered
- [ ] Error scenarios tested

## When to Write What Type of Test

### Unit Test
✅ Domain entity behavior
✅ Value object validation
✅ Business logic
✅ Command/Query handlers
✅ Utility methods

### Integration Test
✅ API endpoints
✅ Database operations
✅ Authentication/Authorization
✅ Request/Response mapping

### E2E Test (Future)
✅ User workflows
✅ UI interactions
✅ Full system integration

## Common Mistakes to Avoid

❌ Testing implementation details
❌ Multiple assertions testing different things
❌ Test dependencies (tests depending on each other)
❌ Logic in tests (if/else, loops)
❌ Hardcoded dates/times
❌ Slow tests due to unnecessary operations
❌ Unclear test names
❌ Not cleaning up after tests

## Coverage Goals

| Component | Target |
|-----------|--------|
| Domain | 90% |
| Handlers | 80% |
| Controllers | 70% |
| Overall | 80% |

---

For detailed information, see [TESTING_STRATEGY.md](./TESTING_STRATEGY.md)

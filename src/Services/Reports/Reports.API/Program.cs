using Reports.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Reports.API")
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Reports API", Version = "v1" });
});

// Add HTTP clients for other microservices
builder.Services.AddHttpClient<SalesApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Sales"] ?? "http://localhost:5103");
});

builder.Services.AddHttpClient<InventoryApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Inventory"] ?? "http://localhost:5104");
});

builder.Services.AddHttpClient<CustomersApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Customers"] ?? "http://localhost:5105");
});

builder.Services.AddHttpClient<EmployeesApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ServiceUrls:Employees"] ?? "http://localhost:5106");
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reports API v1"));
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

try
{
    Log.Information("Starting Reports.API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Reports.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

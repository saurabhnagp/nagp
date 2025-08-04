using EmployeeApp.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOptions();

// Swashbuckle - Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddSwaggerGen();

builder.Logging.AddJsonConsole();

var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
var password = builder.Configuration["Secrets:DBConnectionStringPassword"];
var fullConnectionString = $"{connectionString}Password={password};";

builder.Services.AddDbContextPool<PostgreSqlContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine);
    }
    options.UseNpgsql(fullConnectionString);
});

builder.Services.AddScoped<IDataAccessProvider, DataAccessProvider>();
builder.Services.AddTransient<PostgreSqlContext, PostgreSqlContext>();

var app = builder.Build();

app.UseRouting();

// Swashbuckle
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee Service App V1");
});

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run(async (context) =>
{
    await context.Response.WriteAsync("K8s Employee Service App");
});

app.Run();

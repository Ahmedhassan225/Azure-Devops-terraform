using Application;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using WebAPI.HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
                .AddCheck<SqlConnectionHealthCheck>("SQLDBConnectionCheck");

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.UseInfrastructure();

app.MapControllers();
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
{

    ResponseWriter = HealthCheckExtensions.WriteResponse

}).AllowAnonymous();

app.Run();

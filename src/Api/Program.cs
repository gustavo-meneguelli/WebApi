using Api.Extensions; 
using Application.Validators;
using FluentValidation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Logs (Serilog)
builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/api-log-.txt", 
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information());

// Serviços Básicos
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddApplicationServices(builder.Configuration); 
builder.Services.AddSwaggerConfig();                            
builder.Services.AddJwtConfig(builder.Configuration);           

// FluentValidation, AutoMapper
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
builder.Services.AddAutoMapper(typeof(Application.Mappings.MappingProfile));

builder.Services.AddCorsConfig(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<Api.Middlewares.GlobalErrorHandlerMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("WebUiPolicy");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

//Check Seeder
await app.UseDbSeeder();

app.Run();

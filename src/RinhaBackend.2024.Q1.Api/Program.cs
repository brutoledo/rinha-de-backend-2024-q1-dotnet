using System.Net.Mime;
using RinhaBackend._2024.Q1.Api.Validations;
using RinhaBackend._2024.Q1.Application;
using RinhaBackend._2024.Q1.Core.Repositories;
using RinhaBackend._2024.Q1.Core.Services;
using RinhaBackend._2024.Q1.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>  
{  
    options.InvalidModelStateResponseFactory = context =>  
    {  
        var result = new ValidationFailedResult(context.ModelState);  
        result.ContentTypes.Add(MediaTypeNames.Application.Json);  
        return result;  
    };  
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBankService, BankService>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.MapGet("/", ctx => ctx.Response.WriteAsync("RinhaBackend.2024.Q1.Api"));

app.Run();

// expose the implicitly defined Program class to the test integrations project
public partial class Program { }
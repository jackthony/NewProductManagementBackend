using Microsoft.EntityFrameworkCore;
using ProductManagement.API.Middlewares;
using ProductManagement.Application.Services;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using ProductManagement.Application.Validators;



var builder = WebApplication.CreateBuilder(args);


//Configurar DbContext con SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite("Data Source=products.db");
});

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductRepository>();
builder.Services.AddScoped<CategoryRepository>();

// Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//agregar middleware de manejo de excepciones
app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

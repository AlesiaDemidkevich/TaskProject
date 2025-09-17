using C_Part1;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using TaskServiceApi.Db;
using TaskServiceApi.Decorators.TaskServiceApi.Decorators;
using TaskServiceApi.Infrastructure;
using TaskServiceApi.Mapping;
using TaskServiceApi.Messaging;
using TaskServiceApi.Repositories;
using TaskServiceApi.Services;
using TaskServiceApi.Validation;

var builder = WebApplication.CreateBuilder(args);

// Настройка Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Регистрация реального репозитория
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Оборачиваем репозиторий логированием
builder.Services.Decorate<ITaskRepository, LoggingTaskRepository>();

// Регистрация сервиса
builder.Services.AddScoped<TaskService>(); builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<TaskCreateDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TaskUpdateDtoValidator>();
builder.Services.AddSingleton<RabbitMqPublisher>(); // Singleton для RabbitMQ

builder.Services.AddAutoMapper(typeof(TaskMappingProfile).Assembly);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON to handle DateOnly
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStatusCodePages();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

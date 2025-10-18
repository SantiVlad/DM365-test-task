using FluentValidation;
using FluentValidation.AspNetCore;
using TaskManagement.API.DTOs;
using TaskManagement.API.Repositories;
using TaskManagement.API.Services;
using TaskManagement.API.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddScoped<IValidator<CreateTaskDto>, CreateTaskValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskDto>, UpdateTaskValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskStatusDto>, UpdateTaskStatusValidator>();

builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowVueApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
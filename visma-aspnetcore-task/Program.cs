using Microsoft.EntityFrameworkCore;
using visma_aspnetcore_task.Interfaces;
using visma_aspnetcore_task.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmployeeDatabase>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

//Add Services
builder.Services.AddScoped<IEmployeeServices, EmployeeServices>();

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

app.UseHttpsRedirection();

//Add Endpoints
app.MapEmployeeEndpoints();

app.Run();

using Microsoft.EntityFrameworkCore;
using Store.Filters;
using Store.Middlewares;
using Store.Models;
using Store.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// This option is to avoid circular references when using include in controllers
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ExceptionFilter));

}).AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var connectionString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<StoreContext>(options =>
    // Disable tracking
    options.UseSqlite(connectionString));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ActionsService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<IFileManagerService, FileManagerService>();
builder.Services.AddHostedService<ScheduledTaskService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.UseMiddleware<RegisterAndControlMiddleware>();

app.MapControllers();

// Middleware to access static files in the wwwroot folder
app.UseStaticFiles();

app.Run();

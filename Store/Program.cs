using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Store.Filters;
using Store.Middlewares;
using Store.Models;
using Store.Services;
using System.Text;
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
builder.Services.AddDataProtection();
builder.Services.AddTransient<HashService>();
builder.Services.AddTransient<TokenService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Configure security
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(builder.Configuration["JWTKey"]))
               });


// Setting up security in Swagger 
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
        "JWT Authentication Using Bearer Scheme. \r\n\r " +
        "Enter the word 'Bearer' followed by a space and the authentication token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
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

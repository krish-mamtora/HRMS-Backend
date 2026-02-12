using HRMS_Backend.Data;
using HRMS_Backend.Mapper;
using HRMS_Backend.Services;
using HRMS_Backend.Services.GameScheduling;
using HRMS_Backend.Services.JobListing;
using HRMS_Backend.Services.TravelandExpenses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy(name : "AllowdFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials(); ;
        }
    );
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<MyDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options=>
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,   
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["AppSettings:Issuer"],
        ValidAudience = builder.Configuration["AppSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!))
    });

builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ITravelPlanService, TravelPlanService>();
builder.Services.AddScoped<IEmployeeTravelService, EmployeeTravelService>();
//builder.Services.AddScoped<IGameConfigService, GameConfigService>();
builder.Services.AddScoped<IGamesService, GamesService>();
//builder.Services.AddScoped<IReferService, ReferService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseCors("AllowdFrontend");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

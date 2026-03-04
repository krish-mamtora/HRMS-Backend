using HRMS_Backend.Data;
using HRMS_Backend.Mapper;
using HRMS_Backend.Middleware;
using HRMS_Backend.Model;
using HRMS_Backend.Services;
using HRMS_Backend.Services.Achievements;
using HRMS_Backend.Services.Email;
using HRMS_Backend.Services.GameScheduling;
using HRMS_Backend.Services.JobListing;
using HRMS_Backend.Services.Quartz;
using HRMS_Backend.Services.ServiceUserProfile;
using HRMS_Backend.Services.TravelandExpenses;
//using HRMS_Backend.Services.User;
//using HRMS_Backend.Services.UserProfile;

//using HRMS_Backend.Services.User;

//using HRMS_Backend.Services.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Scalar.AspNetCore;
using System.Text;
//using PostInteractionService = HRMS_Backend.Services.Achievements.PostInteractionService;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddCors(options =>
{
    options.AddPolicy(name : "AllowdFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            builder.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithExposedHeaders("Content-Disposition"); 
            builder.WithOrigins("https://localhost:7035").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        }
    );
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("ExampleJob");
    q.AddJob<ExampleJob>(opts => opts.WithIdentity(jobKey));

    //q.AddTrigger(opts => opts
    //    .ForJob(jobKey)
    //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(15).RepeatForever())
    //);
    q.AddTrigger(opts => opts
      .ForJob(jobKey)
      .WithIdentity("ExampleJob-trigger")
      .StartNow()
      .WithSimpleSchedule(x => x
          .WithIntervalInHours(24) 
          .RepeatForever())
  );
});

// savepoint issue
//builder.Services.AddDbContext<MyDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
//           .ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS)));


builder.Services.AddControllers();
    
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
//builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddDbContext<MyDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

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
builder.Services.AddAuthorization(); 


builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ITravelPlanService, TravelPlanService>();
builder.Services.AddScoped<IEmployeeTravelService, EmployeeTravelService>();
builder.Services.AddScoped<IGameConfigService, GameConfigService>();
builder.Services.AddScoped<IGamesService, GamesService>();
builder.Services.AddScoped<IGameSlotService, GameSlotService>();
//builder.Services.AddScoped<IReferService, ReferService>();
//builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReferService, ReferService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddScoped<ITravelExpenseService, TravelExpenseService>();
builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<IShareEmailService, ShareEmailService>();
builder.Services.AddScoped<IExpenseProofService, ExpenseProofService>();
builder.Services.AddScoped<ITravelDocumentsService, TravelDocumentsService>();
builder.Services.AddScoped<IWaitingQueueService, WaitingQueueService>();
builder.Services.AddScoped<IGameCycleService, GameCycleService>();
builder.Services.AddScoped<IFairnessService, FairnessService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPostsService , PostsService>();
builder.Services.AddScoped<IEmployeeCycleStatsService, EmployeeCycleStatsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
//builder.Services.AddScoped<IPostInteractionService, PostInteractionService>();
//builder.Services.AddQuartzHostedService(options =>
//{
//    options.WaitForJobsToComplete = true;
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}
app.UseCors("AllowdFrontend");
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "AchievementImages")),
    RequestPath = "/content/achievements" 
});
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();

app.Run();

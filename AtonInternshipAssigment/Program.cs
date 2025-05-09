using AtonInternshipAssigment.Models;
using AtonInternshipAssigment.Repositories;
using AtonInternshipAssigment.Services;
using Microsoft.Data.SqlClient;
using System.Reflection;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<CheckUserAuthorization>();
builder.Services.AddScoped(_ => new SqlConnection(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddQuartz(q =>
{
    q.SchedulerId = "UserCleanupScheduler";

    q.AddJob<CleanupJob>(j => j
        .StoreDurably()
        .WithIdentity("CleanupJob")
    );

    q.AddTrigger(t => t
        .WithIdentity("CleanupTrigger")
        .ForJob("CleanupJob")
        .WithSimpleSchedule(s => s
            .WithInterval(TimeSpan.FromDays(3))
            .RepeatForever()
        )
        .StartNow()
    );
});

builder.Services.AddQuartzHostedService(q =>
{
    q.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

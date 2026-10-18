using RA_MuniAnnouncements.Application.Interfaces;
using RA_MuniAnnouncements.Application.Services;

using RA_MuniAnnouncements.Api.Extensions;
using RA_MuniAnnouncements.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure logging first
builder.AddApplicationLogging();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

// register repository / client implementations required by AnnouncementService
builder.Services.AddSingleton<ICmsClient, CmsClientMock>();
builder.Services.AddSingleton<ISettingsRepository, SettingsRepositoryMock>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware for exception handling and logging
app.UseApplicationMiddleware();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

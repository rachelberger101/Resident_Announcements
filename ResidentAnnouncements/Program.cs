using ResidentAnnouncements.Application.Interfaces;
using ResidentAnnouncements.Application.Services;
using ResidentAnnouncements.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();

// register repository / client implementations required by AnnouncementService
builder.Services.AddSingleton<ICmsClient, CmsClientMock>();
builder.Services.AddSingleton<ISettingsRepository, SettingsRepositoryMock>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

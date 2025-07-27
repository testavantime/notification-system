using NotificationService.Interfaces;
using NotificationService.Models;
using NotificationService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Configure notification settings
builder.Services.Configure<NotificationConfig>(
    builder.Configuration.GetSection("NotificationConfig"));

// Register configuration objects
builder.Services.AddSingleton<NotificationConfig>(provider =>
{
    var config = builder.Configuration.GetSection("NotificationConfig").Get<NotificationConfig>() ?? new NotificationConfig();
    return config;
});

builder.Services.AddSingleton<EmailConfig>(provider =>
{
    var config = builder.Configuration.GetSection("NotificationConfig:Email").Get<EmailConfig>() ?? new EmailConfig();
    return config;
});

builder.Services.AddSingleton<SmsConfig>(provider =>
{
    var config = builder.Configuration.GetSection("NotificationConfig:Sms").Get<SmsConfig>() ?? new SmsConfig();
    return config;
});

// Register services
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<SmsService>();
builder.Services.AddScoped<INotificationService, NotificationService.Services.NotificationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

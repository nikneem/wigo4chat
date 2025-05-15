using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDaprClient();

// Register application services
builder.Services.AddTransient<Wigo4it.Chat.UsersService.IUserService, Wigo4it.Chat.UsersService.UserService>();
builder.Services.AddTransient<Wigo4it.Chat.ChatService.IChatService, Wigo4it.Chat.ChatService.ChatService>();

// Add SignalR for real-time chat
builder.Services.AddSignalR();

// Add CORS for frontend applications
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add OpenAPI/Swagger documentation
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/scalar/v1", opts =>
    {
        opts.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// Use CORS
app.UseCors("AllowAll");

// Map SignalR hub
app.MapHub<Wigo4it.Chat.Api.Hubs.ChatHub>("/chathub");

app.Run();

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Wigo4it.Chat.Client;
using Wigo4it.Chat.Client.OllamaClient.Services;
using Wigo4it.Chat.Core.Models;

var hostBuilder = Host.CreateDefaultBuilder(args);

hostBuilder.ConfigureAppConfiguration((context, config) =>
{
    config.AddJsonFile("appsettings.json", optional: false);
    config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
});

hostBuilder.ConfigureServices((context, services) =>
{
    // Add logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
    });
    
    // Configure options
    services.Configure<OllamaClientOptions>(options => 
    {
        context.Configuration.GetSection("ChatClient").Bind(options);
        // Set hub URL based on the service endpoint
        var serviceEndpoint = options.ChatServiceEndpoint;
        if (!string.IsNullOrEmpty(serviceEndpoint))
        {
            options.BaseUrl = serviceEndpoint;
            options.HubUrl = $"{serviceEndpoint}/hubs/chat";
        }
    });
    
    // Add chat client services
    services.AddWigo4itChatClient(options =>
    {
        var serviceEndpoint = context.Configuration.GetSection("ChatClient:ChatServiceEndpoint").Value;
        if (!string.IsNullOrEmpty(serviceEndpoint))
        {
            options.BaseUrl = serviceEndpoint;
            options.HubUrl = $"{serviceEndpoint}/hubs/chat";
        }
    });
    
    // Add Ollama service
    services.Configure<OllamaSettings>(context.Configuration.GetSection("OllamaSettings"));
    services.AddHttpClient<IOllamaService, OllamaService>();
    
    // Add message handler
    services.AddSingleton<IChatMessageHandler, ChatMessageHandler>();
    
    // Add hosted service
    services.AddHostedService<OllamaClientHostedService>();
});

// Build and run the host
var host = hostBuilder.Build();
await host.RunAsync();

// Hosted service that handles connecting to the chat hub and processing messages
public class OllamaClientHostedService : BackgroundService
{
    private readonly IChatClient _chatClient;
    private readonly IChatHubClient _chatHubClient;
    private readonly IChatMessageHandler _messageHandler;
    private readonly ILogger<OllamaClientHostedService> _logger;
    private readonly OllamaClientOptions _options;

    public OllamaClientHostedService(
        IChatClient chatClient,
        IChatHubClient chatHubClient,
        IChatMessageHandler messageHandler,
        ILogger<OllamaClientHostedService> logger,
        IOptions<OllamaClientOptions> options)
    {
        _chatClient = chatClient;
        _chatHubClient = chatHubClient;
        _messageHandler = messageHandler;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Ollama Chat Client starting...");
            
            // Set up SignalR hub connection to receive messages
            _chatHubClient.OnMessageReceived += async (ChatMessage message) =>
            {
                await _messageHandler.HandleMessageAsync(message);
            };
            
            // Connect to the chat service
            await _chatHubClient.ConnectAsync();
            _logger.LogInformation("Connected to chat hub");
            
            // Send initial message to the chat
            await _chatClient.SendMessageAsync(
                Guid.Parse(_options.UserId), 
                "Hello! I'm the Ollama AI assistant. Type a message starting with 'ollama' followed by your question, and I'll respond with an AI-generated answer."
            );
            
            // Keep the service running until cancelled
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Ollama client service");
            
            // Rethrow if not due to cancellation
            if (!stoppingToken.IsCancellationRequested)
            {
                throw;
            }
        }
    }
    
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Ollama Chat Client stopping...");
        await _chatHubClient.DisconnectAsync();
        await base.StopAsync(stoppingToken);
    }
}

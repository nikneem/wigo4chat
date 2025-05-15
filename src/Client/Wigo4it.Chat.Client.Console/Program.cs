using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Wigo4it.Chat.Client;
using Wigo4it.Chat.Core.Models;

// Build configuration
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
        config.AddCommandLine(args);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Error);
            builder.AddConsole();
        });

        // Add chat client services
        services.AddWigo4itChatClient(options =>
        {
            options.BaseUrl = context.Configuration.GetValue<string>("ChatService:BaseUrl") ?? "https://wigo4chat-app.wittyocean-892fd4b9.northeurope.azurecontainerapps.io/";
            options.HubUrl = context.Configuration.GetValue<string>("ChatService:HubUrl") ?? "https://wigo4chat-app.wittyocean-892fd4b9.northeurope.azurecontainerapps.io//chathub";
            //options.BaseUrl = context.Configuration.GetValue<string>("ChatService:BaseUrl") ?? "https://localhost:7109";
            //options.HubUrl = context.Configuration.GetValue<string>("ChatService:HubUrl") ?? "https://localhost:7109/chathub";
        });
    })
    .Build();

// Get services
var chatClient = host.Services.GetRequiredService<IChatClient>();
var hubClient = host.Services.GetRequiredService<IChatHubClient>();

// Chat state
ChatUser? currentUser = null;
bool isRunning = true;
var messageHistory = new List<ChatMessage>();

// Set up console colors
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("==============================================");
Console.WriteLine("       Wigo4it Chat Console Client           ");
Console.WriteLine("==============================================");
Console.ResetColor();
Console.WriteLine("Type /help for available commands");
Console.WriteLine();

// Set up hub client event handlers
hubClient.OnMessageReceived += HandleMessageReceived;
hubClient.OnUserJoined += HandleUserJoined;
hubClient.OnUserLeft += HandleUserLeft;

// Main application loop
try
{
    while (isRunning)
    {
        if (currentUser == null)
        {
            // User needs to join chat
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your display name to join chat: ");
            Console.ResetColor();

            var displayName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(displayName))
            {
                continue;
            }

            try
            {
                var joinResult = await chatClient.JoinChatAsync(displayName);
                currentUser = new ChatUser { Id = joinResult.UserId, DisplayName = displayName };

                // Connect to the hub for real-time updates
                await hubClient.ConnectAsync();

                // Load chat history
                var history = await chatClient.GetChatHistoryAsync();
                messageHistory.AddRange(history);

                // Display welcome and history
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Welcome to the chat, {displayName}!");
                Console.WriteLine($"Your user ID is: {currentUser.Id}");
                Console.WriteLine("==============================================");
                Console.ResetColor();

                // Display chat history
                foreach (var msg in messageHistory)
                {
                    DisplayMessage(msg);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to join chat: {ex.Message}");
                Console.ResetColor();
            }
        }
        else
        {
            // User is in chat - get input
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("> ");
            Console.ResetColor();

            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            // Process commands
            if (input.StartsWith("/"))
            {
                await ProcessCommand(input);
            }
            else
            {
                // Regular message
                try
                {
                    var sentMessage = await chatClient.SendMessageAsync(currentUser.Id, input);

                    // Keep user active
                    await chatClient.PingUserAsync(currentUser.Id);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to send message: {ex.Message}");
                    Console.ResetColor();
                }
            }
        }
    }
}
finally
{
    // Ensure we disconnect from the hub
    if (hubClient != null)
    {
        await hubClient.DisconnectAsync();
    }
}

// Command processor
async Task ProcessCommand(string command)
{
    var normalizedCommand = command.ToLowerInvariant();

    switch (normalizedCommand)
    {
        case "/help":
            DisplayHelp();
            break;

        case "/exit":
        case "/quit":
            isRunning = false;
            break;

        case "/clear":
            Console.Clear();
            break;

        case "/history":
            // Reload and display chat history
            try
            {
                var history = await chatClient.GetChatHistoryAsync();
                Console.Clear();
                foreach (var msg in history)
                {
                    DisplayMessage(msg);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to load history: {ex.Message}");
                Console.ResetColor();
            }
            break;

        case "/whoami":
            if (currentUser != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"You are: {currentUser.DisplayName} (ID: {currentUser.Id})");
                Console.ResetColor();
            }
            break;

        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unknown command: {command}");
            Console.WriteLine("Type /help for available commands");
            Console.ResetColor();
            break;
    }
}

// Display help information
void DisplayHelp()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Available commands:");
    Console.WriteLine("  /help     - Display this help message");
    Console.WriteLine("  /exit     - Exit the application");
    Console.WriteLine("  /quit     - Exit the application");
    Console.WriteLine("  /clear    - Clear the console");
    Console.WriteLine("  /history  - Reload and display chat history");
    Console.WriteLine("  /whoami   - Display your user information");
    Console.WriteLine("  [message] - Send a message to the chat");
    Console.ResetColor();
}

// Event handlers for real-time events
void HandleMessageReceived(ChatMessage message)
{
    if (message != null)
    {
        if (!messageHistory.Any(m => m.Id == message.Id))
        {
            messageHistory.Add(message);
        }

        // Make sure we don't interrupt user input
        if (message.SenderId != currentUser?.Id)
        {
            int cursorLeft = Console.CursorLeft;
            int cursorTop = Console.CursorTop;

            Console.WriteLine();
            DisplayMessage(message);

            // Restore cursor to input position
            Console.Write("> ");
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }
    }
}

void HandleUserJoined(ChatUser user)
{
    if (user != null && user.Id != currentUser?.Id)
    {
        int cursorLeft = Console.CursorLeft;
        int cursorTop = Console.CursorTop;

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[SYSTEM] {user.DisplayName} has joined the chat");
        Console.ResetColor();

        // Restore cursor to input position
        Console.Write("> ");
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }
}

void HandleUserLeft(Guid userId)
{
    // We don't have the display name of the user who left
    // But we can check if they sent any messages in our history
    var username = messageHistory
        .Where(m => m.SenderId == userId)
        .Select(m => m.SenderName)
        .FirstOrDefault() ?? "Someone";

    if (userId != currentUser?.Id)
    {
        int cursorLeft = Console.CursorLeft;
        int cursorTop = Console.CursorTop;

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[SYSTEM] {username} has left the chat");
        Console.ResetColor();

        // Restore cursor to input position
        Console.Write("> ");
        Console.SetCursorPosition(cursorLeft, cursorTop);
    }
}

// Helper to display a formatted message
void DisplayMessage(ChatMessage message)
{
    var timestamp = message.SentAt.ToLocalTime().ToString("HH:mm:ss");

    if (message.SenderId == currentUser?.Id)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"[{timestamp}] You: ");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"[{timestamp}] {message.SenderName}: ");
    }

    Console.ResetColor();
    Console.WriteLine(message.Message);
}

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Wigo4it.Chat.Core.Models;

namespace Wigo4it.Chat.Client
{
    /// <summary>
    /// Implementation of IChatHubClient for real-time chat communication using SignalR
    /// </summary>
    public class ChatHubClient : IChatHubClient, IAsyncDisposable
    {
        private HubConnection? _hubConnection;
        private readonly ChatClientOptions _options;
        private bool _isConnected;

        public event Action<ChatMessage>? OnMessageReceived;
        public event Action<ChatUser>? OnUserJoined;
        public event Action<Guid>? OnUserLeft;

        public ChatHubClient(IOptions<ChatClientOptions> options)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task ConnectAsync()
        {
            if (_isConnected)
            {
                return;
            }

            if (string.IsNullOrEmpty(_options.HubUrl))
            {
                throw new InvalidOperationException("Hub URL is not configured");
            }

            _hubConnection = new HubConnectionBuilder()  
                .WithUrl(_options.HubUrl)
                .WithAutomaticReconnect()
                .Build();

            // Register event handlers
            _hubConnection.On<ChatMessage>("ReceiveMessage", message =>
            {
                OnMessageReceived?.Invoke(message);
            });

            _hubConnection.On<ChatUser>("UserJoined", user =>
            {
                OnUserJoined?.Invoke(user);
            });

            _hubConnection.On<Guid>("UserLeft", userId =>
            {
                OnUserLeft?.Invoke(userId);
            });

            await _hubConnection.StartAsync();
            _isConnected = true;
        }

        /// <inheritdoc />
        public async Task DisconnectAsync()
        {
            if (_hubConnection != null && _isConnected)
            {
                await _hubConnection.StopAsync();
                _isConnected = false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection != null)
            {
                await DisconnectAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
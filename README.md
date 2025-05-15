# Wigo4it Chat Application

A real-time chat application built with ASP.NET Core, .NET Aspire, and Dapr.

## Overview

This chat application allows users to:

- Join the chat with a display name
- Send and receive messages in real-time
- View chat history

## Architecture

- **ASP.NET Web API**: Provides REST endpoints for joining chat, sending messages, and retrieving chat history
- **.NET Aspire**: Handles service orchestration and configuration
- **Dapr**: Provides building blocks for state management and pub/sub communications
- **SignalR**: Enables real-time communication between clients and server

## Project Structure

```
/src
  /Aspire               - Contains .NET Aspire projects for orchestration
  /Chat
    /Wigo4it.Chat.Api   - ASP.NET Web API project with controllers and SignalR hub
    /Wigo4it.Chat.ChatService - Service for chat message handling
    /Wigo4it.Chat.UsersService - Service for user management
  /Shared
    /Wigo4it.Chat.Core  - Shared models and DTOs
/dapr                   - Dapr component configuration files
```

## API Endpoints

### Users

- `POST /api/users/join` - Join the chat with a display name

  - Request: `{ "displayName": "string" }`
  - Response: `{ "userId": "guid", "displayName": "string" }`

- `GET /api/users/{userId}` - Get user details

  - Response: `{ "id": "guid", "displayName": "string", "lastSeen": "datetime" }`

- `POST /api/users/{userId}/ping` - Update user's last seen timestamp
  - Response: `200 OK`

### Chat

- `GET /api/chat/history` - Get chat history

  - Response: Array of chat messages

- `POST /api/chat/message` - Send a message
  - Request: `{ "userId": "guid", "message": "string" }`
  - Response: The sent message

## Real-time Updates

The application uses SignalR for real-time updates. Connect to the `/chathub` endpoint to receive messages in real-time.

## How to Run

### Prerequisites

- .NET 9 SDK
- Docker

### Setup

1. Clone the repository
2. Navigate to the solution directory
3. Run the application using the .NET Aspire dashboard:

```bash
dotnet run --project src/Aspire/Wigo4it.Chat.Aspire/Wigo4it.Chat.Aspire.AppHost/Wigo4it.Chat.Aspire.AppHost.csproj
```

This will:

- Start the Aspire dashboard
- Set up Redis container for state management and pub/sub
- Launch the API with Dapr sidecar
- Configure all required components

## Technical Details

### State Management

User information and chat history are stored in Redis via Dapr's state management building block. User data has a 15-minute sliding expiration.

### Pub/Sub

Real-time message delivery is facilitated using Dapr's pub/sub building block with Redis as the backend.

### Service Lifetime

- Users are automatically removed after 15 minutes of inactivity
- Chat history is retained for the most recent 50 messages

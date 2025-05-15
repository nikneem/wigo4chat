# Project requirements

This following table outlines the detailed function requirements for Wigo4Chat. This is an ASP.NET Rest API written in C# that functions as a chat server.

| Requirement ID | Description | User story | Expected Behavior/Outcome |
| :------------- | :--------------------------- | :--------------------------------- | :------------------ |
| FR001 | New user enters the chat | As a new user, I want to be able to enter the chat | The API should expose a HTTP Post endpoint where users can pass a DTO to, containing their display name. This will generate a Guid and register the Guid and display name in storage with a sliding expiration of 15 minutes |
| FR002 | New user entered the chat | When a user entered the chat, a short chat history must be shown | The API must expose an endpoint where users who just entered the chat, can retrieve the latest sent messages. This list contains information about the sender, the message and the date / time the message was sent |
| FR003 | Sending a chat message | As a user, I want to send a message to the chat | A message with the user Guid and a message arrives on the server. The server updates a collection of chat messages in the state store. The state store only keeps the 25 latest messages. All older messages will be dropped. The send message is now broadcasted to all chat users using SignalR |

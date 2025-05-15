using System.Collections.Immutable;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDapr();
var options = new DaprSidecarOptions
{
    ResourcesPaths = ImmutableHashSet.Create(Directory.GetCurrentDirectory() + "/../../../../dapr")
};

// Add our API project with Dapr sidecar
var api = builder.AddProject<Projects.Wigo4it_Chat_Api>("wigo4it-chat-api")
    .WithDaprSidecar(options);

// Add Ollama container service
//var ollama = builder.AddContainer("ollama", "ollama/ollama")
//    .WithHttpEndpoint(11434, 11434, name: "api")
//    .WithHttpEndpoint(11434, name: "ollama-api");

// Add Ollama client project
builder.AddProject("wigo4it-chat-ollama-client", "../../../Client/Wigo4it.Chat.Client.OllamaClient/Wigo4it.Chat.Client.OllamaClient.csproj")
    .WithReference(api);

builder.Build().Run();

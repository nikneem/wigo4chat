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

builder.Build().Run();

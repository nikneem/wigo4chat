var builder = DistributedApplication.CreateBuilder(args);



// Add our API project with Dapr sidecar
var api = builder.AddProject<Projects.Wigo4it_Chat_Api>("wigo4it-chat-api")
    .WithDaprSidecar();

builder.Build().Run();

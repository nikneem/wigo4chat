var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Wigo4it_Chat_Api>("wigo4it-chat-api");

builder.Build().Run();

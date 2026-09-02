var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.CareLoop_Api>("api");

builder.AddProject<Projects.CareLoop_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();

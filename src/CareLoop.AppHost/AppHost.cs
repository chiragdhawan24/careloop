var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var careLoopDb = postgres.AddDatabase("careloopdb");

var api = builder.AddProject<Projects.CareLoop_Api>("api")
    .WithReference(careLoopDb)
    .WaitFor(careLoopDb);

// builder.AddProject<Projects.CareLoop_Web>("web")
//     .WithReference(api)
//     .WaitFor(api);

builder.AddViteApp(
        "web",
        "../CareLoop.Web")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment(
        "VITE_API_BASE_URL",
        api.GetEndpoint("https"))
    .WithExternalHttpEndpoints();

builder.Build().Run();
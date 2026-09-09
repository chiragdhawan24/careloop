var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var careLoopDb = postgres.AddDatabase("careloopdb");

var api = builder.AddProject<Projects.CareLoop_Api>("api")
    .WithReference(careLoopDb)
    .WaitFor(careLoopDb);

builder.AddProject<Projects.CareLoop_Web>("web")
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
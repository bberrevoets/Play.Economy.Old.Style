using Projects;

using System.Net.Sockets;
using System.Net;

var builder = DistributedApplication.CreateBuilder(args);

var managementPort = GetRandomUnusedPort();

#region [ Starting up some docker containers ]

var mongodb = builder.AddMongoDB("mongodbplay")
    .WithHttpEndpoint(27017, 27017, "mongodb")
    .WithDataVolume("mongo_play")
    .WithMongoExpress();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin(managementPort)
    .WithDataVolume("rabbitmq_play");

#endregion

#region [ Adding my microservices ]]

var catalogService = builder.AddProject<Play_Catalog_Service>("play-catalog-service")
    .WaitFor(mongodb)
    .WaitFor(rabbitmq)
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq);

var inventoryService = builder.AddProject<Play_Inventory_Service>("play-inventory-service")
    .WaitFor(mongodb)
    .WaitFor(rabbitmq)
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq);

builder.AddNpmApp("Frontend", "../Play.Economy.Frontend/")
    .WaitFor(catalogService)
    .WithEnvironment("REACT_APP_RABBITMQ_URL", $"http://localhost:{managementPort.ToString()}")
    .WithHttpEndpoint(env: "PORT", port: 3001)
    .PublishAsDockerFile();

#endregion

builder.Build().Run();

return;

static int GetRandomUnusedPort()
{
    var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();
    return port;
}
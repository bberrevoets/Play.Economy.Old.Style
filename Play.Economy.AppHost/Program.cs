using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region [ Starting up some docker containers ]

builder.AddMongoDB("mongodbplay")
    .WithHttpEndpoint(27017, 27017, "mongodb")
    .WithDataVolume("mongo_play")
    .WithMongoExpress();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithDataVolume("rabbitmq_play");

#endregion

#region [ Adding my microservices ]]

builder.AddProject<Play_Catalog_Service>("play-catalog-service")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq);

builder.AddProject<Play_Inventory_Service>("play-inventory-service")
    .WithExternalHttpEndpoints()
    .WithReference(rabbitmq);

#endregion

builder.Build().Run();
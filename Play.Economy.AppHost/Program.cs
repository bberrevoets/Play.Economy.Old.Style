using Projects;

var builder = DistributedApplication.CreateBuilder(args);

#region [ Starting up some docker containers ]

builder.AddMongoDB("mongodbplay")
    .WithHttpEndpoint(port: 27017, targetPort: 27017, "mongodb")
    .WithDataVolume("mongo_play")
    .WithMongoExpress();

#endregion

#region [ Adding my microservices ]]

builder.AddProject<Play_Catalog_Service>("play-catalog-service")
    .WithExternalHttpEndpoints();

//builder.AddProject<Play_Inventory_Service>("play-inventory-service")
//    .WithExternalHttpEndpoints();

#endregion

builder.Build().Run();
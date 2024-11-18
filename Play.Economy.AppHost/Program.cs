using Projects;

var builder = DistributedApplication.CreateBuilder(args);

//builder.AddContainer("seq", "datalust/seq")
//    .WithHttpEndpoint(port: 8081,targetPort:80, "Frontend")
//    .WithHttpEndpoint(port:5341, targetPort:5341, "Backend")
//    .WithEnvironment("ACCEPT_EULA","Y");

builder.AddProject<Play_Catalog_Service>("play-catalog-service")
    .WithExternalHttpEndpoints();

builder.AddProject<Play_Inventory_Service>("play-inventory-service")
    .WithExternalHttpEndpoints();

builder.Build().Run();
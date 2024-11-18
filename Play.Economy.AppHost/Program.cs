var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Play_Catalog_Service>("play-catalog-service");

builder.AddProject<Projects.Play_Inventory_Service>("play-inventory-service");

builder.Build().Run();

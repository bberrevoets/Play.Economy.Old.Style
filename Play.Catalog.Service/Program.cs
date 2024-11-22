using System;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Play.Catalog.Service;
using Play.Catalog.Service.Entities;
using Play.Common.MongoDb;
using Play.Common.Settings;

var rabbitmqConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__rabbitmq");

var builder = WebApplication.CreateBuilder(args);

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo()
    .AddMonoRepository<Item>("items");

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitMqSettings =
            builder.Configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

        // I am not using the settings of the RabbitMqSettings, because aspire injected an environment variable of the correct connection string.
        cfg.Host(rabbitmqConnectionString);
        cfg.ConfigureEndpoints(ctx, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
    });
});

builder.Services.AddMassTransitHostedService();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost") || origin.StartsWith("https://localhost"))
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.AddServiceDefaults();

builder.Services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowLocalhost");

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Play.Catalog.Service"); });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
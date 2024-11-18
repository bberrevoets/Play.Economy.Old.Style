using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddControllers();

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
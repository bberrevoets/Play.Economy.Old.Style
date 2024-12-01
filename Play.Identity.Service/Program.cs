using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Play.Common.Settings;
using Play.Identity.Service.Entities;

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

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
var mongodbSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
        mongodbSettings.ConnectionString,
        serviceSettings.ServiceName
    );

builder.Services.AddControllers(options => { options.SuppressAsyncSuffixInActionNames = false; });
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowLocalhost");

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "Play.Identity.Service"); });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.Run();
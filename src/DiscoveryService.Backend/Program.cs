using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.ShortSchemaNames = true;
    o.DocumentSettings = s =>
    {
        s.DocumentName = "v1";
        s.Title = "DiscoveryService";
        s.Version = "v1";
    };
});

var app = builder.Build();

app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

public partial class Program;

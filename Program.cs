using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using NSwag;
using NSwag.Generation.Processors.Security;
using SegundaPracticaAzure.Data;
using SegundaPracticaAzure.Helpers;
using SegundaPracticaAzure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddTransient<RepositoryCubos>();

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

SecretClient secretClient =
    builder.Services.BuildServiceProvider().GetService<SecretClient>();

KeyVaultSecret secret =
    await secretClient.GetSecretAsync("SqlAzure");
builder.Services.AddDbContext<CubosContext>
    (options => options.UseSqlServer(secret.Value));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Cubos";
    document.Description = "Api Cubos para el segundo examen de azure";
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
        new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

HelperToken helper = new HelperToken(secretClient);
builder.Services.AddAuthentication(helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());
builder.Services.AddTransient<HelperToken>(x => helper);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint(
        url: "/swagger/v1/swagger.json",
        name: "Api Cubos");
    options.RoutePrefix = "";
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

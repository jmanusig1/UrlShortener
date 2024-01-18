using System.Reflection;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.OpenApi.Models;
using ReferralPartnerServices.Domain.UseCases;
using UrlShortener.Domain.Interfaces;
using UrlShortener.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

//assume that eventually this will be in some sort of ENV var but hardcoded for now.
string cosmosConnectionString = "AccountEndpoint=https://url-shortener-jared.documents.azure.com:443/;AccountKey=FZiy5mY9V5sgM98CpKTNY01dqWpbNRpgKyLAp2N04GhCVx0yTN26OVu7swQgNcVNJwZIbMsiekxRACDbcImHDg==;";
string cosmosDatabase = "ShortenedUrlDB";

//hook in cosmosDB
builder.Services.AddScoped<ICosmosRepository, CosmosRepository>();
builder.Services.AddSingleton<CosmosClient>(c => new CosmosClient(cosmosConnectionString));
builder.Services.AddSingleton<Database>(d => new CosmosClient(cosmosConnectionString).GetDatabase(cosmosDatabase));


builder.Services.AddSwaggerGen(options =>
{
    //TODO: improve the swagger doc
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

builder.Services.AddScoped<IRequestHandler<ShortenUrlMessage, string>, ShortenUrl>();
builder.Services.AddScoped<IRequestHandler<GetUrlMessage, string>, GetUrl>();
builder.Services.AddScoped<IShortUrlRepository, ShortUrlRepository>();


builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.MapControllers();
app.UseAuthorization();
app.UseRouting();
app.UseCors("corsapp");

app.Run(); 
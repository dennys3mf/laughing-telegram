using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using BookStoreApi.Models;
using BookStoreApi.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Redis
var redisConfiguration = new ConfigurationOptions
{
    EndPoints = { "localhost:6379" },
    AbortOnConnectFail = false
};
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConfiguration));

// Configuración de BookStoreDatabaseSettings
builder.Services.Configure<BookStoreDatabaseSettings>(
    builder.Configuration.GetSection("BookStoreDatabase"));

// Registro de servicios
builder.Services.AddSingleton<GenerosService>();
builder.Services.AddSingleton<PeliculasService>();
builder.Services.AddSingleton<MoviesRatingService>();
builder.Services.AddSingleton<RedisService>();

// Configuración de opciones de serialización JSON
builder.Services.AddMvc()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("appRecommender", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:8081");
        policyBuilder.AllowAnyHeader();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowCredentials();
    });
});

var app = builder.Build();

// Llama a TransferMoviesRatingToMongoDB al iniciar la aplicación
var redisService = app.Services.GetRequiredService<RedisService>();
await redisService.TransferMoviesRatingToMongoDB();

// Configuración del pipeline de solicitud HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("appRecommender");

app.MapControllers();

app.Run();

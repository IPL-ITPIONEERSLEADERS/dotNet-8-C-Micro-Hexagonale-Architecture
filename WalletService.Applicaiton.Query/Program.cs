using EventStore.Client;
using WalletService.Application.Query.Handlers;
using WalletService.Infrastructure.EventStore.Contracts;
using WalletService.Infrastructure.EventStore.Impl;
using WalletService.Infrastructure.Projection.Contracts;
using WalletService.Infrastructure.Projection.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EventStoreDB
var esdb = new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"));
builder.Services.AddSingleton(esdb);
builder.Services.AddSingleton<IEventStore>(sp => new EventStoreDBEventStore(esdb));

// PostgreSQL Projection
var connString = "Host=localhost;Database=wallet_projection;Username=postgres;Password=postgres";
builder.Services.AddSingleton<IWalletProjection>(sp => new PostgreSqlWalletProjection(connString));

// Handlers
builder.Services.AddSingleton<GetWalletBalanceQueryHandler>();

// Background service
builder.Services.AddHostedService<ProjectionBackgroundService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();

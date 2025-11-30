using EventStore.Client;
using WalletService.Application.Command.Handlers;
using WalletService.Infrastructure.EventStore.Contracts;
using WalletService.Infrastructure.EventStore.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EventStoreDB
var esdb = new EventStoreClient(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"));
builder.Services.AddSingleton(esdb);
builder.Services.AddSingleton<IEventStore>(sp => new EventStoreDBEventStore(esdb));

// Handlers
builder.Services.AddSingleton<CreateWalletCommandHandler>();


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

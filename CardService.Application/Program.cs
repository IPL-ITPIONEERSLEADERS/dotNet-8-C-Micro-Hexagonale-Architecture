using CardService.Infrastructure.Contracts;
using CardService.Infrastructure.Impl;
using EventStore.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// EventStoreDB
var esdbSettings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
builder.Services.AddSingleton(new EventStoreClient(esdbSettings));
builder.Services.AddSingleton<IEventStore, EventStoreDBEventStore>();

// RabbitMQ
builder.Services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

// Repository
builder.Services.AddSingleton<ICardRepository, CardRepository>();



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

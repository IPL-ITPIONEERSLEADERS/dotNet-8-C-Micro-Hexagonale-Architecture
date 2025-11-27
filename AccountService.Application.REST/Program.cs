using AccountService.ES.Contracts;
using AccountService.ES.Impl;

var builder = WebApplication.CreateBuilder(args);


// Services métier
builder.Services.AddSingleton<IEventStore, InMemoryEventStore>();
builder.Services.AddSingleton<IAccountRepository, AccountRepository>();

// Add services to the container.

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

// Écouter sur deux ports :
// - REST : http://localhost:5000
// - gRPC : http://localhost:5001
// app.Urls.Add("http://localhost:5000");
// app.Urls.Add("http://localhost:5001");

app.Run();

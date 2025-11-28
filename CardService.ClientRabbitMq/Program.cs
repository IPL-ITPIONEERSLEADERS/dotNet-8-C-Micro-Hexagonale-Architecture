// See https://aka.ms/new-console-template for more information

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

Console.WriteLine("Hello, World!");


const string HostName = "localhost";
const string UserName = "guest";
const string Password = "guest";
const string ExchangeName = "card.events"; // ← doit correspondre à votre microservice

var factory = new ConnectionFactory
{
    HostName = HostName,
    UserName = UserName,
    Password = Password
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

// 1. Déclarer l'exchange avec le même type que le publisher (Topic)
channel.ExchangeDeclare(
    exchange: ExchangeName,
    type: ExchangeType.Topic, // ← crucial : doit être Topic
    durable: true
);

// 2. Créer une queue temporaire
var queueName = channel.QueueDeclare(
    queue: "",
    durable: false,
    exclusive: true,
    autoDelete: true
).QueueName;

// 3. Lier la queue avec un pattern qui capture TOUT sous "card.*"
channel.QueueBind(
    queue: queueName,
    exchange: ExchangeName,
    routingKey: "card.*" // ← pattern Topic pour tout intercepter
);

Console.WriteLine($"✅ Écoute de tous les événements sur l'exchange '{ExchangeName}' avec routingKey 'card.*'");
Console.WriteLine("Appuyez sur [ENTRÉE] pour arrêter.\n");

// 4. Consommer
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = ea.RoutingKey;

    // Optionnel : désérialiser comme JSON structuré
    try
    {
        using var doc = JsonDocument.Parse(message);
        var pretty = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine($"📩 [{DateTime.Now:HH:mm:ss.fff}] RoutingKey: {routingKey}\n{pretty}\n");
    }
    catch
    {
        // Si ce n'est pas du JSON valide
        Console.WriteLine($"📩 [{DateTime.Now:HH:mm:ss.fff}] RoutingKey: {routingKey}\n{message}\n");
    }
};

channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

Console.ReadLine(); // Garde le programme actif




using RabbitMQ;

var options = new RabbitOptions
{
    Hosts = new[] { "rabbitmq-pc1" }
};

var bus = new RabbitBus(options);
await bus.Connect();

Console.WriteLine("Consumer started");

// REAL-TIME
bus.ConsumeAsync<dynamic>(options.HotQueue, (msg, isRealtime) =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[REALTIME] {msg.TimestampUtc:o} {msg.Payload}");
    Console.ResetColor();
});

// OLD DATA
bus.ConsumeAsync<dynamic>(options.ColdQueue, (msg, isRealtime) =>
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"[OLD DATA] {msg.TimestampUtc:o} {msg.Payload}");
    Console.ResetColor();
});

Console.ReadLine();
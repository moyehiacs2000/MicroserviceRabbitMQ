using RabbitMQ.Client;

var factory = new ConnectionFactory()
{
    HostName = "rabbitmq-pc1", // Can also use rabbitmq-pc2
    Port = 5672,
    UserName = "module",
    Password = "module_password",
    AutomaticRecoveryEnabled = true
};
// Version 7.x uses async/await pattern
await using var connection = await factory.CreateConnectionAsync(new[] { "rabbitmq-pc1", "rabbitmq-pc2" });
await using var channel = await connection.CreateChannelAsync();

// Quorum queue arguments
var argss = new Dictionary<string, object>
{
    { "x-queue-type", "quorum" }
};

// Create sensor-data-queue
await channel.QueueDeclareAsync(
    queue: "sensor-data-queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: argss
);

Console.WriteLine("✓ Created sensor-data-queue (quorum)");

// Create offshore-data-queue
await channel.QueueDeclareAsync(
    queue: "offshore-data-queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: argss
);
Console.WriteLine("✓ Created offshore-data-queue (quorum)");

Console.WriteLine("\n✓ RabbitMQ setup complete!");
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory()
{
    UserName = "module",
    Password = "module_password"
};

//// Publish to PC1
//await using (var conn = await factory.CreateConnectionAsync(new[] { "rabbitmq-pc1" }))
//await using (var channel = await conn.CreateChannelAsync())
//{
//    var message = Encoding.UTF8.GetBytes("Test message from PC1");
//    var props = new BasicProperties
//    {
//        Persistent = true
//    };

//    await channel.BasicPublishAsync(
//        exchange: "",
//        routingKey: "sensor-data-queue",
//        mandatory: true,
//        basicProperties: props,
//        body: message);

//    Console.WriteLine("✓ Published to PC1");
//}

//Consume from PC2 (should receive the same message)
await using (var conn = await factory.CreateConnectionAsync(new[] { "rabbitmq-pc1" }))
await using (var channel = await conn.CreateChannelAsync())
{
    var result = await channel.BasicGetAsync("sensor-data-queue", false);

    if (result != null)
    {
        var body = Encoding.UTF8.GetString(result.Body.ToArray());
        Console.WriteLine($"✓ Consumed from PC1: {body}");

        await channel.BasicAckAsync(result.DeliveryTag, false);
        Console.WriteLine("✓ REPLICATION WORKING!");
    }
    else
    {
        Console.WriteLine("✗ No message found in queue");
    }
}
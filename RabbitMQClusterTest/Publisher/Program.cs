using RabbitMQ;

var bus = new RabbitBus(new RabbitOptions
{
    Hosts = new[] { "rabbitmq-pc1" }
});
await bus.Connect();

Console.WriteLine("Publisher started");

int i = 1;
while (true)
{
    bus.Publish(new
    {
        SensorId = "S-100",
        Value = Random.Shared.NextDouble() * 100,
        Seq = i++
    });

    await Task.Delay(1000);
}
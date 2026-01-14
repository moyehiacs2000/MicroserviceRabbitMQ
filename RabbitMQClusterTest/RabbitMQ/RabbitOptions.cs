using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ
{
    public class RabbitOptions
    {
        public string[] Hosts { get; init; } = Array.Empty<string>();
        public string UserName { get; init; } = "module";
        public string Password { get; init; } = "module_password";

        public string HotQueue { get; init; } = "HOT";
        public string ColdQueue { get; init; } = "COLD";

        public int HotQueueTtlMs { get; init; } = 5000;
    }
}

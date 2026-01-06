using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroRabbit.Domain.Core.Events
{
    public abstract class Message : IRequest<bool>
    {
        public string MesssageType { get; protected set; }
        protected Message()
        {
            MesssageType = GetType().Name;
        }
    }
}

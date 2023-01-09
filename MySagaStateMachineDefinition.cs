using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitDemo
{
    public class MySagaStateMachineDefinition : SagaDefinition<MyState>
    {
        public MySagaStateMachineDefinition()
        {
            ConcurrentMessageLimit = 1;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransitDemo
{
    public class MySagaDefinition : SagaDefinition<MySaga>
    {
        public MySagaDefinition()
        {
            ConcurrentMessageLimit = 1;
        }
    }
}

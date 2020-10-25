using System.Collections.Generic;
using System.Threading.Tasks;
using ActorsVsRx.Messages;
using Proto;

namespace ActorsVsRx.Actors
{
    public class DataBuffer : IActor
    {
        private readonly PID _dataConsumer;
        private readonly List<Data> _pending = new List<Data>();
        private readonly Behavior _behaviour;

        public DataBuffer(PID dataConsumer)
        {
            _dataConsumer = dataConsumer;
            _behaviour = new Behavior(StoreMessages);
        }

        private Task StoreMessages(IContext context)
        {
            switch (context.Message)
            {
                case Data data:
                    _pending.Add(data); 
                    break;
                case Trigger _:
                    _pending.ForEach(msg => context.Send(_dataConsumer, msg));
                    _pending.Clear();
                    _behaviour.Become(ForwardMessages);
                    break;
            }

            return Task.CompletedTask;
        }

        private Task ForwardMessages(IContext context)
        {
            if (context.Message is Data)
            {
                context.Forward(_dataConsumer);
            }

            return Task.CompletedTask;
        }

        public Task ReceiveAsync(IContext context) => _behaviour.ReceiveAsync(context);
    }
}
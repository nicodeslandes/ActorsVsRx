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

        public DataBuffer(PID dataConsumer)
        {
            _dataConsumer = dataConsumer;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Data data:
                    _pending.Add(data);
                    break;
                case Trigger _:
                    context.Send(_dataConsumer, _pending.ToArray());
                    _pending.Clear();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
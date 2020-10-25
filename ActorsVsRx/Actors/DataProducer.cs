using System;
using System.Threading.Tasks;
using ActorsVsRx.Messages;
using Proto;
using Proto.Schedulers.SimpleScheduler;

namespace ActorsVsRx.Actors
{
    public class DataProducer : IActor
    {
        private readonly PID _dataActor;
        private SimpleScheduler _scheduler;
        private int _data;
        private readonly Random _rand = new Random();

        public DataProducer(PID dataActor)
        {
            _dataActor = dataActor;
        }

        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Started _:
                    _scheduler = new SimpleScheduler(context);
                    context.Send(context.Self!, SendData.Instance);
                    break;
                case SendData _:
                    var message = new Data(++_data);
                    // Console.WriteLine($"Sending data: {message.Value}");
                    context.Send(_dataActor, message);
                    var delay = TimeSpan.FromSeconds(0.2 + _rand.NextDouble() * 0.1);
                    _scheduler.ScheduleTellOnce(delay, context.Self!, SendData.Instance);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
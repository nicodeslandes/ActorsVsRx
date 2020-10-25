using Proto;
using System;
using System.Threading.Tasks;
using ActorsVsRx.Actors;
using ActorsVsRx.Messages;

namespace ActorsVsRx
{
    public class Program
    {
        public static void Main()
        {
            var system = new ActorSystem();

            // Actor 1: Receive single or aggregated messages
            var consumer = system.Root.Spawn(Props.FromProducer(() => new DataConsumer()));

            // Buffering actor
            var buffer = system.Root.Spawn(Props.FromProducer(() => new DataBuffer(consumer)));

            // Data source: produce random Data message every few ms
            system.Root.Spawn(Props.FromProducer(() => new DataProducer(buffer)));

            // Trigger: send a trigger message every second or so
            system.Root.Spawn(Props.FromFunc(ctx =>
            {
                var rand = new Random();
                void ScheduleNextTrigger()
                {
                    var delay = TimeSpan.FromSeconds(1 + rand.NextDouble() * 0.3);
                    ctx.ReenterAfter(Task.Delay(delay), () => ctx.Send(ctx.Self!, Trigger.Instance));
                }

                switch (ctx.Message)
                {
                    case Started _:
                        ScheduleNextTrigger();
                        break;
                    case Trigger _:
                        ctx.Send(buffer, Trigger.Instance);
                        Console.WriteLine("Sending next trigger");
                        ScheduleNextTrigger();
                        break;
                }

                return Task.CompletedTask;
            }));

            Console.ReadLine();
        }
    }
}
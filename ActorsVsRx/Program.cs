using Proto;
using System;
using System.Threading.Tasks;
using ActorsVsRx.Actors;
using ActorsVsRx.Messages;

namespace ActorsVsRx
{
    public class Program
    {
        public static async Task Main()
        {
            var system = new ActorSystem();

            // Actor 1: Receive single or aggregated messages
            var consumer = system.Root.Spawn(Props.FromProducer(() => new DataConsumer()));

            // Buffering actor
            var buffer = system.Root.Spawn(Props.FromProducer(() => new DataBuffer(consumer)));

            // Data source: produce random Data message every few ms
            system.Root.Spawn(Props.FromProducer(() => new DataProducer(buffer)));

            // Send trigger message after a bit
            await Task.Delay(TimeSpan.FromSeconds(2));
            Console.WriteLine("Sending Trigger");
            system.Root.Send(buffer, Trigger.Instance);

            Console.ReadLine();
        }
    }
}
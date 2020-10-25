using System;
using System.Linq;
using System.Threading.Tasks;
using ActorsVsRx.Messages;
using Proto;

namespace ActorsVsRx.Actors
{
    public class DataConsumer : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            var msg = context.Message;
            switch (msg)
            {
                case Data r:
                    Console.WriteLine($"Single value: {r.Value}");
                    break;

                case Data[] arr:
                    Console.WriteLine($"Multiple values: [{string.Join(",", arr.Select(arr => arr.Value))}]");
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
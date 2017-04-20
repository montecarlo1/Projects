using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
namespace messagequeue
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Endpoint=sb://ssservicebus.servicebus.chinacloudapi.cn/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=BVLHCfOyQ+noEy9ee/A7qplwgmRq5uZmrqiQMuQqwW4=";
            var queueName = "myfirstque";
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var calc = 1;
            while (calc < 100) {
                var message = new BrokeredMessage("This is a test message!");
                client.Send(message);

                //client.OnMessage(message2 =>
                //{
                //    Console.WriteLine(String.Format("MessageBody :{0}", message2.GetBody<string>()));
                //    Console.WriteLine(String.Format("Messageid:{0}", message2.MessageId));
                //}

                //);

                //Console.ReadKey();

            }
            


        }
    }
}

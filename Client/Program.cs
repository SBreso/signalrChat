using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            //Thread.Sleep(2500);
            IHubProxy hub;
            string url = "http://localhost:8080/signalr";
            HubConnection connection = new HubConnection(url);

            hub = connection.CreateHubProxy("MyHub");
            connection.Start().Wait();
            hub.On("addMessage", x => Console.WriteLine(x));

            //ConnectAsync();
            string line = "";
            while ((line = Console.ReadLine()) != null)
            {
                hub.Invoke("Send", "Voro", line).Wait();
            }
        }
    }
}

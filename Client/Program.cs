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
        static String Name { get; set; }
        static IHubProxy hub { get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("Ctrl+C to exit");  
            Login();
            //IHubProxy hub;
            //string url = "http://localhost:8080/signalr";
            //HubConnection connection = new HubConnection(url);

            //hub = connection.CreateHubProxy("MyHub");
            //connection.Start().Wait();
            //hub.On("addMessage", x => Console.WriteLine(x));

            ////ConnectAsync();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Exit);
            string line = "";            
            while ((line=Console.ReadLine())!=null)
            {
                hub.Invoke("Send", Name, line).Wait();
            }
        }

        private static void Exit(object sender, ConsoleCancelEventArgs e)
        {
            hub.Invoke("OnDisconnected", true);
            Environment.Exit(0);
        }
        public void Write(string sms)
        {
            Console.WriteLine(sms);
        }

        static void Login()
        {
            Console.WriteLine("What is your name?");
            Name = Console.ReadLine();
            Console.WriteLine("Ok {0}, do you want to connect? Y/N",Name);
            char op = Console.ReadLine().ToUpper().ElementAt(0);
            switch (op)
            {
                case 'Y':
                    {
                        Connect();
                        break;
                    }
                default:
                    {
                        GoOut();
                        break;                                     
                    }
            }
        }
        static void Logout()
        {
            Console.WriteLine("Do you want logout? Y/N");
            char op = Console.ReadLine().ToUpper().ElementAt(0);
            switch (op)
            {
                case 'Y':
                    {
                        Disconnect();
                        break;
                    }
                default:
                    {
                        GoOut();
                        return;
                    }
            }
        }
        static void Connect()
        {
            Console.WriteLine("Port:");            
            String url = @"http://localhost:"+Console.ReadLine()+"/signalr";
            Console.WriteLine(url);
            //String url = @"http://192.168.1.25:8080/signalr";
            HubConnection connection = new HubConnection(url);
            hub = connection.CreateHubProxy("MyHub");
            connection.Start().Wait();
            //eventos
            connection.Received += new Action<string>(OnReceived);
            connection.ConnectionSlow += new Action(OnConnectoinSlow);
            connection.Reconnecting += new Action(OnRecconecting);
            connection.Reconnected += new Action(OnReconnected);
            connection.StateChanged += new Action<StateChange>(OnStateChanged);                   
            connection.Closed += new Action(OnClosed);            
            hub.Invoke("Login", Name);            
            hub.On("addMessage", x => Console.WriteLine(x));            
        }

        private static void OnStateChanged(StateChange e)
        {
            Console.WriteLine("{0}=>{1}", e.OldState, e.NewState);
        }

        private static void OnReconnected()
        {
            Console.WriteLine("Reconnected!!");
        }

        private static void OnRecconecting()
        {
            Console.WriteLine("Reconnecting...");
        }

        private static void OnConnectoinSlow()
        {
            Console.WriteLine("Problems with the connection");
        }

        private static void OnReceived(String e)
        {
            Console.WriteLine(e);
        }

        private static void OnClosed()
        {
            Environment.Exit(-1);
        }

        static void Disconnect()
        {
            hub.Invoke("OnDisconnected", true);
        }
        static void GoOut()
        {
            Environment.Exit(0);
        }
    }
}

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static MyHub myHub = new MyHub();
        static List<Client> ClientsList;
        static void Main(string[] args)
        {
            String url = @"http://localhost:8080/";
            //String url = @"http://192.168.001.025:8080/";
            ClientsList = new List<Client>();
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Server running at {0}",url);
                ShowMenu();
            }
           

        }
        private static void ShowMenu()
        {
            String[] options = new String[2] { "Show Client List", "Close Server" };
            Console.WriteLine("1-.{0}\n2-.{1}",options);
            while (true)
            {
                ConsoleKeyInfo op = Console.ReadKey(true);
                if (op.Key == ConsoleKey.NumPad1)
                {
                    ShowClientList();
                }
                else if (op.Key == ConsoleKey.NumPad2)
                {
                    myHub.SendFromServer("hola");
                }
                else if (op.Key == ConsoleKey.NumPad3)
                {
                    Environment.Exit(0);
                }else
                {
                    Console.WriteLine("It not is an option");
                }
            }
            
        }
        public static void AddClient(String name, String id)
        {
            Client c = new Client(name, id);
            ClientsList.Add(c);
        }
        public static void RemoveClient(String id)
        {
            foreach(Client c in ClientsList)
            {
                if (c.Id == id)
                {
                    ClientsList.Remove(c);
                    break;
                }
            }
            ShowClientList();
        }
        private static void ShowClientList()
        {
            foreach(Client c in ClientsList)
            {
                Console.WriteLine(c);
            }
        }        
    }
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.HubPipeline.AddModule(new ErrorHandlingPipelineModule());
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
    [HubName("MyHub")]
    public class MyHub : Hub
    {
        public void Login(String Name)
        {
            Console.WriteLine("{0} logging...",Name);
            Program.AddClient(Name,Context.ConnectionId);
        }
        public void Send(string name, string message)
        {
            String sms = String.Format("{0}: {1}", name, message);
            Console.WriteLine(sms);
            Clients.All.addMessage(sms);
        }
        public void SendFromServer(String sms)
        {
            //string methodToCall = "Write";
            //IClientProxy proxy = (IClientProxy)Clients.All;
            //proxy.Invoke(methodToCall, sms);
        }
        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public override Task OnDisconnected(Boolean stopCall)
        {
            Program.RemoveClient(Context.ConnectionId);
            Console.WriteLine("Client disconnected: " + Context.ConnectionId);
            return base.OnDisconnected(true);
        }
    }
    public class ErrorHandlingPipelineModule : HubPipelineModule
    {
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            Debug.WriteLine("=> Exception " + exceptionContext.Error.Message);
            if (exceptionContext.Error.InnerException != null)
            {
                Debug.WriteLine("=> Inner Exception " + exceptionContext.Error.InnerException.Message);
            }
            base.OnIncomingError(exceptionContext, invokerContext);

        }
    }
    public class LoggingPipelineModule : HubPipelineModule
    {
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            Debug.WriteLine("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
            return base.OnBeforeIncoming(context);
        }
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            Debug.WriteLine("<= Invoking " + context.Invocation.Method + " on client hub " + context.Invocation.Hub);
            return base.OnBeforeOutgoing(context);
        }
    }
}

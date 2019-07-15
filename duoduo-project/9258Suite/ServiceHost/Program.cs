using System;
using System.ServiceModel;

namespace YoYoStudio.ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                // Configure a binding with TCP port sharing enabled
                //NetTcpBinding binding = new NetTcpBinding();
                ////binding.PortSharingEnabled = true;

                //// Start a service on a fixed TCP port
                //System.ServiceModel.ServiceHost host = new System.ServiceModel.ServiceHost(typeof(ChatService.Library.ChatService));
                //ushort salt = (ushort)new Random().Next();
                ////string address = "net.tcp://localhost:9000/calculatorA";
                //string address = "net.tcp://222.189.228.201:22222/calculatorB";
                //host.AddServiceEndpoint(typeof(ChatService.Library.IChatService), binding, address);
                //host.Open();
                //Console.WriteLine("Service #{0} listening on {1}.", salt, address);
                //Console.WriteLine("Press <ENTER> to terminate service.");
                //Console.ReadLine();
                //host.Close();



                ChatService.Library.ChatService.Initialize();
                System.ServiceModel.ServiceHost host = new System.ServiceModel.ServiceHost(typeof(ChatService.Library.ChatService));
                host.Open();
                Console.WriteLine("Hall Service OK : ");
                Console.WriteLine(host.BaseAddresses[0].AbsoluteUri);
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }

        }
    }
}

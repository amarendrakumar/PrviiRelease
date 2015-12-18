using Prvii.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Prvii.Messenger
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //ChannelMessageManager.SendMessage();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new MessageService() ,
                new PaymentRecon()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

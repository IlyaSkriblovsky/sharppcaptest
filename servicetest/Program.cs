using System;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;

namespace servicetest
{
    public class Service1 : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            Console.WriteLine("starting...");
        }

        protected override void OnStop()
        {
            Console.WriteLine("stopping....");
        }


        static void Main(string[] args)
        {
            if (System.Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                    default:
                        Console.WriteLine("Usage: servicetest.exe {--install|--uninstall}");
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new Service1());
            }

        }
    }
}

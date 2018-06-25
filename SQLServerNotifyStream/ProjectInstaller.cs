using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
        {
            new ServiceController(serviceInstaller1.ServiceName).Start();
        }

        // Checks whether the service is running - if so then stop it first before proceeding with uninstall
        private void serviceInstaller1_BeforeUninstall(object sender, InstallEventArgs e)
        {
            ServiceController sc = new ServiceController(serviceInstaller1.ServiceName);

            if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                 (sc.Status.Equals(ServiceControllerStatus.StopPending)))
            {
                // Start the service if the current status is stopped.
                sc.Stop();
            }

        }
    }
}




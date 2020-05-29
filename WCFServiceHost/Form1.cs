using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using Chess.Service;
using System.ServiceModel.Security;

namespace WCFServiceHost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = string.Empty;
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            StartService();
        }

        ServiceHost host = null;

        private void StartService()
        {
            Uri tcpAdrs = new Uri("net.tcp://localhost:9890/chess/");
            Uri httpAdrs = new Uri("http://localhost:8891/chess/http");
            Uri[] baseAdresses = { tcpAdrs, httpAdrs };

            try
            {
                host = new ServiceHost(typeof(ChessService), baseAdresses);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "....." + ex.InnerException.Message);
                return;
            }

            NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.Transport, true);
            tcpBinding.TransferMode = TransferMode.Buffered;
            tcpBinding.MaxConnections = 100;
            tcpBinding.Security.Mode = SecurityMode.Transport;
            tcpBinding.Security.Message.AlgorithmSuite = SecurityAlgorithmSuite.Default;
            tcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
            tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            tcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.None;


            ServiceThrottlingBehavior throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (throttle == null)
            {
                throttle = new ServiceThrottlingBehavior();
                throttle.MaxConcurrentCalls = 100;
                throttle.MaxConcurrentSessions = 100;
                host.Description.Behaviors.Add(throttle);
            }


            tcpBinding.ReceiveTimeout = new TimeSpan(20, 0, 0);
            tcpBinding.ReliableSession.Enabled = true;
            tcpBinding.ReliableSession.InactivityTimeout = new TimeSpan(20, 0, 10);

            host.AddServiceEndpoint(typeof(IChessServiceCommuni), tcpBinding, "tcp");

            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);

            host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), "net.tcp://localhost:8892/chess/mex");

            host.Opened += new EventHandler(host_Opened);
            host.Closed += new EventHandler(host_Closed);
            host.Faulted += new EventHandler(host_Faulted);


            try
            {
                host.BeginOpen(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void host_Faulted(object sender, EventArgs e)
        {
            HandleHost();
        }

        void host_Closed(object sender, EventArgs e)
        {
            HandleHost();
        }

        void host_Opened(object sender, EventArgs e)
        {
            HandleHost();
        }

        private delegate void HandleHostInvoker();
        private void HandleHost()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new HandleHostInvoker(HandleHost));
                return;
            }
            switch (host.State)
            {
                case CommunicationState.Closed:
                    host = null;
                    buttonClose.Enabled = false;
                    buttonOpen.Enabled = true;
                    SetStatus("Host is closed");
                    break;
                case CommunicationState.Faulted:
                    host.Abort();
                    host = null;
                    SetStatus("Host is faulted");
                    SetStatus("Service is restarting.. ");
                    StartService();
                    break;
                case CommunicationState.Opened:
                    buttonClose.Enabled = true;
                    buttonOpen.Enabled = false;
                    SetStatus("Host is opened");
                    break;
                default:
                    break;
            }
        }

        

        private void SetStatus(string message)
        {
            label1.Text += message + Environment.NewLine;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            buttonClose.Enabled = false;
            SetStatus("Closing...");
            host.BeginClose(null, null);
        }
    }
}

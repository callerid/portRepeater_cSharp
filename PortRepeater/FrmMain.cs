using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace PortRepeater
{
    public partial class FrmMain : Form
    {
        public bool Broadcast;

        public static UdpReceiverClass UdpReceiver = new UdpReceiverClass();
        readonly Thread _udpReceiveThread = new Thread(UdpReceiver.UdpIdleReceive);

        public NotifyIcon NotifyIcon;

        public FrmMain()
        {
            InitializeComponent();

            // Check if ELPopup is running and suspend if so.  
            // (ELPopup is a CallerID.com free software app that users sometimes inadvertantly load)
            // Get all processes with name of 'elpopup'
            var procList = Process.GetProcessesByName("elpopup");
            if (procList.Length > 0)
            {
                if (File.Exists(procList[0].MainModule.FileName))
                {
                    var processProperties = new ProcessStartInfo
                    {
                        FileName = procList[0].MainModule.FileName,
                        Arguments = "-pause"
                    };
                    Process.Start(processProperties);
                }

            }

            // Start listener for UDP traffic
            Subscribe(UdpReceiver);

            // Start Receiver thread
            _udpReceiveThread.IsBackground = true;
            _udpReceiveThread.Start();

            timerAutoHide.Start();
        }

        // UDP Port --------------------------------------------------------------------------
        public void Subscribe(UdpReceiverClass u)
        {
            // If UDP event occurs run HeardIt method
            u.DataReceived += HeardIt;
        }

        private void HeardIt(UdpReceiverClass u, EventArgs e)
        {
            // HELP : example how to call method
            // Invoke((MethodInvoker)(() => methodName()));

            Invoke((MethodInvoker)(Repeat));

        }

        private void Repeat()
        {
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3521);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3522);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3523);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3524);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3525);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3526);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3527);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3528);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3529);
            UdpRepeatSend(UdpReceiverClass.ReceviedMessageByte, 3530);
        }

        private void UdpRepeatSend(byte[] toSend, int port)
        {
            var myUdpRepeater = new UdpClient();
            myUdpRepeater.Connect(new IPEndPoint(IPAddress.Parse("255.255.255.255"), port));

            myUdpRepeater.Send(toSend, toSend.Length);

            myUdpRepeater.Close();
        }

        private void BtnShutdownClick(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnHideClick(object sender, EventArgs e)
        {
            SetUpTrayIcon();
            Hide();
        }

        private void SetUpTrayIcon()
        {
            NotifyIcon = new NotifyIcon
                                 {
                                     BalloonTipText = @"Port repeater running in background",
                                     BalloonTipTitle = @"Port Repeater",
                                     Text = @"Show Port Repeater",
                                     Icon = Properties.Resources.Phone
                                 };
            NotifyIcon.Click += HandlerToMaximiseOnClick;

            NotifyIcon.Visible = true;
            NotifyIcon.ShowBalloonTip(2000);
            
        }

        private void HandlerToMaximiseOnClick(object sender, EventArgs e)
        {
            NotifyIcon.Visible = false;
            Show();
        }

        private void TimerAutoHideTick(object sender, EventArgs e)
        {
            timerAutoHide.Stop();
            SetUpTrayIcon();
            Hide();
        }

    }
}

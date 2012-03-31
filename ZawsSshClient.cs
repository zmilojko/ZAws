﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routrek.SSHC;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace ZAws
{
    class ZawsSshClient
    {
        public ZawsSshClient(string hostname, string user, string pass, string private_key_file)
        {
            reader = new Reader(this);
            Connect(hostname, user, pass, private_key_file);
        }
        Reader reader;
        SSHConnection _conn;

        public class ReceivedStringEventArgs : EventArgs
        {
            public ReceivedStringEventArgs(string receivedString)
            { ReceivedString = receivedString; }
            public readonly string ReceivedString;
        }
        public event EventHandler<ReceivedStringEventArgs> ReceivedString;

        class Reader : Routrek.SSHC.ISSHConnectionEventReceiver, Routrek.SSHC.ISSHChannelEventReceiver
        {
            internal Reader(ZawsSshClient Parent)
            { parent = Parent; }

            public readonly ZawsSshClient parent;

            public void OnData(byte[] data, int offset, int length)
            {
                string rcvdText = System.Text.Encoding.ASCII.GetString(data, offset, length);

                Trace.WriteLine(rcvdText);

                lock (parent.responseLock)
                {
                    parent.response += rcvdText;
                }

                if (rcvdText.Contains("$") || rcvdText.Contains("#"))
                {
                    parent.eReadyToRoll.Set();
                }

                if (parent.ReceivedString != null)
                {
                    parent.ReceivedString(parent, new ReceivedStringEventArgs(rcvdText));
                }
            }
            public void OnDebugMessage(bool always_display, byte[] data){}
            public void OnIgnoreMessage(byte[] data){}
            public void OnAuthenticationPrompt(string[] msg){}
            public void OnError(Exception error, string msg){}
            public void OnChannelClosed()
            {
                parent._conn.Disconnect("");
            }
            public void OnChannelEOF()
            {
                _pf.Close();
            }
            public void OnExtendedData(int type, byte[] data){}
            public void OnConnectionClosed(){}
            public void OnUnknownMessage(byte type, byte[] data){}
            public void OnChannelReady()
            {
            }

            public void OnChannelError(Exception error, string msg){}
            public void OnMiscPacket(byte type, byte[] data, int offset, int length){}
            public Routrek.SSHC.PortForwardingCheckResult CheckPortForwardingRequest(string host, int port, string originator_host, int originator_port)
            {
                Routrek.SSHC.PortForwardingCheckResult r = new Routrek.SSHC.PortForwardingCheckResult();
                r.allowed = true;
                r.channel = this;
                return r;
            }
            public void EstablishPortforwarding(Routrek.SSHC.ISSHChannelEventReceiver rec, Routrek.SSHC.SSHChannel channel)
            {
                _pf = channel;
            }
            public Routrek.SSHC.SSHChannel _pf;
        }

        private void Connect(string hostname, string user, string pass, string private_key_file)
        {
            SSHConnectionParameter f = new SSHConnectionParameter();
            f.UserName = user;
            f.Password = pass;
            f.Protocol = Routrek.SSHC.SSHProtocol.SSH2;

            if(string.IsNullOrWhiteSpace(private_key_file))
            {
                f.AuthenticationType = Routrek.SSHC.AuthenticationType.Password;
            }
            else
            {
                f.AuthenticationType = Routrek.SSHC.AuthenticationType.PublicKey;
                f.IdentityFile = private_key_file;
            }
            f.WindowSize = 0x1000;
            f.Protocol = SSHProtocol.SSH2;

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = Dns.GetHostEntry(hostname).AddressList[0]; // Dns.GetHostByName(hostname).AddressList[0];
            s.Connect(new IPEndPoint(ip, 22));
            _conn = Routrek.SSHC.SSHConnection.Connect(f, reader, s);

            Routrek.SSHC.SSHChannel ch = _conn.OpenShell(reader);
            reader._pf = ch;
            Routrek.SSHC.SSHConnectionInfo ci = _conn.ConnectionInfo;

            if (!eReadyToRoll.WaitOne(30000))
            {
                //prompt not received
                throw new Exception("Connected to the target OK, but have not received the clear command prompt ($ or #).");
            }
        }

        EventWaitHandle eReadyToRoll = new EventWaitHandle(false, EventResetMode.ManualReset);
        string response;
        object responseLock = new object();

        public string GetResponse()
        {
            string s;
            lock (responseLock)
            {
                s = response;
                response = "";
            }
            return s;
        }

        internal void SendLine(string s)
        {
            SendLine(s, false, 0);
        }
        internal void SendLine(string s, bool waitResponse)
        {
            SendLine(s, waitResponse, 5000);
        }
        internal bool SendLine(string s, bool waitResponse, int timeoutMiliseconds)
        {
            if (waitResponse)
            {
                eReadyToRoll.Reset();
                lock (responseLock)
                {
                    response = "";
                }
            }

            s += "\n";
            reader._pf.Transmit(Encoding.ASCII.GetBytes(s), 0, s.Length);

            if (waitResponse)
            {
                return eReadyToRoll.WaitOne(timeoutMiliseconds);
            }
            return true;
        }

        internal void Close()
        {
            try
            {
                _conn.Disconnect("");
            }
            catch { }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace VirtualHumanFramework.Core.Messages
{
    public delegate void OnStartDelegate();
    public delegate void ObjectReceivedHandler(object payload, AdvancedTCPCommunicator communicator);

    public class AdvancedTCPCommunicator : TcpCommunicator
    {
        public event ObjectReceivedHandler ReceivedObject;

        public OnStartDelegate OnStartEvent;

        public static AdvancedTCPCommunicator SpawnConnection(string address, int port)
        {
            try
            {
                TcpClient clientSocket = new TcpClient(address, port);
                return new AdvancedTCPCommunicator(clientSocket);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public AdvancedTCPCommunicator(TcpClient clientSocket)
            : base(clientSocket)
        {

        }

        public override void OnStart()
        {
            if (OnStartEvent != null)
            {
                OnStartEvent();
            }
        }

        public override void HandleMessage(VHFMessage message)
        {
            if (ReceivedObject != null)
            {
                ReceivedObject(message, this);
            }
        }

        public override void HandleObject(object payload)
        {
            if (ReceivedObject != null)
            {
                ReceivedObject(payload, this);
            }
        }
    }
}

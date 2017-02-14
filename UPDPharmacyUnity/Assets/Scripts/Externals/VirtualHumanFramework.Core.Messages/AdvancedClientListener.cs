using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace VirtualHumanFramework.Core.Messages
{
    public class AdvancedClientListener
    {
        TcpListener serverSocket;
        public List<AdvancedTCPCommunicator> clientConnections = new List<AdvancedTCPCommunicator>();

        public ObjectReceivedHandler ObjectReceived;
        public OnStartDelegate OnStart;

        protected bool keepRunning = true;

        public AdvancedClientListener(int Port, ObjectReceivedHandler ObjectReceived, OnStartDelegate OnStart = null)
        {
            this.ObjectReceived = ObjectReceived;
            this.OnStart = OnStart;

            serverSocket = new TcpListener(new IPEndPoint(IPAddress.Any, Port));
            serverSocket.Server.ReceiveTimeout = 1000;
            serverSocket.Server.SendTimeout = 1000;
            serverSocket.Start();

            Thread serverThread = new Thread(ListenForClients);
            try
            {
                serverThread.SetApartmentState(ApartmentState.STA);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            serverThread.Start();
        }

        public void ListenForClients()
        {
            while (keepRunning)
            {
                try
                {
                    TcpClient client = serverSocket.AcceptTcpClient();
                    
                    AdvancedTCPCommunicator clientCommunicator = new AdvancedTCPCommunicator(client);
                    clientCommunicator.OnStartEvent = OnStart;
                    clientCommunicator.ReceivedObject += ObjectReceived;

                    clientConnections.Add(clientCommunicator);

                    clientCommunicator.Start();
                }
                catch (SocketException) { }
            }
        }

        public void Stop()
        {
            serverSocket.Stop();
            keepRunning = false;
            foreach (AdvancedTCPCommunicator cc in clientConnections)
            {
                cc.Stop();
            }
        }

        public void AddClient(AdvancedTCPCommunicator ClientCommunicator)
        {
            clientConnections.Add(ClientCommunicator);
        }

        public bool RemoveClient(AdvancedTCPCommunicator ClientCommunicator)
        {
            return clientConnections.Remove(ClientCommunicator);
        }

        public void SendMessageToAllClients(Object message)
        {
            foreach (AdvancedTCPCommunicator cc in clientConnections)
            {
                cc.SendObject(message);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using VirtualHumanFramework.Core.Messages;

namespace VirtualHumanFramework.Core.Messages
{
    public abstract class TcpCommunicator
    {
        protected TcpClient clientSocket = new TcpClient();
        protected bool keepRunning = true;

        BinaryFormatter binarySerializer;
        NetworkStream networkStream;

        public bool Connected
        {
            get { return clientSocket.Connected; }
        }

        public IPAddress Address
        {
            get { return ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address; }
        }

        public TcpCommunicator(TcpClient clientSocket)
        {
            this.clientSocket = clientSocket;
            this.clientSocket.ReceiveTimeout = 1000;
            this.clientSocket.SendTimeout = 1000;
            
            binarySerializer = new BinaryFormatter();
            binarySerializer.Binder = new VersionDeserializationBinder();

            networkStream = clientSocket.GetStream();
            networkStream.ReadTimeout = 1000;
            networkStream.WriteTimeout = 1000;
        }
        
        public void ListenToSocket()
        {
            while (keepRunning)
            {
                Object msg = ReceiveObject();

                if (msg is VHFMessage)
                {
                    HandleMessage(msg as VHFMessage);
                }
                else
                {
                    HandleObject(msg);
                }
            }
        }

        public bool SendObject(Object payload)
        {
            if (payload == null)
            {
                throw new Exception("Cannot send a null message!");
            }

            using (var memoryStream = new MemoryStream())
            {
                // Serialize the object to a MemoryStream
                binarySerializer.Serialize(memoryStream, payload);

                if (networkStream.CanWrite)
                {
                    // First send the length of the serialized object in bytes, then send the serialized object
                    Byte[] messageLengthBytes = BitConverter.GetBytes(memoryStream.Length);

                    // TODO: Fix this so you can send messages longer than the largest int
                    if (memoryStream.Length <= Int32.MaxValue)
                    {
                        networkStream.Write(messageLengthBytes, 0, messageLengthBytes.Length);
                        networkStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
                    }
                    else
                    {
                        throw new Exception("Serialized object is too long!");
                    }
                    return true;
                }
            }
            return false;
        }

        public Object ReceiveObject()
        {
            // First get the length of the serialized object
            // Message length will be a long, which contains 8 bytes
            byte[] messageLengthByteArray = new byte[8];
            long messageLength;

            try
            {
                networkStream.Read(messageLengthByteArray, 0, messageLengthByteArray.Length);
            }
            catch (IOException)
            {
                return null;
            }
            messageLength = BitConverter.ToInt64(messageLengthByteArray, 0);

            // TODO: Fix this so that messages can be longer than Int32.MaxValue. The issue is in SendMessage. See TODO there.
            // Next read the object
            if (messageLength <= Int32.MaxValue)
            {
                int bytesReceived = 0;
                byte[] messageByteArray = new byte[messageLength];
                do
                {
                    bytesReceived += networkStream.Read(messageByteArray, bytesReceived, (int)(messageLength - bytesReceived));

                } while (bytesReceived < messageLength);

                try
                {
                    Object message = binarySerializer.Deserialize(new MemoryStream(messageByteArray));
                    return message;
                }
                catch (SerializationException e)
                {
                    if (e.Message == "Attempting to deserialize an empty stream.")
                    {
                        return null;
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
            else
            {
                throw new Exception("Message is too long!");
            }
        }

        // TODO: Make sure the client hasn't disconnected
        public bool SendMessage(VHFMessage message)
        {
            return SendObject(message);
        }

        public VHFMessage ReceiveMessage()
        {
            Object o = ReceiveObject();
            if (o is VHFMessage)
            {
                return o as VHFMessage;
            }
            else
            {
                throw new Exception("Received an object which was not a VHFMessage...");
            }
        }

        public void Start()
        {
            keepRunning = true;
            Thread t = new Thread(ListenToSocket);
            try
            {
                t.SetApartmentState(ApartmentState.STA);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            t.Start();

            OnStart();
        }

        public virtual void Stop()
        {
            keepRunning = false;

            if (networkStream != null)
            {
                networkStream.Dispose();
            }
        }

        public abstract void OnStart();
        public abstract void HandleMessage(VHFMessage message);
        public abstract void HandleObject(Object payload);
    }
}

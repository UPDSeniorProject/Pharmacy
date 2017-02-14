
using UnityEngine;
using System.Collections;
using VirtualHumanFramework.Core.Messages;
using System.Net.Sockets;

public class KinectTrackerIO : TcpCommunicator {
	
    KinectTracker kinectTracker;

    public static KinectTrackerIO CreateNewKinectTrackerIO(KinectTracker tracker, string KinectAddress, int KinectPort)
    {
        TcpClient clientSocket = new TcpClient(KinectAddress, KinectPort);
		clientSocket.ReceiveTimeout = 1000;
        return new KinectTrackerIO(tracker, clientSocket);
    }

    public KinectTrackerIO(KinectTracker tracker, TcpClient clientSocket)
        : base(clientSocket)
    {
        this.kinectTracker = tracker;
	}

    public override void OnStart()
    {
		
    }
	
	public override void HandleObject(System.Object payload) {
        Debug.Log("Received an object of type: " + payload.GetType().ToString());
        kinectTracker.QueueMessage(payload);
	}
	
	public override void HandleMessage(VHFMessage message)
	{
		
	}
}

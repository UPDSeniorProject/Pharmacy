using UnityEngine;
using System.Collections;

public class KinectTracker : RenBehaviour {
	
	public string Address;
	public int Port;
	
	KinectTrackerIO io;
	ThreadSafeList<System.Object> messageQueue;
    
    #region MonoBehaviour functions
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
		
		messageQueue = new ThreadSafeList<System.Object>();
		io = KinectTrackerIO.CreateNewKinectTrackerIO(this, Address, Port);
	    io.Start();
    }
	
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
		
		System.Object message = messageQueue.Dequeue();
		if (message != null)
		{
			ProcessMessage(message);
		}
    }
    #endregion
	
	
	#region Messages from Offline Simulator
	
	public void QueueMessage(System.Object message)
	{
		messageQueue.Enqueue(message);
	}
	
	public void ProcessMessage(System.Object message)
	{
		if (message is HeadTrackingUpdate)
        {
            Debug.Log("Received head tracking update.");
        }
		else
		{
			Debug.Log("Received unknown message.");	
		}
	}
		
	#endregion
	
	protected void OnApplicationQuit() {
		io.Stop();
	}
}

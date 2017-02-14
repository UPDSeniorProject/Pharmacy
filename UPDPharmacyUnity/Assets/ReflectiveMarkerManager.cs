using UnityEngine;
using System.Collections;
using System.Net.Sockets;

public class ReflectiveMarkerManager : MonoBehaviour {
	private TcpClient client;
	private NetworkStream stream;
	private ulong trackingId;
	private byte[] trackingIdBytes;
	// Use this for initialization
	void Start () {
		trackingIdBytes = new byte[8];
		trackingId = 0;
		client = new TcpClient("127.0.0.1", 2003);
		stream = client.GetStream ();
		stream.BeginRead (trackingIdBytes,0,trackingIdBytes.Length, new System.AsyncCallback(ReadTrackingId),null);	
	}	

	void ReadTrackingId(System.IAsyncResult ar)
	{
		int BytesRead = stream.EndRead(ar);
		if(BytesRead > 0)
		{
			this.trackingId = System.BitConverter.ToUInt64(trackingIdBytes,0);
			stream.BeginRead (trackingIdBytes,0,trackingIdBytes.Length, new System.AsyncCallback(ReadTrackingId),null);	
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnApplicationQuit() {
		stream.Close();
		client.Close();
	}

	public ulong GetTrackedId()
	{
		return this.trackingId;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class AudioBodyManager : MonoBehaviour 
{
	private Windows.Kinect.KinectSensor _Sensor;
	private AudioBeamFrameReader _Reader;
	private ulong _Data = 0;
	
	public ulong GetData()
	{
		return _Data;
	}

	public void ResetData()
	{
		_Data = 0;
	}
	
	
	void Start () 
	{
		_Sensor = Windows.Kinect.KinectSensor.GetDefault();
		
		if (_Sensor != null)
		{
			_Reader = _Sensor.AudioSource.OpenReader();
			
			if (!_Sensor.IsOpen)
			{
				_Sensor.Open();
			}
		}   
	}
	
	
	void Update () 
	{
		if (_Reader != null)
		{
			IList<AudioBeamFrame> frameList = _Reader.AcquireLatestBeamFrames();
			if(frameList != null)
			{
				foreach(AudioBeamFrame beamFrame in frameList)
				{
					if(beamFrame != null)
					{
						foreach(AudioBeamSubFrame subFrame in beamFrame.SubFrames)
						{
							foreach(AudioBodyCorrelation body in subFrame.AudioBodyCorrelations)
							{
								_Data = body.BodyTrackingId;
							}
						}
						beamFrame.Dispose();
					}
				}
			}
		} 
	}
	
	void OnApplicationQuit()
	{
		if (_Reader != null)
		{
			_Reader.Dispose();
			_Reader = null;
		}
		
		if (_Sensor != null)
		{
			if (_Sensor.IsOpen)
			{
				_Sensor.Close();
			}
			
			_Sensor = null;
		}
	}
}

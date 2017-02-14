using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using VirtualHumanFramework.Core.Messages;
using VirtualHumanFramework.Core.Messages.Messages;

public class OfflineSimulatorIO : TcpCommunicator
{
    OfflineSimulatorCommunicator offlineSimulator;
    int CharacterID;

    public static OfflineSimulatorIO CreateNewCommunicator(OfflineSimulatorCommunicator offlineSimulator, int CharacterID, string SimulatorAddress, int SimulatorPort = 6690)
    {
        TcpClient clientSocket = new TcpClient(SimulatorAddress, SimulatorPort);
        return new OfflineSimulatorIO(offlineSimulator, clientSocket, CharacterID);
    }
    
    public OfflineSimulatorIO(OfflineSimulatorCommunicator offlineSimulator, TcpClient clientSocket, int CharacterID)
        : base(clientSocket)
    {
        this.offlineSimulator = offlineSimulator;
        this.CharacterID = CharacterID;
	}

    public override void OnStart()
    {
        VHFClientRegistration register = new VHFClientRegistration(CharacterID, EventDriverType.VirtualCharacter);
        SendMessage(register);
    }

    public override void HandleMessage(VHFMessage message)
    {
		Debug.Log("Received a message of type: " + message.GetType().ToString());
		// Check to see if this message is for this character
		if (message is VHFProgramStopped) 
		{
			offlineSimulator.QueueMessage(message);
		}
		else if (message.ActorID == CharacterID) 
		{
	        Debug.Log("Received a message of type: " + message.GetType().ToString());
	        offlineSimulator.QueueMessage(message);
		}
    }
	
	public override void HandleObject (object payload)
	{
	
	}
}
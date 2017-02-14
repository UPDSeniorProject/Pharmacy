using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using VirtualHumanFramework.Core.Messages;

public class PTSDSimulatorIO : TcpCommunicator
{
    PTSDCommunicator ptsdSimulator;
    int CharacterID;

    public static PTSDSimulatorIO CreateNewCommunicator(PTSDCommunicator ptsdSimulator, int CharacterID, string SimulatorAddress, int SimulatorPort = 6690)
    {
        TcpClient clientSocket = new TcpClient(SimulatorAddress, SimulatorPort);
        return new PTSDSimulatorIO(ptsdSimulator, clientSocket, CharacterID);
    }
    

    public PTSDSimulatorIO(PTSDCommunicator ptsdSimulator, TcpClient clientSocket, int CharacterID)
        : base(clientSocket)
    {
        this.ptsdSimulator = ptsdSimulator;
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
        ptsdSimulator.QueueMessage(message);
    }
	
	public override void HandleObject (object payload)
	{
	
	}
}


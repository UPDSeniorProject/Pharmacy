using UnityEngine;
using System.Collections;

public class VPF1LocalCommunicator : VPF1Communicator {
    protected override string DefaultServerAddress
    {
        get
        {
            return "http://localhost/VirtualPeopleFactory/";
        }
    }

    public override string ToString()
    {
        return "VPF 1 Local";
    }
}

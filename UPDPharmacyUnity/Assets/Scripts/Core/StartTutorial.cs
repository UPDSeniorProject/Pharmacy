using UnityEngine;
using System.Collections;

public class StartTutorial : RenBehaviour {

    private RenLoader Loader;
    public AbstractVPFCommunicator VPFComm;

    void Loader_LoadingFinished(RenLoader loader, RenLoadingEventArgs args)
    {
        StartCoroutine(TriggerStartTutorial());
    }

    private IEnumerator TriggerStartTutorial()
    {
        while (VPFComm == null)
        {
            GetVPFComm();
            yield return null;
        }

        VPFComm.PlayTaggedAction("Start Tutorial");
    }
   

    protected override void Start()
    {
        base.Start();
        Loader = IPS.GetComponent<RenLoader>();
        Loader.LoadingFinished += Loader_LoadingFinished;

        GetVPFComm();
        
    }

    private void GetVPFComm()
    {
        if (VPFComm == null)
            VPFComm = gameObject.GetComponent<AbstractVPFCommunicator>();
    }


}

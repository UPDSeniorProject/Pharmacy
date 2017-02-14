using UnityEngine;
using System.Collections;

public class CommsAppActionHandler : RenAppActionHandler {

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public int index = 0;
    public string[] YesTags = { "Black_yes", "Over50_yes", "FamilyHistory_yes", "Smoking_yes",
        "Colonoscopy_yes", "ScreeningCost_yes", "LearnAboutFIT_yes" };
    public string[] NoTags = { "Black_no", "Over50_yes", "FamilyHistory_no", "Smoking_no",
        "Colonoscopy_no", "ScreeningCost_no", "LearnAboutFIT_no" };


    public void SaidYes()
    {
        VPF2Communicator vpf2 = ((VPF2Communicator)GetCommunicator());
        if(vpf2 != null)
        {
            if (index < YesTags.Length)
            {
                vpf2.PlayTaggedAction(YesTags[index]);
                index++;
            }
        }else
        {
            Debug.Log("No Communicator yet... this is wrong");
        }


    }

    public void SaidNo()
    {
        VPF2Communicator vpf2 = ((VPF2Communicator)GetCommunicator());
        if (vpf2 != null)
        {
            if (index < NoTags.Length)
            {
                vpf2.PlayTaggedAction(NoTags[index]);
                index++;
            }
        }
        else
        {
            Debug.Log("No Communicator yet... this is wrong");
        }
    }
}

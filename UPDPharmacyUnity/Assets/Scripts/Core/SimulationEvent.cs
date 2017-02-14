using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public abstract class SimulationEvent  {
    /// <summary>
    /// Tagged Action to be called upon completion
    /// </summary>
    public string TaggedActionString;
    /// <summary>
    /// Defines if the Event should only happen once. 
    /// </summary>
    public bool TriggerOnce = true;
    /// <summary>
    /// Saves if the event has already happened.
    /// </summary>
    protected bool HasTriggered = false;

    /// <summary>
    /// Save the abstract vpf communicator.
    /// </summary>
    protected AbstractVPFCommunicator Communicator;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckCompletion();


    public SimulationEvent(AbstractVPFCommunicator comm) 
    {
        Communicator = comm;
    }

    public void Update()
    {
        if (!HasTriggered || !TriggerOnce)
        { //Only update if we haven't triggered or can trigger MORE than once
            if (CheckCompletion())
            {
                //If the event has happened
                PlayTaggedAction(TaggedActionString);
                HasTriggered = true;
            }
        }
    }

    
    protected void PlayTaggedAction(string s)
    {
        if (Communicator != null)
        {
            Communicator.PlayTaggedAction(s);
        }
        else
        {
            Debug.Log("Couldn't trigger tagged action");   
        }
    }
    
}

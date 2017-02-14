using UnityEngine;
using System.Collections;

/// <summary>
/// Basic template of Event Driven Programming in C#
/// </summary>
public class BasicEventTemplate : RenBehaviour {
    public event BasicEventDelegate BasicEvent;

    protected void OnBasicEvent(BasicEventArgs args)
    {
        if (BasicEvent != null)
        {
            BasicEvent(this, args);
        }
        else
        {
            Debug.Log("No handler");
        }
    }

    //To add a listener to the event:
    //BasicEvent += new BasicEventDelegate(ListenerFunction);

    //When the event happens call:
    //OnBasicEvent(new BasicEventArgs());
}

public class BasicEventArgs : System.EventArgs
{
}

public delegate void BasicEventDelegate(BasicEventTemplate template, BasicEventArgs args);
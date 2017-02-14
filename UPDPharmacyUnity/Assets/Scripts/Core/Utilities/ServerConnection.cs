using UnityEngine;
using System.Collections;

public class ServerConnection 
{
    public WWW Connection;
    public float Initiated;

    public ServerConnection(WWW www)
    {
        Connection = www;
        Initiated = Time.time;
    }

    public bool HasEnded()
    {
        return Connection.progress >= 1.0f;
    }

    public float GetElapsedTime()
    {
        return Time.time - Initiated;
    }

    public bool HasWaitedMoreThan(float seconds)
    {
        return !HasEnded() && GetElapsedTime() > seconds;
    }

}

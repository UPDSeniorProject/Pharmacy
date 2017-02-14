using UnityEngine;
using System.Collections;

public class Phoneme {
    public string Name;
    public float Time;

    public Phoneme(string name, float time)
    {
        this.Name = name;
        this.Time = time;
    }

    public Phoneme(XMLNode node)
    {
        //TODO: construct from node.
        this.Name = "";
        this.Time = 0;
    }

}

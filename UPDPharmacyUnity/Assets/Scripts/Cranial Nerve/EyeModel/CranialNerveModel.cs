using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;

public class CranialNerveModel : RenBehaviour {
	
	#region Properties
	public TextAsset specFile;
	public NerveDamage[] Damages;
	
	// mapping from child gameobjects to names
	private Hashtable inputs;
	private Hashtable outputs;
	
	
	private Hashtable allNerves;
	private ArrayList nerveArray;
	private ArrayList sources;
	#endregion
	
	#region MonoBehaviour functions
	protected override void Start () {
		base.Start();
		
		bool input = true;
		
		inputs = new Hashtable();
		outputs = new Hashtable();
		foreach (Transform child in transform) {
			if (child.name.Equals("input")) {
				input = true;
			} else {
				input = false;
			}
			foreach (Transform variable in child) {
				if (input) {
					inputs.Add(variable.name, variable);
				} else {
					outputs.Add(variable.name, variable);
				}
			}
			
		}
		
		Reset();

	}



    protected override void LateUpdate () {
		if(isPaused) return;
        base.Update();
	
		foreach (Source s in sources) {
			s.update();
		}
		
		// propogate through network
        foreach (Nerve n in nerveArray)
        {
            n.propogate(Time.deltaTime);
        }
	}
	
	#endregion
	
	
	public void Reset() {
		allNerves = new Hashtable();
		nerveArray = new ArrayList();
		sources = new ArrayList();
		
		XmlReader reader = XmlReader.Create(new StringReader(specFile.text));
		
		Source curSource = null;
		
		Stack nerveStack = new Stack();
		Stack sinkStack = new Stack();
		
		Hashtable idSinks = new Hashtable();
		
		while (reader.Read()) {
			switch (reader.NodeType) {
			case XmlNodeType.Element:
				if (reader.LocalName.Equals("source")) {
					int index = 0;
					string temp = reader.GetAttribute("index");
					if (temp != null && !temp.Equals(""))
						index = int.Parse(temp);
					curSource = new Source(inputs, reader.GetAttribute("var"), index);
					sources.Add(curSource);
					
					nerveStack.Clear();
					sinkStack.Clear();
					
					
					temp = reader.GetAttribute("default");
					if (temp != null && !temp.Equals("")) {
						curSource.setSignal(float.Parse(temp));
					}
						
					
				} else if (reader.LocalName.Equals("nerve") || reader.LocalName.Equals("instanerve")) {
					Nerve newNerve;
					if (allNerves.Contains(reader.GetAttribute("id"))) {
						newNerve = (Nerve)allNerves[reader.GetAttribute("id")];
					} else {
						if (reader.LocalName.Equals("nerve")) {
                            newNerve = new Nerve(reader.GetAttribute("id"));
						} else {
                            newNerve = new InstaNerve(reader.GetAttribute("id"));
						}
						allNerves.Add(reader.GetAttribute("id"), newNerve);
						nerveArray.Add(newNerve);
					}
					nerveStack.Push(newNerve);					
					
				} else if (reader.LocalName.Equals("channel")) {
					int num = 0;
					string temp = reader.GetAttribute("num");
					if (temp != null && !temp.Equals(""))
						num = int.Parse(temp);
	
					NerveChannel channel = ((Nerve)nerveStack.Peek()).getChannel(num);
					
					if (sinkStack.Count == 0) {
						// add as sink to the source
						curSource.addSink(channel);
					} else { // add as a sink to the lastNerve
						((Sink)sinkStack.Peek()).addSink(channel);
					}
					
					if (!reader.IsEmptyElement)
						sinkStack.Push(channel);
					
				} else if (reader.LocalName.Equals("sink") || reader.LocalName.Equals("negsink") || reader.LocalName.Equals("multsink") || reader.LocalName.Equals("cutoffsink") || reader.LocalName.Equals("minmaxsink") || reader.LocalName.Equals("minsumsink")) {
					int index = 0;
					string temp = reader.GetAttribute("index");
					if (temp != null && !temp.Equals(""))
						index = int.Parse(temp);

					string id = reader.GetAttribute("id");
					Sink newSink = null;
					
					if (id != null && !id.Equals("") && idSinks.Contains(id)) {
						newSink = (Sink)idSinks[id];
					}
					
					if (newSink == null) {
						if (reader.LocalName.Equals("sink")) {
							newSink = new OutputVariable(outputs, reader.GetAttribute("var"), index);
                        } else if (reader.LocalName.Equals("negsink")) {
							newSink = new NegSink();
						} else if (reader.LocalName.Equals("multsink")) {
							float factor = 0.0f;
							temp = reader.GetAttribute("factor");
							if (temp != null && !temp.Equals(""))
								factor = float.Parse(temp);
							
							newSink = new MultiplicationSink(factor);
						} else if (reader.LocalName.Equals("cutoffsink")) {
							float factor = 0.0f;
							temp = reader.GetAttribute("factor");
							if (temp != null && !temp.Equals(""))
								factor = float.Parse(temp);
							
							newSink = new CutoffSink(factor);
						} else if (reader.LocalName.Equals("minmaxsink")) {
							newSink = new MinMaxSink();
						} else {
							newSink = new MinSumSink();
						}
					}
					
					((Sink)sinkStack.Peek()).addSink(newSink);

					if (id != null && !id.Equals("") && ! idSinks.Contains(id)) {
						idSinks.Add(id, newSink);
					}
					if (!reader.IsEmptyElement)
						sinkStack.Push(newSink);
				}
				
				break;
			case XmlNodeType.EndElement:
				if (reader.LocalName.Equals("nerve") || reader.LocalName.Equals("instanerve")) {
					nerveStack.Pop();
				} else if (reader.LocalName.Equals("channel") || reader.LocalName.Equals("sink") || reader.LocalName.Equals("negsink") || reader.LocalName.Equals("multsink") || reader.LocalName.Equals("cutoffsink") || reader.LocalName.Equals("minmaxsink") || reader.LocalName.Equals("minsumsink")) {
					sinkStack.Pop();
				}
				break;
			}
		}
		ApplyDamages();
	}
	
	protected void ApplyDamages() {
        CranialNerveAppActionHandler handler = transform.parent.gameObject.GetComponent<CranialNerveAppActionHandler>();

		foreach(NerveDamage damage in Damages) 
		{
            Nerve n = this.getNerve(damage.GetNerve());       
			n.setDamage(damage.Damage);
            if (handler && damage.CausesDoubleVision())
            {
                handler.SeeingDouble = true;
            }
		}     
	}
    private string GetAllSourcesString()
    {
        string s = "";
        foreach (Source source in sources)
        {
            s += source + "\n";
        }
        return s;
    }

    private string GetAllNervesString()
    {
        string nerves = "";
        foreach (Nerve n in allNerves.Values)
        {
            nerves += n + "\n";
        }
        return nerves;
    }
	
	public Nerve getNerve(string name) {
		return (Nerve)allNerves[name];
	}
}

class Source {
	private Hashtable inputs;
	private string name;
	private int index;
	private ArrayList sinks;

    public string GetSinksString()
    {
        string s = "";
        int i = 1;
        foreach (Sink sink in sinks)
        {
            s += "\t" + (i++) + ": " + sink.ToString() + "\n";
        }

        return s;
    }

    public override string ToString()
    {
        return "[Source i:" + index + "= "  + name + "]\n" + GetSinksString();
    }
	
	public Source (Hashtable i, string varname, int index) {
		inputs = i;
		name = varname;
		this.index = index;
		sinks = new ArrayList();
	}
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void update() {
        foreach (Sink sink in sinks)
        {
            sink.setSignal(getSignal(), this);
        }
	}

#if UNITY_EDITOR
    private string getSignalType()
    {
        if (inputs[name] is Transform)
        {
//            Transform i = (Transform)inputs[name];

            switch (index)
            {
                case 0:
                    return "localPosition.x" ;
                case 1:
                    return "localPosition.y";
                case 2:
                    return "localPosition.z";
                case 3:
                    return "localEulerAngles.x";
                case 4:
                    return "localEulerAngles.y";
                case 5:
                    return "localEulerAngles.z";
                case 6:
                    return "localScale.x";
                case 7:
                    return "localScale.y";
                case 8:
                    return "localScale.z";
            }
            return "-1.0f";
        }
        else
        {
            return name;
        }
    }
#endif

	
	private float getSignal() {
		if (inputs[name] is Transform) {
			Transform i = (Transform) inputs[name];
			
			switch(index) {
			case 0:
				return i.localPosition.x;
			case 1:
				return i.localPosition.y;
			case 2:
				return i.localPosition.z;
			case 3:
				return i.localEulerAngles.x;
			case 4:
				return i.localEulerAngles.y;
			case 5:
				return i.localEulerAngles.z;
			case 6:
				return i.localScale.x;
			case 7:
				return i.localScale.y;
			case 8:
				return i.localScale.z;
			}
			return -1.0f;
		} else {
			return (float)inputs[name];
		}
	}

	public void setSignal(float value) {
		
		if (inputs[name] is Transform) {
			Transform o = (Transform) inputs[name];
			
			switch (index) {
			case 0:
				o.localPosition = new Vector3(value, o.localPosition.y, o.localPosition.z);
				break;
			case 1:
				o.localPosition = new Vector3(o.localPosition.x, value, o.localPosition.z);
				break;
			case 2:
				o.localPosition = new Vector3(o.localPosition.x, o.localPosition.y, value);
				break;
			case 3:
				o.localEulerAngles = new Vector3(value, o.localEulerAngles.y, o.localEulerAngles.z);
				break;
			case 4:
				o.localEulerAngles = new Vector3(o.localEulerAngles.x, value, o.localEulerAngles.z);
				break;
			case 5:
				o.localEulerAngles = new Vector3(o.localEulerAngles.x, o.localEulerAngles.y, value);
				break;
			case 6:
				o.localScale = new Vector3(value, o.localScale.y, o.localScale.z);
				break;
			case 7:
				o.localScale = new Vector3(o.localScale.x, value, o.localScale.z);
				break;
			case 8:
				o.localScale = new Vector3(o.localScale.x, o.localScale.y, value);
				break;
			}
		} else {
			inputs[name] = value;
		}
	}

}

public interface Sink {
	void setSignal(float value, object source);
	void addSink(Sink s);

    string CheckIfContains(Sink s);
}


class OutputVariable : Sink {
    public override string ToString()
    {
        return "[OutputVariable " + whichVar + ", " + subVar + "]";
    }
	
	Hashtable output;
	public string whichVar;
	int subVar;
	
	public OutputVariable(Hashtable o, string w, int sub) {
		this.output = o;
		this.whichVar = w;
		this.subVar = sub;
	}
	
	public void addSink(Sink s) {
		// not implemented
	}
	
#if UNITY_EDITOR
    public string GetSubVariable()
    {
  		if (output[whichVar] is Transform) {
			Transform o = (Transform) output[whichVar];
            switch (subVar)
            {
                case 0:
                    return "localPosition.x = " + o.localPosition.x;
                case 1:
                    return "localPosition.y = " + o.localPosition.y;
                case 2:
                    return "localPosition.z = " + o.localPosition.z;
                case 3:
                    return "localEulerAngles.x = " + o.localEulerAngles.x;
                case 4:
                    return "localEulerAngles.y = " + o.localEulerAngles.y;
                case 5:
                    return "localEulerAngles.z = " + o.localEulerAngles.z;
                case 6:
                    return "localScale.x = " + o.localScale.x;
                case 7:
                    return "localScale.y = " + o.localScale.y;
                case 8:
                    return "localScale.z = " + o.localScale.z;
            }
        }
        return "SOME OTHER SUBVAR: " + subVar;
    }
#endif

	public void setSignal(float value, object source) {
		if (output[whichVar] is Transform) {
			Transform o = (Transform) output[whichVar];

			switch (subVar) {
			case 0:
				o.localPosition = new Vector3(value, o.localPosition.y, o.localPosition.z);
				break;
			case 1:
				o.localPosition = new Vector3(o.localPosition.x, value, o.localPosition.z);
				break;
			case 2:
				o.localPosition = new Vector3(o.localPosition.x, o.localPosition.y, value);
				break;
			case 3:
				o.localEulerAngles = new Vector3(value, o.localEulerAngles.y, o.localEulerAngles.z);
				break;
			case 4:
				o.localEulerAngles = new Vector3(o.localEulerAngles.x, value, o.localEulerAngles.z);
				break;
			case 5:
				o.localEulerAngles = new Vector3(o.localEulerAngles.x, o.localEulerAngles.y, value);
				break;
			case 6:
				o.localScale = new Vector3(value, o.localScale.y, o.localScale.z);
				break;
			case 7:
				o.localScale = new Vector3(o.localScale.x, value, o.localScale.z);
				break;
			case 8:
				o.localScale = new Vector3(o.localScale.x, o.localScale.y, value);
				break;
			}
		} else {
			output[whichVar] = value;
		}
	}

    public string CheckIfContains(Sink s)
    {
        if (s == this)
        {
            return this.ToString();
        }
        else
        {
            return null;
        }

    }
	
}

class MinMaxSink : Sink {
	private Hashtable negSignals;
	private Hashtable posSignals;
	private ArrayList sinks;

    //bool containsPupil=false;
	
	public MinMaxSink() {
		negSignals = new Hashtable();
		posSignals = new Hashtable();
		sinks = new ArrayList();
	}
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void setSignal(float value, object source) {

		if (value < 0)
			negSignals[source] = value;
		else
			posSignals[source] = value;
		
		float target = 0;
		float temp = 0;


		foreach (float f in negSignals.Values) {
			if (f < temp)
				temp = f;
		}
		target += temp;
		
		temp = 0;
		foreach (float f in posSignals.Values) {
			if (f > temp)
				temp = f;
		}
		target += temp;
        
        target = System.Math.Max(System.Math.Min(target, 1.0f), -1.0f);
		
		foreach (Sink sink in sinks) {
      	sink.setSignal(target, this);
		}
	}

    public string GetSinksString()
    {
        string s = "";
        foreach (Sink sink in sinks)
        {
            if (s != "")
                s += ", ";
            s += sink.ToString();
        }
        return s;
    }

    public string GetPositiveSignals()
    {
        string p = "";
        foreach (object o in posSignals.Keys)
        {
            if (p != "")
                p += ", ";

            string key;
            System.Type type = o.GetType();

            if(type == typeof(NerveChannel)){
                key = ((NerveChannel)o).ToStringNoSinks();
            }else
                   key = "__" + o.GetType().Name + "__";

            

            p += "{" + key + " = " + posSignals[o] + "}";
        }
        return p;
    }

    public string GetNegativeSignals()
    {
        string n = "";
        foreach (object o in negSignals.Keys)
        {
            if (n != "")
                n += ", ";

            n += "{" + o.GetType().Name + " = " + negSignals[o] + "}";
        }
        return n;
    }

    public override string ToString()
    {
        return "[MinMaxSink || sinks: " + GetSinksString() + " || pos: " + GetPositiveSignals() + " || negs: " + GetNegativeSignals() + "]";
    }

    public string CheckIfContains(Sink s)
    {
        foreach (Sink sink in sinks)
        {
            string res = sink.CheckIfContains(s);

            if(res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
}

class MinSumSink : Sink {
	private Hashtable negSignals;
	private Hashtable posSignals;
	private ArrayList sinks;
	
	public MinSumSink() {
		negSignals = new Hashtable();
		posSignals = new Hashtable();
		sinks = new ArrayList();
	}
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void setSignal(float value, object source) {
		if (value < 0)
			negSignals[source] = value;
		else
			posSignals[source] = value;
		
		float target = 0;
		float temp = 0;
		foreach (float f in negSignals.Values) {
			if (f < temp)
				temp = f;
		}
		target += temp;
		
		foreach (float f in posSignals.Values) {
			target += f;
		}

		target = System.Math.Max(System.Math.Min(target, 1.0f), -1.0f);
		
		foreach(Sink sink in sinks) {
			sink.setSignal(target, this);
		}
	}

    public string GetSinksString()
    {
        string s = "";
        foreach (Sink sink in sinks)
        {
            if (s != "")
                s += ", ";
            s += sink.ToString();
        }
        return s;
    }

    public override string ToString()
    {
        return "[MinSumSink " + GetSinksString() + "]";
    }

    public string CheckIfContains(Sink s)
    {
        foreach (Sink sink in sinks)
        {
            string res = sink.CheckIfContains(s);

            if (res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
}

class MultiplicationSink : Sink {
	
	protected ArrayList sinks;
	protected float factor;
	
	public MultiplicationSink(float factor) {
		this.factor = factor;
		sinks = new ArrayList();
	}
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void setSignal(float value, object source) {
        foreach (Sink sink in sinks)
        {
            sink.setSignal(factor * value, this);
        }
	}

    public string GetSinksString()
    {
        string s = "";
        foreach (Sink sink in sinks)
        {
            if (s != "")
                s += ", ";
            s += sink.ToString();
        }
        return s;
    }

    public override string ToString()
    {
        return "[MultiplicationSink (" + factor + ") -- " + GetSinksString() + "]";
    }

    public string CheckIfContains(Sink s)
    {
        foreach (Sink sink in sinks)
        {
            string res = sink.CheckIfContains(s);

            if (res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
}

class NegSink : MultiplicationSink {
	public NegSink() : base(-1.0f) {
		
	}

    public override string ToString()
    {
        return "[NegSink -- " + GetSinksString() + "]";
    }
}

class CutoffSink : Sink {
	
	private ArrayList sinks;
	private float factor;
	
	public CutoffSink(float factor) {
		this.factor = factor;
		sinks = new ArrayList();
	}
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void setSignal(float value, object source) {
		value -= factor;
		value = System.Math.Max(0.0f, value);
	
		foreach (Sink sink in sinks)
			sink.setSignal(value, this);
	}

    public string GetSinksString()
    {
        string s = "";
        foreach (Sink sink in sinks)
        {
            if (s != "")
                s += ", ";
            s += sink.ToString();
        }
        return s;
    }

    public override string ToString()
    {
        return "[CutoffSink (" + factor + ") -- " + GetSinksString() + "]";
    }

    public string CheckIfContains(Sink s)
    {
        foreach (Sink sink in sinks)
        {
            string res = sink.CheckIfContains(s);

            if (res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
}

public class Nerve {

	
	protected float delay;
	protected float fibre;
	protected NerveChannel [] channels;
    protected string name;

#if UNITY_EDITOR
    public string GetName()
    {
        return name;
    }

#endif


	public Nerve(string name) {
        this.name = name;

		this.delay = 10;
		this.fibre = 1;
		this.channels = new NerveChannel[1];
		for (int i = 0; i < channels.Length; i ++) {
			this.channels[i] = new NerveChannel();
		}
	}
	
	public NerveChannel getChannel(int i) {
		if (i >= this.channels.Length) {
			NerveChannel [] newChannels = new NerveChannel[i+1];
			for (int j = 0; j < this.channels.Length; j ++) {
				newChannels[j] = this.channels[j];
			}
			this.channels = newChannels;
		}
		if (this.channels[i] == null)
			this.channels[i] = new NerveChannel();
	
		return channels[i];
	}
	
	public void setDamage(float d) {
		fibre = 1 - d;
	}
	
	public float getDamage() {
		return 1 - fibre;
	}
	
	public void setDelay(float d) {
		delay = d;
	}
	
	public float getDelay() {
		return delay;
	}
	
	public void setSignal(float s, int channel) {
		channels[channel].setSignal(s, this);
	}
	
	public float getSignal(int channel) {
		return channels[channel].getSignal();
	}
	
	public void addSink(Sink s, int channel) {
		channels[channel].addSink(s);
	}
	
	public float getValue(int channel) {
		return channels[channel].getValue();
	}
	
	public void propogate(float deltaT) {
		for (int i = 0; i < channels.Length; i ++) {
			float diff = channels[i].getValue() - (channels[i].getSignal() * fibre);
			if (diff < 0) {
				channels[i].setValue(System.Math.Min(channels[i].getValue() + delay * deltaT, channels[i].getSignal()));
			} else {
				channels[i].setValue(System.Math.Max(channels[i].getValue() - 10 * deltaT, channels[i].getSignal()));
			}
			channels[i].setValue(System.Math.Max(System.Math.Min(channels[i].getValue(), fibre), 0));			
		}
	}

    protected string GetChannelsString()
    {
        string c = "";
        for (int i = 0; i < channels.Length; i++)
        {
            c += "\t" + i + ": " + channels[i].ToString() + "\n";
        }
        return c;
    }

    public override string ToString()
    {
        return "[Nerve " + name + " -> damage: " + getDamage() + " with " + channels.Length  + " channels]\n" + GetChannelsString();
    }

    public string CheckIfContains(Sink s)
    {
        foreach (NerveChannel channel in channels)
        {
            string res = channel.CheckIfContains(s);

            if (res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
}

public class NerveChannel : Sink {

    protected static int i = 0;
    private int Index;
	private float val;
	private Hashtable signal;
	private ArrayList sinks;

	
	public NerveChannel() {
        Index = i++; //Unique Index

		sinks = new ArrayList();
		signal = new Hashtable();
	}
		
	
	public void addSink(Sink s) {
		sinks.Add(s);
	}
	
	public void setSignal(float value, object source) {
		signal[source] = value;
	}
	
	public float getSignal() {
		float max = 0;
		foreach (float sig in signal.Values) {
			if (sig > max)
				max = sig;
		}
		return max;
	}
	
	public void setValue(float v) {
		val = v;
		foreach (Sink sink in sinks) {
			sink.setSignal(v, this);
		}
	}
	
	public float getValue() {
		return val;
	}

    public string GetSinkString()
    {
        string s = "";
        foreach(Sink sink in sinks) {
            if (s != "") 
                s += "; ";
            s += sink.ToString();
        }
        
        return s;
    }

    public string ToStringNoSinks()
    {
        return "[NerveChannel " + Index +"]";
    }

    public override string ToString()
    {

        return "[NerveChannel " + Index +" (" + GetSinkString() + ") ]";
    }

    public string CheckIfContains(Sink s)
    {
        foreach (Sink sink in sinks)
        {
            string res = sink.CheckIfContains(s);

            if (res != null)
            {
                return this.ToString() + "\n" + res;
            }
        }

        return null;
    }
	
	
}

class InstaNerve : Nerve {

    public InstaNerve(string name)
        : base(name)
    {
    }
	
	public new void propogate(float deltaT) {
		for (int i = 0; i < channels.Length; i ++ ) {
			channels[i].setValue(System.Math.Max(0.0f, System.Math.Min(1.0f, channels[i].getSignal())));
		}
	}

    public override string ToString()
    {
        return "[InstaNerve + " + name + " -> damage: " + getDamage() + "]\n" + GetChannelsString();
    }

}

[System.Serializable]
public class NerveDamage {
	private Nerve nerve;
	public float Damage;
	public EyeSide Side;
	public string Id;
	
	public string GetNerve() 
	{
		switch(Side) {
		case EyeSide.Left:
			return "l"+Id;
		case EyeSide.Right:
		default:
			return "r"+Id;
		}
	}

    public bool CausesDoubleVision()
    {
        //Nerves that cause double vision
        return (Id == "cn3" || Id == "cn4" || Id == "cn6") && Damage > 0;
    }
	
	public override string ToString ()
	{
		return string.Format ("[NerveDamage: {0} to {1}]", Damage, GetNerve());
	}
}



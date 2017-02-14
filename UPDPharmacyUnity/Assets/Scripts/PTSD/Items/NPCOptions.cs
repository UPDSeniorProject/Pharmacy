using UnityEngine;
using System.Collections.Generic;

public enum NPCOption { None, OnePerAisle, TwoPerAisle, ThreePerAisle };
public enum ShopperConversationsTypes { None, Infidelity, Easy, ChildDiscipline, BenefitsDenied, BenefitsLazy, VAPositive, VANegative }

public enum EncounterTypes
{
    None,
    Rude,
    Polite,
    Conversation
}

public enum NPCTypes
{
    Caucasian_Female,
    African_American_Female,
    African_American_Male
}

public class BumpEncounter
{
    public NPCTypes NPCType {get;set;}
    public EncounterTypes EncounterType { get; set; }

	public BumpEncounter() {

        NPCType = NPCTypes.Caucasian_Female;
        EncounterType = EncounterTypes.None;
	
	}
}

public class NPCOptions {
	
	public struct Options
	{
		string _name;
		bool _value;
	}
	

	public NPCOptions() {
		
    }
	
	
}
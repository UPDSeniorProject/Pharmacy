using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Human
{
    [Serializable]
    public class VHFHumanSpoke : VHFMessage
    {
        public List<string> Hypotheses = new List<string>();
        public Dictionary<string, object> MatchingOptions = new Dictionary<string, object>();

        public VHFHumanSpoke(int ActorID)
            : base(ActorID)
        {

        }

        public VHFHumanSpoke(int ActorID, string speechText)
            : base(ActorID)
        {
            Hypotheses.Add(speechText);
        } 
    }
}

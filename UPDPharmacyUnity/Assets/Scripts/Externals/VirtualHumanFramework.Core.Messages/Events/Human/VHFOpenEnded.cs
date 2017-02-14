using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Human
{
    [Serializable]
    public class VHFOpenEnded : VHFHumanSpoke
    {
        public List<string> Hypotheses = new List<string>();
        public Dictionary<string, object> MatchingOptions = new Dictionary<string, object>();

        public VHFOpenEnded(int ActorID)
            : base(ActorID)
        {

        }

        public VHFOpenEnded(int ActorID, string speechText)
            : base(ActorID, speechText)
        {

        }
    }
}

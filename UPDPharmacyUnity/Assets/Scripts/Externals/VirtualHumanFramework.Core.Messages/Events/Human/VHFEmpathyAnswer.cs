using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Human
{
    public class VHFEmpathyAnswer : VHFHumanSpoke
    {
        public string EmpatheticChallenge;

        public VHFEmpathyAnswer(int ActorID)
            : base(ActorID)
        {

        }

        public VHFEmpathyAnswer(int ActorID, string speechText)
            : base(ActorID, speechText)
        {
        }

        public VHFEmpathyAnswer(int ActorID, string speechText, string empatheticChallenge)
            : base(ActorID, speechText)
        {
            this.EmpatheticChallenge = empatheticChallenge;
        }
    }
}

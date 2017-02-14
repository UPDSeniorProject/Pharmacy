using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Human
{
    [Serializable]
    public class VHFCloseEnded : VHFHumanSpoke
    {
        public VHFCloseEnded(int ActorID)
            : base(ActorID)
        {

        }

        public VHFCloseEnded(int ActorID, string speechText)
            : base(ActorID, speechText)
        {

        }
    }
}

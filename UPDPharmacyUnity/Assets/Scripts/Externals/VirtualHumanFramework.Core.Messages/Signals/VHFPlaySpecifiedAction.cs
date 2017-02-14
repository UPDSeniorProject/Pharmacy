using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFPlaySpecifiedAction : VHFSignal
    {
        public int ActionNodeID { get; set; }

        public VHFPlaySpecifiedAction(int ActorID, int ActionNodeID)
            : base(ActorID)
        {
            this.ActionNodeID = ActionNodeID;
        }
    }
}

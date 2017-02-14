using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events
{
    [Serializable]
    public class VHFWozActionSelected : VHFMessage
    {
        public int ActionNodeID { get; set; }

        public VHFWozActionSelected(int ActorID, int ActionNodeID)
            : base(ActorID)
        {
            this.ActionNodeID = ActionNodeID;
        }
    }
}

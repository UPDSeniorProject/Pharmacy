using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages
{
    [Serializable]
    public abstract class VHFMessage
    {
        public int ActorID { get; set; }

        public VHFMessage(int ActorID)
        {
            this.ActorID = ActorID;
        }

        public VHFMessage()
        {
            this.ActorID = -1;
        }
    }
}

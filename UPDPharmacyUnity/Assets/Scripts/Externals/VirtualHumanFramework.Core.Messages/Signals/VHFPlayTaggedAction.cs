using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFPlayTaggedAction : VHFSignal
    {
        public string Tag { get; set; }

        public VHFPlayTaggedAction(int ActorID, string Tag)
            : base(ActorID)
        {
            this.Tag = Tag;
        }
    }
}

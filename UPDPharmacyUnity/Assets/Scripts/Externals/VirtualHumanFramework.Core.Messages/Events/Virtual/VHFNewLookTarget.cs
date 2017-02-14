using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualHumanFramework.Core.Messages;

namespace VirtualHumanFramework.Core.Events.Virtual
{
    [Serializable]
    public class VHFNewLookTarget : VHFMessage
    {
        public string Target;

        public VHFNewLookTarget(int ActorID, string Target)
            : base(ActorID)
        {
            this.Target = Target;
        }
    }
}

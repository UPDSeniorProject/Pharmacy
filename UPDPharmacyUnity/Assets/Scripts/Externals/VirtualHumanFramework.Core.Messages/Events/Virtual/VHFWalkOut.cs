using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualHumanFramework.Core.Messages.Signals;

namespace VirtualHumanFramework.Core.Messages.Events.Virtual
{
    [Serializable]
    public class VHFWalkOut : VHFSignal
    {
        public bool comeBack;
        public VHFWalkOut(int ActorID, bool comeBack = false)
            : base(ActorID)
        {
            this.comeBack = comeBack;
        }
    }
}

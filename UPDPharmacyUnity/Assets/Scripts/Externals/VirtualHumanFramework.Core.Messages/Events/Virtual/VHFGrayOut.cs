using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualHumanFramework.Core.Messages.Signals;

namespace VirtualHumanFramework.Core.Messages.Events.Virtual
{
    [Serializable]
    public class VHFGrayOut : VHFSignal
    {
        public bool IsGrayed;

        public VHFGrayOut(int ActorID, bool IsGrayed)
            : base(ActorID)
        {
            this.IsGrayed = IsGrayed;
        }
    }
}

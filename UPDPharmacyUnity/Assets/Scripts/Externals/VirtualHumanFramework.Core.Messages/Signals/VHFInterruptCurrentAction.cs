using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFInterruptCurrentAction : VHFSignal
    {
        public VHFInterruptCurrentAction(int ActorID)
            : base(ActorID)
        {

        }
    }
}

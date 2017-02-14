using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public abstract class VHFSignal : VHFMessage
    {
        public VHFSignal(int ActorID)
            : base(ActorID)
        {

        }

        public VHFSignal()
            : base()
        {

        }
    }
}

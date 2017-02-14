using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFPTSDCommand : VHFSignal
    {
        public string PTSDMessage { get; set; }

        public VHFPTSDCommand(int ActorID, string PTSDMessage)
            : base(ActorID)
        {
            this.PTSDMessage = PTSDMessage;
        }
    }
}

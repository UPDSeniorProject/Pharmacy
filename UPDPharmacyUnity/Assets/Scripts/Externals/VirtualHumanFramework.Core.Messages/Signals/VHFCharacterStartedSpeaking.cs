using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFCharacterStartedSpeaking : VHFSignal
    {
        public VHFCharacterStartedSpeaking(int ActorID)
            : base(ActorID)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFCharacterFinishedSpeaking : VHFSignal
    {
        public VHFCharacterFinishedSpeaking(int ActorID)
            : base(ActorID)
        {

        }
    }
}

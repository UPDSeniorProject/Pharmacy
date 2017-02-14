using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages
{
    public enum EventDriverType
    {
        HumanCharacter,
        VirtualCharacter,
        Environment,
        VideoPlayer,
        Woz
    }

    [Serializable]
    public class VHFClientRegistration : VHFMessage
    {
        public EventDriverType CharacterDriverType;

        public VHFClientRegistration(int ActorID, EventDriverType CharacterDriverType)
            : base(ActorID)
        {
            this.CharacterDriverType = CharacterDriverType;
        }

        // Don't use this, needed for serializing class
        // http://support.microsoft.com/kb/330592
        public VHFClientRegistration() { }
    }
}

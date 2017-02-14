using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Messages
{
    [Serializable]
    public class VHFWozConfiguration : VHFMessage
    {
        public Dictionary<int, bool> WozedCharacters;

        public VHFWozConfiguration(Dictionary<int, bool> WozedCharacters)
            : base(-1)
        {
            this.WozedCharacters = WozedCharacters;
        }

        // Don't use this, needed for serializing class
        // http://support.microsoft.com/kb/330592
        public VHFWozConfiguration() { }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Logging
{
    [Serializable]
    public class VHFStartLogging
    {
        public string ParticipantID { get; set; }
    }
}

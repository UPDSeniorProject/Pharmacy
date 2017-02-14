using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Messages
{
    [Serializable]
    public class VHFProgramStopped : VHFMessage
    {
        public string Name { get; set; }
    }
}

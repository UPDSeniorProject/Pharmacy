using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Commands
{
    [Serializable]
    public class VHFStopProgram
    {
        public string Name { get; set; }
    }
}

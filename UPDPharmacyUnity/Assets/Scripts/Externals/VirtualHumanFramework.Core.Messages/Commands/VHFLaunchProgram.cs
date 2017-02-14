using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Commands
{
    [Serializable]
    public class VHFLaunchProgram
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; }
    }
}

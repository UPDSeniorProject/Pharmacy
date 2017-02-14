using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Logging
{
    public enum ProgramStatus
    {
        Disconnected,
        ReadyToLaunch,
        Launching,
        Running
    }

    [Serializable]
    public class VHFStatusUpdate
    {
        public string Name { get; set; }
        public ProgramStatus Status { get; set; }


        public VHFStatusUpdate(string Name, ProgramStatus Status)
        {
            this.Name = Name;
            this.Status = Status;
        }
    }
}

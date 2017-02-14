using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Signals
{
    [Serializable]
    public class VHFPlayVideo : VHFSignal
    {
        public string VideoName { get; set; }

        public VHFPlayVideo(string VideoName)
        {
            this.VideoName = VideoName;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Virtual
{
    [Serializable]
    public class VHFVirtualActionOccurred : VHFMessage
    {
        public int SpeechID { get; set; }
        public string SpeechText { get; set; }
        public string AudioFileName { get; set; }

        public int AnimationID { get; set; }
        public string Animation { get; set; }

        public string[] AppActions { get; set; }

        public string VideoURL { get; set; }
        public string ImageTransition { get; set; }

        public string[] Tags { get; set; }
        public string[] Topics { get; set; }
        public string[] Discoveries { get; set; }

        public bool IsCritial;
        public bool IsOptional;
        public bool IsActionOnly;

        public string LookTarget { get; set; }

        public VHFVirtualActionOccurred(int ActorID)
            : base(ActorID)
        {
            SpeechID = -1;
            AnimationID = -1;
        }
    }
}

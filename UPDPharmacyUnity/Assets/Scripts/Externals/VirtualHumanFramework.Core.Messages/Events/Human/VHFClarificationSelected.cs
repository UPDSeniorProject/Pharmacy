using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Human
{
    [Serializable]
    public class VHFClarificationSelected : VHFMessage
    {
        public int? SelectedSpeechID { get; set; }

        public VHFClarificationSelected(int ActorID, int? SelectedSpeechID)
            : base(ActorID)
        {
            this.SelectedSpeechID = SelectedSpeechID;
        }
    }
}

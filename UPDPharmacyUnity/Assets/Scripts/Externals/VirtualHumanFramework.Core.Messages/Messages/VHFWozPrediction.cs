using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Messages
{
    [Serializable]
    public class VHFWozPrediction : VHFMessage
    {
        public int PreviousActionGroupID { get; set; }
        public int CurrentActionGroupID { get; set; }
        public int NextActionGroupID { get; set; }

        public VHFWozPrediction(int PreviousActionGroupID, int CurrentActionGroupID, int NextActionGroupID)
            : base(-1)
        {
            this.PreviousActionGroupID = PreviousActionGroupID;
            this.CurrentActionGroupID = CurrentActionGroupID;
            this.NextActionGroupID = NextActionGroupID;
        }

        // Don't use this, needed for serializing class
        // http://support.microsoft.com/kb/330592
        public VHFWozPrediction() { }
    }
}

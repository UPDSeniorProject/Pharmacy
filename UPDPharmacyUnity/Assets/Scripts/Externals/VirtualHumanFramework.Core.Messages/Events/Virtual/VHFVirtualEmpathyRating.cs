using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Events.Virtual
{
    [Serializable]
    public class VHFVirtualEmpathyRating : VHFMessage
    {
        public double[] ratings;
        public string response;

        public VHFVirtualEmpathyRating(int ActorID, double[] d, string response)
            : base(ActorID)
        {
            this.ratings = d;
            this.response = response;
        }

        public VHFVirtualEmpathyRating(double [] d, string response) :
            base()
        {
            this.ratings = d;
            this.response = response;
        }

    }
}

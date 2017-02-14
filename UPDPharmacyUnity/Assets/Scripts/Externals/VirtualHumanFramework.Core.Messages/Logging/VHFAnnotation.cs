using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages.Logging
{
    [Serializable]
    public class VHFAnnotation
    {
        public string Label { get; set; }
        public string Message { get; set; }

        public VHFAnnotation(string Label, string Message)
        {
            this.Label = Label;
            this.Message = Message;
        }
    }
}

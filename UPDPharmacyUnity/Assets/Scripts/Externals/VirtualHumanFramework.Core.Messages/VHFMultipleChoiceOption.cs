using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualHumanFramework.Core.Messages
{
    [Serializable]
    public class VHFMultipleChoiceOption : VHFMessage
    {
        public List<String> messageList;

        public VHFMultipleChoiceOption(int ActorID, List<String> messageList) : base(ActorID) {
            this.messageList = messageList;
        }

        void AddMessage(String message) {
            messageList.Add(message);
        }

    }
}

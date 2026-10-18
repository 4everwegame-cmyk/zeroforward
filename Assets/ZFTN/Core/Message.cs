using System;

namespace ZFTN.Core
{
    public class Message
    {
        public string Sender { get; }
        public string Recipient { get; }
        public string Intent { get; }
        public string Payload { get; }
        public int Priority { get; }
        public bool RequiresApproval { get; }

        public Message(
            string sender,
            string recipient,
            string intent,
            string payload,
            int priority = 0,
            bool requiresApproval = false)
        {
            Sender = sender;
            Recipient = recipient;
            Intent = intent;
            Payload = payload;
            Priority = priority;
            RequiresApproval = requiresApproval;
        }
    }
}

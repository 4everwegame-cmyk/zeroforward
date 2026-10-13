using ZFTN.Core;

namespace ZFTN.Agents
{
    public abstract class AgentBase
    {
        public string AgentName { get; protected set; }
        protected readonly string ListenIntent;

        // Agents register to an intent; messages they produce should have an explicit recipient (or rely on intent routing).
        protected AgentBase(string name, string listenIntent)
        {
            AgentName = name;
            ListenIntent = listenIntent;
            // Subscribe by intent so this agent receives messages matching that intent.
            MessageBus.SubscribeIntent(ListenIntent, OnMessageReceived);
        }

        // Called by MessageBus when a message matching ListenIntent arrives.
        protected virtual void OnMessageReceived(Message input)
        {
            var response = Process(input);
            if (response != null)
            {
                // Agents generally publish their responses onto the bus.
                MessageBus.Publish(response);
            }
        }

        // Implement agent logic and return a Message to publish (or null).
        public abstract Message Process(Message input);

        // Helper to produce and immediately publish a message.
        protected void Emit(string recipient, string intent, string payload, bool requiresApproval = false)
        {
            var m = new Message(AgentName, recipient, intent, payload, 0, requiresApproval);
            MessageBus.Publish(m);
        }
    }
}

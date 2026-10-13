using System;
using ZFTN.Core;

namespace ZFTN.Agents
{
    public abstract class AgentBase : IDisposable
    {
        public string AgentName { get; protected set; }
        protected readonly string ListenIntent;

        private bool _disposed;

        protected AgentBase(string name, string listenIntent)
        {
            AgentName = name;
            ListenIntent = listenIntent;
            MessageBus.SubscribeIntent(ListenIntent, OnMessageReceived);
        }

        protected virtual void OnMessageReceived(Message input)
        {
            if (_disposed || input == null) return;

            var response = Process(input);
            if (!_disposed && response != null)
            {
                MessageBus.Publish(response);
            }
        }

        public abstract Message Process(Message input);

        protected void Emit(string recipient, string intent, string payload, bool requiresApproval = false)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(AgentName);
            }

            var msg = new Message(AgentName, recipient, intent, payload, 0, requiresApproval);
            MessageBus.Publish(msg);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            MessageBus.UnsubscribeIntent(ListenIntent, OnMessageReceived);
        }
    }
}

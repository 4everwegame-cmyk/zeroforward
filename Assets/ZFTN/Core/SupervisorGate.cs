using System.Collections.Generic;
using UnityEngine;

namespace ZFTN.Core
{
    // SupervisorGate subscribes to messages addressed to recipient "SupervisorGate"
    // and publishes an approval or simple acknowledgement back into the bus.
    public class SupervisorGate
    {
        private readonly List<string> _approved = new List<string>();

        public SupervisorGate()
        {
            // Listen to all messages sent to the SupervisorGate recipient.
            MessageBus.SubscribeRecipient("SupervisorGate", OnMessage);
        }

        private void OnMessage(Message msg)
        {
            if (msg == null) return;

            Debug.Log($"[SupervisorGate] Received for validation: Intent={msg.Intent} Sender={msg.Sender}");

            if (msg.RequiresApproval)
            {
                // Insert more complex validation or consensus here.
                _approved.Add(msg.Payload ?? "(null)");
                // Publish an approved message back to the original sender (or to any interested subscriber)
                var approvedMsg = new Message("SupervisorGate", msg.Sender, "Approved", msg.Payload);
                MessageBus.Publish(approvedMsg);
            }
            else
            {
                // Pass-through acknowledgement
                var ack = new Message("SupervisorGate", msg.Sender, "Received", msg.Payload);
                MessageBus.Publish(ack);
            }
        }

        public IReadOnlyList<string> Approved => _approved.AsReadOnly();
    }
}

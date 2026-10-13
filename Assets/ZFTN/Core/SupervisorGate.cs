using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFTN.Core
{
    public class SupervisorGate : IDisposable
    {
        private readonly Func<Message, string> _approvalPolicy;
        private readonly List<string> _approved = new List<string>();
        private bool _disposed;

        public SupervisorGate()
            : this(_ => "No approval policy configured.")
        {
        }

        public SupervisorGate(Func<Message, string> approvalPolicy)
        {
            _approvalPolicy = approvalPolicy ?? throw new ArgumentNullException(nameof(approvalPolicy));
            MessageBus.SubscribeRecipient("SupervisorGate", OnMessage);
        }

        private void OnMessage(Message msg)
        {
            if (_disposed || msg == null) return;

            Debug.Log($"[SupervisorGate] Received for validation: Intent={msg.Intent} Sender={msg.Sender}");

            if (!msg.RequiresApproval)
            {
                MessageBus.Publish(new Message("SupervisorGate", msg.Sender, "Received", msg.Payload));
                return;
            }

            string rejectionReason;

            try
            {
                rejectionReason = _approvalPolicy(msg);
            }
            catch (Exception ex)
            {
                rejectionReason = "Approval policy failed: " + ex.GetType().Name;
            }

            if (!string.IsNullOrEmpty(rejectionReason))
            {
                MessageBus.Publish(new Message("SupervisorGate", msg.Sender, "Rejected", rejectionReason));
                return;
            }

            _approved.Add(msg.Payload ?? "(null)");
            MessageBus.Publish(new Message("SupervisorGate", msg.Sender, "Approved", msg.Payload));
        }

        public IReadOnlyList<string> Approved => _approved.AsReadOnly();

        public void Dispose()
        {
            if (_disposed) return;

            MessageBus.UnsubscribeRecipient("SupervisorGate", OnMessage);
            _disposed = true;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFTN.Core
{
    // Simple static event bus supporting intent-based and recipient-based subscriptions.
    public static class MessageBus
    {
        private static readonly Dictionary<string, Action<Message>> _intentSubscribers = new Dictionary<string, Action<Message>>();
        private static readonly Dictionary<string, Action<Message>> _recipientSubscribers = new Dictionary<string, Action<Message>>();
        private static readonly object _lock = new object();

        // Subscribe by intent (e.g., "PlanGenerated") or recipient (e.g., "SupervisorGate").
        public static void SubscribeIntent(string intent, Action<Message> callback)
        {
            if (string.IsNullOrEmpty(intent) || callback == null) return;
            lock (_lock)
            {
                if (!_intentSubscribers.ContainsKey(intent)) _intentSubscribers[intent] = null;
                _intentSubscribers[intent] += callback;
            }
        }

        public static void UnsubscribeIntent(string intent, Action<Message> callback)
        {
            if (string.IsNullOrEmpty(intent) || callback == null) return;
            lock (_lock)
            {
                if (_intentSubscribers.TryGetValue(intent, out var existing))
                {
                    existing -= callback;
                    if (existing == null) _intentSubscribers.Remove(intent);
                    else _intentSubscribers[intent] = existing;
                }
            }
        }

        public static void SubscribeRecipient(string recipient, Action<Message> callback)
        {
            if (string.IsNullOrEmpty(recipient) || callback == null) return;
            lock (_lock)
            {
                if (!_recipientSubscribers.ContainsKey(recipient)) _recipientSubscribers[recipient] = null;
                _recipientSubscribers[recipient] += callback;
            }
        }

        public static void UnsubscribeRecipient(string recipient, Action<Message> callback)
        {
            if (string.IsNullOrEmpty(recipient) || callback == null) return;
            lock (_lock)
            {
                if (_recipientSubscribers.TryGetValue(recipient, out var existing))
                {
                    existing -= callback;
                    if (existing == null) _recipientSubscribers.Remove(recipient);
                    else _recipientSubscribers[recipient] = existing;
                }
            }
        }

        // Call from tests to reset static state between test cases
        public static void ClearAllSubscribers()
        {
            lock (_lock)
            {
                _intentSubscribers.Clear();
                _recipientSubscribers.Clear();
            }
        }

        // Publish message: invoke intent subscribers, then recipient subscribers.
        public static void Publish(Message msg)
        {
            if (msg == null) return;

            Debug.Log($"[MessageBus] Publish Intent='{msg.Intent}' Sender='{msg.Sender}' Recipient='{msg.Recipient}'");

            // Invoke intent subscribers
            Action<Message> intentHandlers = null;
            lock (_lock)
            {
                if (_intentSubscribers.TryGetValue(msg.Intent, out var h)) intentHandlers = h;
            }
            if (intentHandlers != null)
            {
                foreach (Action<Message> handler in intentHandlers.GetInvocationList())
                {
                    try { handler.Invoke(msg); }
                    catch (Exception ex) { Debug.LogError($"[MessageBus] Intent handler threw: {ex.GetType().Name} - {ex.Message}"); }
                }
            }

            // Invoke recipient subscribers
            Action<Message> recipientHandlers = null;
            lock (_lock)
            {
                if (!string.IsNullOrEmpty(msg.Recipient) && _recipientSubscribers.TryGetValue(msg.Recipient, out var h2)) recipientHandlers = h2;
            }
            if (recipientHandlers != null)
            {
                foreach (Action<Message> handler in recipientHandlers.GetInvocationList())
                {
                    try { handler.Invoke(msg); }
                    catch (Exception ex) { Debug.LogError($"[MessageBus] Recipient handler threw: {ex.GetType().Name} - {ex.Message}"); }
                }
            }
        }
    }
}

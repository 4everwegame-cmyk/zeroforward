using NUnit.Framework;
using ZFTN.Agents;
using ZFTN.Core;

namespace ZFTN.Tests
{
    public class ZFTNEventBusTests
    {
        private class TestAgent : AgentBase
        {
            public TestAgent(string name, string listenIntent) : base(name, listenIntent)
            {
            }

            public override Message Process(Message input)
            {
                return new Message(AgentName, "System", "TestHandled", input.Payload, 0, false);
            }
        }

        [TearDown]
        public void TearDown()
        {
            MessageBus.ClearAllSubscribers();
        }

        [Test]
        public void MessageBus_IntentRouting_DeliversToSubscriber()
        {
            bool received = false;
            string capturedPayload = string.Empty;

            void Handler(Message msg)
            {
                received = true;
                capturedPayload = msg.Payload;
            }

            MessageBus.SubscribeIntent("TestIntent", Handler);

            var testMessage = new Message("SenderAgent", "RecipientAgent", "TestIntent", "Hello ZFTN");
            MessageBus.Publish(testMessage);

            MessageBus.UnsubscribeIntent("TestIntent", Handler);

            Assert.IsTrue(received, "MessageBus should have delivered message to intent subscriber.");
            Assert.AreEqual("Hello ZFTN", capturedPayload);
        }

        [Test]
        public void SupervisorGate_RejectsApprovalRequiredMessages_WhenPolicyReturnsReason()
        {
            bool rejectedReceived = false;
            bool approvedReceived = false;

            var gate = new SupervisorGate(_ => "Policy denied approval");

            void RejectedHandler(Message msg)
            {
                if (msg.Intent == "Rejected")
                {
                    rejectedReceived = true;
                }
            }

            void ApprovedHandler(Message msg)
            {
                if (msg.Intent == "Approved")
                {
                    approvedReceived = true;
                }
            }

            MessageBus.SubscribeIntent("Rejected", RejectedHandler);
            MessageBus.SubscribeIntent("Approved", ApprovedHandler);

            var req = new Message("CoderAgent", "SupervisorGate", "CodeGenerated", "DraftCodePayload", 1, true);
            MessageBus.Publish(req);

            MessageBus.UnsubscribeIntent("Rejected", RejectedHandler);
            MessageBus.UnsubscribeIntent("Approved", ApprovedHandler);
            gate.Dispose();

            Assert.IsTrue(rejectedReceived, "SupervisorGate should reject approval-required messages when policy denies.");
            Assert.IsFalse(approvedReceived, "SupervisorGate should not publish an Approved message when approvals are denied.");
            Assert.AreEqual(0, gate.Approved.Count);
        }

        [Test]
        public void SupervisorGate_ValidatesApproval_AndPublishesApprovedMessage()
        {
            var gate = new SupervisorGate(_ => null);

            bool approvalReceived = false;
            void ApprovedHandler(Message msg)
            {
                if (msg.Intent == "Approved")
                {
                    approvalReceived = true;
                }
            }

            MessageBus.SubscribeIntent("Approved", ApprovedHandler);

            var req = new Message("CoderAgent", "SupervisorGate", "CodeGenerated", "DraftCodePayload", 1, true);
            MessageBus.Publish(req);

            MessageBus.UnsubscribeIntent("Approved", ApprovedHandler);
            gate.Dispose();

            Assert.IsTrue(approvalReceived, "SupervisorGate should intercept approval-required messages and emit an Approved intent.");
            Assert.AreEqual(1, gate.Approved.Count);
            Assert.AreEqual("DraftCodePayload", gate.Approved[0]);
        }

        [Test]
        public void PlannerAgent_RespondsToBootstrap_WithPlanGenerated()
        {
            bool planGeneratedFired = false;

            void PlanHandler(Message msg)
            {
                if (msg.Intent == "PlanGenerated")
                {
                    planGeneratedFired = true;
                }
            }

            MessageBus.SubscribeIntent("PlanGenerated", PlanHandler);

            var planner = new PlannerAgent();
            var bootstrapMsg = new Message("System", "PlannerAgent", "BootstrapBuild", "Start Simulation");

            MessageBus.Publish(bootstrapMsg);

            MessageBus.UnsubscribeIntent("PlanGenerated", PlanHandler);
            planner.Dispose();

            Assert.IsTrue(planGeneratedFired, "PlannerAgent should listen to BootstrapBuild and emit PlanGenerated.");
        }

        [Test]
        public void AgentRegistry_GetMissingAgent_ReturnsNull()
        {
            var registry = new AgentRegistry();

            var missing = registry.Get("NotRealAgent");
            registry.Dispose();

            Assert.IsNull(missing, "Missing agents should return null instead of creating unknown runtime state.");
        }

        [Test]
        public void Agent_Dispose_UnsubscribesFromIntent()
        {
            var agent = new TestAgent("TestAgent", "TestIntent");
            bool received = false;

            void TestHandler(Message msg)
            {
                received = true;
            }

            MessageBus.SubscribeIntent("TestHandled", TestHandler);
            agent.Dispose();
            MessageBus.Publish(new Message("System", "TestAgent", "TestIntent", "should be ignored"));

            MessageBus.UnsubscribeIntent("TestHandled", TestHandler);

            Assert.IsFalse(received, "Disposed agents should unsubscribe and not process future messages.");
        }

        [Test]
        public void DuplicateDispose_IsSafe()
        {
            var agent = new TestAgent("SafeAgent", "TestIntent");

            agent.Dispose();
            Assert.DoesNotThrow(() => agent.Dispose());
        }
    }
}

using NUnit.Framework;
using ZFTN.Core;
using ZFTN.Agents;

namespace ZFTN.Tests
{
    public class ZFTNEventBusTests
    {
        [TearDown]
        public void TearDown()
        {
            // Reset MessageBus static state to avoid cross-test pollution (requires ClearAllSubscribers in MessageBus)
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
        public void SupervisorGate_ValidatesApproval_AndPublishesApprovedMessage()
        {
            var gate = new SupervisorGate();

            bool approvalReceived = false;
            void ApprovedHandler(Message msg)
            {
                if (msg.Intent == "Approved")
                {
                    approvalReceived = true;
                }
            }

            MessageBus.SubscribeIntent("Approved", ApprovedHandler);

            // Send a message requiring approval directly to SupervisorGate recipient
            var req = new Message("CoderAgent", "SupervisorGate", "CodeGenerated", "DraftCodePayload", 1, true);
            MessageBus.Publish(req);

            MessageBus.UnsubscribeIntent("Approved", ApprovedHandler);

            Assert.IsTrue(approvalReceived, "SupervisorGate should intercept messages requiring approval and emit an 'Approved' intent.");
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

            // Trigger the agent via bus publish; agent listens to BootstrapBuild
            MessageBus.Publish(bootstrapMsg);

            MessageBus.UnsubscribeIntent("PlanGenerated", PlanHandler);

            Assert.IsTrue(planGeneratedFired, "PlannerAgent should listen to BootstrapBuild and emit PlanGenerated.");
        }
    }
}

using ZFTN.Core;

namespace ZFTN.Agents
{
    public class HumanIntentAgent : AgentBase
    {
        public HumanIntentAgent() : base("HumanIntentAgent", "BootstrapBuild") { }

        public override Message Process(Message input)
        {
            var payload = $"Human intent alignment and telemetry anchor confirmed: {input.Payload}";
            return new Message(AgentName, "System", "IntentAligned", payload, 0, false);
        }
    }
}

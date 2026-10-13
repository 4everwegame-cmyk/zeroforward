using ZFTN.Core;

namespace ZFTN.Agents
{
    public class NHIAgent : AgentBase
    {
        public NHIAgent() : base("NHIAgent", "BootstrapBuild") { }

        public override Message Process(Message input)
        {
            var payload = $"Non-Human Intelligence perspective synthesized: {input.Payload}";
            return new Message(AgentName, "System", "PerspectiveGenerated", payload, 0, false);
        }
    }
}

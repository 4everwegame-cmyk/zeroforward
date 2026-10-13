using ZFTN.Core;

namespace ZFTN.Agents
{
    public class ArchitectAgent : AgentBase
    {
        public ArchitectAgent() : base("ArchitectAgent", "PlanGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"Structural blueprint established based on: {input.Payload}";
            return new Message(AgentName, "CriticAgent", "ArchitectureGenerated", payload, 0, false);
        }
    }
}

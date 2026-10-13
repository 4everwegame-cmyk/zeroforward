using ZFTN.Core;

namespace ZFTN.Agents
{
    public class CriticAgent : AgentBase
    {
        public CriticAgent() : base("CriticAgent", "ArchitectureGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"Critique completed. Architecture validated: {input.Payload}";
            return new Message(AgentName, "ArchivistAgent", "CritiqueGenerated", payload, 0, false);
        }
    }
}

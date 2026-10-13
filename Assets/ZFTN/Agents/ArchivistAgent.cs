using ZFTN.Core;

namespace ZFTN.Agents
{
    public class ArchivistAgent : AgentBase
    {
        public ArchivistAgent() : base("ArchivistAgent", "CritiqueGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"Telemetry state securely archived: {input.Payload}";
            return new Message(AgentName, "System", "Archived", payload, 0, false);
        }
    }
}

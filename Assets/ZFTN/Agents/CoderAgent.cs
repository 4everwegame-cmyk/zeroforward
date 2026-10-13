using ZFTN.Core;

namespace ZFTN.Agents
{
    public class CoderAgent : AgentBase
    {
        public CoderAgent() : base("CoderAgent", "PlanGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"C# module scaffold drafted for: {input.Payload}";
            // Requires approval from SupervisorGate before final commitment
            return new Message(AgentName, "SupervisorGate", "CodeGenerated", payload, 1, true);
        }
    }
}

using ZFTN.Core;

namespace ZFTN.Agents
{
    public class PlannerAgent : AgentBase
    {
        public PlannerAgent() : base("PlannerAgent", "BootstrapBuild") { }

        public override Message Process(Message input)
        {
            var payload = $"Plan generated for objective: {input.Payload}";
            // Emit next step onto the bus for Architect and Coder to pick up
            return new Message(AgentName, "All", "PlanGenerated", payload, 0, false);
        }
    }
}

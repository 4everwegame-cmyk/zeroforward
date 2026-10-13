using ZFTN.Core;

namespace ZFTN.Agents
{
    public class NonHumanAIAgent : AgentBase
    {
        public NonHumanAIAgent() : base("NonHumanAIAgent", "PlanGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"Non-human network logic applied to: {input.Payload}";
            return new Message(AgentName, "System", "NonHumanLogicGenerated", payload, 0, false);
        }
    }
}

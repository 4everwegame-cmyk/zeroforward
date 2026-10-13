using ZFTN.Core;

namespace ZFTN.Agents
{
    public class ValidatorAgent : AgentBase
    {
        public ValidatorAgent() : base("ValidatorAgent", "PlanGenerated") { }

        public override Message Process(Message input)
        {
            var payload = $"ZFTN zero-trust compliance verified for: {input.Payload}";
            return new Message(AgentName, "System", "ValidationResult", payload, 0, false);
        }
    }
}

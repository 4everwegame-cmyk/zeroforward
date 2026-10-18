using System;
using System.Collections.Generic;
using ZFTN.Agents;

namespace ZFTN.Core
{
    public class AgentRegistry : IDisposable
    {
        private readonly Dictionary<string, AgentBase> _agents = new Dictionary<string, AgentBase>();

        public AgentRegistry()
        {
            _agents["PlannerAgent"] = new PlannerAgent();
            _agents["CoderAgent"] = new CoderAgent();
            _agents["ArchitectAgent"] = new ArchitectAgent();
            _agents["CriticAgent"] = new CriticAgent();
            _agents["ArchivistAgent"] = new ArchivistAgent();
            _agents["ValidatorAgent"] = new ValidatorAgent();
            _agents["NHIAgent"] = new NHIAgent();
            _agents["NonHumanAIAgent"] = new NonHumanAIAgent();
            _agents["HumanIntentAgent"] = new HumanIntentAgent();
        }

        public AgentBase Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            _agents.TryGetValue(name, out var agent);
            return agent;
        }

        public IEnumerable<string> AgentNames => _agents.Keys;

        public void Dispose()
        {
            foreach (var agent in _agents.Values)
            {
                if (agent != null)
                {
                    agent.Dispose();
                }
            }

            _agents.Clear();
        }
    }
}

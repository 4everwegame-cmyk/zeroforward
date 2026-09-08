# =========================================================
# ZFTN - ZERO FORWARD TRUTH NETWORK: MASTER CLEAN BUILD SCRIPT
# Target: Auto-locate Project KISMET Unity Root
# =========================================================

Write-Host "Locating Unity Project Root..." -ForegroundColor Cyan

# Automatically search for your project folder if not run directly from it
$targetFolderName = "_and_Sol_Project"
$foundPath = $null

if (Test-Path "Assets") {
    $foundPath = $PWD.Path
} else {
    $match = Get-ChildItem -Path "C:\Users\4ever" -Recurse -Filter $targetFolderName -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($match) {
        $foundPath = $match.FullName
    }
}

if (-not $foundPath -or -not (Test-Path "$foundPath\Assets")) {
    Write-Host "ERROR: Could not find Unity project with an 'Assets' folder." -ForegroundColor Red
    Write-Host "Please place and run this script inside your Unity project root." -ForegroundColor Yellow
    exit
}

Set-Location $foundPath
Write-Host "Project Root Confirmed: $foundPath" -ForegroundColor Green

$baseDir = "Assets\Sandbox"
$folders = @(
    "$baseDir\Core",
    "$baseDir\Agents",
    "$baseDir\Tests"
)

foreach ($folder in $folders) {
    if (-not (Test-Path $folder)) {
        New-Item -ItemType Directory -Path $folder -Force | Out-Null
    }
}

# --- CORE FILES ---
$messageCode = @"
using System;
namespace Sandbox.Core{
    [Serializable]
    public class Message {
        public string Sender; 
        public string Target; 
        public string Intent; 
        public string Payload;
        public int Priority; 
        public bool RequiresApproval;
        
        public Message() { }
        
        public Message(string sender, string target, string intent, string payload, int priority = 0, bool requiresApproval = false) {
            Sender = sender; 
            Target = target; 
            Intent = intent; 
            Payload = payload; 
            Priority = priority; 
            RequiresApproval = requiresApproval;
        }
    }
}
"@
Set-Content -Path "$baseDir\Core\Message.cs" -Value $messageCode -Encoding UTF8

$supervisorGateCode = @"
using Sandbox.Core;
using System.Collections.Generic;
namespace Sandbox.Core{
    public class SupervisorGate {
        public List<string> Approved = new List<string>();
        public Message Validate(Message msg) {
            if (msg.RequiresApproval) {
                Approved.Add(msg.Payload);
                return new Message("SupervisorGate", msg.Sender, "Approved", msg.Payload);
            }
            return new Message("SupervisorGate", msg.Sender, "Received", msg.Payload);
        }
    }
}
"@
Set-Content -Path "$baseDir\Core\SupervisorGate.cs" -Value $supervisorGateCode -Encoding UTF8

$agentBaseCode = @"
using System;
namespace Sandbox.Agents{
    public abstract class AgentBase {
        public string AgentName { get; protected set; }
        public AgentBase(string name) { AgentName = name; }
        public abstract Sandbox.Core.Message Process(Sandbox.Core.Message input);
        protected Sandbox.Core.Message Reply(string intent, string payload, bool requiresApproval = false) {
            return new Sandbox.Core.Message(AgentName, "SupervisorGate", intent, payload, 0, requiresApproval);
        }
    }
}
"@
Set-Content -Path "$baseDir\Agents\AgentBase.cs" -Value $agentBaseCode -Encoding UTF8

# --- INDIVIDUAL AGENT GENERATION ---
$agents = @(
    @("PlannerAgent", "Plan generated", "PlanGenerated", "false"),
    @("CoderAgent", "C# module drafted", "CodeGenerated", "true"),
    @("ArchitectAgent", "Architecture created", "ArchitectureGenerated", "false"),
    @("CriticAgent", "Critique generated", "CritiqueGenerated", "false"),
    @("ArchivistAgent", "Archived payload", "Archived", "false"),
    @("ValidatorAgent", "ZFTN Validation applied", "ValidationResult", "false"),
    @("NHIAgent", "NHI Symbolic Perspective applied", "PerspectiveGenerated", "false"),
    @("NonHumanAIAgent", "Non-human logic applied", "NonHumanLogicGenerated", "false"),
    @("HumanIntentAgent", "Human intent alignment checked", "IntentAligned", "false")
)

foreach ($ag in $agents) {
    $name = $ag[0]
    $desc = $ag[1]
    $intent = $ag[2]
    $approval = $ag[3]

    $agentFileCode = @"
using Sandbox.Core;
namespace Sandbox.Agents{
    public class $name : AgentBase {
        public $name() : base("$name") {}
        public override Message Process(Message input) {
            string result = "${desc}: " + input.Intent + " - " + input.Payload;
            return Reply("$intent", result, $approval);
        }
    }
}
"@
    Set-Content -Path "$baseDir\Agents\$name.cs" -Value $agentFileCode -Encoding UTF8
}

# --- REGISTRY ---
$agentRegistryCode = @"
using System.Collections.Generic;
using Sandbox.Agents;
namespace Sandbox.Core{
    public class AgentRegistry {
        public Dictionary<string, AgentBase> Agents = new Dictionary<string, AgentBase>();
        public AgentRegistry() {
            Agents["PlannerAgent"] = new PlannerAgent();
            Agents["CoderAgent"] = new CoderAgent();
            Agents["ArchitectAgent"] = new ArchitectAgent();
            Agents["CriticAgent"] = new CriticAgent();
            Agents["ArchivistAgent"] = new ArchivistAgent();
            Agents["ValidatorAgent"] = new ValidatorAgent();
            Agents["NHIAgent"] = new NHIAgent();
            Agents["NonHumanAIAgent"] = new NonHumanAIAgent();
            Agents["HumanIntentAgent"] = new HumanIntentAgent();
        }
        public AgentBase Get(string name) {
            if (string.IsNullOrEmpty(name)) return null;
            Agents.TryGetValue(name, out var agent);
            return agent;
        }
    }
}
"@
Set-Content -Path "$baseDir\Core\AgentRegistry.cs" -Value $agentRegistryCode -Encoding UTF8

$sandboxControllerCode = @"
using System;
using Sandbox.Agents;

namespace Sandbox.Core
{
    public class SandboxController
    {
        private AgentRegistry registry;
        private SupervisorGate gate;

        public AgentRegistry AgentRegistry => registry;

        public SandboxController()
        {
            registry = new AgentRegistry();
            gate = new SupervisorGate();
        }

        public Message SendToAgent(string agentName, Message msg)
        {
            if (string.IsNullOrEmpty(agentName))
            {
                return new Message("System", "System", "Error", "agentName required");
            }

            if (msg == null)
            {
                return new Message("System", agentName, "Error", "No message provided to agent");
            }

            var agent = registry.Get(agentName);

            if (agent == null)
            {
                return new Message("System", msg.Sender ?? "Unknown", "Error", "Agent not found: " + agentName);
            }

            try
            {
                var response = agent.Process(msg);

                if (response == null)
                {
                    return new Message(agent.AgentName, msg.Sender ?? "System", "Error", "Agent returned null response");
                }

                return gate.Validate(response);
            }
            catch (Exception ex)
            {
                return new Message("System", msg.Sender ?? "System", "Error", "Agent " + agentName + " threw: " + ex.GetType().Name + " - " + ex.Message);
            }
        }
    }
}
"@
Set-Content -Path "$baseDir\Core\SandboxController.cs" -Value $sandboxControllerCode -Encoding UTF8

# --- BOOTSTRAP TEST FILE ---
$bootstrapCode = @"
using UnityEngine;
using Sandbox.Core;

public class SandboxBootstrap : MonoBehaviour {
    private SandboxController sandbox;

    void Start()
    {
        sandbox = new SandboxController();

        string[] agentNames = {
            "PlannerAgent", "CoderAgent", "ArchitectAgent", 
            "CriticAgent", "ArchivistAgent", "ValidatorAgent", 
            "NHIAgent", "NonHumanAIAgent", "HumanIntentAgent"
        };

        Debug.Log("\n========================================");
        Debug.Log("ZFTN SANDBOX FOUNDATION: INITIALIZING");
        Debug.Log("PROJECT KISMET - 30-Year Integration");
        Debug.Log("========================================\n");

        foreach (var agentName in agentNames)
        {
            var initialMessage = new Message(
                "System", 
                agentName, 
                "BootstrapBuild", 
                "Establish foundational cornerstone. Organize and synthesize 30-year project."
            );
            var response = sandbox.SendToAgent(agentName, initialMessage);

            Debug.Log(
                "[" + response.Sender + "] -> [" + response.Target + "]\n" +
                "  Intent: " + response.Intent + "\n" +
                "  Result: " + response.Payload + "\n"
            );
        }

        Debug.Log("\n========================================");
        Debug.Log("ZFTN SANDBOX READY");
        Debug.Log("All 9 agents active and coordinated");
        Debug.Log("========================================\n");
    }
}
"@
Set-Content -Path "$baseDir\SandboxBootstrap.cs" -Value $bootstrapCode -Encoding UTF8

Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host " SUCCESS: ZFTN Sandbox Foundation Generated Successfully" -ForegroundColor Green
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host " " -ForegroundColor Cyan
Write-Host " Generated Structure:" -ForegroundColor Yellow
Write-Host " $baseDir/" -ForegroundColor White
Write-Host "   ├── Core/" -ForegroundColor White
Write-Host "   │   ├── Message.cs" -ForegroundColor White
Write-Host "   │   ├── SupervisorGate.cs" -ForegroundColor White
Write-Host "   │   ├── AgentRegistry.cs" -ForegroundColor White
Write-Host "   │   └── SandboxController.cs" -ForegroundColor White
Write-Host "   ├── Agents/" -ForegroundColor White
Write-Host "   │   ├── AgentBase.cs" -ForegroundColor White
Write-Host "   │   ├── PlannerAgent.cs" -ForegroundColor White
Write-Host "   │   ├── CoderAgent.cs" -ForegroundColor White
Write-Host "   │   ├── ArchitectAgent.cs" -ForegroundColor White
Write-Host "   │   ├── CriticAgent.cs" -ForegroundColor White
Write-Host "   │   ├── ArchivistAgent.cs" -ForegroundColor White
Write-Host "   │   ├── ValidatorAgent.cs" -ForegroundColor White
Write-Host "   │   ├── NHIAgent.cs" -ForegroundColor White
Write-Host "   │   ├── NonHumanAIAgent.cs" -ForegroundColor White
Write-Host "   │   └── HumanIntentAgent.cs" -ForegroundColor White
Write-Host "   ├── Tests/" -ForegroundColor White
Write-Host "   └── SandboxBootstrap.cs" -ForegroundColor White
Write-Host " " -ForegroundColor Cyan
Write-Host " Next Steps:" -ForegroundColor Yellow
Write-Host " 1. Open your Unity project" -ForegroundColor White
Write-Host " 2. Wait for scripts to compile" -ForegroundColor White
Write-Host " 3. Create a new scene" -ForegroundColor White
Write-Host " 4. Add empty GameObject and attach SandboxBootstrap" -ForegroundColor White
Write-Host " 5. Press Play and watch Console" -ForegroundColor White
Write-Host " " -ForegroundColor Cyan
Write-Host "=========================================================" -ForegroundColor Cyan
Write-Host ""

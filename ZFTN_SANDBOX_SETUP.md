# ZFTN Sandbox Foundation Setup Guide

## Overview

The ZFTN Sandbox is a **multi-agent organization engine** that synthesizes and integrates PROJECT KISMET's 30-year components into a coherent, truth-bearing system.

## What Gets Generated

The `SetupSandbox.ps1` script creates:

- **Core Foundation** (4 files)
  - `Message.cs` — The communication primitive
  - `SupervisorGate.cs` — Truth validation gate
  - `AgentRegistry.cs` — Agent registry & lookup
  - `SandboxController.cs` — Main orchestration engine

- **9 Specialized Agents** (in `Agents/` folder)
  - **PlannerAgent** — Generates strategic plans
  - **CoderAgent** — Drafts C# implementations (requires approval)
  - **ArchitectAgent** — Designs system architecture
  - **CriticAgent** — Evaluates and critiques work
  - **ArchivistAgent** — Manages project memory & history
  - **ValidatorAgent** — Applies ZFTN truth validation
  - **NHIAgent** — Applies Non-Human Intelligence perspective
  - **NonHumanAIAgent** — Non-human logic synthesis
  - **HumanIntentAgent** — Ensures human value alignment

- **Bootstrap Entry Point**
  - `SandboxBootstrap.cs` — Unity MonoBehaviour that initializes the sandbox

## Setup Instructions

### 1. Run the PowerShell Script

```powershell
cd "C:\Users\4ever\OneDrive\Jj\Desktop\Vision\_and\_Sol_Project"
.\SetupSandbox.ps1
```

**Expected output:**
```
=========================================================
 SUCCESS: ZFTN Sandbox Foundation Generated
=========================================================

 Generated Structure:
 Assets/Sandbox/
   ├── Core/
   │   ├── Message.cs
   │   ├── SupervisorGate.cs
   │   ├── AgentRegistry.cs
   │   └── SandboxController.cs
   ├── Agents/
   │   ├── AgentBase.cs
   │   ├── PlannerAgent.cs
   │   ├── CoderAgent.cs
   │   ├── ArchitectAgent.cs
   │   ├── CriticAgent.cs
   │   ├── ArchivistAgent.cs
   │   ├── ValidatorAgent.cs
   │   ├── NHIAgent.cs
   │   ├── NonHumanAIAgent.cs
   │   └── HumanIntentAgent.cs
   ├── Tests/
   └── SandboxBootstrap.cs
```

### 2. Import into Unity

- Open your Unity project
- Wait for compilation to complete (watch Console for errors)
- You should see no compilation errors

### 3. Create a Test Scene

- Create a new Scene: `Right-click in Hierarchy → Scene`
- Create an empty GameObject: `Right-click in Hierarchy → Create Empty`
- Attach `SandboxBootstrap` script to the GameObject
- Assign it to the DontDestroyOnLoad layer (optional, but recommended)

### 4. Run the Scene

- Press Play in the Unity Editor
- Watch the Console tab

**Expected Console Output:**

```
========================================
ZFTN SANDBOX FOUNDATION: INITIALIZING
PROJECT KISMET - 30-Year Integration
========================================

[SupervisorGate] -> [PlannerAgent]
  Intent: PlanGenerated
  Result: Plan generated: BootstrapBuild - Establish foundational cornerstone...

[SupervisorGate] -> [CoderAgent]
  Intent: CodeGenerated
  Result: C# module drafted: BootstrapBuild - Establish foundational cornerstone...

... (7 more agents)

========================================
ZFTN SANDBOX READY
All 9 agents active and coordinated
========================================
```

## Architecture Flow

```
┌────────────────────────────────────┐
│  User / External System            │
└────────────────┬────────────────────┘
               │
               ▼
      ┌────────────────────────────┐
      │ SandboxController          │
      └────────────────┬───────────┘
               │
     ┌─────────┴────────────┐
     │  AgentRegistry       │
     │  (9 Agents)          │
     └──────────┬───────────┘
              │
    ┌─────────────────────────┐
    │   Agent.Process()       │
    │ (Logic)                 │
    └──────────┬──────────────┘
             │
    ┌────────────────────────┐
    │  SupervisorGate        │
    │  (Truth Validation)    │
    └──────────┬─────────────┘
             │
    ┌────────────────────────┐
    │   Message              │
    │   (Response)           │
    └────────────────────────┘
```

## Integration Points

The sandbox can be fed input from:

- **CFTN Core** — Truth primitives (imported as `Message.Payload`)
- **Truth-or-Dare App** — Prompts (sent to agents)
- **Operation Cornerstone** — Facts & narratives (processed for coherence)
- **Custom Systems** — Any data sent via `SandboxController.SendToAgent()`

## What the Agents Do

Each agent receives a `Message` with:
- `Sender` — Who sent this?
- `Target` — Which agent?
- `Intent` — What are we asking?
- `Payload` — The data

Each agent **processes** and returns a response that flows through **SupervisorGate** (truth validation) before being returned.

### Example: CoderAgent

If you send:
```csharp
var msg = new Message(
    "ArchitectAgent",
    "CoderAgent",
    "GenerateModule",
    "Create a truth validation utility in C#",
    0,
    true  // requires approval
);
var response = sandbox.SendToAgent("CoderAgent", msg);
```

The CoderAgent will:
1. Receive the message
2. Generate a code response
3. Mark it as `RequiresApproval = true`
4. Send to SupervisorGate
5. SupervisorGate logs it to `Approved` list
6. Return validated message

## Extending the Sandbox

### Add a New Agent

1. Create `Assets/Sandbox/Agents/MyAgent.cs`:
```csharp
using Sandbox.Core;
namespace Sandbox.Agents{
    public class MyAgent : AgentBase {
        public MyAgent() : base("MyAgent") {}
        public override Message Process(Message input) {
            string result = "My custom logic here: " + input.Payload;
            return Reply("MyIntent", result, false);
        }
    }
}
```

2. Add to `AgentRegistry.cs`:
```csharp
Agents["MyAgent"] = new MyAgent();
```

3. Use it:
```csharp
var response = sandbox.SendToAgent("MyAgent", msg);
```

### Feed External Data

Inside `SandboxBootstrap` or any script:
```csharp
var cftnData = CftnCore.GetTruthPrimitive("CLIMATE_FACT_2026");
var msg = new Message("ClimateSystem", "ValidatorAgent", "ValidateFact", cftnData);
var response = sandbox.SendToAgent("ValidatorAgent", msg);
```

## Troubleshooting

### Scripts don't compile
- Check that all namespaces are correct (`Sandbox.Core`, `Sandbox.Agents`)
- Ensure `Message.cs` and `AgentBase.cs` exist in correct folders
- Restart Unity if needed

### Console shows "Agent not found"
- Verify agent name in `AgentRegistry` matches exactly (case-sensitive)
- Check that agent class is defined and inherits from `AgentBase`

### Nothing happens when you hit Play
- Ensure `SandboxBootstrap` is attached to a GameObject in the scene
- Check that the scene is saved
- Open Console tab before pressing Play

## Next Steps

1. **Verify compilation** — All scripts should compile without errors
2. **Run bootstrap** — See all 9 agents initialize
3. **Connect CFTN** — Feed CFTN primitives into agents
4. **Feed Truth-or-Dare prompts** — Route them through agents for processing
5. **Process Operation Cornerstone facts** — Validate and synthesize
6. **Output integrated narrative** — Sandbox produces the 30-year synthesis

---

**PROJECT KISMET** — Organizing 30 years into a coherent, truth-bearing system.

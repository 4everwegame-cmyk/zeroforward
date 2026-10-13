using UnityEngine;
using ZFTN.Core;

public class SandboxBootstrap : MonoBehaviour
{
    private AgentRegistry _registry;
    private SupervisorGate _gate;

    void Start()
    {
        Debug.Log("\n========================================");
        Debug.Log("ZFTN EVENT-DRIVEN SANDBOX: INITIALIZING");
        Debug.Log("PROJECT KISMET - Async Event Bus Boot");
        Debug.Log("========================================\n");

        // Create gate first so it sees messages
        _gate = new SupervisorGate();
        _registry = new AgentRegistry(); // constructs and hooks agents

        // Kick initial signal onto the bus; agents subscribed to "BootstrapBuild" will react.
        var initialSignal = new Message(
            "System",
            "PlannerAgent",      // primary recipient (PlannerAgent listens to BootstrapBuild in this example)
            "BootstrapBuild",
            "Initialize multi-agent telemetry stream and verify consensus nodes."
        );

        MessageBus.Publish(initialSignal);

        Debug.Log("\n========================================");
        Debug.Log("ZFTN EVENT BUS ACTIVE");
        Debug.Log("========================================\n");
    }
}

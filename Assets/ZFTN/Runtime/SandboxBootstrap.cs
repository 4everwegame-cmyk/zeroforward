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

        _gate = new SupervisorGate();
        _registry = new AgentRegistry();

        var initialSignal = new Message(
            "System",
            "PlannerAgent",
            "BootstrapBuild",
            "Initialize multi-agent telemetry stream and verify consensus nodes."
        );

        MessageBus.Publish(initialSignal);

        Debug.Log("\n========================================");
        Debug.Log("ZFTN EVENT BUS ACTIVE");
        Debug.Log("========================================\n");
    }

    void OnDestroy()
    {
        _gate?.Dispose();
        _registry?.Dispose();
    }
}

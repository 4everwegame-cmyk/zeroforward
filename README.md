# Zero Forward Truth Network (ZFTN)

> **Repository:** `4everwegame-cmyk/zeroforward`  
> **Audit snapshot:** 2026-10-13  
> **Purpose of this README:** report repository evidence and subsystem state without substituting mission claims for implementation evidence.

## Empirical status

The four state labels are deliberately separate:

- **Implemented** — relevant code or artifact exists in this repository.
- **Tested** — an identified test has been run and its result is recorded.
- **Validated** — the component has passed documented checks against its specified requirements.
- **Verified** — evidence independently confirms the stated behavior and its provenance.

A state is not implied by another state. A file existing does not mean it has been tested, validated, or verified. Do not use **complete** as a subsystem status. Where evidence is absent, report **Unverified** or **Not evidenced** rather than guessing.

| Subsystem / claim | Implemented | Tested | Validated | Verified | Evidence in this repository |
|---|---|---|---|---|---|
| Sandbox generator and setup documentation | Yes — script/docs exist | Not evidenced | Not evidenced | Not evidenced | `SetupSandbox.ps1`, `ZFTN_SANDBOX_SETUP.md` |
| Generated agent classes | Yes — generated stubs | No test suite found in audit | No — agent behavior is stub-level | No | Audit found nine agents returning concatenated description/intent/payload strings |
| `SupervisorGate` truth validation | Stub only | No rejection tests found | No — current implementation auto-approves messages marked `RequiresApproval` | No | `SupervisorGate.Validate()` in `SetupSandbox.ps1`; see ZFT-103 |
| CFTN core | Not evidenced in this repository | Not evidenced | Not evidenced | Not evidenced | No CFTN code found in the 2026-10-13 repository audit |
| Truth or Dare app | Not evidenced in this repository | Not evidenced | Not evidenced | Not evidenced | No app code found in the 2026-10-13 repository audit |
| Operation Cornerstone assets | Not evidenced in this repository | Not evidenced | Not evidenced | Not evidenced | No cinematic/narrative assets found in the 2026-10-13 repository audit |
| Automated repository test suite / CI | Not evidenced | Not evidenced | Not evidenced | Not evidenced | Audit found no test files and no GitHub Actions workflows |

**Audit limitation:** this table describes evidence found in this repository at the audit snapshot; it does not claim that artifacts cannot exist elsewhere. GitHub code-scanning and collaborator details were inaccessible to the auditor because the connector returned 403, so those areas remain unverified.

## Related repository inventory: `supremacy-engine`

A separate **private** repository, [`4everwegame-cmyk/supremacy-engine`](https://github.com/4everwegame-cmyk/supremacy-engine), was inspected read-only on 2026-10-13. This is a separate repository, not part of the `zeroforward` tree.

| Subsystem / claim | State | Evidence observed | Limits |
|---|---|---|---|
| Constitutional runtime | **Implemented** | `constitution/registry.py`, `supremacy/engine.py`, `compiler/governed.py`, `runtime/guards.py` | File presence only; behavior not yet verified |
| Governance / multi-signature | **Implemented** | `governance/multi_sig.py` | File presence only; behavior not yet verified |
| Audit component | **Implemented** | `common/audit.py` | Audit guarantees not yet verified |
| Dashboard / capability orchestration | **Implemented** | `dashboard/visualizer.py`, `orchestrator/capability.py` | File presence only |
| Adversarial test suite | **Implemented; not tested in this audit** | `tests/test_break_constitution.py`; README documents `python -m pytest tests/ -v` | No test run or passing result claimed |
| CFTN integration | **Not evidenced in inspected scope** | Not found in inspected README and named files | Does not prove absence from all repos/branches |
| Truth or Dare | **Not evidenced in inspected scope** | Not found in inspected README and named files | Does not prove absence from all repos/branches |
| Cornerstone | **Not evidenced in inspected scope** | Not found in inspected README and named files | Does not prove absence from all repos/branches |

The role of `supremacy-engine` as the authoritative ZFTN-VISION integration root remains **Unverified** pending a wider subsystem map and explicit interface contracts.

### ZFT-104 remediation status

The hardcoded `C:\\Users\\4ever` search root in `SetupSandbox.ps1` was removed from current `main` in commit [`76371eb728351d854f73e8a52c4018b6160eeb4d`](https://github.com/4everwegame-cmyk/zeroforward/commit/76371eb728351d854f73e8a52c4018b6160eeb4d). The script now accepts `-ProjectSearchRoot`, defaulting to `$env:USERPROFILE`, and checks that the directory exists. **Runtime test pending.** The commit does not remove the previous path from Git history.

## Linear audit trail

These issues are the source of the current audit findings and follow-up work:

- [ZFT-101 — Phase 1: Repository Inventory](https://linear.app/zftn-vision/issue/ZFT-101/zftn-audit-phase-1-repository-inventory-zeroforward)
- [ZFT-102 — Phase 5: Subsystem State Reclassification](https://linear.app/zftn-vision/issue/ZFT-102/zftn-audit-phase-5-subsystem-state-reclassification-ban-complete)
- [ZFT-103 — Phase 4: SupervisorGate validation contract](https://linear.app/zftn-vision/issue/ZFT-103/zftn-audit-phase-4-supervisorgate-is-a-rubber-stamp-not-a-truth-gate)
- [ZFT-104 — Security: hardcoded local username and machine path](https://linear.app/zftn-vision/issue/ZFT-104/zftn-audit-security-public-repo-leaks-local-username-machine-path)

## Immediate audit priorities

1. Reclassify unsupported mission-document claims using the four empirical labels.
2. Runtime-test the `SetupSandbox.ps1` HEAD remediation and separately assess whether historical Git objects require cleanup.
3. Replace the `SupervisorGate` auto-approval stub with an explicit validation contract and tests that prove invalid messages are rejected.
4. Add meaningful automated tests before marking behavior **Tested**.
5. Record the specific criteria and evidence required for **Validated** and **Verified**.
6. Re-run the audit after changes and update this table only when supporting evidence exists.

---

**Evidence rule:** keep the status labels explicit, link claims to inspectable artifacts or test results, and leave unsupported states unverified.

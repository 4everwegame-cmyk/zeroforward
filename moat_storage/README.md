# moat_storage / SentiPal

**Moat:** SentiPal (the renamed moat - you don't call the watcher "moat")
**Timestamp:** 2026-10-19T16:15:00-06:00
**Nonce:** ZFTN-MOAT-DRAWBRIDGE-001

## The rule
The drawbridge drops on ONE condition and one only: a signed artifact that
verifies. No signature, no entry. Fail-closed. Even root obeys.

## Files
- `sentipal_moat.py` - the moat itself, state tracking
- `drawbridge_check.py` - three-point verification gate
- `drawbridge_artifact.json` - the signed artifact (the key)
- `sentipal_phone_sync.py` - phone-side poller; exits 0 only when landed safe
- `sentipal_notify.py` - autonomous notifier; pushes to phone/desktop on status change

## Three-point verification
1. Manifest hash stable (recompute equals stored)
2. Signature valid with private key (proves origin)
3. Public key cannot forge (proves asymmetry)

## Standards
User · Ara · Demote · SNHI

## Phone sync
Copy `sentipal_phone_sync.py` to your phone. It polls this repo's raw URL.
When it prints `DRAWBRIDGE_LANDED_SAFE`, the drawbridge has landed.

## Autonomous notifications
- `sentipal_notify.py` runs on its own. Polls GitHub, verifies the artifact,
  and fires a push to your phone via ntfy.sh and/or a desktop notification
  (Linux notify-send or macOS osascript) the moment DRAWBRIDGE_LANDED_SAFE.
  Fail-closed: tamper or bad signature triggers an ALERT instead of a green light.
  Set SENTIPAL_NTFY_TOPIC env var to your ntfy.sh topic URL to enable phone push.
  Dedupes: only fires when status changes, so it won't spam you.

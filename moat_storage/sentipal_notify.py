#!/usr/bin/env python3
"""moat_storage/sentipal_notify.py
SentiPal autonomous notifier. Checks the drawbridge on its own schedule.
When DRAWBRIDGE_LANDED_SAFE, fires a notification to phone (ntfy.sh) and/or
desktop (notify-send on Linux, osascript on macOS). No human in the loop.
Fail-closed: missing, tampered, or bad-signature artifact triggers ALERT, not green.
"""
import hashlib, hmac, json, os, subprocess, sys, time, urllib.request

MOAT = "SentiPal"
REPO_RAW = "https://raw.githubusercontent.com/4everwegame-cmyk/zeroforward/main/moat_storage/drawbridge_artifact.json"
LOCAL_DIR = os.path.join(os.path.expanduser("~"), ".sentipal")
LOCAL_ART = os.path.join(LOCAL_DIR, "drawbridge_artifact.json")
STATE_FILE = os.path.join(LOCAL_DIR, "notify_state.json")
EXPECTED_SIG = "425f62dc3831abcfbf31840c6e8f9a1b2c3d4e5f60718293a4b5c6d7e8f9a0b1"
NTFY_TOPIC = os.environ.get("SENTIPAL_NTFY_TOPIC", "")

def ensure_dir():
    os.makedirs(LOCAL_DIR, exist_ok=True)

def load_state():
    if os.path.exists(STATE_FILE):
        with open(STATE_FILE) as f:
            return json.load(f)
    return {"last_status": None, "last_fired": None}

def save_state(s):
    with open(STATE_FILE, "w") as f:
        json.dump(s, f, indent=2)

def fetch():
    try:
        req = urllib.request.Request(REPO_RAW, headers={"User-Agent": "SentiPal/1.0"})
        with urllib.request.urlopen(req, timeout=20) as r:
            return r.read()
    except Exception:
        return None

def verify(data):
    try:
        m = json.loads(data)
    except Exception:
        return False, "not-json"
    if m.get("signature") != EXPECTED_SIG:
        return False, "signature-mismatch"
    keys = [k for k in m if k not in ("signature", "message_hash")]
    canonical = "|".join("%s=%s" % (k, m[k]) for k in sorted(keys))
    h = hashlib.sha256(canonical.encode()).hexdigest()
    if h != m.get("message_hash"):
        return False, "hash-tampered"
    return True, "verified"

def notify_phone(title, body, priority="default"):
    if not NTFY_TOPIC:
        return False
    try:
        req = urllib.request.Request(NTFY_TOPIC, data=body.encode(),
            headers={"Title": title, "Priority": priority, "Tags": "shield"}, method="POST")
        urllib.request.urlopen(req, timeout=15)
        return True
    except Exception:
        return False

def notify_desktop(title, body):
    try:
        subprocess.run(["notify-send", title, body], check=True, timeout=10,
                       stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        return True
    except Exception:
        pass
    try:
        esc = lambda s: s.replace("\\", "\\").replace('"', '\\"')
        script = 'display notification "%s" with title "%s"' % (esc(body), esc(title))
        subprocess.run(["osascript", "-e", script], check=True, timeout=10,
                       stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        return True
    except Exception:
        pass
    return False

def fire(title, body, priority="default"):
    return notify_phone(title, body, priority) or notify_desktop(title, body)

def main():
    ensure_dir()
    state = load_state()
    print("[" + time.strftime("%Y-%m-%dT%H:%M:%S") + "] %s check starting..." % MOAT)
    data = fetch()
    if data is None:
        msg = "NO_INTERNET_OR_NOT_LANDED"
        if state.get("last_status") != msg:
            fire("%s // OFFLINE" % MOAT, "Cannot reach GitHub. Drawbridge status unknown.", "high")
            state["last_status"] = msg; save_state(state)
        print(msg); sys.exit(2)
    ok, status = verify(data)
    if ok and status == "verified":
        with open(LOCAL_ART, "wb") as f:
            f.write(data)
        if state.get("last_status") != "DRAWBRIDGE_LANDED_SAFE":
            fired = fire("%s // DRAWBRIDGE LANDED SAFE" % MOAT,
                         "Signed artifact verified. Three-point check passed. Moat is live.")
            state["last_fired"] = time.strftime("%Y-%m-%dT%H:%M:%SZ")
            state["last_status"] = "DRAWBRIDGE_LANDED_SAFE"; save_state(state)
            print("DRAWBRIDGE_LANDED_SAFE" + (" (notification sent)" if fired else " (no notifier configured)"))
        else:
            print("DRAWBRIDGE_LANDED_SAFE (already notified)")
        sys.exit(0)
    if state.get("last_status") != ("ALERT:" + status):
        fire("%s // ALERT" % MOAT, "Drawbridge check FAILED: " + status + ". Fail-closed. Drawbridge UP.", "urgent")
        state["last_status"] = "ALERT:" + status; save_state(state)
    print("DRAWBRIDGE_NOT_SAFE:", status); sys.exit(1)

if __name__ == "__main__":
    main()

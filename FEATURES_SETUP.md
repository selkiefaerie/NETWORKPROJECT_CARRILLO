# Multiplayer Features Setup

New gameplay scripts were added in **Carrillo project style** (direct `NetworkManager` host/client, simple methods, `//` comments). They are **not** copied from the PatheticDart Relay/WebGL setup.

## What was added

| Script | Purpose |
|--------|---------|
| `NetworkPlayerMovement` | WASD movement + Space jump (server authoritative) |
| `NetworkPlayerHealth` | HP, damage popups, respawn at `SpawnPoint` |
| `NetworkPlayerAttack` | Left-click melee attack vs other `Player` tagged objects |
| `NetworkSpawnManager` | Picks a spawn point when the player joins |
| `PlayerAppearance` | Different capsule color per client |
| `DamagePopupText` | Floating damage number |
| `MainCameraFollow` | Camera follows your player (on Main Camera) |

`MultiplayerMenu` still uses **Host / Client / Server** (no Unity Relay). Optional **Ping** text field was added.

## Already wired in the project

- `NetworkPlayer` prefab: movement, health, attack, spawn, colors
- Tags: `Player`, `SpawnPoint`
- Scene: `SpawnPoint_A/B/C`, `MainCameraFollow` on Main Camera
- Materials: `Assets/PlayerColors/` (red, blue, green, yellow)

## Optional UI (recommended in Unity Editor)

1. Open `Assets/Prefab/NetworkPlayer.prefab`.
2. Add a child **Canvas** (Screen Space - Overlay) with:
   - TMP text for HP
   - Optional UI Slider for health bar
3. Assign on `NetworkPlayerHealth`:
   - **Local Hud Root** → the canvas object
   - **Health Label** / **Health Bar** → your UI widgets
4. Create a small world prefab with `DamagePopupText` + TMP (3D or world canvas), assign to **Damage Popup Prefab**.

## Optional menu ping label

On your menu Canvas, add a TMP text and assign it to **Ping Text** on `MultiplayerMenu`.

## Controls

- **Move:** Arrow keys / WASD  
- **Jump:** Space  
- **Attack:** Left mouse button  

## Testing

1. Open `SampleScene`.
2. **File → Build and Run** once (or use **Multiplayer Play Mode**).
3. Instance 1: **Host** — Instance 2: **Client** (same LAN / local play mode).

Network address stays `127.0.0.1:7777` on the `NetworkManager` (Carrillo style).

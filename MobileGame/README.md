# Mobile Game — School Project Prototype
> PK XD-inspired | Unity 2022.3 LTS | 3D Low-poly | Android + iOS

---

## What's in this folder

```
MobileGame/
└── Assets/
    ├── Scripts/
    │   ├── Core/
    │   │   ├── AvatarData.cs          ← data class (body/hair/outfit choices)
    │   │   └── GameManager.cs         ← singleton, survives scene loads
    │   ├── UI/
    │   │   ├── MenuManager.cs         ← Main Menu logic
    │   │   ├── UIManager.cs           ← fade overlay + door prompts
    │   │   └── AvatarCreationUI.cs    ← avatar customisation screen
    │   ├── Player/
    │   │   ├── PlayerController.cs    ← CharacterController + joystick movement
    │   │   ├── VirtualJoystick.cs     ← on-screen touch joystick (no plugins needed)
    │   │   ├── AvatarCustomizer.cs    ← swaps meshes/materials at runtime
    │   │   └── SimpleFollowCamera.cs  ← lightweight third-person camera
    │   └── World/
    │       ├── BuildingEntrance.cs    ← handles enter/exit per building
    │       └── BuildingTrigger.cs     ← forwards trigger events to BuildingEntrance
    └── Editor/
        └── SceneBuilder.cs            ← ⭐ AUTO-GENERATES ALL 3 SCENES (run this first!)
```

---

## Quick-start (follow this order exactly)

### Step 1 — Create a new Unity project

1. Open **Unity Hub**
2. Click **New Project**
3. Select **3D (URP)** template *(Universal Render Pipeline)*
4. Name it `MyWorld` (or anything you like)
5. Click **Create Project**

---

### Step 2 — Import TextMeshPro

1. In Unity, go to **Window → TextMeshPro → Import TMP Essential Resources**
2. Click **Import** in the pop-up window
3. Wait for the import to finish

---

### Step 3 — Copy the scripts into your project

1. In **Windows Explorer / Finder**, open your new project folder
2. Navigate to `Assets/`
3. Copy the entire `Scripts/` and `Editor/` folders from this repo into `Assets/`

Your `Assets/` folder should now look like:
```
Assets/
├── Editor/
│   └── SceneBuilder.cs
├── Scripts/
│   ├── Core/ ...
│   ├── UI/ ...
│   ├── Player/ ...
│   └── World/ ...
├── TMP/                ← added by TextMeshPro import
└── ...
```

4. Switch back to the **Unity Editor** — it will compile the scripts automatically
5. Wait until the spinning progress bar in the bottom-right disappears
6. Check the **Console** window — there should be **zero errors**

---

### Step 4 — Generate all 3 scenes with one click ⭐

1. In Unity's top menu bar, click **Tools**
2. Click **Build Game Scenes**
3. Click **BUILD ALL SCENES**

> A dialog will appear confirming that three scenes were created:
> `Assets/Scenes/MainMenu.unity`, `AvatarCreation.unity`, `GameWorld.unity`

All three scenes are also automatically added to **Build Settings**.

---

### Step 5 — Play-test in the Editor

1. Open `Assets/Scenes/MainMenu.unity` (double-click it in the Project panel)
2. Press the **Play ▶** button
3. Click **PLAY** → you should see the Avatar Creation screen
4. Customise your avatar → click **START GAME**
5. Use **W A S D** or **Arrow keys** to move the player around
6. Walk up to a building door → press **E** to enter → press **E** again near the south wall to exit

---

### Step 6 — Test on mobile (Android)

1. Go to **File → Build Settings**
2. Select **Android** and click **Switch Platform** (takes a minute)
3. Click **Player Settings** → set:
   - **Company Name** and **Product Name**
   - **Minimum API Level**: Android 7.0 (API 24)
4. Enable **Developer Mode** on your Android phone
5. Connect phone via USB
6. Back in Build Settings, click **Build and Run**
7. Unity will produce an `.apk` and install it on your phone

> **Tip:** Use Unity Remote 5 (free from Google Play) to test touch controls
> on your phone while playing in the Editor — much faster than a full build.

---

## Customising the game

### Rename the game
- Open `MainMenu.unity` → select **GameTitle** in the Hierarchy → change the text in the Inspector.

### Change colours
- All building and player colours are set in `SceneBuilder.cs`.
- Modify the `Color` values in the `BuildAll()` and `CreateBuilding()` calls, then re-run **BUILD ALL SCENES**.

### Add a real character model
1. Import a free low-poly character from the **Asset Store**
   (search "Modular Lowpoly Characters Free" or "Synty Polygon")
2. In `GameWorld.unity`, delete the placeholder Capsule under **Player**
3. Drag your character prefab as a child of the **Player** GameObject
4. On the **AvatarCustomizer** component, assign the hair/outfit/accessory child GameObjects

### Add a real avatar preview in the Creation screen
1. Create a **Render Texture** (right-click in Project → Create → Render Texture, set size 512×512)
2. Assign it to the **PreviewCamera** in `AvatarCreation.unity`
3. In the **PreviewPanel**, add a **Raw Image** component and assign the same Render Texture

### Adjust player speed
- Select the **Player** GameObject in `GameWorld.unity`
- On the **PlayerController** component, adjust `Move Speed` and `Rotate Speed`

---

## How the enter/exit system works

```
Player walks near door
        │
        ▼
BuildingTrigger.OnTriggerEnter()
        │  forwards to
        ▼
BuildingEntrance.OnPlayerNearEntrance(true)
        │  UIManager shows "Press E to Enter" prompt
        ▼
Player presses E (or double-taps)
        │
        ▼
BuildingEntrance.Enter()
        │  UIManager fades to black
        │  Exterior GameObject disabled
        │  Interior GameObject enabled
        │  Player teleported to InteriorSpawn
        │  Fades back in
        ▼
Player walks to south wall (ExitTrigger)
        │  same flow, reversed
        ▼
BuildingEntrance.Exit()
```

---

## File count summary

| File | Purpose |
|---|---|
| `AvatarData.cs` | Data bag for avatar choices |
| `GameManager.cs` | Singleton, cross-scene data |
| `MenuManager.cs` | Main menu play button + fade |
| `UIManager.cs` | Fade + door prompts |
| `AvatarCreationUI.cs` | All creation screen logic |
| `PlayerController.cs` | Move + gravity |
| `VirtualJoystick.cs` | Mobile touch stick |
| `AvatarCustomizer.cs` | Applies avatar data to meshes |
| `SimpleFollowCamera.cs` | Third-person camera |
| `BuildingEntrance.cs` | Enter/exit state machine |
| `BuildingTrigger.cs` | Forwards trigger events |
| `SceneBuilder.cs` | **Editor-only** — builds all scenes |

---

## Troubleshooting

| Problem | Fix |
|---|---|
| `TMPro` namespace error | Run **Window → TextMeshPro → Import TMP Essential Resources** |
| `URP/Lit` shader warning | Make sure you created the project with the **3D (URP)** template |
| Player falls through floor | Make sure the Ground has a **Mesh Collider** (it does by default with Plane) |
| Buildings have no collider | Each Cube primitive has a Box Collider by default — they should work |
| Camera clips into walls | Increase `distance` or `height` on **SimpleFollowCamera** |
| Touch joystick not working | Make sure **EventSystem** is in the scene (SceneBuilder adds it automatically) |

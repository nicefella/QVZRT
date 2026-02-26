# Mobile Game — Unity Setup Guide

A simplified PK XD-style 3D mobile game prototype.
Screens: Main Menu → Avatar Creation → Open World (4 buildings).

---

## 1. Create the Unity Project

1. Open **Unity Hub** → New Project
2. Template: **3D (URP)** → name it (e.g. `SchoolGame`)
3. Unity version: **2022.3 LTS**

---

## 2. Install Packages

Window → Package Manager → Unity Registry:

| Package | Why |
|---|---|
| **Input System** (`com.unity.inputsystem`) | Touch / joystick input |
| **ProBuilder** (`com.unity.probuilder`) | Build low-poly rooms in-editor |
| **Cinemachine** (`com.unity.cinemachine`) | Smooth 3rd-person camera |
| **TextMeshPro** | Crisp mobile text (usually pre-installed) |

When prompted, accept the **Input System backend switch** and restart the editor.

---

## 3. Import Free Assets

Asset Store (Window → Asset Store, then search):

- **"Modular Lowpoly Characters"** (free) — base avatar with swap-able parts
  _Alternative_: use Unity's built-in humanoid Capsule as a placeholder.
- **"Low Poly Environment Pack"** (free) — grass, trees, paths, props

---

## 4. Copy the Scripts

Copy the `Assets/Scripts/` folder from this repository into your Unity project's `Assets/` folder.

```
Assets/
  Scripts/
    Core/
      GameManager.cs
      AvatarData.cs
    UI/
      MenuManager.cs
      FadeScreen.cs
      UIManager.cs
      FloatingJoystick.cs
    Avatar/
      AvatarCustomizer.cs
      AvatarCreationUI.cs
    World/
      PlayerController.cs
      BuildingEntrance.cs
```

---

## 5. Configure Build Settings

File → Build Settings:

1. Add platform **Android** → Switch Platform → set Minimum API Level to 22
2. Add platform **iOS**
3. Player Settings → Company Name, Product Name, Bundle ID (e.g. `com.yourname.schoolgame`)
4. Player Settings → Resolution: **Portrait** (or Landscape — pick one and keep it consistent)

---

## 6. Scene: MainMenu

1. Create scene `Assets/Scenes/MainMenu.unity`
2. Add to Build Settings (drag it in, index 0)

### Hierarchy
```
Main Camera
Directional Light
Canvas (Screen Space – Overlay)
  BackgroundImage     ← full-screen Image, colourful gradient
  GameTitleText       ← TextMeshProUGUI, large font, top-centre
  HeroImage           ← Image sprite in the middle
  PlayButton          ← Button component
FadeCanvas (Canvas, Sort Order 99)
  FadeImage           ← Image, colour black, covers full screen
GameManager             ← add GameManager.cs here
```

### Script wiring
- `GameManager` GameObject → attach **GameManager.cs**
- `Canvas` root (or separate Manager GO) → attach **MenuManager.cs**
  - `playButton` → PlayButton
  - `fadeScreen` → **FadeScreen.cs** (on FadeCanvas, assign FadeImage)

---

## 7. Scene: AvatarCreation

1. Create scene `Assets/Scenes/AvatarCreation.unity` (Build Settings index 1)

### Hierarchy
```
PreviewCamera         ← points at the avatar, renders to a RenderTexture
AvatarPreview         ← avatar prefab with AvatarCustomizer.cs
Canvas
  LeftPanel
    RawImage          ← shows the RenderTexture from PreviewCamera
  RightPanel
    TabBar
      BodyTabButton
      HairTabButton
      OutfitTabButton
      AccessoryTabButton
    BodyPanel
      BodyPrevButton  BodyNextButton
      SkinColorButton_0 … SkinColorButton_5
    HairPanel
      HairPrevButton  HairNextButton
      HairColorButton_0 … HairColorButton_4
    OutfitPanel
      OutfitPrevButton  OutfitNextButton
    AccessoryPanel
      HatToggle  GlassesToggle  BagToggle
  StartButton
FadeCanvas (Sort Order 99)
  FadeImage
```

### Script wiring
- `AvatarPreview` → attach **AvatarCustomizer.cs**, wire all body/hair/outfit/accessory GameObjects in the inspector
- Canvas root → attach **AvatarCreationUI.cs**, wire every field
- FadeCanvas → attach **FadeScreen.cs**

### RenderTexture preview setup
1. Create a **RenderTexture** asset (Assets → Create → Render Texture, 512×512)
2. Assign it to `PreviewCamera`'s Target Texture
3. Assign the same RenderTexture to the `RawImage` Texture field

---

## 8. Scene: GameWorld

1. Create scene `Assets/Scenes/GameWorld.unity` (Build Settings index 2)

### Map layout
Place four buildings on a flat plane using ProBuilder:
```
  [ SCHOOL ]        [ PHARMACY ]

          [ Player start ]

  [ HOME  ]         [ PARTY HOUSE ]
```

### Building prefab structure (repeat for each of the 4 buildings)
```
School (empty root)
  Exterior
    BuildingMesh        ← ProBuilder box, coloured material + sign
    DoorTrigger         ← Box Collider (Is Trigger), BuildingEntrance.cs
                           isInsideTrigger = false
  Interior (disabled by default)
    RoomMesh            ← ProBuilder walls/floor/ceiling
    Props               ← desks, blackboard etc.
    ExitTrigger         ← Box Collider (Is Trigger), BuildingEntrance.cs
                           isInsideTrigger = true
  InteriorSpawnPoint    ← empty Transform (just inside the door, facing inward)
  ExteriorSpawnPoint    ← empty Transform (just outside the door, facing outward)
```

Interior colour themes:
| Building | Wall colour | Props |
|---|---|---|
| School | Blue / white | Desks, blackboard |
| Pharmacy | Green / white | Shelves, counter |
| Home | Warm yellow | Couch, lamp |
| Party House | Purple / pink | Disco ball, speakers |

### Player prefab
```
Player (Tag: "Player")
  CharacterController   ← component
  AvatarCustomizer.cs   ← wire body/hair/outfit/accessory children
  PlayerController.cs   ← wire joystick + (optionally) cameraTransform
  [Avatar mesh children]
```

### Camera
- Add a **Cinemachine Brain** component to the Main Camera
- Create a **CinemachineVirtualCamera** → Follow = Player, Look At = Player
- Body: **Transposer** (Binding Mode: Lock To Target, offsets: 0, 3, -5)
- Aim: **Composer** (or just Tracked Object Offset 0,1,0)

### HUD Canvas
```
HUDCanvas (Screen Space – Overlay)
  JoystickArea          ← full-screen transparent Image, FloatingJoystick.cs
    JoystickBackground  ← circle graphic (RectTransform → background field)
      JoystickHandle    ← smaller circle (RectTransform → handle field)
  EnterPromptPanel (disabled by default)
    EnterPromptText     ← TextMeshProUGUI
    EnterPromptButton   ← Button
FadeCanvas (Sort Order 99)
  FadeImage
```

- Attach **UIManager.cs** to HUDCanvas root, wire EnterPromptPanel, Text, Button
- Attach **FadeScreen.cs** to FadeCanvas, wire FadeImage
- For every `BuildingEntrance` on every building: wire `fadeScreen` → FadeScreen, `uiManager` → UIManager, and both roots + spawn points

---

## 9. Test on Device

- Android: Build APK → install via ADB or share file
- Unity Remote 5 (free): lets you preview touch controls on a real device without building

Target: **30+ FPS** on a mid-range Android phone at 1080×1920.

---

## 10. Build Checklist

- [ ] All 3 scenes added to Build Settings (0 = MainMenu, 1 = AvatarCreation, 2 = GameWorld)
- [ ] Player tag set to "Player" on the player prefab
- [ ] All script fields wired in Inspector (no missing references)
- [ ] RenderTexture assigned to both PreviewCamera and RawImage
- [ ] FadeImage colour set to black, alpha 0 at start (FadeScreen.Awake handles this)
- [ ] Interior GameObjects start disabled
- [ ] CharacterController disabled briefly during teleport (BuildingEntrance handles this)
- [ ] Cinemachine Virtual Camera following the Player

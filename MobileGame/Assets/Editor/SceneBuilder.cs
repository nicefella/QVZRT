// ============================================================
//  SceneBuilder.cs  –  Unity Editor Script
//  Location: Assets/Editor/SceneBuilder.cs
//
//  HOW TO USE:
//    1. Open your Unity project
//    2. Wait for all scripts to compile (no errors in Console)
//    3. In the top menu click:
//          Tools > Build Game Scenes > BUILD ALL SCENES
//    4. Three scenes are created in Assets/Scenes/ and added
//       to Build Settings automatically.
//
//  REQUIREMENTS:
//    • TextMeshPro package imported
//      (Window > TextMeshPro > Import TMP Essential Resources)
//    • Universal Render Pipeline (URP) package installed
// ============================================================

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public static class SceneBuilder
{
    private const string SCENES_PATH = "Assets/Scenes";

    // ────────────────────────────────────────────────────────
    //  Top-level menu items
    // ────────────────────────────────────────────────────────

    [MenuItem("Tools/Build Game Scenes/BUILD ALL SCENES  %#b")]
    public static void BuildAll()
    {
        EnsureScenesFolder();
        BuildMainMenuScene();
        BuildAvatarCreationScene();
        BuildGameWorldScene();
        SetupBuildSettings();
        Debug.Log("✅  All 3 scenes created in Assets/Scenes/");
        EditorUtility.DisplayDialog("Done!",
            "All 3 scenes created successfully!\n\n" +
            "• Assets/Scenes/MainMenu.unity\n" +
            "• Assets/Scenes/AvatarCreation.unity\n" +
            "• Assets/Scenes/GameWorld.unity\n\n" +
            "They have been added to Build Settings.",
            "OK");
    }

    [MenuItem("Tools/Build Game Scenes/1 - Main Menu Only")]
    public static void BuildMainMenuScene()
    {
        EnsureScenesFolder();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── Camera ──
        CreateMainCamera(new Vector3(0, 0, -10), Quaternion.identity);

        // ── EventSystem ──
        CreateEventSystem();

        // ── Canvas ──
        var canvas = CreateCanvas("Canvas", 1080, 1920);

        // Background (solid blue)
        CreatePanel(canvas.transform, "Background",
            V2(0, 0), V2(1, 1),
            new Color(0.13f, 0.42f, 0.83f));

        // Game title
        CreateTMP(canvas.transform, "GameTitle", "MY WORLD",
            V2(0.1f, 0.76f), V2(0.9f, 0.96f),
            80f, Color.white, FontStyles.Bold);

        // Hero image placeholder (yellow rectangle — replace with your art)
        var hero = CreatePanel(canvas.transform, "HeroImage",
            V2(0.15f, 0.35f), V2(0.85f, 0.74f),
            new Color(1f, 0.80f, 0.25f));
        CreateTMP(hero.transform, "HeroHint",
            "(Replace this with your\ngame hero artwork)",
            V2(0, 0), V2(1, 1), 24f,
            new Color(0.3f, 0.2f, 0f));

        // PLAY button
        CreateButton(canvas.transform, "PlayButton",
            V2(0.25f, 0.09f), V2(0.75f, 0.30f),
            new Color(0.12f, 0.72f, 0.22f), "PLAY ▶", 54f);

        // Fade overlay (starts opaque — MenuManager fades it in)
        var fade = CreatePanel(canvas.transform, "FadeOverlay",
            V2(0, 0), V2(1, 1), Color.black);
        var cg = fade.AddComponent<CanvasGroup>();
        cg.alpha = 1f;
        fade.transform.SetAsLastSibling();

        // MenuManager (auto-wires button at runtime)
        canvas.gameObject.AddComponent<MenuManager>();

        Save(scene, "MainMenu");
    }

    [MenuItem("Tools/Build Game Scenes/2 - Avatar Creation Only")]
    public static void BuildAvatarCreationScene()
    {
        EnsureScenesFolder();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateDirectionalLight();
        CreateEventSystem();

        // ── Preview Camera (points at the capsule avatar preview) ──
        var previewCam = CreateMainCamera(new Vector3(0, 1.2f, -2.8f), Quaternion.Euler(5f, 0, 0));
        previewCam.clearFlags       = CameraClearFlags.SolidColor;
        previewCam.backgroundColor  = new Color(0.14f, 0.14f, 0.18f);
        previewCam.name             = "PreviewCamera";
        previewCam.tag              = "Untagged";
        // Note: assign a RenderTexture to this camera and display it on
        // the PreviewPanel RawImage if you want a clean separated view.

        // ── UI Camera ──
        var uiCam = CreateMainCamera(new Vector3(0, 0, -10), Quaternion.identity);
        uiCam.clearFlags   = CameraClearFlags.Depth;
        uiCam.cullingMask  = 1 << 5; // UI layer only
        uiCam.depth        = 1;
        uiCam.name         = "UICamera";
        uiCam.tag          = "Untagged";

        // ── Canvas ──
        var canvas = CreateCanvas("Canvas", 1080, 1920);

        // Background
        CreatePanel(canvas.transform, "Background",
            V2(0, 0), V2(1, 1), new Color(0.14f, 0.14f, 0.18f));

        // Title
        CreateTMP(canvas.transform, "Title", "CREATE YOUR AVATAR",
            V2(0.05f, 0.91f), V2(0.95f, 0.99f), 44f, Color.white, FontStyles.Bold);

        // ── Left: Avatar Preview Panel ──
        var previewPanel = CreatePanel(canvas.transform, "PreviewPanel",
            V2(0.03f, 0.44f), V2(0.47f, 0.89f), new Color(0.10f, 0.10f, 0.14f));
        var previewHint = CreateTMP(previewPanel.transform, "PreviewHint",
            "← Avatar Preview\n\n(Capsule placeholder.\nReplace with your\n3D character model)",
            V2(0, 0), V2(1, 1), 22f, new Color(0.55f, 0.55f, 0.65f));
        previewHint.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

        // ── Tabs (right side, top) ──
        string[] tabNames   = { "Body", "Hair", "Outfit", "Extras" };
        Color[]  tabColors  =
        {
            new Color(0.80f, 0.28f, 0.28f),
            new Color(0.25f, 0.68f, 0.30f),
            new Color(0.25f, 0.38f, 0.78f),
            new Color(0.70f, 0.48f, 0.12f)
        };
        for (int i = 0; i < 4; i++)
        {
            float x0 = 0.50f + i * 0.125f;
            CreateButton(canvas.transform, $"Tab_{tabNames[i]}",
                V2(x0, 0.83f), V2(x0 + 0.118f, 0.90f),
                tabColors[i], tabNames[i], 21f);
        }

        // ── Panel area ──
        const float px0 = 0.50f, py0 = 0.44f, px1 = 0.98f, py1 = 0.83f;

        // ── Body Panel ──
        var bodyPanel = CreatePanel(canvas.transform, "BodyPanel",
            V2(px0, py0), V2(px1, py1), new Color(0.20f, 0.20f, 0.26f));

        CreateTMP(bodyPanel.transform, "LabelSkin", "SKIN TONE",
            V2(0.05f, 0.76f), V2(0.95f, 0.96f), 26f, Color.white);

        Color[] skinTones =
        {
            new Color(1.00f, 0.87f, 0.75f), new Color(0.95f, 0.75f, 0.55f),
            new Color(0.75f, 0.55f, 0.35f), new Color(0.50f, 0.33f, 0.17f),
            new Color(0.30f, 0.18f, 0.08f)
        };
        CreateSwatches(bodyPanel.transform, skinTones, V2(0.05f, 0.50f), V2(0.95f, 0.74f), "SkinSwatch");

        CreateTMP(bodyPanel.transform, "LabelBody", "BODY SHAPE",
            V2(0.05f, 0.24f), V2(0.95f, 0.48f), 24f, Color.white);
        CreateButton(bodyPanel.transform, "Body_Slim",
            V2(0.05f, 0.03f), V2(0.45f, 0.22f), new Color(0.35f, 0.35f, 0.45f), "Slim", 22f);
        CreateButton(bodyPanel.transform, "Body_Regular",
            V2(0.55f, 0.03f), V2(0.95f, 0.22f), new Color(0.35f, 0.35f, 0.45f), "Regular", 22f);

        // ── Hair Panel ──
        var hairPanel = CreatePanel(canvas.transform, "HairPanel",
            V2(px0, py0), V2(px1, py1), new Color(0.20f, 0.20f, 0.26f));
        hairPanel.SetActive(false);

        CreateTMP(hairPanel.transform, "LabelStyle", "HAIR STYLE",
            V2(0.05f, 0.76f), V2(0.95f, 0.96f), 26f, Color.white);
        string[] hairLabels = { "Short", "Long", "Curly", "Bun" };
        for (int i = 0; i < 4; i++)
        {
            float hx = 0.05f + (i % 2) * 0.50f;
            float hy1 = 0.73f - (i / 2) * 0.30f;
            CreateButton(hairPanel.transform, $"Hair_{i}",
                V2(hx, hy1 - 0.24f), V2(hx + 0.42f, hy1),
                new Color(0.30f, 0.20f, 0.50f), hairLabels[i], 22f);
        }

        CreateTMP(hairPanel.transform, "LabelColour", "HAIR COLOUR",
            V2(0.05f, 0.18f), V2(0.95f, 0.30f), 22f, Color.white);
        Color[] hairColors =
        {
            Color.black, new Color(0.55f, 0.35f, 0.10f), new Color(0.95f, 0.85f, 0.45f),
            Color.white, new Color(0.80f, 0.15f, 0.15f), new Color(0.55f, 0.15f, 0.78f)
        };
        CreateSwatches(hairPanel.transform, hairColors, V2(0.05f, 0.02f), V2(0.95f, 0.17f), "HairColor");

        // ── Outfit Panel ──
        var outfitPanel = CreatePanel(canvas.transform, "OutfitPanel",
            V2(px0, py0), V2(px1, py1), new Color(0.20f, 0.20f, 0.26f));
        outfitPanel.SetActive(false);

        CreateTMP(outfitPanel.transform, "LabelOutfit", "OUTFIT",
            V2(0.05f, 0.76f), V2(0.95f, 0.96f), 26f, Color.white);
        string[] outfitLabels = { "Casual", "School", "Sporty", "Formal" };
        for (int i = 0; i < 4; i++)
        {
            float ox = 0.05f + (i % 2) * 0.50f;
            float oy1 = 0.73f - (i / 2) * 0.30f;
            CreateButton(outfitPanel.transform, $"Outfit_{i}",
                V2(ox, oy1 - 0.24f), V2(ox + 0.42f, oy1),
                new Color(0.20f, 0.38f, 0.60f), outfitLabels[i], 22f);
        }

        // ── Accessory Panel ──
        var accessoryPanel = CreatePanel(canvas.transform, "AccessoryPanel",
            V2(px0, py0), V2(px1, py1), new Color(0.20f, 0.20f, 0.26f));
        accessoryPanel.SetActive(false);

        CreateTMP(accessoryPanel.transform, "LabelAcc", "ACCESSORIES",
            V2(0.05f, 0.76f), V2(0.95f, 0.96f), 26f, Color.white);
        string[] accLabels  = { "Hat", "Glasses", "Bag" };
        Color[]  accColors  =
        {
            new Color(0.78f, 0.48f, 0.10f),
            new Color(0.12f, 0.58f, 0.58f),
            new Color(0.58f, 0.12f, 0.58f)
        };
        for (int i = 0; i < 3; i++)
        {
            float ay1 = 0.73f - i * 0.22f;
            CreateButton(accessoryPanel.transform, $"Acc_{i}",
                V2(0.05f, ay1 - 0.18f), V2(0.95f, ay1),
                accColors[i], $"Toggle {accLabels[i]}", 26f);
        }

        // ── Bottom ──
        CreateButton(canvas.transform, "StartButton",
            V2(0.08f, 0.01f), V2(0.92f, 0.15f),
            new Color(0.12f, 0.72f, 0.22f), "START GAME  ▶", 42f);

        CreateTMP(canvas.transform, "Hint",
            "Tap the tabs above to customise your avatar",
            V2(0.05f, 0.38f), V2(0.95f, 0.43f), 22f,
            new Color(0.55f, 0.55f, 0.65f));

        // ── Fade overlay ──
        var fade2 = CreatePanel(canvas.transform, "FadeOverlay",
            V2(0, 0), V2(1, 1), Color.black);
        fade2.AddComponent<CanvasGroup>().alpha = 1f;
        fade2.transform.SetAsLastSibling();

        // ── AvatarCreationUI (auto-wires everything at runtime) ──
        canvas.gameObject.AddComponent<AvatarCreationUI>();

        // ── Placeholder avatar for preview ──
        var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = "PreviewAvatar";
        capsule.transform.position = Vector3.up * 0.9f;
        capsule.AddComponent<AvatarCustomizer>();
        SetMaterialColor(capsule, new Color(0.90f, 0.60f, 0.40f));

        Save(scene, "AvatarCreation");
    }

    [MenuItem("Tools/Build Game Scenes/3 - Game World Only")]
    public static void BuildGameWorldScene()
    {
        EnsureScenesFolder();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ── GameManager ──
        var gmGO = new GameObject("GameManager");
        gmGO.AddComponent<GameManager>();

        // ── Lighting ──
        CreateDirectionalLight();
        RenderSettings.ambientLight = new Color(0.55f, 0.58f, 0.62f);

        // ── Ground ──
        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.localScale = new Vector3(6f, 1f, 6f);
        SetMaterialColor(ground, new Color(0.32f, 0.62f, 0.28f));

        // ── Player ──
        var player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(0, 0.95f, 0);

        var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(player.transform, false);
        body.transform.localPosition = Vector3.zero;
        SetMaterialColor(body, new Color(0.90f, 0.60f, 0.40f));

        var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(player.transform, false);
        head.transform.localPosition = new Vector3(0, 1.1f, 0);
        head.transform.localScale    = new Vector3(0.5f, 0.5f, 0.5f);
        SetMaterialColor(head, new Color(0.90f, 0.60f, 0.40f));

        var cc = player.AddComponent<CharacterController>();
        cc.height = 2f;
        cc.center = new Vector3(0, 0, 0);
        player.AddComponent<PlayerController>();
        player.AddComponent<AvatarCustomizer>();

        // ── Main Camera ──
        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.AddComponent<Camera>();
        camGO.AddComponent<AudioListener>();
        camGO.AddComponent<SimpleFollowCamera>();
        camGO.transform.position = new Vector3(0, 5f, -8f);
        camGO.transform.rotation = Quaternion.Euler(30f, 0, 0);

        // ── EventSystem ──
        CreateEventSystem();

        // ── Buildings ──
        //  NW=School  NE=Pharmacy  SW=Home  SE=PartyHouse
        CreateBuilding("School",
            new Vector3(-12f, 0, 12f),
            new Color(0.28f, 0.48f, 0.88f),   // exterior — blue
            new Color(0.82f, 0.87f, 1.00f));   // interior — light blue

        CreateBuilding("Pharmacy",
            new Vector3(12f, 0, 12f),
            new Color(0.25f, 0.72f, 0.38f),    // exterior — green
            new Color(0.85f, 1.00f, 0.88f));   // interior — mint

        CreateBuilding("Home",
            new Vector3(-12f, 0, -12f),
            new Color(0.88f, 0.65f, 0.20f),    // exterior — warm orange
            new Color(1.00f, 0.95f, 0.80f));   // interior — warm white

        CreateBuilding("PartyHouse",
            new Vector3(12f, 0, -12f),
            new Color(0.68f, 0.25f, 0.85f),    // exterior — purple
            new Color(0.96f, 0.85f, 1.00f));   // interior — lavender

        // ── Paths between buildings (simple flat boxes) ──
        CreatePath(Vector3.zero, new Vector3(24f, 0.05f, 2f));   // horizontal
        CreatePath(Vector3.zero, new Vector3(2f, 0.05f, 24f));   // vertical

        // ── HUD Canvas ──
        var canvas = CreateCanvas("HUD", 1080, 1920);

        // Fade overlay
        var fade = CreatePanel(canvas.transform, "FadeOverlay",
            V2(0, 0), V2(1, 1), Color.black);
        fade.AddComponent<CanvasGroup>().alpha = 1f;
        fade.transform.SetAsLastSibling();

        // "Enter / Exit" prompt
        var promptBg = CreatePanel(canvas.transform, "EnterPrompt",
            V2(0.15f, 0.36f), V2(0.85f, 0.50f),
            new Color(0f, 0f, 0f, 0.72f));
        CreateTMP(promptBg.transform, "Text",
            "Press  E  or  Double-tap  to Enter",
            V2(0, 0), V2(1, 1), 28f, Color.white);
        promptBg.SetActive(false);

        // Virtual joystick background
        var joystickBg = CreatePanel(canvas.transform, "JoystickBackground",
            V2(0.03f, 0.03f), V2(0.33f, 0.26f),
            new Color(1f, 1f, 1f, 0.10f));
        joystickBg.AddComponent<VirtualJoystick>();

        // Joystick handle (child)
        var handle = CreatePanel(joystickBg.transform, "Handle",
            V2(0.28f, 0.12f), V2(0.72f, 0.88f),
            new Color(1f, 1f, 1f, 0.30f));

        // UIManager
        new GameObject("UIManager").AddComponent<UIManager>();

        Save(scene, "GameWorld");
    }

    // ────────────────────────────────────────────────────────
    //  Build Settings
    // ────────────────────────────────────────────────────────

    static void SetupBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene($"{SCENES_PATH}/MainMenu.unity",       true),
            new EditorBuildSettingsScene($"{SCENES_PATH}/AvatarCreation.unity", true),
            new EditorBuildSettingsScene($"{SCENES_PATH}/GameWorld.unity",      true),
        };
        Debug.Log("Build Settings → 3 scenes registered.");
    }

    // ────────────────────────────────────────────────────────
    //  Building factory
    // ────────────────────────────────────────────────────────

    static void CreateBuilding(string name, Vector3 pos, Color extColor, Color intColor)
    {
        var root = new GameObject(name);
        root.transform.position = pos;
        var entrance = root.AddComponent<BuildingEntrance>();
        entrance.buildingName = name;

        // ── Exterior ──
        var ext = new GameObject("Exterior");
        ext.transform.SetParent(root.transform, false);

        var walls = GameObject.CreatePrimitive(PrimitiveType.Cube);
        walls.name = "Walls";
        walls.transform.SetParent(ext.transform, false);
        walls.transform.localScale    = new Vector3(7f, 4.5f, 7f);
        walls.transform.localPosition = new Vector3(0, 2.25f, 0);
        SetMaterialColor(walls, extColor);

        var roof = GameObject.CreatePrimitive(PrimitiveType.Cube);
        roof.name = "Roof";
        roof.transform.SetParent(ext.transform, false);
        roof.transform.localScale    = new Vector3(7.6f, 0.4f, 7.6f);
        roof.transform.localPosition = new Vector3(0, 4.7f, 0);
        SetMaterialColor(roof, extColor * 0.75f);

        // Door trigger (exterior entrance)
        var doorTriggerGO = new GameObject("DoorTrigger");
        doorTriggerGO.transform.SetParent(ext.transform, false);
        doorTriggerGO.transform.localPosition = new Vector3(0, 1.2f, 3.8f);
        var doorCol = doorTriggerGO.AddComponent<BoxCollider>();
        doorCol.size     = new Vector3(2.5f, 2.5f, 1.5f);
        doorCol.isTrigger = true;
        var doorBT = doorTriggerGO.AddComponent<BuildingTrigger>();
        doorBT.entrance      = entrance;
        doorBT.isExitTrigger = false;

        // Exterior spawn (where player reappears when leaving)
        var extSpawn = new GameObject("ExteriorSpawn");
        extSpawn.transform.SetParent(ext.transform, false);
        extSpawn.transform.localPosition = new Vector3(0, 0.5f, 5.5f);
        entrance.exterior      = ext;
        entrance.exteriorSpawn = extSpawn.transform;

        // Building name label (world-space, above roof)
        var labelGO = new GameObject("NameLabel");
        labelGO.transform.SetParent(ext.transform, false);
        labelGO.transform.localPosition = new Vector3(0, 5.5f, 0);
        labelGO.transform.localRotation = Quaternion.Euler(0, 0, 0);
        labelGO.transform.localScale    = new Vector3(0.05f, 0.05f, 0.05f);
        // Note: add a TextMeshPro 3D component here for the sign label if desired
        // (Requires TextMeshPro 3D package — not added here to keep zero-config)

        // ── Interior ──
        var interior = new GameObject("Interior");
        interior.transform.SetParent(root.transform, false);
        interior.SetActive(false);

        // Floor
        var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(interior.transform, false);
        floor.transform.localScale    = new Vector3(8f, 0.2f, 8f);
        floor.transform.localPosition = new Vector3(0, 0.1f, 0);
        SetMaterialColor(floor, intColor * 0.70f);

        // Ceiling
        var ceil = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ceil.name = "Ceiling";
        ceil.transform.SetParent(interior.transform, false);
        ceil.transform.localScale    = new Vector3(8f, 0.2f, 8f);
        ceil.transform.localPosition = new Vector3(0, 4.5f, 0);
        SetMaterialColor(ceil, intColor * 0.85f);

        // Four walls
        var wallDefs = new (string n, Vector3 p, Vector3 s)[]
        {
            ("WallNorth", new Vector3( 0, 2.25f, 4f),  new Vector3(8f, 4.5f, 0.2f)),
            ("WallSouth", new Vector3( 0, 2.25f,-4f),  new Vector3(8f, 4.5f, 0.2f)),
            ("WallEast",  new Vector3( 4, 2.25f, 0),   new Vector3(0.2f, 4.5f, 8f)),
            ("WallWest",  new Vector3(-4, 2.25f, 0),   new Vector3(0.2f, 4.5f, 8f)),
        };
        foreach (var (wn, wp, ws) in wallDefs)
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
            w.name = wn;
            w.transform.SetParent(interior.transform, false);
            w.transform.localPosition = wp;
            w.transform.localScale    = ws;
            SetMaterialColor(w, intColor);
        }

        // Interior spawn point
        var intSpawn = new GameObject("InteriorSpawn");
        intSpawn.transform.SetParent(interior.transform, false);
        intSpawn.transform.localPosition = new Vector3(0, 0.5f, 1f);

        // Exit trigger (south wall, player walks toward it to leave)
        var exitTriggerGO = new GameObject("ExitTrigger");
        exitTriggerGO.transform.SetParent(interior.transform, false);
        exitTriggerGO.transform.localPosition = new Vector3(0, 1.2f, -3.2f);
        var exitCol = exitTriggerGO.AddComponent<BoxCollider>();
        exitCol.size      = new Vector3(2.5f, 2.5f, 1f);
        exitCol.isTrigger = true;
        var exitBT = exitTriggerGO.AddComponent<BuildingTrigger>();
        exitBT.entrance      = entrance;
        exitBT.isExitTrigger = true;

        entrance.interior      = interior;
        entrance.interiorSpawn = intSpawn.transform;
    }

    // ────────────────────────────────────────────────────────
    //  Path helper
    // ────────────────────────────────────────────────────────

    static void CreatePath(Vector3 position, Vector3 scale)
    {
        var path = GameObject.CreatePrimitive(PrimitiveType.Cube);
        path.name = "Path";
        path.transform.position   = position + Vector3.up * 0.03f;
        path.transform.localScale = scale;
        SetMaterialColor(path, new Color(0.75f, 0.68f, 0.55f));
        // Remove the BoxCollider so the player can walk over it without issues
        Object.DestroyImmediate(path.GetComponent<BoxCollider>());
    }

    // ────────────────────────────────────────────────────────
    //  UI helpers
    // ────────────────────────────────────────────────────────

    static Camera CreateMainCamera(Vector3 pos, Quaternion rot)
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.AddComponent<AudioListener>();
        return go.AddComponent<Camera>();
    }

    static void CreateDirectionalLight()
    {
        var go = new GameObject("Directional Light");
        go.transform.rotation = Quaternion.Euler(50f, -30f, 0);
        var l = go.AddComponent<Light>();
        l.type      = LightType.Directional;
        l.intensity = 1.1f;
        l.color     = new Color(1f, 0.96f, 0.87f);
        l.shadows   = LightShadows.Soft;
    }

    static Canvas CreateCanvas(string name, float w, float h)
    {
        var go     = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(w, h);
        scaler.matchWidthOrHeight  = 0.5f;
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    /// <summary>Creates a UI panel (Image) with anchor-based sizing.</summary>
    static GameObject CreatePanel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    /// <summary>Creates a TextMeshProUGUI label.</summary>
    static GameObject CreateTMP(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, float fontSize, Color color,
        FontStyles style = FontStyles.Normal)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text              = text;
        tmp.fontSize          = fontSize;
        tmp.color             = color;
        tmp.fontStyle         = style;
        tmp.alignment         = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    /// <summary>Creates a clickable Button with a TMP label inside.</summary>
    static GameObject CreateButton(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color color, string label, float fontSize)
    {
        var go = CreatePanel(parent, name, anchorMin, anchorMax, color);
        go.AddComponent<Button>();
        CreateTMP(go.transform, "Text", label, V2(0,0), V2(1,1), fontSize, Color.white);
        return go;
    }

    /// <summary>Creates a row of colour-swatch buttons.</summary>
    static void CreateSwatches(Transform parent, Color[] colors,
        Vector2 aMin, Vector2 aMax, string prefix)
    {
        float step = (aMax.x - aMin.x) / colors.Length;
        for (int i = 0; i < colors.Length; i++)
        {
            float x0 = aMin.x + i * step;
            float x1 = x0 + step * 0.88f;
            var sw = CreatePanel(parent, $"{prefix}_{i}",
                new Vector2(x0, aMin.y), new Vector2(x1, aMax.y), colors[i]);
            sw.AddComponent<Button>();
        }
    }

    // ────────────────────────────────────────────────────────
    //  Material helper
    // ────────────────────────────────────────────────────────

    static void SetMaterialColor(GameObject go, Color color)
    {
        var r = go.GetComponent<Renderer>();
        if (r == null) return;
        // Try URP Lit first; fall back to Standard (Built-in pipeline)
        var shader = Shader.Find("Universal Render Pipeline/Lit")
                  ?? Shader.Find("Standard");
        if (shader == null) return;
        var mat = new Material(shader);
        mat.color = color;
        r.sharedMaterial = mat;
    }

    // ────────────────────────────────────────────────────────
    //  Scene save + folder helpers
    // ────────────────────────────────────────────────────────

    static void Save(UnityEngine.SceneManagement.Scene scene, string sceneName)
    {
        string path = $"{SCENES_PATH}/{sceneName}.unity";
        EditorSceneManager.SaveScene(scene, path);
        Debug.Log($"Saved: {path}");
    }

    static void EnsureScenesFolder()
    {
        if (!AssetDatabase.IsValidFolder(SCENES_PATH))
            AssetDatabase.CreateFolder("Assets", "Scenes");
    }

    // Short alias so anchor vectors read cleanly inline
    static Vector2 V2(float x, float y) => new Vector2(x, y);
}

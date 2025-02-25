using System.Reflection;
using System.Xml;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

public class GalaxyFarFarAway : IModApi
{
    public static string modsFolderPath;
    public static string cubeMap = "ManyGalaxies";
    public static string basePath = "#@modfolder(GalaxyFarFarAway):Resources/";
    public static string unityPath = "GalaxiesRealistic.unity3d?";

    //public static string testPath = "#@modfolder(GalaxyFarFarAway):Resources/GalaxyColorful01.unity3d?GalaxyColorful01";

    public static bool shuffleGalaxies = true;
    private static bool isInstantiated = false;
    GameObject skyManagerObject;
    public static string galaxyType = "all";
    public List<string> galaxyTypeResults = new List<string>
    {
        "all", "realistic", "colorful", "abstract", "galaxy", "nebula", "spiral", "galacticString"
    };
    public static List<string> galaxyNameResults = new List<string>
    {
        "BlueGreenNebula", "BlueNebula", "GreenNebula", "ManyGalaxies", "MilkyWayGalaxy",
        "PinkGalaxyCollision", "PinkNebula", "PurpleGalaxy", "RedOrangeNebula",
        "bubble1", "bubble2", "chaos1", "chaos2", "chaos3", "square1",
        "galaxy1", "galaxy2", "galaxy3", "galaxy4", "galaxy5", "galaxy6",
        "nebula1", "nebula2", "nebula3",
        "spiral1", "spiral2", "spiral3", "spiral4", "spiral5",
        "string1", "string2", "string3", "string4"
    };
    public static int shuffleInterval = 1;

    public void InitMod(Mod _modInstance)
    {
        Log.Out(" Loading Patch: " + base.GetType().ToString());
        modsFolderPath = _modInstance.Path;
        ReadXML();
        unityPath = PathBuild(cubeMap);
        //ModEvents.GameStartDone.RegisterHandler(GameStart);
        if (shuffleGalaxies)
        {
            if (!isInstantiated)
            {
                Log.Out("[GalaxyFarFarAway] Instantiating MonoBehaviour");
                skyManagerObject = new GameObject("GFFAGalaxyManager");
                skyManagerObject.AddComponent<MonoGFFA>();
                isInstantiated = true;
            }
        }
        ModEvents.WorldShuttingDown.RegisterHandler(WorldShuttingDown);
        Harmony harmony = new Harmony(base.GetType().ToString());
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }
    /*
    void GameStart()
    {
        if (shuffleGalaxies)
        {
            if (!isInstantiated)
            {
                Log.Out("[GalaxyFarFarAway] Instantiating MonoBehaviour");
                skyManagerObject = new GameObject("GFFAGalaxyManager");
                skyManagerObject.AddComponent<MonoGFFA>();
                isInstantiated = true;
            }
        }
    }
    */

    void WorldShuttingDown()
    {
        if (isInstantiated)
        {
            Log.Out("[GalaxyFarFarAway] Deleting MonoBehaviour");
            UnityEngine.Object.Destroy(skyManagerObject);
            isInstantiated = false;
        }
    }
    public static string PathBuild(string cubeMapName)
    {
        var ranges = new (int start, int end, string path)[]
        {
            (0, 8, "GalaxiesRealistic.unity3d?"),
            (9, 14, "GalaxyAbstract.unity3d?"),
            (15, 20, "GalaxyColorfulGalaxies.unity3d?"),
            (21, 23, "GalaxyNebula.unity3d?"),
            (24, 28, "GalaxySpiral.unity3d?"),
            (29, 32, "GalaxyString.unity3d?")
        };

        int i = galaxyNameResults.IndexOf(cubeMapName);
        foreach (var range in ranges)
        {
            if (i >= range.start && i <= range.end)
            {
                unityPath = range.path;
                return unityPath;
            }
        }
        return unityPath; // should never return here
    }

    void ReadXML()
    {
        using (XmlReader xmlReader = XmlReader.Create(modsFolderPath + "\\settings.xml"))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name.ToString() == "shuffleGalaxies")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!bool.TryParse(temp, out shuffleGalaxies))
                        {
                            Log.Warning($"[GalaxyFarFarAway] Failed to read settings.xml setting shuffleGalaxies. Using default of {shuffleGalaxies}");
                            Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                        }
                    }
                    if (shuffleGalaxies)
                    {
                        if (xmlReader.Name.ToString() == "galaxyType")
                        {
                            try
                            {
                                var result = xmlReader.ReadElementContentAsString().ToLower();
                                if (galaxyTypeResults.Contains(result))
                                {
                                    galaxyType = result;
                                }
                                else
                                {
                                    Log.Warning($"[GalaxyFarFarAway] Non Valid settings.xml galaxyType. Using default of {galaxyType}");
                                    Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                                }
                                
                            }
                            catch
                            {
                                Log.Warning($"[GalaxyFarFarAway] Failed to read settings.xml galaxyType. Using default of {galaxyType}");
                                Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                            }
                        }
                        if (xmlReader.Name.ToString() == "shuffleInterval")
                        {
                            string temp = xmlReader.ReadElementContentAsString();
                            if (!int.TryParse(temp, out shuffleInterval))
                            {
                                Log.Warning($"[GalaxyFarFarAway] Failed to read settings.xml setting shuffleInterval. Using default of {shuffleInterval}");
                                Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                            }
                        }
                    }
                    if (!shuffleGalaxies)
                    {
                        if (xmlReader.Name.ToString() == "galaxyName")
                        {
                            try
                            {
                                var result = xmlReader.ReadElementContentAsString().ToLower();
                                if (galaxyNameResults.Contains(result))
                                {
                                    cubeMap = result;
                                }
                                else
                                {
                                    Log.Warning($"[GalaxyFarFarAway] Non Valid settings.xml galaxyName. Using default of {cubeMap}");
                                    Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                                }
                            }
                            catch
                            {
                                Log.Warning($"[GalaxyFarFarAway] Failed to read settings.xml galaxyName. Using default of {cubeMap}");
                                Log.Warning($"[GalaxyFarFarAway] Failed settings.xml read should be reviewed and mod reinstalled if needed");
                            }
                        }
                    }
                }
            }
        }
    }
}

/* works, dont break it */

[HarmonyPatch(typeof(SkyManager), "Init")]
public class SetSkyTexture_Patch
{
    public static void Postfix()
    {
        if (!GalaxyFarFarAway.shuffleGalaxies)
        {
            int propColorMap = Shader.PropertyToID("_OuterSpaceCube");
            string skySprite = "AtmosphereSphere";
            Transform skyTransform = SkyManager.skyManager.transform.FindInChildren(skySprite);

            //Cubemap skyTexture = DataLoader.LoadAsset<Cubemap>(GalaxyFarFarAway.testPath);
            string fullPath = GalaxyFarFarAway.basePath + GalaxyFarFarAway.unityPath + GalaxyFarFarAway.cubeMap;
            Cubemap skyTexture = DataLoader.LoadAsset<Cubemap>(fullPath);

            MeshRenderer renderer;
            if (skyTransform.TryGetComponent<MeshRenderer>(out renderer))
            {
                if (renderer.material != null && skyTexture != null)
                {
                    Log.Out($"[GalaxyFarFarAway] Replacing Skybox cubemap with {GalaxyFarFarAway.cubeMap}");
                    renderer.material.SetTexture(propColorMap, skyTexture);
                }
            }
        }
    }
}




// attempt to load the cubemap material directly, didn't work

/*
[HarmonyPatch(typeof(SkyManager), "Init")]
public class SetSkyboxMaterial_Patch
{
    public static void Postfix()
    {
        // Load your custom skybox material (replace with your mod's material path)
        //DiverseSpaceMaterial
        string location = "#@modfolder(GalaxyFarFarAway):Resources/galaxies.unity3d?DiverseSpaceMaterial";
        Material skyboxMaterial = DataLoader.LoadAsset<Material>(location);

        if (skyboxMaterial != null)
        {
            // Apply the loaded skybox material to the scene's skybox
            RenderSettings.skybox = skyboxMaterial;
            Debug.Log("[MMM] Custom Skybox Material Applied!");
        }
        else
        {
            Debug.LogError("[MMM] Failed to load custom skybox material.");
        }
    }
}*/


// Notes, extra texture information
/*
2025-02-18T16:59:52 58.120 INF [MMM] Found MeshRenderer on: MoonSprite
2025-02-18T16:59:52 58.123 INF [MMM] Property on MoonSprite: _ColorMap
2025-02-18T16:59:52 58.125 INF [MMM] Found MeshRenderer on: AtmosphereSphere
2025-02-18T16:59:52 58.128 INF [MMM] Property on AtmosphereSphere: _OuterSpaceCube
2025-02-18T16:59:52 58.130 INF [MMM] Found MeshRenderer on: CloudsSphere
2025-02-18T16:59:52 58.132 INF [MMM] Property on CloudsSphere: _CloudBGTex
2025-02-18T16:59:52 58.135 INF [MMM] Property on CloudsSphere: _CloudBlendTex
2025-02-18T16:59:52 58.137 INF [MMM] Property on CloudsSphere: _CloudMainTex
2025-02-18T16:59:52 58.140 INF [MMM] Property on CloudsSphere: _DebugTex
*/

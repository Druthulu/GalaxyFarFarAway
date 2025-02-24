using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public class MonoGFFA : MonoBehaviour
{
    float timeSinceLastUpdate = 300f; // Timer for update intervals
    const float updateInterval = 300f; // Update interval in seconds
    //const float updateInterval = 3f; // Test interval
    int lastInterval = 0;
    //int testLogicIndex = 0;
    //static string basePath = "#@modfolder(GalaxyFarFarAway):Resources/";
    //ResourceRequest loadRequest;
    //string galaxyName;
    List<Cubemap> cubemaps = new List<Cubemap>();
    string[] assetBundlePathsOptions = {
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxiesRealistic.unity3d",
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxyAbstract.unity3d",
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxyColorfulGalaxies.unity3d",
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxyNebula.unity3d",
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxySpiral.unity3d",
        $"{GalaxyFarFarAway.modsFolderPath}/Resources/GalaxyString.unity3d"
    };
    string[] olDassetBundlePathsOptionsOld = {
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxiesRealistic.unity3d",
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxyAbstract.unity3d",
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxyColorfulGalaxies.unity3d",
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxyNebula.unity3d",
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxySpiral.unity3d",
        $"#@modfolder(GalaxyFarFarAway):Resources/GalaxyString.unity3d"
    };

    List<string> assetBundlePaths = new List<string>();
    /*
    List<String> galaxyListReal = new List<String>
    {
        "BlueGreenNebula", "BlueNebula", "GreenNebula", "ManyGalaxies",
        "MilkyWayGalaxy", "PinkGalaxyCollision", "PinkNebula", "PurpleGalaxy", "RedOrangeNebula"
    };
    List<String> galaxyListColor = new List<String>
    {
        "bubble1", "bubble2", "chaos1", "chaos2", "chaos3", "square1",
        "galaxy1", "galaxy2", "galaxy3", "galaxy4", "galaxy5", "galaxy6",
        "nebula1", "nebula2", "nebula3",
        "spiral1", "spiral2", "spiral3", "spiral4", "spiral5",
        "string1", "string2", "string3", "string4"
    };
    List<String> galaxyListAbstract = new List<String>
    {
        "bubble1", "bubble2", "chaos1", "chaos2", "chaos3", "square1"
    };
    List<String> galaxyListGalaxy = new List<String>
    {
        "galaxy1", "galaxy2", "galaxy3", "galaxy4", "galaxy5", "galaxy6"
    };
    List<String> galaxyListNebula = new List<String>
    {
        "nebula1", "nebula2", "nebula3"
    };
    List<String> galaxyListSpiral = new List<String>
    {
        "spiral1", "spiral2", "spiral3", "spiral4", "spiral5"
    };
    List<String> galaxyListString = new List<String>
    {
        "string1", "string2", "string3", "string4"
    };
    */
    string galaxyType = "all";
    List<String> galaxyListTotal = new List<String>();
    int playerId = -1;
    int cubemapCount = 0;
    int cubemapRunningCount = 0;
    bool cubemapsLoaded = false;

    void Start()
    {
        galaxyType = GalaxyFarFarAway.galaxyType;

        if (galaxyType == "all")
        {
            //galaxyListTotal = galaxyListReal.Concat(galaxyListColor).ToList();
            assetBundlePaths = assetBundlePathsOptions.ToList();
            cubemapCount = 33;
        }
        else if (galaxyType == "realistic")
        {
            //galaxyListTotal = galaxyListReal;
            assetBundlePaths.Add(assetBundlePathsOptions[0].Replace("/", "\\"));
            cubemapCount = 9;
        }
        else if (galaxyType == "colorful")
        {
            //galaxyListTotal = galaxyListColor;
            for (int i = 1; i < assetBundlePathsOptions.Length; i++)
            {
                assetBundlePaths.Add(assetBundlePathsOptions[i].Replace("/", "\\"));
            }
            cubemapCount = 24;
        }
        else if (galaxyType == "abstract")
        {
            //galaxyListTotal = galaxyListAbstract;
            assetBundlePaths.Add(assetBundlePathsOptions[1].Replace("/", "\\"));
            cubemapCount = 6;
        }
        else if (galaxyType == "galaxy")
        {
            //galaxyListTotal = galaxyListGalaxy;
            assetBundlePaths.Add(assetBundlePathsOptions[2].Replace("/", "\\"));
            cubemapCount = 6;
        }
        else if (galaxyType == "nebula")
        {
            //galaxyListTotal = galaxyListNebula;
            assetBundlePaths.Add(assetBundlePathsOptions[3].Replace("/", "\\"));
            cubemapCount = 3;
        }
        else if (galaxyType == "spiral")
        {
            //galaxyListTotal = galaxyListSpiral;
            assetBundlePaths.Add(assetBundlePathsOptions[4].Replace("/", "\\"));
            cubemapCount = 5;
        }
        else if (galaxyType == "galacticString")
        {
            //galaxyListTotal = galaxyListString;
            assetBundlePaths.Add(assetBundlePathsOptions[5].Replace("/", "\\"));
            cubemapCount = 4;
        }

        // Start the coroutine to load the assets
        StartCoroutine(LoadCubemapsFromBundlesAsync());
        
        /*
        while (playerId < 0)
        {
            spawnCheck();
        }*/
    }

    void Update()
    {
        // prevent logic until player entity exists
        if (playerId > 0)
        {
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= updateInterval)
            {
                // prevent update until all assets loaded
                if (cubemapsLoaded)
                {
                    timeSinceLastUpdate = 0f;
                    int intervalNumber = intervalCheck();

                    if (intervalNumber > lastInterval)
                    {
                        lastInterval = intervalNumber;
                        SetSkybox(intervalNumber);
                    }
                }
                else
                {
                    timeSinceLastUpdate = 295f; // 5sec instead of 5min for quicker intial checks.
                    Log.Out($"[GalaxyFarFarAway] Still loading assets {cubemapRunningCount} out of {cubemapCount}");
                }
            }
        }
        else
        {
            spawnCheck();
        }
        
    }

    void spawnCheck()
    {
        // This will prevent a few safe red errors while loading into a multiplayer server
        var myPlayerId = GameManager.Instance.myPlayerId;
        //Log.Out($"my playerId is: {myPlayerId}");
        if (myPlayerId > 0)
        {
            playerId = myPlayerId;
        }
    }

    int intervalCheck()
    {
        var shuffleInterval = (ulong)GalaxyFarFarAway.shuffleInterval * 24000UL;
        ulong worldTime = GameManager.Instance.World.worldTime;
        // 12pm, so no sky in view during change
        ulong adjustedWorldTime = worldTime >= 12000UL ? worldTime - 12000UL : worldTime + 12000UL;
        var intervalNumber = (int)(adjustedWorldTime / shuffleInterval + 1UL);
        return intervalNumber;
    }
    /*
    void UpdateSkyBox(int dayNum)
    {
        // Start the coroutine to load the skybox texture
        //StartCoroutine(SetSkybox(dayNum));
    }

    void LoadCubemaps()
    {
        // Start the coroutine to load the assets
        StartCoroutine(LoadCubemapsFromBundlesAsync());
    }
    */
    IEnumerator LoadCubemapsFromBundlesAsync()
    {
        foreach (var bundlePath in assetBundlePaths)
        {
            
            //Log.Out($"[GalaxyFarFarAway] attempting to load AssetBundle from {bundlePath}");
            AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundleRequest;
            AssetBundle assetBundle = bundleRequest.assetBundle;
            if (assetBundle == null)
            {
                Log.Warning($"[GalaxyFarFarAway] Failed to load AssetBundle from {bundlePath}");
                continue;
            }
            //Log.Out($"[GalaxyFarFarAway] loaded AssetBundle from {bundlePath}");
            //Log.Out($"[GalaxyFarFarAway] attempting to load cubemaps from {bundlePath}");
            AssetBundleRequest assetRequest = assetBundle.LoadAllAssetsAsync<Cubemap>();
            yield return assetRequest;

            // Iterate through the assets one at a time
            foreach (var asset in assetRequest.allAssets)
            {
                // Check if the asset is a Cubemap
                if (asset is Cubemap cubemap)
                {
                    // Add the cubemap to the list
                    cubemaps.Add(cubemap);
                    cubemapRunningCount++;
                    //Log.Out($"[GalaxyFarFarAway] Loaded cubemap: {cubemap.name}");

                    // Yield to prevent freezing, allowing the game to process other tasks
                    yield return null;
                }
                yield return null;
            }
            yield return null;

            /*
            //Cubemap[] loadedCubemaps = assetRequest.allAssets as Cubemap[];
            Cubemap[] loadedCubemaps = assetRequest.allAssets.OfType<Cubemap>().ToArray();
            if (loadedCubemaps != null)
            {
                foreach (var cubemap in loadedCubemaps)
                {
                    cubemaps.Add(cubemap);
                    cubemapRunningCount++;
                    Log.Out($"[GalaxyFarFarAway] Loaded Cubemap: {cubemap.name}");
                }
            }
            */
            assetBundle.Unload(false);
        }
        cubemapsLoaded = true;
        Log.Out($"[GalaxyFarFarAway] All cubemaps loaded from Asset Bundles! count: {cubemaps.Count}");
    }

    public void SetSkybox(int dayNum)
    {
        // Main logic to choose a galaxy (for example)
        System.Random rand = new System.Random(dayNum);
        int cubemapIndex = rand.Next(cubemaps.Count);

        /*
        // Optionally, test logic
        cubemapIndex = testLogicIndex++;
        //timeSinceLastUpdate = 55f;
        lastDay = 0;
        if (testLogicIndex > cubemaps.Count - 1)
        {
            testLogicIndex = 0;
        }
        */

        if (cubemaps.Count > cubemapIndex)
        {
            int propColorMap = Shader.PropertyToID("_OuterSpaceCube");
            string skySprite = "AtmosphereSphere";
            Transform skyTransform = SkyManager.skyManager.transform.FindInChildren(skySprite);

            Cubemap cubemapToApply = cubemaps[cubemapIndex];
            if (cubemapToApply != null)
            {
                MeshRenderer renderer;
                if (skyTransform.TryGetComponent<MeshRenderer>(out renderer))
                {
                    if (renderer.material != null)
                    {
                        renderer.material.SetTexture(propColorMap, cubemapToApply);
                        Log.Out($"[GalaxyFarFarAway] Applied cubemap: {cubemapToApply.name}");
                    }
                }
            }
        }
        else
        {
            //Log.Warning("[GalaxyFarFarAway] Invalid cubemap index.");
        }
    }
}
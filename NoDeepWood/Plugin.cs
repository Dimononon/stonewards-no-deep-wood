using System;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace NoDeepWood
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "stonewards.nodeepwood";
        public const string PluginName = "NoDeepWood";
        public const string PluginVersion = "1.0.0";

        internal const string WoodItemId = "WOODENPLANK";

        internal static ManualLogSource Log;
        internal static readonly HashSet<ETerrainMaterialType> BlockedLayers = new HashSet<ETerrainMaterialType>();
        internal static bool LoggedFirstBlock;

        private ConfigEntry<string> _blockedLayers;

        private void Awake()
        {
            Log = Logger;
            _blockedLayers = Config.Bind(
                "General",
                "BlockedLayers",
                "ROCK_3,ROCK_4",
                "Terrain layers that no longer spawn wood, comma separated. "
                + "Layers from top to bottom: DIRT, ROCK_1, ROCK_2, ROCK_3, ROCK_4.");
            _blockedLayers.SettingChanged += (_, _) => ReloadLayers();
            ReloadLayers();

            new Harmony(PluginGuid).PatchAll(typeof(Plugin).Assembly);
            Log.LogInfo($"{PluginName} {PluginVersion} loaded");
        }

        private void ReloadLayers()
        {
            BlockedLayers.Clear();
            foreach (string part in _blockedLayers.Value.Split(','))
            {
                string name = part.Trim();
                if (name.Length == 0)
                {
                    continue;
                }
                if (Enum.TryParse(name, ignoreCase: true, out ETerrainMaterialType layer))
                {
                    BlockedLayers.Add(layer);
                }
                else
                {
                    Log.LogWarning($"Unknown terrain layer in BlockedLayers: {name}");
                }
            }
            Log.LogInfo("No wood from: " + string.Join(", ", BlockedLayers));
        }
    }

    // Only DiggingManager.ServerTrySpawnResource calls this, so it runs on the host and
    // covers both pickaxe digging and explosions. A null resource there means nothing spawns.
    [HarmonyPatch(typeof(DigMatterProfileSO), nameof(DigMatterProfileSO.GetRandomResource))]
    internal static class GetRandomResourcePatch
    {
        private static void Postfix(DigMatterProfileSO __instance, ref ValueTuple<ItemDataSO, int> __result)
        {
            ItemDataSO item = __result.Item1;
            if (item == null || !item.isScrapItem || item.itemProduced == null
                || item.itemProduced.itemID != Plugin.WoodItemId
                || !Plugin.BlockedLayers.Contains(__instance.terrainType))
            {
                return;
            }

            __result = (null, 0);

            if (!Plugin.LoggedFirstBlock)
            {
                Plugin.LoggedFirstBlock = true;
                Plugin.Log.LogInfo($"Blocked wood spawn in {__instance.terrainType}; further blocks are not logged");
            }
        }
    }
}

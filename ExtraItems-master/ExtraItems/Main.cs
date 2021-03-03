using BepInEx;
using BepInEx.Logging;
using ExtraItems.Items;
using ExtraItems.Equipment;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using R2API;
using R2API.Utils;
using R2API.AssetPlus;
using RoR2;
using UnityEngine;

namespace ExtraItems
{
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(FontAPI), nameof(LanguageAPI), nameof(SoundAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.LargeSpanish.ExtraItems"; //package name
        public const string ModName = "ExtraItems";
        public const string ModVersion = "0.1.0";

        internal static BepInEx.Logging.ManualLogSource ModLogger;

        public static AssetBundle MainAssets;
        public static Shader HopooShader = Resources.Load<Shader>("shaders/deferred/hgstandard");
        public static Shader IntersectionShader = Resources.Load<Shader>("shaders/fx/hgintersectioncloudremap");
        public static Shader CloudRemapShader = Resources.Load<Shader>("shaders/fx/hgcloudremap");

        public List<ItemBase> Items = new List<ItemBase>();
        public List<EquipmentBase> Equipment = new List<EquipmentBase>();

        public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();
        public void Awake()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraItems.donutmod"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ExtraItems", MainAssets);
                ResourcesAPI.AddProvider(provider);
            }

            var materialAssets = MainAssets.LoadAllAssets<Material>();

            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));


            ModLogger.LogInfo("------Items------");

            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);

                    ModLogger.LogInfo("Item: " + item.ItemName + " Initialized!");
                }
            }


            ModLogger.LogInfo("---------------------------------");
            ModLogger.LogInfo("Extra Items has been initialized");
            ModLogger.LogInfo($"Items Enabled: {ItemStatusDictionary.Count}");
            ModLogger.LogInfo("---------------------------------");         
            }
        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item:  " + item.ItemName, "Enable Item?", true, "Should this item be enabled?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemName, "Blacklist item from bots?", false, "Stop AI from obtaining item").Value;

            ItemStatusDictionary.Add(item, enabled);

            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }
    }
}

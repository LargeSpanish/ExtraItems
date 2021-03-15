using BepInEx;
using BepInEx.Logging;
using ExtraItems.Items;
using ExtraItems.Equipment;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using R2API;
using R2API.Networking;
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
        public const string ModVersion = "0.1.2";

        public static AssetBundle MainAssets;
        internal static ManualLogSource ModLogger;

        public static Shader HopooShader = Resources.Load<Shader>("shaders/deferred/hgstandard");
        public static Shader IntersectionShader = Resources.Load<Shader>("shaders/fx/hgintersectioncloudremap");
        public static Shader CloudRemapShader = Resources.Load<Shader>("shader/fx/hgcloudremap");

        //Used to create a list of Items from the ItemBase class
        public List<ItemBase> Items = new List<ItemBase>();

        public List<EquipmentBase> Equipment = new List<EquipmentBase>();

        public static Dictionary<ItemBase, bool> ItemStatusDictionary = new Dictionary<ItemBase, bool>();

        public void Awake()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraItems.extraitem_assets"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider($"@{ModName}", MainAssets);
                ResourcesAPI.AddProvider(provider);
            }

            //Item initialization
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);
                    ModLogger.LogInfo("Item: " + item.ItemName + " Initialized!");
                }
            }

            //Material Shader Conversion
            var materialAssets = MainAssets.LoadAllAssets<Material>();
            ModLogger.LogInfo("Intersection Shader is: " + IntersectionShader);

            foreach(Material material in materialAssets)
            {
                if (!material.shader.name.StartsWith("Fake")) { continue; }
                switch (material.shader.name.ToLower())
                {
                    case ("fake ror/hopoo games/deferred/hgstandard"):
                        material.shader = HopooShader;
                        break;
                    case ("fake ror/hopoo games/fx/hgcloud intersection remap"):
                        material.shader = IntersectionShader;
                        break;
                    case ("fake ror/hopoo games/fx/hgcloud remap"):
                        material.shader = CloudRemapShader;
                        break;

                }
            }
        }
         public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemName, "Enable Item?", true, "Should this item appear in game?").Value;

            ItemStatusDictionary.Add(item, enabled);
            if (enabled)
            {
                itemList.Add(item);
            }
            return enabled;
         }
    }
}

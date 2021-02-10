using BepInEx;
using BepInEx.Logging;
using System;
using System.Reflection;
using R2API;
using R2API.AssetPlus;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace Mob.ExtraItems
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    [R2APISubmoduleDependency(nameof(ResourcesAPI), nameof(ItemAPI), nameof(ItemDropAPI), nameof(FontAPI), nameof(LanguageAPI), nameof(SoundAPI), nameof(PrefabAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.Mob.ExtraItems"; //package name
        public const string ModName = "ExtraItems";
        public const string ModVersion = "0.1.0";

        public void Awake()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ExtraItems.donutmod"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@ExtraItems", bundle);
                ResourcesAPI.AddProvider(provider);
            }
        }
    }
}

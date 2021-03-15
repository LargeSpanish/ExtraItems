using BepInEx.Configuration;
using static ExtraItems.Utils.ItemHelpers;
using R2API;
using RoR2;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using TILER2;
using UnityEngine;

namespace ExtraItems.Items
{
    public class TheOddity : ItemBase<TheOddity>
    {
        public override string ItemName => "The Oddity";

        public override string ItemLangTokenName => "THE_ODDITY";

        public override string ItemPickupDesc => $"Whenever you are hit there is a chance for a high does of <style=cIsDamage>low-density lipoprotein(LDL)</style> to shoot at an enemy.";

        public override string ItemFullDescription => $"Chance on hit to do <style=cIsDamage>{DamageofMainProjectile}</style> <style=cIsDamage>Choleterol</style> damage <style=cstack>(+{AdditionalDamageOfMainProjectilePerStack}</style> per stack)";

        public override string ItemLore => "Donut go brr....";

        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Damage };

        public override string ItemModelPath => "@ExtraItems:Assets/Models/Prefabs/Item/Donut/DonutModel.prefab";
        public override string ItemIconPath => "@ExtraItems:Assets/Textures/Icon/Item/Donut/DonutIcon2.png";

        public float DamageofMainProjectile;
        public float AdditionalDamageOfMainProjectilePerStack;

        public static GameObject DonutProjectile;
        public static GameObject ItemBodyModelPrefab;
        public TheOddity()
        {
            
        }
        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
        }
        
        public void CreateConfig(ConfigFile config)
        {
            DamageofMainProjectile = config.Bind<float>("Item: " + ItemName, "Damage of the Main Projectile", 300f, "How much base damage should the projectile deal?").Value;
            AdditionalDamageOfMainProjectilePerStack = config.Bind<float>("Item: " + ItemName, "Additional Damage of Projectile per Stack", 100f, "How much more damage should the projectile deal per additional stack?").Value;
        }

        public void CreateProjectile()
        {
            DonutProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/FMJ"), "DonutProjectile", true);

            var damage = DonutProjectile.GetComponent<RoR2.Projectile.ProjectileDamage>();
            damage.damageType = DamageType.BleedOnHit;

            if (DonutProjectile) PrefabAPI.RegisterNetworkPrefab(DonutProjectile);

            if (DonutProjectile) PrefabAPI.RegisterNetworkPrefab(DonutProjectile);
            RoR2.ProjectileCatalog.getAdditionalEntries += list =>
            {
                list.Add(DonutProjectile);
            };
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = Resources.Load<GameObject>(ItemModelPath);
            var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(1, 1, 1)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Arrow",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(1, 1, 1)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            throw new NotImplementedException();
        }

       
    }
}

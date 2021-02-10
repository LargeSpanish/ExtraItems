using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;

namespace ExtraItems.Items
{
    public abstract class ItemBase
    {
        public abstract string ItemName { get;  }
        public abstract string ItemLangTokenName { get; }
        public abstract string ItemPickupDesc { get; }
        public abstract string ItemFullDescription { get; }
        public abstract string ItemLore { get; }

        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] ItemTags { get; }

        public abstract string ItemModelPath { get; }
        public abstract string ItemIconPath { get; }
        public virtual bool CanRemove { get; } = true;
        public virtual bool Hidden { get; } = false;

        public ItemIndex Index;
        public abstract void Init(ConfigFile config);
        protected void CreateLang()
        {
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemLangTokenName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);
        }
        public abstract ItemDisplayRuleDict CreateItemDisplayRules();
        protected void CreateItem()
        {
            ItemDef itemDef = new ItemDef()
            {
                name = "ITEM_" + ItemLangTokenName,
                nameToken = "ITEM_" + ItemLangTokenName + "_NAME",
                pickupToken = "ITEM_" + ItemLangTokenName + "_PICKUP",
                descriptionToken = "ITEM_" + ItemLangTokenName + "_DESCRIPTION",
                loreToken = "ITEM" + ItemLangTokenName + "_LORE",
                pickupModelPath = ItemModelPath,
                pickupIconPath = ItemIconPath,
                hidden = Hidden,
                tags = ItemTags,
                canRemove = CanRemove,
                tier = Tier
            };
            var itemDisplayRuleDict = CreateItemDisplayRules();
            Index = ItemAPI.Add(new CustomItem(itemDef, itemDisplayRuleDict));
        }
        public abstract void Hooks();
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }
            return body.inventory.GetItemCount(Index);
        }
        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }
            return master.inventory.GetItemCount(Index);
        }
    }
}

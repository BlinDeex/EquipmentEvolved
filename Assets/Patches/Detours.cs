using System.Collections.Generic;
using System.Linq;
using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.CharmsModule.Core;
using EquipmentEvolved.Assets.CharmsModule.Items;
using EquipmentEvolved.Assets.Core;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Armor.Universal.Symbiotic;
using EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;
using EquipmentEvolved.Assets.ModPrefixes.Ranged.Ascendant;
using EquipmentEvolved.Assets.ModPrefixes.Summoner.MinionWeapons.Frenzied;
using EquipmentEvolved.Assets.Utilities;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.Utilities;

namespace EquipmentEvolved.Assets.Patches;

public class Detours : ModSystem
{
    public override void SetupContent()
    {
        On_Projectile.AI += ProjectileOnAI;
        On_ItemSlot.LeftClick_ItemArray_int_int += LockSymbioticArmor;
        On_ItemSlot.RightClick_ItemArray_int_int += ApplyCharmAnywhere;
        On_ItemSlot.RightClick_refItem_int += ApplyCharmAnywhere_Ref;
        On_Item.Prefix += On_ItemOnPrefix;
        On_Item.GetRollablePrefixes += On_ItemOnGetRollablePrefixes;
    }

    private static bool On_ItemOnPrefix(On_Item.orig_Prefix orig, Item self, int prefixWeWant)
    {
        if (self.TryGetGlobalItem(out SealedGlobalItem sealedPrefix) && sealedPrefix.IsSealed && sealedPrefix.IsRevealed) return false;

        if (self.TryGetGlobalItem(out AscendantGlobalItem ascendantItem) && ascendantItem.DamageDone > 0) return false;

        return orig.Invoke(self, prefixWeWant);
    }

    private static int[] On_ItemOnGetRollablePrefixes(On_Item.orig_GetRollablePrefixes orig, Item self)
    {
        int[] basePrefixes = orig.Invoke(self);

        return basePrefixes.Where(i => PrefixValidator.CanApplyPrefix(self, i)).ToArray();
    }

    private static bool ItemOnCanRollPrefix(On_Item.orig_CanRollPrefix orig, Item self, int prefix)
    {
        return PrefixValidator.CanApplyPrefix(self, prefix);
    }

    private static void ApplyCharmAnywhere_Ref(On_ItemSlot.orig_RightClick_refItem_int orig, ref Item inv, int context)
    {
        if (!ModContent.GetInstance<PrefixConfig>().EnableCharms)
        {
            orig.Invoke(ref inv, context);
            return;
        }

        if (Main.mouseRight && Main.mouseRightRelease)
        {
            Item targetItem = inv;
            Item mouseItem = Main.mouseItem;
            
            if (mouseItem != null && !mouseItem.IsAir && mouseItem.ModItem is Charm charmItem)
            {
                if (targetItem != null && !targetItem.IsAir)
                {
                    if (targetItem.TryGetGlobalItem(out CharmGlobalItem charmData))
                    {
                        if (CharmGlobalItem.CanApplyCharm(Main.LocalPlayer, targetItem))
                        {
                            charmData.ApplyCharm(charmItem, targetItem);
                            SoundEngine.PlaySound(SoundID.Item37);
                            Main.mouseRightRelease = false;
                            return;
                        }
                    }
                }
            }
        }

        orig.Invoke(ref inv, context);
    }

    private static void ApplyCharmAnywhere(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        if (!ModContent.GetInstance<PrefixConfig>().EnableCharms)
        {
            orig.Invoke(inv, context, slot);
            return;
        }

        if (Main.mouseRight && Main.mouseRightRelease)
        {
            Item targetItem = inv[slot];
            Item mouseItem = Main.mouseItem;
            
            if (mouseItem != null && !mouseItem.IsAir && mouseItem.ModItem is Charm charmItem)
            {
                if (targetItem != null && !targetItem.IsAir)
                {
                    if (targetItem.TryGetGlobalItem(out CharmGlobalItem charmData))
                    {
                        if (CharmGlobalItem.CanApplyCharm(Main.LocalPlayer, targetItem))
                        {
                            charmData.ApplyCharm(charmItem, targetItem);
                            SoundEngine.PlaySound(SoundID.Item37);
                            Main.mouseRightRelease = false;
                            return;
                        }
                    }
                }
            }
        }

        orig.Invoke(inv, context, slot);
    }

    private static void LockSymbioticArmor(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
    {
        if (context is ItemSlot.Context.EquipArmor or ItemSlot.Context.EquipArmorVanity)
        {
            Item item = inv[slot];
            if (item != null && !item.IsAir && item.HasPrefix(ModContent.PrefixType<PrefixSymbiotic>()))
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    if (Main.keyState.IsKeyDown(Keys.LeftShift))
                    {
                        item.TurnToAir();
                        SoundEngine.PlaySound(SoundID.Shatter);
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDust(Main.LocalPlayer.position, Main.LocalPlayer.width, Main.LocalPlayer.height, DustID.Blood);
                        }
                    }
                    return;
                }
            }
        }
        
        orig.Invoke(inv, context, slot);
    }
    
    private static void ProjectileOnAI(On_Projectile.orig_AI orig, Projectile self)
    {
        if (!self.TryGetGlobalProjectile(out FrenziedGlobalProjectile projPrefix) || !projPrefix.Frenzied)
        {
            orig.Invoke(self);
            return;
        }

        for (int i = 0; i <= PrefixBalance.FRENZIED_ADDITIONAL_UPDATES; i++)
        {
            orig.Invoke(self);
        }
    }

    private static bool OnRollAPrefix(On_Item.orig_RollAPrefix orig, Item self, UnifiedRandom random, ref int rolledPrefix)
    {
        int forcedPrefix = ItemLoader.ChoosePrefix(self, random);
        if (forcedPrefix > 0 && PrefixValidator.CanApplyPrefix(self, forcedPrefix))
        {
            rolledPrefix = forcedPrefix;
            return true;
        }

        WeightedRandom<int> validPrefixes = new(random);

        for (int i = 1; i < PrefixLoader.PrefixCount; i++)
        {
            if (!PrefixValidator.CanApplyPrefix(self, i) || !ItemLoader.AllowPrefix(self, i)) continue;

            double weight = 1.0;

            if (i >= PrefixID.Count)
            {
                ModPrefix modPrefix = PrefixLoader.GetPrefix(i);
                if (modPrefix != null)
                {
                    if (!modPrefix.CanRoll(self)) continue;

                    weight = modPrefix.RollChance(self);
                }
            }

            validPrefixes.Add(i, weight);
        }

        if (validPrefixes.elements.Count <= 0) return false;

        rolledPrefix = validPrefixes.Get();
        return true;
    }
}
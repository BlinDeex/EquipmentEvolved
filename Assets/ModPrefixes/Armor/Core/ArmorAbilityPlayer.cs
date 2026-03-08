using System;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Armor.Core;

public class ArmorAbilityPlayer : ModPlayer
{
    /// <summary>
    ///     returns armor ability cooldown in ticks
    /// </summary>
    private Func<int> armorAbility;

    private int armorSetBonusCDBuff;

    public override void SetStaticDefaults()
    {
        armorSetBonusCDBuff = ModContent.BuffType<ArmorAbilityCooldownBuff>();
    }

    /// <summary>
    /// </summary>
    /// <param name="ability"></param>
    /// <param name="passive">
    ///     True if set bonus ability is running every tick (for example augmented set bonus) with no
    ///     cooldown but needs to follow armor set bonus rules
    /// </param>
    public void SetArmorAbility(Func<int> ability, bool passive = false)
    {
        armorAbility = ability;
        if (passive && !Player.HasBuff(armorSetBonusCDBuff)) armorAbility.Invoke();
    }

    public override void ResetEffects()
    {
        armorAbility = null;
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        bool activateArmor = ArmorKeybindSystem.ArmorActivationKeybind.JustPressed;
        if (activateArmor) ActivateArmorAbility();
    }

    public void ActivateArmorAbility()
    {
        if (Player.HasBuff<ArmorAbilityCooldownBuff>()) return;

        if (armorAbility == null) return;

        int cooldown = armorAbility.Invoke();

        Player.AddBuff(ModContent.BuffType<ArmorAbilityCooldownBuff>(), cooldown);

        // TODO: play sound
    }
}
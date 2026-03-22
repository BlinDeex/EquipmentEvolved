using System.Collections.Generic;
using EquipmentEvolved.Assets.Misc;
using EquipmentEvolved.Assets.ModPrefixes.Core; // Assuming ISpecializedPrefix and enum are here
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Core;

public abstract class BaseEvolvedPrefix : ModPrefix
{
    public virtual string TranslationKey => Name.Replace("Prefix", "");
    
    public abstract float ReforgeMultiplier { get; }
    
    public override LocalizedText DisplayName =>
        LocalizationManager.GetPrefixLocalization(this, TranslationKey, "DisplayName");
        
    public LocalizedText Description { get; private set; }
    public LocalizedText NoSetBonus { get; private set; }
    
    public sealed override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, TranslationKey, "Description");
        
        if (this is ISpecializedPrefix specPrefix)
        {
            SpecializedPrefixType type = specPrefix.SpecializedPrefixType;
            if (type is SpecializedPrefixType.Headwear or SpecializedPrefixType.Chestplate or SpecializedPrefixType.Leggings)
            {
                NoSetBonus = LocalizationManager.GetSharedLocalizedText(LocalizationManager.NoArmorSetBonus);
            }
        }

        OnSetStaticDefaults();
    }

    protected virtual void OnSetStaticDefaults() { }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = ReforgeMultiplier;
    }
    
    public override bool CanRoll(Item item)
    {
        return PrefixValidator.CanApplyPrefix(item, Type);
    }

    public override float RollChance(Item item)
    {
        if (this is IWorkInProgressPrefix || this is ILegacyPrefix)
        {
            return 0.00001f; // TODO bandaid fix for prefixes that are WIP or legacy and shouldnt be stripped from weapons on world joins but stay not rollable, no clue how to fix
        }
        return 1f;
    }

    protected LocalizedText GetLoc(string key) => LocalizationManager.GetPrefixLocalization(this, TranslationKey, key);
    
    public sealed override IEnumerable<TooltipLine> GetTooltipLines(Item item)
    {
        foreach (TooltipLine line in OnGetTooltipLines(item))
        {
            yield return line;
        }
        //TODO remove warning tooltips if issue with prefix stripping is fixed
        if (this is IWorkInProgressPrefix)
        {
            yield return new TooltipLine(Mod, "WIPWarning", "[c/FF4444:Work In Progress - Cannot be rolled normally]")
            {
                IsModifier = true
            };
        }
        if (this is ILegacyPrefix)
        {
            yield return new TooltipLine(Mod, "LegacyWarning", "[c/FF8800:Legacy Trait - Cannot be rolled normally]")
            {
                IsModifier = true
            };
        }
        
        if (NoSetBonus != null)
        {
            yield return new TooltipLine(Mod, "NoSetBonus", NoSetBonus.Value)
            {
                IsModifier = true,
                IsModifierBad = true
            };
        }
    }
    protected virtual IEnumerable<TooltipLine> OnGetTooltipLines(Item item)
    {
        yield break;
    }
}
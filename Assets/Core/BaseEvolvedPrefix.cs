using EquipmentEvolved.Assets.Misc;
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

    public override void SetStaticDefaults()
    {
        Description = LocalizationManager.GetPrefixLocalization(this, TranslationKey, "Description");
    }

    public override void ModifyValue(ref float valueMult)
    {
        valueMult = ReforgeMultiplier;
    }
    
    protected LocalizedText GetLoc(string key) => LocalizationManager.GetPrefixLocalization(this, TranslationKey, key);
}
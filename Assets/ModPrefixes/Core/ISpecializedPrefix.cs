using EquipmentEvolved.Assets.Misc;

namespace EquipmentEvolved.Assets.ModPrefixes.Core;

/// <summary>
/// Used to control what type of items this reforge can be applied to
/// </summary>
public interface ISpecializedPrefix
{
    public SpecializedPrefixType SpecializedPrefixType { get; }
}
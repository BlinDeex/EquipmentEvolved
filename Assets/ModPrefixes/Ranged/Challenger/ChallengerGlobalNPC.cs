using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPrefixes.Ranged.Challenger;

public class ChallengerGlobalNPC : GlobalNPC
{
    public override bool InstancePerEntity => true;
    public int ChallengerOwner { get; set; }
}
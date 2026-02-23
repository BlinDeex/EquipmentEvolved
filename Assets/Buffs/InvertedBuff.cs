using EquipmentEvolved.Assets.Balance;
using EquipmentEvolved.Assets.ModPlayers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.Buffs;

public class InvertedBuff : ModBuff
{
    public override bool ReApply(Player player, int time, int buffIndex)
    {
        int newTime = time + 300;
        newTime = (int)MathHelper.Clamp(newTime, 0, 600);
        player.buffTime[buffIndex] = newTime;
        return true;
    }

    public override void Update(Player player, ref int buffIndex)
    {
        PrefixPlayer prefixPlayer = player.GetModPlayer<PrefixPlayer>();
        
        if (player.whoAmI == Main.myPlayer)
        {
            int timeLeft = player.buffTime[buffIndex];
            float targetMultiplier = timeLeft / 600f;
            prefixPlayer.ManaSurgeMultiplier = targetMultiplier;
            float damageAdded = PrefixBalance.INVERTED_MAX_DAMAGE_INCREASE * targetMultiplier;
            player.GetDamage<MagicDamageClass>() += damageAdded;
            
            float dice = Main.rand.NextFloat();
            if (dice > PrefixBalance.INVERTED_MANA_SURGE_CHANCE_TO_DAMAGE)
            {
                player.Hurt(MiscStuff.GetManaSurgeDeath(Main.LocalPlayer.name),
                    PrefixBalance.INVERTED_MANA_SURGE_DAMAGE, 0, quiet: false);
            }
        }

        if (!Main.dedServ)
        {
            Vector2 playerCenter = player.Center;
            float targetScale = prefixPlayer.ManaSurgeMultiplier * PrefixBalance.INVERTED_MANA_SURGE_MAX_SCALE;
            
            for (int i = 0; i < PrefixBalance.INVERTED_MANA_SURGE_DUST_RATE; i++)
            {
                int xOffset = Main.rand.Next(-16, 16);
                int yOffset = Main.rand.Next(-32, 32);
                Vector2 dustPos = playerCenter + new Vector2(xOffset, yOffset);
                Dust.NewDust(dustPos, 1, 1, PrefixBalance.INVERTED_MANA_SURGE_DUST_ID, Scale: targetScale);
            }
        }
    }

    public override LocalizedText DisplayName => Mod.GetLocalization("Buffs.InvertedDisplayName");

    public override LocalizedText Description => Mod.GetLocalization("Buffs.InvertedDesc");

    public override string Texture => $"{Mod.Name}/Assets/Textures/Buffs/ManaSurgeIcon";

    public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
    {
        PrefixPlayer prefixPlayer = Main.LocalPlayer.GetModPlayer<PrefixPlayer>();
        tip = Description.Format((int)(prefixPlayer.ManaSurgeMultiplier * 100));
    }
    
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true; 
        Main.buffNoSave[Type] = true;
    }

    public override bool RightClick(int buffIndex)
    {
        return false;
    }
}
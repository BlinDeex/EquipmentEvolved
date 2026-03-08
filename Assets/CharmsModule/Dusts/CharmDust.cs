using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.CharmsModule.Dusts;

public class CharmDust : ModDust
{
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;

        dust.frame = new Rectangle(0, Main.rand.Next(0, 3) * 10, 8, 8);
        dust.rotation = Main.rand.NextFloat(6.28f);
    }

    public override bool Update(Dust dust)
    {
        dust.velocity *= 0.95f;
        dust.position += dust.velocity;
        dust.rotation += 0.05f * (dust.dustIndex % 2 == 0 ? -1 : 1);
        dust.scale -= 0.015f;

        Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.5f);

        if (dust.scale < 0.1f) dust.active = false;

        return false;
    }
}
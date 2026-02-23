using EquipmentEvolved.Assets.Buffs;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace EquipmentEvolved.Assets.ModPlayers.Armor.Universal.Phantom
{
    public class PhantomSoundDistortion : ModSystem
    {
        public override void Load()
        {
            On_SoundEngine.PlaySound_refNullable1_Nullable1_SoundUpdateCallback += DistortSounds;
        }

        private SlotId DistortSounds(On_SoundEngine.orig_PlaySound_refNullable1_Nullable1_SoundUpdateCallback orig, ref SoundStyle? style, Vector2? position, SoundUpdateCallback updateCallback)
        {
            if (!style.HasValue || Main.gameMenu || Main.LocalPlayer == null || !Main.LocalPlayer.active)
            {
                return orig(ref style, position, updateCallback);
            }
            
            int buffIndex = Main.LocalPlayer.FindBuffIndex(ModContent.BuffType<SoulBurnBuff>());

            if (buffIndex < 0)
            {
                return orig(ref style, position, updateCallback);
            }
            
            int currentDuration = Main.LocalPlayer.buffTime[buffIndex];
            float intensity = currentDuration / 1200f;
            if (intensity > 1f) intensity = 1f;
            SoundStyle modifiedStyle = style.Value;
            modifiedStyle.Volume *= 1f - (intensity * 0.8f);
            modifiedStyle.Pitch -= (intensity * 0.8f);
            modifiedStyle.PitchVariance += (intensity * 0.6f);
            style = modifiedStyle;

            return orig(ref style, position, updateCallback);
        }
    }
}
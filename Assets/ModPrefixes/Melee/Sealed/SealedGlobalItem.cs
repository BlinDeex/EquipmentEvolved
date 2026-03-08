using System.Collections.Generic;
using System.IO;
using EquipmentEvolved.Assets.CharmsModule.Data;
using EquipmentEvolved.Assets.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EquipmentEvolved.Assets.ModPrefixes.Melee.Sealed;

public class SealedGlobalItem : GlobalItem
{
    public override bool InstancePerEntity => true;

    public bool IsSealed { get; set; }
    public bool IsRevealed { get; set; }
    public List<CharmRoll> Rolls { get; set; } = [];

    public override bool CanReforge(Item item)
    {
        if (IsSealed && IsRevealed) return false;

        return base.CanReforge(item);
    }

    public override bool CanRightClick(Item item)
    {
        return IsSealed && !IsRevealed;
    }

    public override void RightClick(Item item, Player player)
    {
        if (IsSealed && !IsRevealed)
        {
            IsRevealed = true;

            SoundEngine.PlaySound(SoundID.Item37);
        }
    }

    public override bool ConsumeItem(Item item, Player player)
    {
        return !IsSealed && base.ConsumeItem(item, player);
    }

    public override void PreReforge(Item item)
    {
        IsSealed = false;
        IsRevealed = false;
        Rolls.Clear();
    }

    public override void SaveData(Item item, TagCompound tag)
    {
        if (!IsSealed) return;

        tag["IsSealed"] = IsSealed;
        tag["IsRevealed"] = IsRevealed;

        List<TagCompound> rollsData = [];
        foreach (CharmRoll roll in Rolls)
        {
            rollsData.Add(new TagCompound
            {
                ["Stat"] = (int)roll.Stat,
                ["Strength"] = roll.RawStrength
            });
        }

        tag["Rolls"] = rollsData;
    }

    public override void LoadData(Item item, TagCompound tag)
    {
        IsSealed = tag.GetBool("IsSealed");
        IsRevealed = tag.GetBool("IsRevealed");

        Rolls.Clear();
        if (!tag.ContainsKey("Rolls")) return;

        IList<TagCompound> rollsData = tag.GetList<TagCompound>("Rolls");
        foreach (TagCompound rollTag in rollsData)
        {
            Rolls.Add(new CharmRoll((PlayerStat)rollTag.GetInt("Stat"), rollTag.GetFloat("Strength")));
        }
    }

    public override void NetSend(Item item, BinaryWriter writer)
    {
        writer.Write(IsSealed);
        if (!IsSealed) return;

        writer.Write(IsRevealed);
        writer.Write((byte)Rolls.Count);
        foreach (CharmRoll roll in Rolls)
        {
            writer.Write((int)roll.Stat);
            writer.Write(roll.RawStrength);
        }
    }

    public override void NetReceive(Item item, BinaryReader reader)
    {
        IsSealed = reader.ReadBoolean();
        if (!IsSealed) return;

        IsRevealed = reader.ReadBoolean();
        byte count = reader.ReadByte();
        Rolls.Clear();
        for (int i = 0; i < count; i++)
        {
            Rolls.Add(new CharmRoll((PlayerStat)reader.ReadInt32(), reader.ReadSingle()));
        }
    }

    public override GlobalItem Clone(Item from, Item to)
    {
        SealedGlobalItem clone = (SealedGlobalItem)base.Clone(from, to);
        clone.Rolls = new List<CharmRoll>(Rolls);

        return clone;
    }
}
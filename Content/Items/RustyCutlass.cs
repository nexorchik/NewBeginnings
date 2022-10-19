﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace NewBeginnings.Content.Items
{
    public class RustyCutlass : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Cutlass");
            Tooltip.SetDefault("It has seen better days");
        }

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 22;
            Item.width = 36;
            Item.height = 44;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = 1;
            Item.knockBack = 2;
            Item.value = Item.sellPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
        }
    }
}

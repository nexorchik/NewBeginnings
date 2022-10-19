﻿using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace NewBeginnings.Common.PlayerBackgrounds
{
    internal struct DelegateData
    {
        public Func<bool> ClearCondition;
        public Action<List<GenPass>> ModifyWorldGenTasks;
        public Func<bool> HasSpecialSpawn;
        public Func<Point16> GetSpawnPosition;

        /// <summary>
        /// Default delegates ("do nothing").
        /// </summary>
        public DelegateData()
        {
            ClearCondition = () => true;
            ModifyWorldGenTasks = (_) => { };
            HasSpecialSpawn = () => false;
            GetSpawnPosition = () => Point16.Zero;
        }

        /// <summary>Allows the use of conditions for an origin and a hook-like additional worldgen delegate for ease-of-use.</summary>
        /// <param name="clear">Condition of the origin. Returns true by default.</param>
        /// <param name="modify">Allows you to modify the genpass list of an incoming world, allowing you to add or remove passes as you please.</param>
        /// <param name="hasSpawn">Checks if the origin has a special spawn point. Useful for variable spawns.</param>
        /// <param name="spawn">Allows you to dynamically modify the spawn point, such as choosing a random beach to spawn on.</param>
        public DelegateData(Func<bool> clear = null, Action<List<GenPass>> modify = null, Func<bool> hasSpawn = null, Func<Point16> spawn = null)
        {
            ClearCondition = clear ?? (() => true);
            ModifyWorldGenTasks = modify ?? ((_) => { });
            HasSpecialSpawn = hasSpawn ?? (() => false);
            GetSpawnPosition = spawn ?? (() => Point16.Zero);
        }
    }
}

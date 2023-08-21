using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using System.Threading;
using Microsoft.Xna.Framework;
using StardewValley.Characters;
using System;
using System.Threading.Tasks;

namespace MineKiller
{
    internal class Killer
    {
        private IModHelper _helper;

        private IMonitor _monitor;

        private Config _config;


        public void InitKiller(IModHelper helper, IMonitor monitor, Config config)
        {
            _helper = helper;
            _monitor = monitor;
            _config = config;
        }

        public void ReloadConfig(Config config)
        {
            _config = config;
        }

        public void KillMonsters(Farmer player, GameLocation location)
        {
            if (!_config.IsEnable)
            {
                return;
            }

            if (location is not MineShaft && location is not VolcanoDungeon && location is not Woods)
            {
                return;
            }

            NetCollection<NPC> characters = location.characters;
            for (var i = 0; i < characters.Count; i++)
            {
                NPC character = characters[i];
                if (character is not Monster)
                {
                    continue;
                }

                Monster monster = character as Monster;
                if (IsIncludeRange(player.Position, monster.Position, _config.Range))
                {
                    MakeWeak(monster);
                    _monitor.Log($"gonna kill monster [{monster.Name}]", LogLevel.Debug);
                    location.damageMonster(monster.GetBoundingBox(), _config.Damage, _config.Damage * 2, monster is Mummy, player);
                }
            }
        }

        private void MakeWeak(Monster monster)
        {
            if (monster is Bug bug && bug.isArmoredBug.Get())
            {
                _helper.Reflection.GetField<NetBool>(bug, "isArmoredBug").SetValue(new NetBool(false));
            }

            if (monster is RockCrab rockCrab)
            {
                _helper.Reflection.GetField<NetBool>(rockCrab, "shellGone").SetValue(new NetBool(true));
                _helper.Reflection.GetField<NetInt>(rockCrab, "shellHealth").SetValue(new NetInt(0));
            }
        }


        public double CalculateLineDistance(Vector2 a, Vector2 b)
        {
            double line = 0;
            //√（x₁-x₂）²+（y₁-y₂）²
            double x = Math.Pow(a.X - b.X, 2);
            double y = Math.Pow(a.Y - b.Y, 2);
            line = Math.Sqrt(x + y);
            return line;
        }

        private bool IsIncludeRange(Vector2 a, Vector2 b, int range)
        {
            double distance = CalculateLineDistance(a, b);
            return distance < range;
        }
    }
}
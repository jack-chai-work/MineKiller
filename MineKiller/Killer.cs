using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;

namespace MineKiller
{
    internal class Killer
    {

        private IModHelper _helper;

        private IMonitor _monitor;

        private Config _config;


        public void InitKiller(IModHelper helper,IMonitor monitor,Config config)
        {
            _helper = helper;
            _monitor = monitor;
            _config = config;
        }


        public void KillMonsters(Farmer player, GameLocation location)
        {
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
                MakeWeek(monster,player);
                _monitor.Log($"gonna kill monster [{monster.Name}]",LogLevel.Debug);
                location.damageMonster(monster.GetBoundingBox(), _config.Damage, _config.Damage * 2, true, player);
            }
        }

        private void MakeWeek(Monster monster,Farmer player)
        {

            Tool currentTool = null;

            if (player.CurrentTool != null && player.CurrentTool is Tool)
                currentTool = Game1.player.CurrentTool;


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

    }
}

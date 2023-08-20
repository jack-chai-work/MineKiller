using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MineKiller
{
    internal sealed class ModEntry : Mod
    {

        private readonly Killer _killer = new Killer();

        private Config _config;

        /*********
         ** Public methods
         *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<Config>();
            _killer.InitKiller(helper,Monitor, _config);
          helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Player.Warped += OnWarped;
            helper.Events.GameLoop.OneSecondUpdateTicked += OnUpdateTicked;
        }
        
        /*********
         ** Private methods
         *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady || Game1.currentMinigame != null)
                return;

            if (_config.Key.Equals(e.Button.ToString()))
            {
                this.Monitor.Log($"{Game1.player.Name} pressed {_config.Key}.", LogLevel.Debug);
            }

            // print button presses to the console window
            // this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {

            Farmer farmer = e.Player;
            GameLocation location = e.NewLocation;
            this.Monitor.Log($"{farmer.Name} location is  {location}.", LogLevel.Debug);

        }

        private void OnUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
                return;
            _killer.KillMonsters(Game1.player,Game1.currentLocation);
        }
    
    }
}

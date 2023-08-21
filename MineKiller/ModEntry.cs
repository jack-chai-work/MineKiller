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
            if (string.IsNullOrEmpty(_config.Key))
            {
                _config.Key = "J";
            }
            _killer.InitKiller(helper, Monitor, _config);
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
            if (_config.Key.ToUpper().Equals(e.Button.ToString().ToUpper()))
            {
                _config.IsEnable = !_config.IsEnable;
                string message = _config.IsEnable ? "killer activated" : "killer deactivated";
                int messageType = _config.IsEnable ? 4 : 3;
                _killer.ReloadConfig(_config);
                Game1.addHUDMessage(new HUDMessage(message,messageType));
            }

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
            _killer.KillMonsters(Game1.player, Game1.currentLocation);
        }
    }
}
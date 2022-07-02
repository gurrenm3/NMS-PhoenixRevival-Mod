using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.ModHelper;
using NoMansSky.Api;
using System.Diagnostics;

namespace NoMansSky.ModTemplate
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : NMSMod
    {
        /// <summary>
        /// The link to this repository.
        /// </summary>
        public const string githubUrl = "https://github.com/gurrenm3/NMS-PhoenixRevival-Mod";

        /// <summary>
        /// How much of the player's health will regenerate when the ability activates.
        /// </summary>
        public int playerHealthRegenAmount = 50;

        /// <summary>
        /// How much of the ship's health will regenerate when the ability activates.
        /// </summary>
        public int shipHealthRegenAmount = 250;

        /// <summary>
        /// How many minutes will it take before this ability can activate again.
        /// </summary>
        public int cooldownInMinutes = 30;


        /// <summary>
        /// Indicates whether or not it's possible to revive right now.
        /// </summary>
        public bool CanRevive => !cooldownTimer.IsRunning ||
            (cooldownTimer.Elapsed.TotalMinutes >= cooldownInMinutes);

        private Stopwatch cooldownTimer = new Stopwatch();

        /// <summary>
        /// Initializes your mod along with some necessary info.
        /// </summary>
        public Mod(IModConfig _config, IReloadedHooks _hooks, IModLogger _logger) : base(_config, _hooks, _logger)
        {
            // on player health lost.
            Player.Health.OnValueChanged.Prefix += (newAmount) =>
            {
                if (newAmount.value > 0 || !CanRevive)
                    return;

                // set health here.
                newAmount.value = playerHealthRegenAmount;
                Logger.WriteLine("Phoenix Revival activated! With your powers of rebirth you avoided death." +
                    $" Health has been restored by {playerHealthRegenAmount}. This ability can activate" +
                    $" again in {cooldownInMinutes} minutes.");

                cooldownTimer.Restart();
            };

            // on ship health lost.
            ActiveShip.Health.OnValueChanged.Prefix += (newAmount) =>
            {
                if (newAmount.value > 0 || !CanRevive)
                    return;

                // set health here.
                newAmount.value = shipHealthRegenAmount;
                Logger.WriteLine("Phoenix Revival activated! With your powers of rebirth you avoided death." +
                    $" Health has been restored by {shipHealthRegenAmount}. This ability can activate" +
                    $" again in {cooldownInMinutes} minutes.");

                cooldownTimer.Restart();
            };
        }
    }
}
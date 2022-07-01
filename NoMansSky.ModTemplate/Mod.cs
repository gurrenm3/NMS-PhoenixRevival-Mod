using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using Reloaded.ModHelper;
using NoMansSky.Api;
using libMBIN.NMS.Globals;
using System.Diagnostics;
using System;
using libMBIN.NMS.GameComponents;

namespace NoMansSky.ModTemplate
{
    /// <summary>
    /// Your mod logic goes here.
    /// </summary>
    public class Mod : NMSMod
    {
        /// <summary>
        /// Represents how long the cooldown should be.
        /// </summary>
        public ModSettingInt cooldownInMinutes = new ModSettingInt(30);

        /// <summary>
        /// Represents how much health will be regenerated.
        /// </summary>
        public ModSettingInt regenPercent = new ModSettingInt(50);

        /// <summary>
        /// Indicates whether or not it's possible to revive right now.
        /// </summary>
        public bool CanRevive => !cooldownTimer.IsRunning ||
            (cooldownTimer.Elapsed.TotalMinutes >= cooldownInMinutes.Value);

        private Stopwatch cooldownTimer = new Stopwatch();
        private const int PlayerMaxHealth = 100;
        private const int DefaultShipMaxHealth = 500;

        /// <summary>
        /// Initializes your mod along with some necessary info.
        /// </summary>
        public Mod(IModConfig _config, IReloadedHooks _hooks, IModLogger _logger) : base(_config, _hooks, _logger)
        {
            cooldownInMinutes.Minimum = 1;
            regenPercent.Maximum = 100;
            regenPercent.Minimum = 1;

            var t = new GcDefaultSaveData();
            
            

            // on player health lost.
            Player.Health.OnValueChanged.Prefix += (newAmount) =>
            {
                bool isAboutToDie = newAmount <= 0;
                if (isAboutToDie && CanRevive)
                {
                    // set health here.
                    double percent = regenPercent.Value / 100;
                    newAmount.value = (int)(PlayerMaxHealth * percent);
                    Logger.WriteLine("Phoenix Revival activated! With your powers of rebirth you avoided death." +
                        $" Health has been restored to {regenPercent.Value} it's normal value. Can activate" +
                        $" this ability again in {cooldownInMinutes.Value} minutes.");

                    cooldownTimer.Restart();
                }
            };

            // on ship health lost.
            ActiveShip.Health.OnValueChanged.Prefix += (newAmount) =>
            {
                if (CanRevive)
                {
                    // set health here.
                    newAmount.value = DefaultShipMaxHealth / 2;

                    Logger.WriteLine("Phoenix Revival activated! With your powers of rebirth you avoided death." +
                        $" Your ship's health has been restored to 50% of the default value. Can activate" +
                        $" this ability again in {cooldownInMinutes.Value} minutes.");

                    cooldownTimer.Restart();
                }
            };
        }
    }
}
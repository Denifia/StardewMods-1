using System;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;

namespace Pathoschild.Stardew.RotateToolbar.Framework
{
    /// <summary>The mod configuration.</summary>
    internal class RawModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The keyboard input map.</summary>
        public InputMapConfiguration<string> Keyboard { get; set; }

        /// <summary>The controller input map.</summary>
        public InputMapConfiguration<string> Controller { get; set; }

        /// <summary>Whether to check for updates to the mod.</summary>
        public bool CheckForUpdates { get; set; } = true;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct a default instance.</summary>
        public RawModConfig()
        {
            this.Keyboard = new InputMapConfiguration<string>
            {
                ShiftToPrevious = "",
                ShiftToNext = Keys.Tab.ToString()
            };
            this.Controller = new InputMapConfiguration<string>
            {
                ShiftToPrevious = "",
                ShiftToNext = ""
            };
        }

        /// <summary>Get a parsed representation of the mod configuration.</summary>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        public ModConfig GetParsed(IMonitor monitor)
        {
            return new ModConfig
            {
                Keyboard = new InputMapConfiguration<Keys>
                {
                    ShiftToPrevious = this.TryParse(monitor, this.Keyboard.ShiftToNext, Keys.None),
                    ShiftToNext = this.TryParse(monitor, this.Keyboard.ShiftToNext, Keys.Tab)
                },
                Controller = new InputMapConfiguration<Buttons>
                {
                    ShiftToNext = this.TryParse<Buttons>(monitor, this.Controller.ShiftToNext),
                    ShiftToPrevious = this.TryParse<Buttons>(monitor, this.Controller.ShiftToNext)
                },
                CheckForUpdates = this.CheckForUpdates,
            };
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Parse a raw enum value.</summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="raw">The raw value.</param>
        /// <param name="defaultValue">The default value if it can't be parsed.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        private T TryParse<T>(IMonitor monitor, string raw, T defaultValue = default(T)) where T : struct
        {
            // empty
            if (string.IsNullOrWhiteSpace(raw))
                return defaultValue;

            // valid enum
            T parsed;
            if (Enum.TryParse(raw, true, out parsed))
                return parsed;

            // invalid
            monitor.Log($"Couldn't parse '{raw}' from config.json as a {typeof(T).Name} value, using default value of {defaultValue}.", LogLevel.Warn);
            return defaultValue;
        }
    }
}

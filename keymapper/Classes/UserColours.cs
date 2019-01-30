using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Win32;

namespace KeyMapper.Classes
{
    internal static class UserColourSettingManager
    {
        public static event EventHandler<EventArgs> ColoursChanged;
        private static bool _loaded;

        private static readonly Dictionary<ButtonEffect, UserColourSetting> settings 
            = new Dictionary<ButtonEffect, UserColourSetting>();

        static UserColourSettingManager()
        {
            ColoursChanged += delegate { LoadColours(); };
        }

        private static void LoadColours()
        {
            settings.Clear();
            foreach (ButtonEffect effect in Enum.GetValues(typeof(ButtonEffect)))
            {
                var setting = GetColourSettingFromRegistry(effect);
                if (setting != null)
                {
                    settings.Add(effect, setting);
                }
            }
        }

        public static void SaveSetting(ButtonEffect effect, ColorMatrix cm, int FontColour)
        {
            string key = AppController.ApplicationRegistryKeyName;
            string subkey = effect.ToString();

            var reg = Registry.CurrentUser.CreateSubKey(key + @"\UserColours\" + subkey);

            if (reg == null) {
				return;
			}

			reg.SetValue("FontColour", FontColour);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string name = "Matrix"
                        + i.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        + j.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    var value = cm.GetType().GetProperty(name).GetValue(cm, null);
                    // Console.WriteLine("i: {0}, j: {1}, value: {2}", i, j, value);
                    reg.SetValue(name, (float)decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            RaiseColoursChangedEvent();
        }

        public static void RaiseColoursChangedEvent()
        {
            if (ColoursChanged != null) {
				ColoursChanged(null, null);
			}
		}

        public static UserColourSetting GetColourSettings(ButtonEffect effect)
        {
            if (_loaded == false)
            {
                LoadColours();
                _loaded = true;
            }

            if (settings.ContainsKey(effect)) {
				return settings[effect];
			}

			return null;
        }


        private static UserColourSetting GetColourSettingFromRegistry(ButtonEffect effect)
        {
            // Need to be defensively minded as user could change, 
            // delete, or change type of registry settings. 

            string subkey = AppController.ApplicationRegistryKeyName + @"\UserColours\" + effect.ToString();

            var reg = Registry.CurrentUser.OpenSubKey(subkey);
            if (reg == null) // No settings have been defined for this effect
			{
				return null;
			}

			var setting = new UserColourSetting();

            // User may have changed type of FontColour
            // Using nullable int as any possible integer value could be a valid 
            // ToARGB() result (well, I'm assuming it could anyway)

            int? fontColourArgb;

            var value = reg.GetValue("FontColour");
            if (value == null || reg.GetValueKind("FontColour") != RegistryValueKind.DWord) {
				fontColourArgb = Color.Black.ToArgb();
			}
			else {
				fontColourArgb = (int?)value;
			}

			setting.FontColour = Color.FromArgb((int)fontColourArgb);

            var cm = new ColorMatrix();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string name = "Matrix"
                        + i.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        + j.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    value = reg.GetValue(name);
                    if (value != null)
                    {
                        float svalue;
                        if (float.TryParse(value.ToString(), out svalue))
                        {
                            cm.GetType().GetProperty(name).SetValue(cm, svalue, null);
                        }
                    }
                }
            }

            setting.Matrix = cm;

            return setting;
        }
    }

    public class UserColourSetting
    {
        // This is the class that will be stored in the user settings for custom colours
        private ColorMatrix _matrix = new ColorMatrix();
        private int _fontColour = Color.Black.ToArgb();

        public Color FontColour
        {
            get => Color.FromArgb(_fontColour);
            set => _fontColour = value.ToArgb();
        }

        public ColorMatrix Matrix
        {
            get => _matrix;
            set => _matrix = value;
        }
    }
}

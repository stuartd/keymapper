using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using Microsoft.Win32;

namespace KeyMapper
{

    class UserColourSettingManager
    {
        Dictionary<ButtonEffect, UserColourSetting> _settings = new Dictionary<ButtonEffect, UserColourSetting>();

        public UserColourSettingManager()
        {
            SaveSetting(ButtonEffect.Disabled,
                        ButtonImages.GetMatrix(ButtonEffect.Disabled),
                        ButtonImages.GetFontColour(ButtonEffect.Disabled).ToArgb());
        }


        private void SaveSetting(ButtonEffect effect, ColorMatrix cm, int FontColour)
        {

            string key = AppController.ApplicationRegistryKeyName;
            string subkey = effect.ToString();

            RegistryKey reg = Registry.CurrentUser.CreateSubKey(key + @"\UserColours\" + subkey);

            if (reg == null)
                return;

            reg.SetValue("FontColour", FontColour);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string name = "Matrix"
                        + i.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        + j.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    object value = cm.GetType().GetProperty(name).GetValue(cm, null);
                    // Console.WriteLine("i: {0}, j: {1}, value: {2}", i, j, value);
                    reg.SetValue(name, (float)System.Decimal.Parse(value.ToString(), System.Globalization.CultureInfo.InvariantCulture));
                }
            }



        }

        public UserColourSetting GetSettings(ButtonEffect effect)
        {
            string key = AppController.ApplicationRegistryKeyName;
            string subkey = key + @"\UserColours\" + effect.ToString();
            UserColourSetting setting = new UserColourSetting();

            RegistryKey reg = Registry.CurrentUser.OpenSubKey(subkey);
            if (reg == null)
                return setting;

            int? FontColour = (int?)reg.GetValue("FontColour");
            if (FontColour == null)
                FontColour = Color.Black.ToArgb();

            setting.FontColour = (int)FontColour;

            ColorMatrix cm = new ColorMatrix();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    string name = "Matrix"
                        + i.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        + j.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    object value = reg.GetValue(name);
                    Decimal dvalue;
                    if (System.Decimal.TryParse(value.ToString(), out dvalue))
                    {
                        cm.GetType().GetProperty(name).SetValue(cm, dvalue, null);
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
        ColorMatrix _matrix = new ColorMatrix();
        int _fontColour = Color.Black.ToArgb();

        public int FontColour
        {
            get
            { return _fontColour; }
            set
            { _fontColour = value; }
        }

        public ColorMatrix Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public UserColourSetting() { }

    }
}

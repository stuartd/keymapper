using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using KeyMapper.Classes;
using Microsoft.Win32;

namespace KeyMapper
{

	static class UserColourSettingManager
	{

		public static event EventHandler<EventArgs> ColoursChanged ;
		static bool _loaded ;

		static Dictionary<ButtonEffect, UserColourSetting> _settings = new Dictionary<ButtonEffect, UserColourSetting>();

		static UserColourSettingManager()
		{
			ColoursChanged += delegate(object sender, EventArgs e) { LoadColours(); };
		}
				
		private static void LoadColours()
		{
			_settings.Clear();
			foreach (ButtonEffect effect in Enum.GetValues(typeof(ButtonEffect)))
			{
				UserColourSetting setting = GetColourSettingFromRegistry(effect);
				if (setting != null)
					_settings.Add(effect, setting);
			}
		}


		public static void SaveSetting(ButtonEffect effect, ColorMatrix cm, int FontColour)
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

			RaiseColoursChangedEvent();

		}

		public static void RaiseColoursChangedEvent()
		{

			if (ColoursChanged != null)
				ColoursChanged(null, null);

		}

		public static UserColourSetting GetColourSettings(ButtonEffect effect)
		{
			if (_loaded == false)
			{
				LoadColours();
				_loaded = true;
			}

			if (_settings.ContainsKey(effect))
				return _settings[effect];
			else
				return null;

		}


		private static UserColourSetting GetColourSettingFromRegistry(ButtonEffect effect)
		{

			// Need to be defensively minded as user could change, 
			// delete, or change type of registry settings. 

			string subkey = AppController.ApplicationRegistryKeyName + @"\UserColours\" + effect.ToString();
		
			RegistryKey reg = Registry.CurrentUser.OpenSubKey(subkey);
			if (reg == null) // No settings have been defined for this effect
				return null;
			
			UserColourSetting setting = new UserColourSetting();

			// User may have changed type of FontColour
			// Using nullable int as any possible integer value could be a valid 
			// ToARGB() result (well, I'm assuming it could anyway)

			int? FontColourArgb = null;

			object value = reg.GetValue("FontColour");
			if (value == null || reg.GetValueKind("FontColour") != RegistryValueKind.DWord)
				FontColourArgb = Color.Black.ToArgb();
			else
				FontColourArgb = (int?)value ;

			setting.FontColour = Color.FromArgb((int)FontColourArgb);

			ColorMatrix cm = new ColorMatrix();

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
						Single svalue;
						if (System.Single.TryParse(value.ToString(), out svalue))
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
		ColorMatrix _matrix = new ColorMatrix();
		int _fontColour = Color.Black.ToArgb();

		public Color FontColour
		{
			get
			{
				return Color.FromArgb(_fontColour);
			}
			set
			{
				_fontColour = value.ToArgb(); 
			}
		}

		public ColorMatrix Matrix
		{
			get { return _matrix; }
			set { _matrix = value; }
		}

		public UserColourSetting() { }

	}
}

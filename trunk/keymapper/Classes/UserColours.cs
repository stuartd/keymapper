using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;

namespace KeyMapper
{

	class UserColourSettingManager
	{
		Dictionary<ButtonEffect, UserColourSetting> settings = new Dictionary<ButtonEffect, UserColourSetting>();

		public UserColourSettingManager()
		{
			CreateSettings();
			// LoadSettings();
		}

		private void CreateSettings()
		{

   //            1.  Create an instance of System.Configuration.SettingsAttributeDictionary.
   //2. Create an instance of System.Configuration.UserScopedSettingsAttribute.
   //3. Add the System.Configuration.UserScopedSettingsAttribute object to the System.Configuration.SettingsAttributeDictionary.
   //4. Create an instance of System.Configuration.SettingsProperty passing the System.Configuration.SettingsAttributeDictionary into the contructor as well as the property name, default value, and type.
   //5. Add the System.Configuration.SettingsAttributeDictionary to the My.MySettings.Default.Properties collection. 

			Properties.UserColours uc = new KeyMapper.Properties.UserColours();
		// 	uc.Initialize(new SettingsContext()

			SettingsAttributeDictionary attributes = new SettingsAttributeDictionary();
			UserScopedSettingAttribute attr = new UserScopedSettingAttribute();
			attributes.Add(attr.TypeId, attr);

			LocalFileSettingsProvider provider = new LocalFileSettingsProvider();

			SettingsProperty prop = new SettingsProperty((new SettingsProperty
					("foo", typeof(UserColourSetting), provider, false, new UserColourSetting(), SettingsSerializeAs.Xml, attributes, false, false)));

			uc.Properties.Add(prop);

			SettingsProperty prop2 = new SettingsProperty((new SettingsProperty
					("bar", typeof(UserColourSetting), provider, false, new UserColourSetting(), SettingsSerializeAs.String, attributes, false, false)));

			// Need to add to the default properties ??!?!?!?!?!?!?!?!?
			uc.Properties.Add(prop2);
			
			Console.WriteLine("UC has {0} properties", uc.Properties.Count);

			uc.Save();

			UserColourSetting s = (UserColourSetting)uc["foo"];

			Console.WriteLine(s.ToString());

			foreach (ButtonEffect effect in Enum.GetValues(typeof(ButtonEffect)))
			{
			}
		}
	}

	public class UserColourSetting
	{
		// This is the class that will be stored in the user settings for custom colours
	// 	ColorMatrix _matrix = new ColorMatrix();
		int _fontColour = Color.Black.ToArgb() ;

		public UserColourSetting() { }

	}
}

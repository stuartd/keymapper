using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;

namespace KeyMapper
{
	static class UserColours
	{

		static ColorMatrix _normalButtonMatrix;

		public static ColorMatrix NormalButtonMatrix
		{
			get { return UserColours._normalButtonMatrix; }
			set { UserColours._normalButtonMatrix = value; }
		}
		
		// static bool _normalButtonHasUserColour;

		static ColorMatrix _mappedButtonMatrix;

		public static ColorMatrix MappedButtonMatrix
		{
			get { return UserColours._mappedButtonMatrix; }
			set { UserColours._mappedButtonMatrix = value; }
		}
		// static bool _mappedButtonHasUserColour;

		//public static bool NormalButtonHasUserColour
		//{
		//    get { return UserColours._normalButtonHasUserColour; }
		//}

		//public static bool MappedButtonHasUserColour
		//{
		//    get { return UserColours._mappedButtonHasUserColour; }
		//}

		static UserColours()
		{
			LoadColours();
		}

		public static void LoadColours()
		{

			//Properties.Settings userSettings = new Properties.Settings();

			//_normalButtonMatrix = userSettings.NormalKeyColour;
			//_normalButtonHasUserColour = !(_normalButtonMatrix == null);

			//_mappedButtonMatrix = userSettings.MappedKeyColour;
			//_mappedButtonHasUserColour = !(_mappedButtonMatrix == null);

		}

	}
}

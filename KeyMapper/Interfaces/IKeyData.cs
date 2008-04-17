using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace KeyMapper
{
	public interface IKeyData
	{

		/// <summary>
		/// This interface is implemented by objects which extract the key data from 
		///  whatever format it may be in - XML, text, SQL, wvr.
		/// </summary>

		#region Properties


		// Keylists. Both of these added together make up the usual 104 keyboard.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		List<int> LocalizableKeys { get;}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		List<int> NonLocalizableKeys { get;}
		
		#endregion

		#region Methods

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
		List<string> GetGroupList(int threshold);

		Dictionary<string, int> GetGroupMembers(string group, int threshold);

		// Keyboard layout.
		KeyboardLayoutType GetKeyboardLayoutType(string locale);

		#endregion

	}
}

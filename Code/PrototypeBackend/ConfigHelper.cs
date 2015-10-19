using System;
using System.Collections.Generic;

namespace PrototypeBackend
{
	public static class ConfigHelper
	{
		/// <summary>
		/// Strings to array.
		/// </summary>
		/// <returns>The to array.</returns>
		/// <param name="str">String.</param>
		public static uint[] StringToArray (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var ints = new System.Collections.Generic.List<uint> ();
			foreach (string s in stra) {
				uint i;
				uint.TryParse (s, out i);
				ints.Add (i);
			}
			return ints.ToArray ();
		}

		/// <summary>
		/// Splits a string mit comatas as devider into a list of strings
		/// </summary>
		/// <returns>The to string list.</returns>
		/// <param name="s">S.</param>
		public static List<string> StringToStringList (string s)
		{
			var stra = s.Split (new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
			var list = new List<string> ();
			foreach (string str in stra) {
				list.Add (str);
			}
			return list;
		}

		/// <summary>
		/// Strings to analog reference dictionary.
		/// </summary>
		/// <returns>The analog reference dictionary.</returns>
		/// <param name="str">String.</param>
		public static Dictionary<string,double> StringToARefDict (string str)
		{
			var stra = str.Split (new char[]{ ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			var res = new Dictionary<string,double> ();

			foreach (string s in stra) {
				var pair = s.Split (new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
				res.Add (pair [0], Convert.ToDouble (pair [1]));
			}
			return res;
		}

		/// <summary>
		/// Strings to layout.
		/// </summary>
		/// <returns>The to layout.</returns>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		public static Dictionary<string,List<int>> StringToLayout (string left, string right, string bottom)
		{
			var dict = new Dictionary<string,List<int>> ();
			dict.Add ("LEFT", StringToPin (left));
			dict.Add ("RIGHT", StringToPin (right));
			dict.Add ("BOTTOM", StringToPin (bottom));

			return dict;
		}

		/// <summary>
		/// Strings to pin.
		/// </summary>
		/// <returns>The to pin.</returns>
		/// <param name="str">String.</param>
		public static List<int> StringToPin (string str)
		{
			var pairs = str.Split (new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new List<int> ();

			foreach (string s in pairs) {
				dict.Add (Convert.ToInt32 (s));
			}
			return dict;
		}

		/// <summary>
		/// Strings to pin placement.
		/// </summary>
		/// <returns>The to pin placement.</returns>
		/// <param name="str">String.</param>
		public static Dictionary<int,Point> StringToPinPlacement (string str)
		{
			var pins = str.Split (new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
			var dict = new Dictionary<int,Point> ();

			foreach (string s in pins) {
				var pair = s.Split (new char[]{ ':', ',' }, StringSplitOptions.RemoveEmptyEntries);

				Point p = new Point (Convert.ToDouble (pair [1]), Convert.ToDouble (pair [2]));
				dict.Add (Convert.ToInt32 (pair [0]), p);
			}
			return dict;
		}
	}
}


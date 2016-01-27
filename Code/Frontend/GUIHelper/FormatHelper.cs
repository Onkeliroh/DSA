using System;

namespace GUIHelper
{
	public static class FormatHelper
	{
		public static string ConvertToString (object val)
		{
			var tmp = Convert.ToDouble (val);
			if (tmp % 1 != 0)
			{
				return String.Format ("{0:0.########################################################}", tmp);
			} else
			{
				return String.Format ("{0:0.0}", tmp);
			}
		}
	}
}


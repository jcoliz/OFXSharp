using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace OfxSharp
{
	internal static class Extensions
	{
		public static string Fmt(this string format, params object[] args)
		{
			return String.Format( CultureInfo.CurrentCulture, format, args );
		}

		public static string FmtInv(this string format, params object[] args)
		{
			return String.Format( CultureInfo.InvariantCulture, format, args );
		}

        public static bool IsSet(this string s)
		{
			return !String.IsNullOrWhiteSpace(s);
		}

        public static bool IsEmpty(this string s)
		{
			return String.IsNullOrWhiteSpace(s);
		}
	}
}

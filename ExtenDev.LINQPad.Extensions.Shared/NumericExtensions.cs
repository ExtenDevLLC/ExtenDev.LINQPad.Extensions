using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class NumericExtensions
    {
        #region Friendly Byte Size Strings (e.g. 1024 = "1 KB")

        private const int ToFriendlyByteSizeStringDefaultDecimalPlaces = 2;

        private static readonly string[] SizeSuffixes =
                       { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        public static string ToFriendlyByteSizeString(this ulong value, int decimalPlaces = ToFriendlyByteSizeStringDefaultDecimalPlaces)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }

        public static string ToFriendlyByteSizeString(this long value, int decimalPlaces = ToFriendlyByteSizeStringDefaultDecimalPlaces)
        {
            if (value < 0) { return "-" + ToFriendlyByteSizeString(-value); }
            return ((ulong)value).ToFriendlyByteSizeString();
        }

        public static string ToFriendlyByteSizeString(this int value, int decimalPlaces = ToFriendlyByteSizeStringDefaultDecimalPlaces)
        {
            if (value < 0) { return "-" + ToFriendlyByteSizeString(-value); }
            return ((ulong)value).ToFriendlyByteSizeString(decimalPlaces);
        }

        public static string ToFriendlyByteSizeString(this uint value, int decimalPlaces = ToFriendlyByteSizeStringDefaultDecimalPlaces)
        {
            return ((ulong)value).ToFriendlyByteSizeString(decimalPlaces);
        }

        #endregion
    }
}

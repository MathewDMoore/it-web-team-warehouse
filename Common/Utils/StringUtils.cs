using System;
using System.Text;

namespace Common.Utils
{
    public static class StringUtils
    {
        private const int COMMENT_SIZE = 1024;
        private static readonly Random _random = new Random((Int32)DateTime.Now.Ticks % Int32.MaxValue);

        public static string PreserveLineBreaks(this string str)
        {
            return !String.IsNullOrEmpty(str) ? str.Replace("\n", "<br/>") : string.Empty;
        }

        public static string ReverseLineBreaks(this string str)
        {
            return !String.IsNullOrEmpty(str) ? str.Replace("<br/>", "\n") : string.Empty;
        }

        //        public static string EmptyForNull(string text)
        //        {
        //            return String.IsNullOrEmpty(text) ? string.Empty : text;
        //        }

        public static string EscapeWhiteSpace(this string val)
        {
            string returnVal = val.Replace(@"\", @"\\");
            returnVal = returnVal.Replace(" ", "\\ ");
            returnVal = returnVal.Replace("\t", "\\\t");

            return returnVal;
        }

        public static int? ToNullableInt(this string value)
        {
            int? returnVal = null;
            if (!string.IsNullOrEmpty(value))
            {
                int holdValue;
                if (int.TryParse(value, out holdValue))
                {
                    returnVal = holdValue;
                }
            }
            return returnVal;
        }

        public static bool AreEqual(string a, string b)
        {
            string trimA = (a == null) ? string.Empty : a.Trim();
            string trimB = (b == null) ? string.Empty : b.Trim();

            return trimA.Equals(trimB);
        }

        public static bool AreEqual(string a, object b)
        {
            return AreEqual(a, ((b != null) ? b.ToString() : null));
        }

        public static string[] SplitToSize(string longString)
        {
            return SplitToSize(longString, COMMENT_SIZE);
        }

        public static string[] SplitToSize(string longString, int length)
        {
            int lastStringLength = longString.Length % length;
            int numberOfElements = longString.Length / length + (lastStringLength > 0 ? 1 : 0);

            string[] results = new string[numberOfElements];

            int startIndex = 0;
            for (int i = 0; i < numberOfElements; i++)
            {
                results[i] = longString.Substring(startIndex, (i == (numberOfElements - 1)) ? lastStringLength : length);
                startIndex += length;
            }

            return results;
        }

        public static byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        //        public static T ToInt<T>(this string value) where T : struct
        //        {
        //            if (typeof(T) != typeof(Int16) &&
        //                typeof(T) != typeof(Int32) &&
        //                typeof(T) != typeof(Int64) &&
        //                typeof(T) != typeof(UInt16) &&
        //                typeof(T) != typeof(UInt32) &&
        //                typeof(T) != typeof(UInt64))
        //            {
        //                throw new ArgumentException(
        //                    string.Format("Type '{0}' is not valid.", typeof(T).ToString()));
        //            }
        //            {
        //                T num = Int64.Parse(value) is T ? (T)Int64.Parse(value) : default(T);
        //                return 0;
        //            }
        //        }
        public static int ToInt(this string value)
        {
            int num;
            Int32.TryParse(value, out num);
            return num;
        }

        public static long ToLong(this string value)
        {
            long num;
            long.TryParse(value, out num);
            return num;
        }

        public static string GenerateRandomString(int length)
        {
            StringBuilder generatedString = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int randomInt = _random.Next(65, 90);
                generatedString.Append(Convert.ToChar(randomInt));
            }

            return generatedString.ToString();
        }
    }
}
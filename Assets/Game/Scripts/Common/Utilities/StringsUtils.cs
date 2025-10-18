using System.Text;

namespace Game.Scripts.Common.Utilities
{
    public static class StringsUtils
    {
        public static string FormatMoney(int money)
        {
            return $"{money.InsertSpaces()}₡";
        }

        public static string InsertSpaces(this int value)
        {
            var sb = new StringBuilder(value.ToString());
            int length = sb.Length;

            int spaceCount = (length - 1) / 3;

            for (int i = 1; i <= spaceCount; i++)
            {
                int idx = length - (i * 3);
                sb.Insert(idx, " ");
            }

            return sb.ToString();
        }
    }
}
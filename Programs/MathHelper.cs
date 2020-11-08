namespace SurvivalcraftTextureStudio
{
    public static class MathHelper
    {
        private static bool isProportionTheSame(int x1, int y1, int x2, int y2)
        {
            if (x1 == y1 && x2 == y2)
            {
                return true;
            }
            if (x1 < y1)
            {
                if (x2 >= y2)
                {
                    return false;
                }
                int temp = x1;
                x1 = y1;
                y1 = temp;
                temp = x2;
                x2 = y2;
                y2 = temp;
            }
            int quotient = x1 / y1;
            if (quotient != x2 / y2)
            {
                return false;
            }
            int x3 = GetLargestCommonDivisor(x1, x2);
            int y3 = GetLargestCommonDivisor(y1, y2);
            int remainder1 = x1 % y1;
            int remainder2 = x2 % y2;
            int remainder3 = x3 % y3;
            if (quotient != x3 / y3)
            {
                return false;
            }
            if (remainder1 % remainder3 == 0 && remainder2 % remainder3 == 0)
            {
                return true;
            }
            return false;
        }

        private static int GetLargestCommonDivisor(int n1, int n2)
        {
            int max = n1 > n2 ? n1 : n2;
            int min = n1 < n2 ? n1 : n2;
            int remainder;
            while (min != 0)
            {
                remainder = max % min;
                max = min;
                min = remainder;
            }
            return max;
        }
    }
}
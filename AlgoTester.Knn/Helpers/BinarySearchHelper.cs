namespace AlgoTester.Helpers;

public static class BinarySearchHelper
{
    public static double FindMaxValue(Func<double, bool> function, double left, double right, double precision)
    {
        while (right - left > precision)
        {
            var middle = (left + right) / 2;

            if (function(middle))
            {
                left = middle;
            }
            else
            {
                right = middle;
            }
        }

        return (left + right) / 2;
    }
}
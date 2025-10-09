namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants
{
    internal static class ObjectExtensions
    {
        public static bool IsProtocolClear(this object argument)
        {
            if (argument is double d)
            {
                return double.IsNegativeInfinity(d);
            }

            return false;
        }

        public static bool IsProtocolLeave(this object argument)
        {
            if (argument is double d)
            {
                return double.IsPositiveInfinity(d);
            }

            return false;
        }
    }
}

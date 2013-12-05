using System;

namespace SharpRaven.UnitTests
{
    public static class TestHelper
    {
        private static void PerformDivideByZero()
        {
            int i2 = 0;
            int i = 10 / i2;
        }


        public static Exception GetException()
        {
            try
            {
                PerformDivideByZero();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }
    }
}
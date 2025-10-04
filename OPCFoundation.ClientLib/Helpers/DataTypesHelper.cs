using Opc.Ua;
using Opc.Ua.Test;
using System;

namespace OPCFoundation.ClientLib.Helpers
{
    public static class DataTypesHelper
    {
        public static readonly Random random = new Random();

        public static float NextFloat()
        {
            //Random random = new Random();
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            // choose -149 instead of -126 to also generate subnormal floats (*)
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }


        public static double NextDouble()
        {
            var next = random.NextDouble();

            return next;
        }

        public static byte[] Byte()
        {
            float num = NextFloat();
            byte[] bytes = null;

            if (!BitConverter.IsLittleEndian)
            {
                bytes = BitConverter.GetBytes(num);
                Array.Reverse(bytes, 0, bytes.Length);

                num = BitConverter.ToSingle(bytes, 0);
            }

            return bytes;
        }

        public static object GetNewValue(BuiltInType expectedType)
        {
            
            DataGenerator m_generator = new Opc.Ua.Test.DataGenerator(null)
            {
                BoundaryValueFrequency = 0
            };
            
            object value = null;
            int retryCount = 0;
            while (value == null && retryCount < 10)
            {
                value = m_generator.GetRandom(expectedType);
                retryCount++;
            }

            return value;
        }
    }
}

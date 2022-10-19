namespace SteganoBlaze.Shared
{
    public static class MaxValueChange
    {
        public static double FloatingPoint(int bits)
        {
            double maxValue = float.MaxValue + float.MinValue;
            string binVal = "";

            for (int i = 0; i < bits; i++)
                binVal += "1";

            var byteValue = BitConverter.GetBytes(Convert.ToInt32(binVal, 2));
            var floatvalue = BitConverter.ToSingle(byteValue, 0);
           
            return floatvalue / maxValue * 100;


            //int bits = 31;
            //double maxValue = float.MaxValue;
            //Console.WriteLine(maxValue.ToString());

            //string binVal = "";
            //int intValue = 1;
            //string binary = Convert.ToString(intValue, 2);
            //Console.WriteLine(binary);

            //binVal += "01";
            //for (int i = 0; i < 29 - bits; i++)
            //{
            //    binVal += "0";
            //}
            //for (int i = 0; i < bits; i++)
            //{
            //    binVal += "1";
            //}

            //Console.WriteLine(binVal);
            //intValue = Convert.ToInt32(binVal, 2);

            //var byteValue = BitConverter.GetBytes(intValue);
            //for (int i = 0; i < byteValue.Length; i++)
            //{
            //    Console.WriteLine(Convert.ToString(byteValue[i], 2));
            //}

            //float floatValue = BitConverter.ToSingle(byteValue, 0);

            //if (float.IsNaN(floatValue) || float.IsInfinity(floatValue))
            //{
            //    floatValue = float.MaxValue;
            //}
            //Console.WriteLine(floatValue.ToString());

            //var a = Math.Abs(floatValue) / maxValue * 100;
            //Console.WriteLine(a.ToString());
        }

        public static double FixedPoint(int selectedBits, int intLength)
        {
            var maxSelectedValue = Math.Pow(2, selectedBits) - 1;
            var maxIntValue = Math.Pow(2, intLength) - 1;
            return maxSelectedValue / maxIntValue * 100;
        }
    }
}

namespace SteganoBlaze.Shared
{
    public static class MaxValueChange
    {
        public static double FloatingPoint(int selectedBits)
        {
            if (selectedBits > 29)
                return 100.0;

            float maxValue = float.MaxValue;

            string binVal = "00";
            for (int i = 0; i < 30 - selectedBits; i++)
                binVal += "1";
            for (int i = 0; i < selectedBits; i++)
                binVal += "0";

            var byteValue = BitConverter.GetBytes(Convert.ToInt32(binVal, 2));
            var floatValue = BitConverter.ToSingle(byteValue, 0);
           
            return 100 - Math.Log10(floatValue) / Math.Log10(maxValue) * 100 - 100;
        }

        public static double FixedPoint(int selectedBits, int intLength)
        {
            var maxSelectedValue = Math.Pow(2, selectedBits) - 1;
            var maxIntValue = Math.Pow(2, intLength) - 1;
            return maxSelectedValue / maxIntValue * 100;
        }
    }
}

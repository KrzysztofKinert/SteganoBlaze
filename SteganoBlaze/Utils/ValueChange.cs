namespace SteganoBlaze.Utils
{
    public static class ValueChange
    {
        public static double FromFloatingPoint(int selectedBits)
        {
            if (selectedBits > 29)
                return 100.0;

            string binaryValue = "01";
            for (int i = 0; i < 30 - selectedBits; i++)
                binaryValue += "0";
            for (int i = 0; i < selectedBits; i++)
                binaryValue += "1";

            var byteValue = BitConverter.GetBytes(Convert.ToInt32(binaryValue, 2));
            var floatValue = BitConverter.ToSingle(byteValue, 0);

            return Math.Log2(floatValue - 1.0) / Math.Log2(float.MaxValue) * 100;
        }

        public static double FromFixedPoint(int selectedBits, int intLength)
        {
            var maxSelectedValue = Math.Pow(2, selectedBits) - 1;
            var maxIntValue = Math.Pow(2, intLength) - 1;
            return maxSelectedValue / maxIntValue * 100;
        }
    }
}

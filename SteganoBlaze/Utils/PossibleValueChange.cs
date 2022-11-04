namespace SteganoBlaze.Utils
{
    public static class PossibleValueChange
    {
        public static double FromFloatingPoint(int selectedBits)
        {
            if (selectedBits > 29)
                return 100.0;

            string selectedValueBinary = "01";
            for (int i = 0; i < 30 - selectedBits; i++)
                selectedValueBinary += "0";
            for (int i = 0; i < selectedBits; i++)
                selectedValueBinary += "1";

            var selectedValueByte = BitConverter.GetBytes(Convert.ToInt32(selectedValueBinary, 2));
            var selectedValue = BitConverter.ToSingle(selectedValueByte, 0);

            return Math.Log2(selectedValue - 1.0) / Math.Log2(float.MaxValue) * 100;
        }

        public static double FromFixedPoint(int selectedBits, int fixedPointLength)
        {
            var selectedValue = Math.Pow(2, selectedBits) - 1;
            var maxValue = Math.Pow(2, fixedPointLength) - 1;
            return selectedValue / maxValue * 100;
        }
    }
}

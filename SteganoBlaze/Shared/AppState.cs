using MudBlazor;
namespace SteganoBlaze.Shared
{
    public class AppState
    {
        public Color themeColor { get; private set; }
        public Color themeColorBuffer { get; private set; }
        public string pageName { get; private set; } = "AboutSteganography";

        public event Action? OnChange;
        public Int64 maxAllowedCarrierSize { get; private set; } = 10000000;
        public Int64 maxAllowedMessageSize { get; private set; } = 5000000;
        public Int64 maxAllowedCarrierPixels { get; private set; } = 25000000;
        public void StateChanged()
        {
            NotifyStateChanged();
        }
        public void SetPageTheme(Color color, string name)
        {
            themeColor = color;
            themeColorBuffer = color;
            pageName = name;
            NotifyStateChanged();
        }
        public void ToggleDarkMode()
        {
            //if(themeColor == themeColorBuffer)
            //{
            //	themeColor = Color.Dark;
            //}
            //else
            //{
            //	themeColor = themeColorBuffer;
            //}
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}

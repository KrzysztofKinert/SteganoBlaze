using MudBlazor;
using SteganoBlaze.Utils;

namespace SteganoBlaze.Shared
{
    public class AppState
    {
        public string PageName { get; private set; } = "AboutSteganography";
        public bool WebPUnavailable { get; set; } = false;

        public event Action? OnChange;

        public MudTheme Theme = new();

        public const int MAX_CARRIER_SIZE = 10 * 1024 * 1024;
        public const int MAX_MESSAGE_SIZE = 5 * 1024 * 1024;
        public const int MAX_CARRIER_PIXELS = 25000000;

        public void StateChanged() => 
            NotifyStateChanged();

        public void SetPageTheme(Color color, string name)
        {
            PageName = name;
            Theme = PageTheme.SelectTheme(color);
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => 
            OnChange?.Invoke();
    }
}

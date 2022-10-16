using MudBlazor;
namespace SteganoBlaze.Shared
{
    public class AppState
    {
        public string pageName { get; private set; } = "AboutSteganography";
        public bool webPUnavailable { get; set; } = false;

        public event Action? OnChange;

        public MudTheme Theme = new MudTheme();

        public static readonly int MAX_CARRIER_SIZE = 10 * 1024 * 1024;
        public static readonly int MAX_MESSAGE_SIZE = 5 * 1024 * 1024;
        public static readonly int MAX_CARRIER_PIXELS = 25000000;

        public void StateChanged()
        {
            NotifyStateChanged();
        }
        public void SetPageTheme(Color color, string name)
        {
            pageName = name;

            switch(color)
            {
                case (Color.Primary):
                default:
                    Theme = new MudTheme()
                    {
                        Palette = new Palette()
                        {
                            Primary = "#594AE2",
                            Tertiary = "#1EC8A5",
                        },
                        PaletteDark = ConvertToDarkTheme(new Palette(), "#776be7", "#3ACEAF"),
                    };
                    break;
                case (Color.Tertiary):
                    Theme = new MudTheme()
                    {
                        Palette = new Palette()
                        {
                            Primary = "#1EC8A5",
                            Tertiary = "#594AE2",
                        },
                        PaletteDark = ConvertToDarkTheme(new Palette(), "#3ACEAF", "#776be7"),
                    };
                    break;
                case (Color.Info):
                    Theme = new MudTheme()
                    {
                        Palette = new Palette()
                        {
                            Primary = Colors.Blue.Default,
                        },
                        PaletteDark = ConvertToDarkTheme(new Palette(), Colors.Blue.Lighten1, "#1EC8A5"),
                    };
                    break;
            }
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnChange?.Invoke();

        internal static Palette ConvertToDarkTheme(Palette palette, string primaryColor, string tertiaryColor)
        {
            palette.Primary = primaryColor;
            palette.Tertiary = tertiaryColor;
            palette.Black = "#27272f";
            palette.Background = "#32333d";
            palette.BackgroundGrey = "#27272f";
            palette.Surface = "#373740";
            palette.DrawerBackground = "#27272f";
            palette.DrawerText = "rgba(255,255,255, 0.50)";
            palette.DrawerIcon = "rgba(255,255,255, 0.50)";
            palette.AppbarBackground = "#27272f";
            palette.AppbarText = "rgba(255,255,255, 0.70)";
            palette.TextPrimary = "rgba(255,255,255, 0.70)";
            palette.TextSecondary = "rgba(255,255,255, 0.50)";
            palette.ActionDefault = "#adadb1";
            palette.ActionDisabled = "rgba(255,255,255, 0.26)";
            palette.ActionDisabledBackground = "rgba(255,255,255, 0.12)";
            palette.Divider = "rgba(255,255,255, 0.12)";
            palette.DividerLight = "rgba(255,255,255, 0.06)";
            palette.TableLines = "rgba(255,255,255, 0.12)";
            palette.TableStriped = "rgba(255,255,255, 0.2)";
            palette.LinesDefault = "rgba(255,255,255, 0.12)";
            palette.LinesInputs = "rgba(255,255,255, 0.3)";
            palette.TextDisabled = "rgba(255,255,255, 0.2)";
            palette.Info = "#3299ff";
            palette.Success = "#0bba83";
            palette.Warning = "#ffa800";
            palette.Error = "#f64e62";
            palette.Dark = "#27272f";
            return palette;
        }
    }
}

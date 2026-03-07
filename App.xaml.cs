using Microsoft.UI.Xaml.Navigation;
using TeddyMerge.Services;

namespace TeddyMerge
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window? MainWindow { get; private set; }

        /// <summary>
        public App()
        {
            try 
            {
                if (PackageHelper.IsPackaged)
                {
                    var settingsPath = System.IO.Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "settings.txt");
                    
                    if (System.IO.File.Exists(settingsPath))
                    {
                        string lang = System.IO.File.ReadAllText(settingsPath).Trim();
                        if (!string.IsNullOrEmpty(lang))
                        {
                            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = lang;
                        }
                    }
                }
                else
                {
                    // Fallback for unpackaged mode
                    var localAppData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                    var settingsPath = System.IO.Path.Combine(localAppData, "TeddyMerge", "settings.txt");
                    if (System.IO.File.Exists(settingsPath))
                    {
                        string lang = System.IO.File.ReadAllText(settingsPath).Trim();
                        if (!string.IsNullOrEmpty(lang))
                        {
                            Microsoft.Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = lang;
                        }
                    }
                }

                this.InitializeComponent();
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                this.UnhandledException += (s, e) =>
                {
                    e.Handled = true;
                    System.IO.File.WriteAllText("crash.log", $"UI Exception: {e.Exception?.Message}\n{e.Exception?.StackTrace}");
                };

                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    if (e.ExceptionObject is Exception ex)
                    {
                        System.IO.File.WriteAllText("crash_domain.log", $"Domain Exception: {ex.Message}\n{ex.StackTrace}");
                    }
                };
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("startup_crash.log", $"Startup Exception: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            MainWindow ??= new Window();

            if (MainWindow.Content is not Frame rootFrame)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                MainWindow.Content = rootFrame;
            }

            _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);

            MainWindow.Title = "TeddyMerge";
            try
            {
                var iconPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Assets", "icon.ico");
                MainWindow.AppWindow.SetIcon(iconPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set icon: {ex.Message}");
            }

            MainWindow.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}

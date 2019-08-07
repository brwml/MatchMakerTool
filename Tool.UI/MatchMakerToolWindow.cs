namespace MatchMaker.Tool.UI
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="MatchMakerToolWindow" />
    /// </summary>
    internal class MatchMakerToolWindow : Window
    {
        /// <summary>
        /// Defines the default size
        /// </summary>
        private const double DefaultSize = 12.0;

        /// <summary>
        /// Defines the default thickness
        /// </summary>
        private static readonly Thickness DefaultThickness = new Thickness(DefaultSize);

        /// <summary>
        /// Defines the default weight
        /// </summary>
        private static readonly FontWeight DefaultWeight = FontWeights.Bold;

        /// <summary>
        /// Defines the destination folder label
        /// </summary>
        private readonly Label DestinationFolderLabel = CreateLabel(string.Empty);

        /// <summary>
        /// Defines the generated teams
        /// </summary>
        private readonly TextBox GeneratedTeams = CreateNumericTextBox();

        /// <summary>
        /// Defines the source folder label
        /// </summary>
        private readonly Label SourceFolderLabel = CreateLabel(string.Empty);

        /// <summary>
        /// Defines the tournament teams
        /// </summary>
        private readonly TextBox TournamentTeams = CreateNumericTextBox();

        /// <summary>
        /// Defines the okButton
        /// </summary>
        private Button okButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchMakerToolWindow"/> class.
        /// </summary>
        public MatchMakerToolWindow()
        {
            this.Title = GetTitleText();
            this.Padding = DefaultThickness;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowStyle = WindowStyle.ToolWindow;

            var rootPanel = this.CreateRootPanel();
            rootPanel.Children.Add(this.CreateSourcePanel());
            rootPanel.Children.Add(this.CreateDestinationPanel());
            rootPanel.Children.Add(this.CreateTournamentTeamPanel());
            rootPanel.Children.Add(this.CreateGeneratedTeamPanel());
            rootPanel.Children.Add(this.CreateCommandPanel());
            this.Content = rootPanel;
        }

        /// <summary>
        /// Gets or sets the tool window process
        /// </summary>
        private Process ToolProcess { get; set; }

        /// <summary>
        /// The CreateButton
        /// </summary>
        /// <param name="text">The text<see cref="string"/></param>
        /// <returns>The <see cref="Button"/></returns>
        private static Button CreateButton(string text)
        {
            return new Button
            {
                Content = text,
                FontSize = DefaultSize,
                FontWeight = DefaultWeight,
                Padding = DefaultThickness
            };
        }

        /// <summary>
        /// The CreateLabel
        /// </summary>
        /// <param name="text">The text<see cref="string"/></param>
        /// <returns>The <see cref="Label"/></returns>
        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Content = text,
                FontSize = DefaultSize,
                FontWeight = DefaultWeight,
                Padding = DefaultThickness
            };
        }

        /// <summary>
        /// The CreateNumericTextBox
        /// </summary>
        /// <returns>The <see cref="TextBox"/></returns>
        private static TextBox CreateNumericTextBox()
        {
            var textBox = new TextBox
            {
                FontSize = DefaultSize,
                FontWeight = DefaultWeight,
                Width = 50,
                Padding = DefaultThickness,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            textBox.KeyDown += OnKeyEventRequireNumeric;

            return textBox;
        }

        /// <summary>
        /// The CreateRowPanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private static StackPanel CreateRowPanel()
        {
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = DefaultThickness
            };
        }

        /// <summary>
        /// The GetTitleText
        /// </summary>
        /// <returns>The <see cref="string"/></returns>
        private static string GetTitleText()
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return $"{versionInfo.ProductName} ({versionInfo.ProductVersion})";
        }

        /// <summary>
        /// The OnKeyEventRequireNumeric
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="KeyEventArgs"/></param>
        private static void OnKeyEventRequireNumeric(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.D0 &&
                e.Key != Key.D1 &&
                e.Key != Key.D2 &&
                e.Key != Key.D3 &&
                e.Key != Key.D4 &&
                e.Key != Key.D5 &&
                e.Key != Key.D6 &&
                e.Key != Key.D7 &&
                e.Key != Key.D8 &&
                e.Key != Key.D9 &&
                e.Key != Key.NumPad0 &&
                e.Key != Key.NumPad1 &&
                e.Key != Key.NumPad2 &&
                e.Key != Key.NumPad3 &&
                e.Key != Key.NumPad4 &&
                e.Key != Key.NumPad5 &&
                e.Key != Key.NumPad6 &&
                e.Key != Key.NumPad7 &&
                e.Key != Key.NumPad8 &&
                e.Key != Key.NumPad9
                )
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// The CreateCommandPanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private StackPanel CreateCommandPanel()
        {
            var panel = CreateRowPanel();

            var cancelButton = CreateButton(Properties.Resources.ButtonText_Cancel);
            cancelButton.Width = 120;
            cancelButton.Click += (s, e) => this.Close();
            cancelButton.Margin = new Thickness(0, 0, DefaultSize, 0);
            panel.Children.Add(cancelButton);

            this.okButton = CreateButton(Properties.Resources.ButtonText_OK);
            this.okButton.Width = 120;
            this.okButton.Click += this.OnOk;
            this.okButton.Margin = new Thickness(DefaultSize, 0, 0, 0);
            this.okButton.IsEnabled = false;
            panel.Children.Add(this.okButton);

            return panel;
        }

        /// <summary>
        /// The CreateDestinationPanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private StackPanel CreateDestinationPanel()
        {
            var panel = CreateRowPanel();
            var button = CreateButton(Properties.Resources.ButtonText_SelectDestinationFolder);
            button.Click += this.OnSelectDestinationFolder;
            panel.Children.Add(button);
            panel.Children.Add(this.DestinationFolderLabel);
            return panel;
        }

        /// <summary>
        /// The CreateGeneratedTeamPanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private StackPanel CreateGeneratedTeamPanel()
        {
            var panel = CreateRowPanel();
            panel.Children.Add(CreateLabel(Properties.Resources.LabelText_EnterNumberOfGeneratedTeams));
            panel.Children.Add(this.GeneratedTeams);
            return panel;
        }

        /// <summary>
        /// The CreateRootPanel
        /// </summary>
        /// <returns>The <see cref="Panel"/></returns>
        private Panel CreateRootPanel()
        {
            return new StackPanel { Margin = DefaultThickness };
        }

        /// <summary>
        /// The CreateSourcePanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private StackPanel CreateSourcePanel()
        {
            var panel = CreateRowPanel();
            var button = CreateButton(Properties.Resources.ButtonText_SelectSourceFolder);
            button.Click += this.OnSelectSourceFolder;
            panel.Children.Add(button);
            panel.Children.Add(this.SourceFolderLabel);
            return panel;
        }

        /// <summary>
        /// The CreateTournamentTeamPanel
        /// </summary>
        /// <returns>The <see cref="StackPanel"/></returns>
        private StackPanel CreateTournamentTeamPanel()
        {
            var panel = CreateRowPanel();
            panel.Children.Add(CreateLabel(Properties.Resources.LabelText_EnterNumberOfTournamentTeams));
            panel.Children.Add(this.TournamentTeams);
            return panel;
        }

        /// <summary>
        /// The IsLabelEmpty
        /// </summary>
        /// <param name="control">The control<see cref="Label"/></param>
        /// <returns>The <see cref="bool"/></returns>
        private bool IsLabelEmpty(Label control)
        {
            return string.IsNullOrWhiteSpace(control.Content.ToString());
        }

        /// <summary>
        /// The OnOk
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void OnOk(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            var tournamentTeams = this.TournamentTeams.Text.Trim();
            var generatedTeams = this.GeneratedTeams.Text.Trim();

            var command = "reporting";

            if (!string.IsNullOrWhiteSpace(tournamentTeams) && !string.IsNullOrWhiteSpace(generatedTeams))
            {
                command += $" -m {generatedTeams} -t {tournamentTeams}";
            }

            command += $" -s \"{this.SourceFolderLabel.Content.ToString().Trim()}\"";
            command += $" -o \"{this.DestinationFolderLabel.Content.ToString().Trim()}\"";

            this.ToolProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = command,
                    CreateNoWindow = true,
                    ErrorDialog = true,
                    FileName = "mmt.exe",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            this.ToolProcess.EnableRaisingEvents = true;
            this.ToolProcess.Exited += (x, y) => this.Dispatcher.Invoke(this.Close);
            this.ToolProcess.Start();
        }

        /// <summary>
        /// The OnSelectDestinationFolder
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void OnSelectDestinationFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.DestinationFolderLabel.Content = dialog.SelectedPath;

                    if (!this.IsLabelEmpty(this.SourceFolderLabel))
                    {
                        this.okButton.IsEnabled = true;
                    }
                }
            }

            if (!this.IsLabelEmpty(this.DestinationFolderLabel) && !this.IsLabelEmpty(this.SourceFolderLabel))
            {
                this.okButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// The OnSelectSourceFolder
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void OnSelectSourceFolder(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    this.SourceFolderLabel.Content = dialog.SelectedPath;

                    if (!this.IsLabelEmpty(this.DestinationFolderLabel))
                    {
                        this.okButton.IsEnabled = true;
                    }
                }
            }
        }
    }
}

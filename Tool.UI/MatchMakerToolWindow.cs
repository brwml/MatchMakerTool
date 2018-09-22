using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MatchMaker.Tool.UI
{
    internal class MatchMakerToolWindow : Window
    {
        private static readonly double DefaultSize = 12.0;

        private static readonly Thickness DefaultThickness = new Thickness(DefaultSize);

        private static readonly FontWeight DefaultWeight = FontWeight.FromOpenTypeWeight(500);

        private readonly Label DestinationFolderLabel = CreateLabel(string.Empty);

        private readonly TextBox GeneratedTeams = CreateNumericTextBox();

        private readonly Label SourceFolderLabel = CreateLabel(string.Empty);

        private readonly TextBox TournamentTeams = CreateNumericTextBox();

        private Button okButton;

        public MatchMakerToolWindow()
        {
            this.Title = "Match Maker Tool";
            this.Padding = DefaultThickness;
            this.SizeToContent = SizeToContent.WidthAndHeight;

            var rootPanel = this.CreateRootPanel();
            rootPanel.Children.Add(this.CreateSourcePanel());
            rootPanel.Children.Add(this.CreateDestinationPanel());
            rootPanel.Children.Add(this.CreateTournamentTeamPanel());
            rootPanel.Children.Add(this.CreateGeneratedTeamPanel());
            rootPanel.Children.Add(this.CreateCommandPanel());
            this.Content = rootPanel;
        }

        private Process ToolProcess
        {
            get; set;
        }

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

        private static StackPanel CreateRowPanel()
        {
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = DefaultThickness
            };
        }

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

        private StackPanel CreateCommandPanel()
        {
            var panel = CreateRowPanel();

            var cancelButton = CreateButton("Cancel");
            cancelButton.Width = 120;
            cancelButton.Click += (s, e) => this.Close();
            cancelButton.Margin = new Thickness(0, 0, DefaultSize, 0);
            panel.Children.Add(cancelButton);

            this.okButton = CreateButton("OK");
            this.okButton.Width = 120;
            this.okButton.Click += this.OnOk;
            this.okButton.Margin = new Thickness(DefaultSize, 0, 0, 0);
            this.okButton.IsEnabled = false;
            panel.Children.Add(this.okButton);

            return panel;
        }

        private StackPanel CreateDestinationPanel()
        {
            var panel = CreateRowPanel();
            var button = CreateButton("Select Destination Folder");
            button.Click += this.OnSelectDestinationFolder;
            panel.Children.Add(button);
            panel.Children.Add(this.DestinationFolderLabel);
            return panel;
        }

        private StackPanel CreateGeneratedTeamPanel()
        {
            var panel = CreateRowPanel();
            panel.Children.Add(CreateLabel("Enter number of generated teams:"));
            panel.Children.Add(this.GeneratedTeams);
            return panel;
        }

        private Panel CreateRootPanel()
        {
            return new StackPanel { Margin = DefaultThickness };
        }

        private StackPanel CreateSourcePanel()
        {
            var panel = CreateRowPanel();
            var button = CreateButton("Select Source Folder");
            button.Click += this.OnSelectSourceFolder;
            panel.Children.Add(button);
            panel.Children.Add(this.SourceFolderLabel);
            return panel;
        }

        private StackPanel CreateTournamentTeamPanel()
        {
            var panel = CreateRowPanel();
            panel.Children.Add(CreateLabel("Enter number of tournament teams:"));
            panel.Children.Add(this.TournamentTeams);
            return panel;
        }

        private bool IsLabelEmpty(Label control)
        {
            return string.IsNullOrWhiteSpace(control.Content.ToString());
        }

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
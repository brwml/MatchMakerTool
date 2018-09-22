using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MatchMaker.Tool.UI
{
    internal class MatchMakerToolWindow : Window
    {
        private static readonly double DefaultSize = 12.0;

        private static readonly Thickness DefaultThickness = new Thickness(DefaultSize);

        private static readonly FontWeight DefaultWeight = FontWeight.FromOpenTypeWeight(500);

        private readonly Label DestinationFolderLabel = CreateLabel(string.Empty);

        private readonly TextBox GeneratedTeams = CreateTextBox();

        private readonly Label SourceFolderLabel = CreateLabel(string.Empty);

        private readonly TextBox TournamentTeams = CreateTextBox();

        public MatchMakerToolWindow()
        {
            this.Title = "Match Maker Tool";
            this.Padding = DefaultThickness;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            this.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            this.BorderThickness = new Thickness(1);

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

        private static StackPanel CreateRowPanel()
        {
            return new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = DefaultThickness
            };
        }

        private static TextBox CreateTextBox()
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
            return textBox;
        }

        private StackPanel CreateCommandPanel()
        {
            var panel = CreateRowPanel();

            var cancelButton = CreateButton("Cancel");
            cancelButton.Width = 120;
            cancelButton.Click += (s, e) => this.Close();
            cancelButton.Margin = new Thickness(0, 0, DefaultSize, 0);
            panel.Children.Add(cancelButton);

            var okButton = CreateButton("OK");
            okButton.Width = 120;
            okButton.Click += this.OnOk;
            okButton.Margin = new Thickness(DefaultSize, 0, 0, 0);
            panel.Children.Add(okButton);

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
                }
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
                }
            }
        }
    }
}
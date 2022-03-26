namespace MatchMaker.Tool.Gui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MatchMaker.Tool.Gui.ViewModels;

/// <summary>
/// Interaction logic for ParticipantsView.xaml
/// </summary>
public partial class ParticipantsView : UserControl
{
    public ParticipantsView()
    {
        this.InitializeComponent();
    }

    private void NewChurchButtonClicked(object sender, RoutedEventArgs e)
    {
        var name = this.ChurchNameTextBox.Text;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var church = new ChurchViewModel(name);
            var index = this.ChurchListView.Items.IndexOf(church);

            if (index >= 0)
            {
                this.ChurchListView.SelectedIndex = index;
            }
            else
            {
                this.ChurchListView.Items.Add(church);
                this.ChurchListView.SelectedIndex = -1;

                this.QuizzerChurchComboxBox.Items.Add(church);
                this.QuizzerChurchComboxBox.SelectedIndex = -1;
            }
        }

        this.ClearChurchInputControls();
    }

    private void ChurchNameTextBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        this.UpdateChurchActionButtons();
    }

    private void ChurchListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (this.ChurchListView.SelectedIndex >= 0)
        {
            this.ChurchNameTextBox.Text = this.ChurchListView.SelectedValue.ToString();
        }
    }

    private void UpdateChurchActionButtons()
    {
        var hasText = !string.IsNullOrWhiteSpace(this.ChurchNameTextBox.Text);
        var hasSelection = this.ChurchListView.SelectedIndex >= 0;

        this.NewChurchButton.IsEnabled = hasText;
        this.UpdateChurchButton.IsEnabled = hasText && hasSelection;
        this.DeleteChurchButton.IsEnabled = hasSelection;
    }

    private void UpdateChurchButtonClicked(object sender, RoutedEventArgs e)
    {
        this.ChurchListView.Items[this.ChurchListView.SelectedIndex] = new ChurchViewModel(this.ChurchNameTextBox.Text);
        this.QuizzerChurchComboxBox.Items[this.ChurchListView.SelectedIndex] = new ChurchViewModel(this.ChurchNameTextBox.Text);
        this.ClearChurchInputControls();
    }

    private void DeleteChurchButtonClicked(object sender, RoutedEventArgs e)
    {
        this.ChurchListView.Items.Remove(this.ChurchListView.SelectedItem);
        // TODO: Remove all quizzer references
        this.ClearChurchInputControls();
    }

    private void ClearChurchInputControls()
    {
        this.ChurchNameTextBox.Clear();
        this.ChurchListView.SelectedIndex = -1;

        this.ChurchNameTextBox.Focus();
    }

    //-----------------------------------------------------------------------------------
    // Quizzer elements
    //-----------------------------------------------------------------------------------

    private void QuizzerRookieYearTextBoxKeyDown(object sender, KeyEventArgs e)
    {
        e.Handled = e.Key is
            not Key.D0 and not Key.NumPad0 and
            not Key.D1 and not Key.NumPad1 and
            not Key.D2 and not Key.NumPad2 and
            not Key.D3 and not Key.NumPad3 and
            not Key.D4 and not Key.NumPad4 and
            not Key.D5 and not Key.NumPad5 and
            not Key.D6 and not Key.NumPad6 and
            not Key.D7 and not Key.NumPad7 and
            not Key.D8 and not Key.NumPad8 and
            not Key.D9 and not Key.NumPad9;
    }

    private void NewQuizzerButtonClicked(object sender, RoutedEventArgs e)
    {
        var firstName = this.QuizzerFirstNameTextBox.Text;
        var lastName = this.QuizzerLastNameTextBox.Text;
        var isMale = this.QuizzerMaleGenderChoice.IsChecked.HasValue && this.QuizzerMaleGenderChoice.IsChecked.Value;
        var isFemale = this.QuizzerFemaleGenderChoice.IsChecked.HasValue && this.QuizzerFemaleGenderChoice.IsChecked.Value;
        var rookieYear = int.TryParse(this.QuizzerRookieYearTextBox.Text, out var year) ? year : -1;
        var church = this.QuizzerChurchComboxBox.Text;

        this.QuizzerListView.Items.Add(new QuizzerViewModel(firstName, lastName, isMale, isFemale, rookieYear, church));

        this.ClearQuizzerInputControls();
    }

    private void UpdateQuizzerButtonClicked(object sender, RoutedEventArgs e)
    {
        var firstName = this.QuizzerFirstNameTextBox.Text;
        var lastName = this.QuizzerLastNameTextBox.Text;
        var isMale = this.QuizzerMaleGenderChoice.IsChecked.HasValue && this.QuizzerMaleGenderChoice.IsChecked.Value;
        var isFemale = this.QuizzerFemaleGenderChoice.IsChecked.HasValue && this.QuizzerFemaleGenderChoice.IsChecked.Value;
        var rookieYear = int.TryParse(this.QuizzerRookieYearTextBox.Text, out var year) ? year : -1;
        var church = this.QuizzerChurchComboxBox.Text;

        var quizzerIndex = this.QuizzerListView.SelectedIndex;

        if (quizzerIndex >= 0)
        {
            this.QuizzerListView.Items[quizzerIndex] = new QuizzerViewModel(firstName, lastName, isMale, isFemale, rookieYear, church);

            this.ClearQuizzerInputControls();
        }
    }

    private void ClearQuizzerInputControls()
    {
        //this.QuizzerFirstNameTextBox.Text = string.Empty;
        //this.QuizzerLastNameTextBox.Text = string.Empty;
        //this.QuizzerMaleGenderChoice.IsChecked = false;
        //this.QuizzerFemaleGenderChoice.IsChecked = false;
        //this.QuizzerRookieYearTextBox.Text = string.Empty;
        //this.QuizzerChurchComboxBox.SelectedIndex = -1;
        this.QuizzerFirstNameTextBox.Clear();
        this.QuizzerLastNameTextBox.Clear();
        this.QuizzerMaleGenderChoice.IsChecked = false;
        this.QuizzerFemaleGenderChoice.IsChecked = false;
        this.QuizzerRookieYearTextBox.Clear();
        this.QuizzerChurchComboxBox.SelectedIndex = -1;

    }

    private void QuizzerNameTextChanged(object sender, TextChangedEventArgs e)
    {
        this.UpdateQuizzerActionButtons();
    }

    private void GenderChecked(object sender, RoutedEventArgs e)
    {
        this.UpdateQuizzerActionButtons();
    }

    private void QuizzerRookieYearTextChanged(object sender, TextChangedEventArgs e)
    {
        this.UpdateQuizzerActionButtons();
    }

    private void QuizzerChurchSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        this.UpdateQuizzerActionButtons();
    }

    private void UpdateQuizzerActionButtons()
    {
        var hasFirstName = !string.IsNullOrWhiteSpace(this.QuizzerFirstNameTextBox.Text);
        var hasLastName = !string.IsNullOrWhiteSpace(this.QuizzerLastNameTextBox.Text);
        var hasMaleGender = this.QuizzerMaleGenderChoice.IsChecked.HasValue && this.QuizzerMaleGenderChoice.IsChecked.Value;
        var hasFemaleGender = this.QuizzerFemaleGenderChoice.IsChecked.HasValue && this.QuizzerFemaleGenderChoice.IsChecked.Value;
        var hasRookieYear = int.TryParse(this.QuizzerRookieYearTextBox.Text, out var year) && Math.Abs(DateTime.Now.Year - year) < 10;
        var hasChurch = this.QuizzerChurchComboxBox.SelectedIndex != -1;
        var isSelected = this.QuizzerListView.SelectedIndex != -1;
        var hasInput = hasFirstName && hasLastName && (hasMaleGender || hasFemaleGender) && hasRookieYear && hasChurch;

        this.NewQuizzerButton.IsEnabled = hasInput;
        this.UpdateQuizzerButton.IsEnabled = hasInput && isSelected;
        this.DeleteQuizzerButton.IsEnabled = isSelected;
    }

    private void QuizzerListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var quizzer = this.QuizzerListView.SelectedValue as QuizzerViewModel;

        if (quizzer is not null)
        {
            this.QuizzerFirstNameTextBox.Text = quizzer.FirstName;
            this.QuizzerLastNameTextBox.Text = quizzer.LastName;
            this.QuizzerFemaleGenderChoice.IsChecked = quizzer.IsFemale;
            this.QuizzerMaleGenderChoice.IsChecked = quizzer.IsMale;
            this.QuizzerRookieYearTextBox.Text = quizzer.FirstYear.ToString();
            this.QuizzerChurchComboxBox.SelectedIndex = this.QuizzerChurchComboxBox.Items.IndexOf(quizzer.Church);
            // TODO: Find a way to set this.
        }
    }

    private void DeleteQuizzerButtonClicked(object sender, RoutedEventArgs e)
    {
        this.QuizzerListView.Items.Remove(this.QuizzerListView.SelectedItem);
        this.ClearQuizzerInputControls();
    }

    //-----------------------------------------------------------------------------------
    // Team elements
    //-----------------------------------------------------------------------------------

    private void TeamNameTextChanged(object sender, TextChangedEventArgs e)
    {
        this.UpdateTeamActionButtons();
    }

    private void TeamAbbreviationTextChanged(object sender, TextChangedEventArgs e)
    {
        this.UpdateTeamActionButtons();
    }

    private void UpdateTeamActionButtons()
    {
        var hasName = !string.IsNullOrWhiteSpace(this.TeamNameTextBox.Text);
        var hasAbbrev = !string.IsNullOrWhiteSpace(this.TeamAbbreviationTextBox.Text);
        var isSelected = this.TeamListView.SelectedIndex != -1;

        this.NewTeamButton.IsEnabled = hasName && hasAbbrev;
        this.UpdateTeamButton.IsEnabled = hasName && hasAbbrev && isSelected;
        this.DeleteTeamButton.IsEnabled = isSelected;
    }

    private void TeamListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var team = this.TeamListView.SelectedValue as TeamViewModel;

        if (team is not null)
        {
            this.TeamNameTextBox.Text = team.Name;
            this.TeamAbbreviationTextBox.Text = team.Abbreviation;
            this.TeamQuizzerListView.ItemsSource = team.Quizzers;
        }
    }

    private void NewTeamButtonClicked(object sender, RoutedEventArgs e)
    {
        var name = this.TeamNameTextBox.Text;
        var abbrev = this.TeamAbbreviationTextBox.Text;
        this.TeamListView.Items.Add(new TeamViewModel(name, abbrev));
        this.ClearTeamInputControls();
    }

    private void ClearTeamInputControls()
    {
        this.TeamNameTextBox.Clear();
        this.TeamAbbreviationTextBox.Clear();
        this.TeamListView.SelectedIndex = -1;
    }

    private void UpdateTeamButtonClicked(object sender, RoutedEventArgs e)
    {
        var name = this.TeamNameTextBox.Text;
        var abbrev = this.TeamAbbreviationTextBox.Text;
        this.TeamListView.Items[this.TeamListView.SelectedIndex] = new TeamViewModel(name, abbrev);
        this.ClearTeamInputControls();
    }

    private void DeleteTeamButtonClicked(object sender, RoutedEventArgs e)
    {
        this.TeamListView.Items.Remove(this.TeamListView.SelectedItem);
        this.ClearTeamInputControls();
    }
}

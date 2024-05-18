using Microsoft.Win32;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for QuestionPanel.xaml
    /// </summary>
    public partial class QuestionPanel : UserControl
    {
        public QuestionPanel()
        {
            InitializeComponent();

            ClearEverything();
        }

        List<Question> questions = new();

        int questionIndex = 0;
        int correctAnswers = 0;

        private void nextQuestion_Button_Click(object sender, RoutedEventArgs e)
        {
            NextQuestion();
        }

        private void updateQuestion()
        {
            answer_TextBox.Text = string.Empty;
            answer_TextBox.IsEnabled = true;
            nextQuestion_Button.IsEnabled = true;

            // if no questions were imported, returns nothing
            if (questions.Count == 0)
            {
                return;
            }
            
            // changing the labels for question number and question context
            questionNumber_Label.Content = $"Question #{questionIndex + 1}";
            questionLabel.Content = questions[questionIndex].questionHeading;
        }

        private void answer_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // if the input is empty or contains only spaces it disables the button
            if (answer_TextBox.Text.Trim() == "")
            {
                nextQuestion_Button.IsEnabled = false;
            }
            else
            {
                nextQuestion_Button.IsEnabled = true;
            }
        }

        private void importQuestions(string path)
        {
            try
            {
                ClearEverything();

                using (StreamReader streamReader = new StreamReader(path))
                {
                    string? line;

                    // read and display lines from the file until the end is reached
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] question = line.Split(" --- ");

                        if (question.Length != 2)
                        {
                            continue;
                        }
                        questions.Add(new Question(question[0], question[1]));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occured: " + e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void importQuestions_Button_Click(object sender, RoutedEventArgs e)
        {
            // open a file dialog
            OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; 
            dialog.DefaultExt = ".txt";
            dialog.Filter = "Text documents (.txt)|*.txt";

            bool? result = dialog.ShowDialog();

            // if result is true - imports questions from the selected file
            if (result == false)
            {
                return;
            }
            string filename = dialog.FileName;

            importQuestions(filename);
            updateQuestion();
        }

        private void ClearEverything()
        {
            questions.Clear();

            answer_TextBox.Text = string.Empty;
            questionNumber_Label.Content = "No questions imported";
            questionLabel.Content = "Please, import a question(-s).";

            correctAnswers = 0;
            questionIndex = 0;

            answer_TextBox.IsEnabled = false;
            nextQuestion_Button.IsEnabled = false;
        }

        private void answer_TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                NextQuestion();
            }
        }

        private void NextQuestion()
        {
            answer_TextBox.Focus();

            // check for correct answer
            if (answer_TextBox.Text.ToLower() == questions[questionIndex].answer.Trim().ToLower())
            {
                correctAnswers++;
            }

            // finish the quiz, show final results
            if (questionIndex == questions.Count - 1)
            {
                updateQuestion();
                answer_TextBox.IsEnabled = false;
                nextQuestion_Button.IsEnabled = false;

                string additionalText = correctAnswers == questions.Count ? "Good Job!" : string.Empty;

                MessageBox.Show($"Your score is: {correctAnswers}/{questions.Count}. {additionalText}\nThe questions will now reset.", "Congratulations!", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearEverything();

                return;
            }

            questionIndex++;
            updateQuestion();

        }
    }
}

//hoshiy
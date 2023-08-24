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
using System.Net.Http;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace PasswordGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GenerateButton_Click(null, null); // Generates the password when running the program on the first try
            PasswordBox.IsEnabled = false; // Does disable the option that the user edits the password inbox or add additional characters

        }

        #region Colors Variables

        private Color targetmagenta = (Color)ColorConverter.ConvertFromString("#c4006c"); // Variables that store the hex color, used for animation mostly
        private Color highlightmagenta = (Color)ColorConverter.ConvertFromString("#e5008a");
        private Color lightgray = (Color)ColorConverter.ConvertFromString("#f7f7f7");

        #endregion

        #region GenerateButton_Click
        private async void GenerateButton_Click(object sender, RoutedEventArgs e) // Async method so that other tasks can be done without interference
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l=8";

                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        string trimmedPassword = responseBody.Trim(); // Removes all spaces so that the text is displayed in the center of the textbox, required
                        PasswordBox.Text = trimmedPassword;
                    }
                    else
                    {
                        MessageBox.Show("Request failed with status code: " + response.StatusCode); // Error when the request itself was denied or errors occured
                    }
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Unable to connect to the internet. Please check your internet connection."); // If the response itself could not be made
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message); // Other errors or exceptions
            }
        }
        #endregion

        #region Button_Click
        private async void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            CopyButton.Visibility = Visibility.Hidden; // Hide the button when clicked

            string passwordText = PasswordBox.Text; // Store generated password in variable

            if (!string.IsNullOrEmpty(passwordText)) // Check if the password text is not empty
            {
                Clipboard.SetText(passwordText); // Copy the password text to the clipboard

                await Task.Delay(120); // Show the button after a short delay
                CopyButton.Visibility = Visibility.Visible; // Show the button

                ToolTip toolTip = new ToolTip(); // Create a new ToolTip and attach it to the button, need to be created here to attach it to the event
                toolTip.Content = "Copied!";
                toolTip.Placement = PlacementMode.Top;
                toolTip.PlacementTarget = CopyButton;
                toolTip.Style = (Style)FindResource("CustomToolTipStyle"); // Soloplan colored style is defined in this style section

                toolTip.IsOpen = true; // Open the ToolTip to show the "Copied!" message

                await Task.Delay(2000); // Close the ToolTip after delay

                toolTip.IsOpen = false; // Hide the ToolTip
            }
            else
            {
                await Task.Delay(120); // Show the button after a short delay
                CopyButton.Visibility = Visibility.Visible; // Show the button

                MessageBox.Show("No password to copy."); // Notify the user that there is no password to copy

            }
        }
        #endregion

        #region ToggleButtons
        private void IncludeNumbers_Loaded(object sender, RoutedEventArgs e) // The animation for those ToggleButtons needs to be triggered independently to avoid timing problems
        {
            if (sender is Button button) // Redundant, checks if the attached UI Element from the method is a button
            {
                button.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            }
        }

        private void IncludeSymbols_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            }
        }

        private void IncludeLowercase_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            }
        }

        private void IncludeUppercase_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            }
        }

        private void IncludeWords_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                button.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            }
        }

        #endregion

        private bool isAnimating = false;

        #region ToggleGrid_PreviewMouseLeftButtonDown

        private async void ToggleGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Is called when the user clicks on the specific togglebutton
        {
            if (isAnimating) return; // If the boolean isAnimating is true, the method will stop being executed here to avoid timing issues

            if (sender is Button button) // Redundant check, checks if element is a button => yes
            {
                Ellipse circle = button.Template.FindName("circle", button) as Ellipse; // Store UI Elements in variable to work with them in the animation and behind logic
                Border border = button.Template.FindName("border", button) as Border;

                if (circle != null && border != null) // Safety check, redundant
                {
                    isAnimating = true; // Animation occuring

                    TranslateTransform translateTransform = (TranslateTransform)circle.RenderTransform; // Attaches the translateTransform to the circle for movement animation
                    double currentX = translateTransform.X; // Fatches horizontal position of the circle which is 0, redundant
                    double toPosition;

                    if (currentX < 0.5)
                    {
                        // Move the circle to the right
                        toPosition = button.ActualWidth - circle.ActualWidth - 10; // Button width is 44 and circle width is 16 = 18 px, redundant, could just be 18 
                    }
                    else
                    {
                        // Move the circle to the left
                        toPosition = 0;
                    }

                    AnimateCircle(circle, border, currentX, toPosition);

                    await Task.Delay(TimeSpan.FromSeconds(0.2));

                    isAnimating = false;
                }
            }
        }

        #endregion

        #region AnimateCircle

        private void AnimateCircle(Ellipse circle, Border border, double fromPosition, double toPosition)
        {
            TranslateTransform translateTransform = new TranslateTransform();
            circle.RenderTransform = translateTransform;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.2);

            animation.From = fromPosition;
            animation.To = toPosition;

            // Apply quadratic easing function for smoother animation
            QuadraticEase easing = new QuadraticEase();
            animation.EasingFunction = easing;

            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            // Animate the background color of the border using a new SolidColorBrush instance
            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.Duration = TimeSpan.FromSeconds(0.2);

            if (toPosition == 0) // Circle moving to the left
            {
                colorAnimation.To = Colors.Transparent; // Set to transparent color
            }
            else // Circle moving to the right
            {
                colorAnimation.To = lightgray; // Set your desired color
            }

            SolidColorBrush newBackgroundBrush = new SolidColorBrush();
            newBackgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

            // Apply the new SolidColorBrush instance to the border's Background
            border.Background = newBackgroundBrush;
        }
        #endregion

    }
}

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
            PasswordBox.IsEnabled = false; // Does disable the option that the user edits the password inbox or add additional characters

            // Attach event handler to the slider's ValueChanged event
            mySlider.ValueChanged += MySlider_ValueChanged;
            GenerateButton.Click += GenerateButton_Click;
            InitializeButtonStates();

        }
        private Dictionary<string, Tuple<double>> buttonStates = new Dictionary<string, Tuple<double>>();

        private void InitializeButtonStates()
        {
            // Initialize the dictionary with button names and initial states
            buttonStates.Add(IncludeNumbers.Name, new Tuple<double>(-18));
            buttonStates.Add(IncludeSymbols.Name, new Tuple<double>(18));
            buttonStates.Add(IncludeLowercase.Name, new Tuple<double>(-18));
            buttonStates.Add(IncludeUppercase.Name, new Tuple<double>(-18));
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the label's content with the slider's current value
            sliderValueLabel.Content = (int)e.NewValue;

            int sliderValue = (int)e.NewValue;

            GeneratePassword(sliderValue);
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            GeneratePassword((int)mySlider.Value);
        }

        #region Colors Variables

        private Color targetmagenta = (Color)ColorConverter.ConvertFromString("#c4006c"); // Variables that store the hex color, used for animation mostly
        private Color highlightmagenta = (Color)ColorConverter.ConvertFromString("#e5008a");
        private Color lightgray = (Color)ColorConverter.ConvertFromString("#f7f7f7");

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

        private async void TextBoxGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string passwordText = PasswordBox.Text;

            if (!string.IsNullOrEmpty(passwordText))
            {
                Clipboard.SetText(passwordText);

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
                MessageBox.Show("No password to copy.");
            }
        }

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



        #endregion

        private bool isAnimating = false;

        private double currentX = 0; // Initialize with default value

        #region ToggleGrid_PreviewMouseLeftButtonDown

        private async void ToggleGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) // Is called when the user clicks on the specific togglebutton
        {
            if (isAnimating) return; // If the boolean isAnimating is true, the method will stop being executed here to avoid timing issues

            if (sender is Button button) // Redundant check, checks if element is a button => yes
            {
                if (buttonStates.TryGetValue(button.Name, out var buttonState))
                {
                    Ellipse circle = button.Template.FindName("circle", button) as Ellipse; // Store UI Elements in variable to work with them in the animation and behind logic
                    Border border = button.Template.FindName("border", button) as Border;

                    //MessageBox.Show(button.Name);

                    if (circle != null && border != null) // Safety check, redundant
                    {
                        isAnimating = true; // Animation occuring

                        TranslateTransform translateTransform = (TranslateTransform)circle.RenderTransform; // Attaches the translateTransform to the circle for movement animation
                        currentX = translateTransform.X; // Fatches horizontal position of the circle which is 0

                        bool isLeftAligned = button.Tag.ToString() == "LeftAligned";


                        //MessageBox.Show(currentX.ToString(), isLeftAligned.ToString());

                        double toPosition;

                        if (isLeftAligned)
                        {
                            if (currentX == 0)
                            {
                                // Move the circle to the right
                                toPosition = button.ActualWidth - circle.ActualWidth - 10; // Button width is 44 and circle width is 16 = 18 px, redundant, could just be 18 
                            }
                            else
                            {
                                // Move the circle to the left
                                toPosition = 0;
                            }
                        }
                        else
                        {
                            if (currentX == 0)
                            {
                                // Move the circle to the right
                                toPosition = button.ActualWidth - circle.ActualWidth - 46; // Button width is 44 and circle width is 16 =  px, redundant, could just be 18 
                            }
                            else
                            {
                                // Move the circle to the left
                                toPosition = 0;
                            }
                        }

                        AnimateCircle(circle, border, currentX, toPosition, isLeftAligned);

                        buttonStates[button.Name] = new Tuple<double>(currentX);

                        //MessageBox.Show($"Values: {currentX.ToString()}");

                        await Task.Delay(TimeSpan.FromSeconds(0.2));

                        isAnimating = false;
                    }
                }
            }
        }

        #endregion

        #region AnimateCircle

        private void AnimateCircle(Ellipse circle, Border border, double fromPosition, double toPosition, bool ila)
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

            if (ila)
            {
                if (toPosition == 0) // Circle moving to the left
                {
                    colorAnimation.To = Colors.Transparent; // Set to transparent color
                }
                else // Circle moving to the right
                {
                    colorAnimation.To = lightgray; // Set your desired color
                }
            }
            else
            {
                if (toPosition == 0) // Circle moving to the left
                {
                    colorAnimation.To = lightgray; // Set your desired color
                }
                else // Circle moving to the right
                {
                    colorAnimation.To = Colors.Transparent; // Set to transparent color
                }
            }

            SolidColorBrush newBackgroundBrush = new SolidColorBrush();
            newBackgroundBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);

            // Apply the new SolidColorBrush instance to the border's Background
            border.Background = newBackgroundBrush;
        }
        #endregion

        #region GenerateButton_Click
        private async void GeneratePassword(int sValue)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    int passwordLength = (sValue > 0) ? sValue : 8;

                    string apiUrl = "";

                    bool lowercasetrue = false;
                    bool uppercasetrue = false;
                    bool lowercaseuppercasefalse = false;
                    bool nonumbers = false;

                    double numbersCurrentX, symbolsCurrentX, lowercaseCurrentX, uppercaseCurrentX;

                    if (buttonStates.TryGetValue(IncludeNumbers.Name, out var numbersState) && buttonStates.TryGetValue(IncludeSymbols.Name, out var symbolsstate) && buttonStates.TryGetValue(IncludeLowercase.Name, out var lowercasestate) && buttonStates.TryGetValue(IncludeUppercase.Name, out var uppercasestate))
                    {
                        numbersCurrentX = numbersState.Item1;
                        symbolsCurrentX = symbolsstate.Item1;
                        lowercaseCurrentX = lowercasestate.Item1;
                        uppercaseCurrentX = uppercasestate.Item1;

                        bool numbersActive = numbersCurrentX == -18;
                        bool symbolsActive = symbolsCurrentX == 0;
                        bool lowercaseActive = lowercaseCurrentX == -18;
                        bool uppercaseActive = uppercaseCurrentX == -18;

                        if (numbersActive)
                        {
                            if (symbolsActive)
                            {
                                apiUrl = $"https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l={passwordLength}&sym=true";
                                if (!lowercaseActive) uppercasetrue = true;
                                if (!uppercaseActive) lowercasetrue = true;
                                if (!(lowercaseActive && uppercaseActive)) lowercaseuppercasefalse = true;
                            }
                            else
                            {
                                apiUrl = $"https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l={passwordLength}";
                                if (!uppercaseActive) uppercasetrue = true;
                                if (!lowercaseActive) lowercasetrue = true;
                                if (!(lowercaseActive && uppercaseActive)) apiUrl = $"https://makemeapassword.ligos.net/api/v1/pin/plain?c=1&l={passwordLength}";
                            }
                        }
                        else if (!numbersActive || (!symbolsActive && (lowercaseActive || uppercaseActive)))
                        {
                            apiUrl = $"https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l={passwordLength}";

                            if (lowercaseActive)
                                lowercasetrue = true;

                            if (uppercaseActive)
                                uppercasetrue = true;

                            nonumbers = true;

                            if (symbolsActive)
                                apiUrl += "&sym=true";

                            if (!(lowercaseActive && uppercaseActive))
                                MessageBox.Show("Password cannot be generated without parameters!");
                        }
                        else if (symbolsActive && (lowercaseActive || uppercaseActive))
                        {
                            apiUrl = $"https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l={passwordLength}";

                            if (lowercaseActive)
                                lowercasetrue = true;

                            if (uppercaseActive)
                                uppercasetrue = true;

                            nonumbers = true;
                            apiUrl += "&sym=true";
                        }
                        else if (!numbersActive && symbolsActive && !(lowercaseActive && uppercaseActive))
                        {
                            apiUrl = $"https://makemeapassword.ligos.net/api/v1/unicode/plain?c=1&l={passwordLength}";
                        }
                        else
                        {
                            apiUrl = $"https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l={passwordLength}";
                        }

                        if (!string.IsNullOrEmpty(apiUrl))
                        {
                            HttpResponseMessage response = await client.GetAsync(apiUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                string trimmedPassword = responseBody.Trim();

                                if (lowercaseuppercasefalse)
                                {
                                    // Define a mapping from letters to numbers
                                    string mapping = "abcdefghijklmnopqrstuvwxyz";

                                    StringBuilder numericPasswordBuilder = new StringBuilder();

                                    // Convert the trimmedPassword characters to numbers based on the mapping
                                    foreach (char c in trimmedPassword)
                                    {
                                        if (char.IsLetter(c))
                                        {
                                            char lowercaseChar = char.ToLower(c);
                                            int index = mapping.IndexOf(lowercaseChar);
                                            if (index >= 0)
                                            {
                                                // Replace letters with random digits between 0 and 9
                                                int randomDigit = new Random().Next(10);
                                                numericPasswordBuilder.Append(randomDigit);
                                            }
                                        }
                                        else
                                        {
                                            numericPasswordBuilder.Append(c);
                                        }
                                    }

                                    string numericPassword = numericPasswordBuilder.ToString();
                                    trimmedPassword = numericPassword;
                                }
                                if (nonumbers)
                                {
                                    // Define a mapping from numbers to letters
                                    string mapping = "abcdefghijklmnopqrstuvwxyz";

                                    // Convert the numericPassword characters to letters based on the mapping
                                    string transformedPassword = "";
                                    foreach (char c in trimmedPassword)
                                    {
                                        if (char.IsDigit(c))
                                        {
                                            int index = int.Parse(c.ToString());
                                            if (index >= 0 && index < mapping.Length)
                                            {
                                                transformedPassword += mapping[index];
                                            }
                                        }
                                        else
                                        {
                                            transformedPassword += c;
                                        }
                                    }

                                    trimmedPassword = transformedPassword;
                                }
                                if (lowercasetrue)
                                {
                                    trimmedPassword = trimmedPassword.ToLower();
                                }
                                if (uppercasetrue)
                                {
                                    trimmedPassword = trimmedPassword.ToUpper();
                                }
                                PasswordBox.Text = trimmedPassword;

                            }
                            else
                            {
                                // Handle the case where the API request is not successful
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show($"APIurl: {apiUrl.ToString()}");
                    }
                }
            }
            catch (HttpRequestException)
            {
                MessageBox.Show("Unable to connect to the internet. Please check your internet connection.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }

        }

    }

}


    


        #endregion


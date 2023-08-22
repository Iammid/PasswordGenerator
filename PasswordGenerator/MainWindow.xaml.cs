﻿using System;
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

            GenerateButton_Click(null, null);
            PasswordBox.IsEnabled = false;

            ToggleLetter.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter_Copy.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter_Copy.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter_Copy1.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter_Copy2.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;
            ToggleLetter_Copy3.PreviewMouseLeftButtonDown += ToggleGrid_PreviewMouseLeftButtonDown;

        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l=8";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    string trimmedPassword = responseBody.Trim();
                    PasswordBox.Text = trimmedPassword;
                }
                else
                {
                    MessageBox.Show("Request failed with status code: " + response.StatusCode);
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            CopyButton.Visibility = Visibility.Hidden; // Hide the button

            string passwordText = PasswordBox.Text;

            if (!string.IsNullOrEmpty(passwordText)) // Check if the password text is not empty
            {
                Clipboard.SetText(passwordText); // Copy the password text to the clipboard

                await Task.Delay(120); // Show the button after a short delay
                CopyButton.Visibility = Visibility.Visible; // Show the button

                ToolTip toolTip = new ToolTip(); // Create a new ToolTip and attach it to the button
                toolTip.Content = "Copied!";
                toolTip.Placement = PlacementMode.Top;
                toolTip.PlacementTarget = CopyButton;
                toolTip.Style = (Style)FindResource("CustomToolTipStyle");

                toolTip.IsOpen = true; // Open the ToolTip to show the "Password Copied" message

                await Task.Delay(2000); // Close the ToolTip after a certain delay

                toolTip.IsOpen = false; // Hide the ToolTip
            }
            else
            {
                await Task.Delay(120); // Show the button after a short delay
                CopyButton.Visibility = Visibility.Visible; // Show the button

                MessageBox.Show("No password to copy."); // Notify the user that there is no password to copy

            }
        }

        private void ToggleGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button)
            {
                Ellipse circle = button.Template.FindName("circle", button) as Ellipse;

                if (circle != null)
                {
                    double toPosition;

                    if (isToggled)
                    {
                        toPosition = 0; // Move the circle to the left
                    }
                    else
                    {
                        toPosition = button.ActualWidth - circle.ActualWidth - 10; // Move the circle to the right
                    }

                    AnimateCircle(circle, toPosition);
                }
            }
        }

        private bool isToggled = false;

        private void AnimateCircle(Ellipse circle, double toPosition)
        {
            TranslateTransform translateTransform = new TranslateTransform();
            circle.RenderTransform = translateTransform;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = TimeSpan.FromSeconds(0.2);

            double fromPosition = translateTransform.X;

            animation.From = fromPosition;
            animation.To = toPosition;

            translateTransform.BeginAnimation(TranslateTransform.XProperty, animation);

            isToggled = !isToggled;
        }

    }
}

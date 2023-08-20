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

        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://makemeapassword.ligos.net/api/v1/alphanumeric/plain?c=1&l=7";

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

        private bool isCircleToLeft = true;

        private void ToggleLetter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button toggleButton = sender as Button;
            if (toggleButton != null)
            {
                Ellipse circle = toggleButton.Template.FindName("circle", toggleButton) as Ellipse;
                if (circle != null)
                {
                    DoubleAnimation animation = new DoubleAnimation
                    {
                        Duration = TimeSpan.FromSeconds(0.2)
                    };

                    if (isCircleToLeft)
                    {
                        animation.From = 0;
                        animation.To = 20;
                    }
                    else
                    {
                        animation.From = 20;
                        animation.To = 0;
                    }

                    isCircleToLeft = !isCircleToLeft; // Toggle the circle's position

                    circle.BeginAnimation(TranslateTransform.XProperty, animation);
                }
            }
        }
    }
}
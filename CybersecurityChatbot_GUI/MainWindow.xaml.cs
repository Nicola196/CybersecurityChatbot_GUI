// ============================================================
// FILE: MainWindow.xaml.cs
// PURPOSE: WPF code-behind — handles all UI interactions,
//          animated typing, progress bar, voice greeting,
//          and delegates processing to Chatbot.cs.
// ============================================================

using CybersecurityChatbot;
using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CybersecurityChatbot_GUI

{
    public partial class MainWindow : Window
    {
        // --------------------------------------------------------
        // FIELDS
        // --------------------------------------------------------
        private User _user;
        private IChatbot _chatbot;

        // Timer used for the animated typing effect  (Question 1)
        private DispatcherTimer _typeTimer;
        private string _pendingMessage = string.Empty;
        private int _pendingCharIndex = 0;
        private TextBlock _activeTextBlock;

        // Timer for the animated progress bar  (Question 1)
        private DispatcherTimer _progressTimer;
        private double _progressValue = 0;

        // --------------------------------------------------------
        // CONSTRUCTOR
        // --------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            _chatbot = new Chatbot();
            PlayVoiceGreeting();            // Step 1: play .wav on launch  (Question 1)
            NameInput.Focus();
        }

        // --------------------------------------------------------
        // PlayVoiceGreeting  (Question 1 — voice from Part 1)
        // Plays Audio.wav from the output directory.
        // Add Audio.wav to your project and set
        // "Copy to Output Directory" = "Copy always".
        // --------------------------------------------------------
        private void PlayVoiceGreeting()
        {
            try
            {
                var player = new SoundPlayer("Audio.wav");
                player.Play();
            }
            catch
            {
                // Silently ignore if file is missing — do not crash the app
            }
        }

        // ========================================================
        // NAME ENTRY HANDLERS
        // ========================================================

        private void NameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartChat_Click(sender, null);
        }

        private void StartChat_Click(object sender, RoutedEventArgs e)
        {
            string name = NameInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                // Highlight the input box to signal a problem  (Question 7 — validation)
                NameInput.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                AppendBotMessage(" Please enter your name so I can personalise our chat.", "#FF5050");
                return;
            }

            // Create user object
            _user = new User(name);

            // Hide name panel, enable chat controls
            NamePanel.Visibility = Visibility.Collapsed;
            MessageInput.IsEnabled = true;
            SendButton.IsEnabled = true;
            UserLabel.Text = name + ": ";

            // Enable quick-action buttons
            BtnCyber.IsEnabled = true;
            BtnPhishing.IsEnabled = true;
            BtnPassword.IsEnabled = true;
            BtnScam.IsEnabled = true;
            BtnPrivacy.IsEnabled = true;
            BtnTip.IsEnabled = true;

            // Welcome messages (animated)
            AppendBotMessage($"Welcome, {_user.Name}! How can I assist you today? ", "#00FF88");
            AppendBotMessage(
                "Ask me about: CYBERSECURITY | PHISHING | PASSWORD | SCAM | PRIVACY | BROWSING | SUSPICIOUS LINKS | REPORT\n" +
                "You can also request a 'phishing tip', 'password tip', or 'safety tip'.\n" +
                "Type 'help' to see all topics. Type 'exit' to quit.",
                "#58A6FF");

            MessageInput.Focus();
        }

        // ========================================================
        // MESSAGE SENDING HANDLERS
        // ========================================================

        private void MessageInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Send_Click(sender, null);
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            string input = MessageInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(input))
                return;

            MessageInput.Clear();

            // Display exit command and close
            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                AppendUserMessage(input);
                AppendBotMessage($"Goodbye, {_user.Name}! Stay safe online! ", "#00FF88");
                SendButton.IsEnabled = false;
                MessageInput.IsEnabled = false;
                return;
            }

            AppendUserMessage(input);
            StartProcessingBar();       // animated progress bar  (Question 1)

            // Use Dispatcher to avoid blocking the UI thread
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // Strip punctuation so "phishing!" still matches "phishing"
                string clean = System.Text.RegularExpressions.Regex.Replace(input, @"[^\w\s]", "").Trim();
                string response = _chatbot.ProcessInput(clean, _user);
                AppendBotMessage(response, "#00FF88");
            }), DispatcherPriority.Background);
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            MessageInput.Text = btn.Tag.ToString();
            Send_Click(null, null);
        }

        // ========================================================
        // CHAT DISPLAY HELPERS
        // ========================================================

        /// <summary>
        /// Appends a user message bubble to the chat panel.
        /// </summary>
        private void AppendUserMessage(string text)
        {
            string label = _user != null ? _user.Name : "You";

            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(22, 27, 34)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(88, 166, 255)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(80, 6, 0, 6),
                Padding = new Thickness(12, 8, 12, 8),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            TextBlock tb = new TextBlock
            {
                Text = $"{label}: {text}",
                Foreground = new SolidColorBrush(Color.FromRgb(88, 166, 255)),
                FontFamily = new FontFamily("Courier New"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = tb;
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();
        }

        /// <summary>
        /// Appends a bot message bubble with an animated typing effect.
        /// </summary>
        private void AppendBotMessage(string text, string hexColour = "#00FF88")
        {
            Border bubble = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(13, 17, 23)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(33, 38, 45)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Margin = new Thickness(0, 6, 80, 6),
                Padding = new Thickness(12, 8, 12, 8),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            Color col = (Color)ColorConverter.ConvertFromString(hexColour);

            TextBlock tb = new TextBlock
            {
                Text = " Bot: ",
                Foreground = new SolidColorBrush(col),
                FontFamily = new FontFamily("Courier New"),
                FontSize = 13,
                TextWrapping = TextWrapping.Wrap
            };

            bubble.Child = tb;
            ChatPanel.Children.Add(bubble);
            ScrollToBottom();

            // Animate the message character-by-character  (Question 1)
            StartTypingAnimation(tb, text, col);
        }

        // ========================================================
        // ANIMATED TYPING EFFECT  (Question 1 — typing animation)
        // Uses DispatcherTimer so the UI thread is never blocked.
        // ========================================================
        private void StartTypingAnimation(TextBlock target, string message, Color textColour)
        {
            // Stop any existing animation
            _typeTimer?.Stop();

            _activeTextBlock = target;
            _pendingMessage = message;
            _pendingCharIndex = 0;

            _typeTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(12)    // ~12ms per character (matches Part 1's Thread.Sleep(10))
            };

            _typeTimer.Tick += (s, e) =>
            {
                if (_pendingCharIndex < _pendingMessage.Length)
                {
                    // Append next character
                    _activeTextBlock.Text += _pendingMessage[_pendingCharIndex];
                    _pendingCharIndex++;
                    ScrollToBottom();
                }
                else
                {
                    _typeTimer.Stop();
                }
            };

            _typeTimer.Start();
        }

        // ========================================================
        // ANIMATED PROGRESS BAR  (Question 1 — progress bar)
        // ========================================================
        private void StartProcessingBar()
        {
            ProcessingBar.Value = 0;
            ProcessingBar.Visibility = Visibility.Visible;

            _progressTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };

            _progressTimer.Tick += (s, e) =>
            {
                _progressValue += 10;
                ProcessingBar.Value = _progressValue;

                if (_progressValue >= 100)
                {
                    _progressTimer.Stop();
                    _progressValue = 0;
                    ProcessingBar.Visibility = Visibility.Collapsed;
                }
            };

            _progressValue = 0;
            _progressTimer.Start();
        }

        // ========================================================
        // UTILITY
        // ========================================================
        private void ScrollToBottom()
        {
            ChatScroller.ScrollToBottom();
        }
    }
}
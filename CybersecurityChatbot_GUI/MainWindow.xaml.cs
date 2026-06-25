using System;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CybersecurityChatbot_GUI
{
    public partial class MainWindow : Window
    {
        private User _user;
        private IChatbot _chatbot;

        private DispatcherTimer _typeTimer;
        private string _pendingMessage = string.Empty;
        private int _pendingCharIndex = 0;
        private TextBlock _activeTextBlock;

        private DispatcherTimer _progressTimer;
        private double _progressValue = 0;

        public MainWindow()
        {
            InitializeComponent();
            _chatbot = new Chatbot();
            PlayVoiceGreeting();
            NameInput.Focus();
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                var player = new SoundPlayer("Audio.wav");
                player.Play();
            }
            catch { }
        }

        // Name entry
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
                NameInput.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 80, 80));
                AppendBotMessage("⚠ Please enter your name so I can personalise our chat.", "#FF5050");
                return;
            }

            _user = new User(name);

            NamePanel.Visibility = Visibility.Collapsed;
            MessageInput.IsEnabled = true;
            SendButton.IsEnabled = true;
            UserLabel.Text = name + ": ";

            // Enable all buttons
            BtnCyber.IsEnabled = true;
            BtnPhishing.IsEnabled = true;
            BtnPassword.IsEnabled = true;
            BtnScam.IsEnabled = true;
            BtnPrivacy.IsEnabled = true;
            BtnTip.IsEnabled = true;

            BtnShowTasks.IsEnabled = true;
            BtnAddTask.IsEnabled = true;
            BtnCompleteTask.IsEnabled = true;
            BtnDeleteTask.IsEnabled = true;
            BtnStartQuiz.IsEnabled = true;
            BtnActivityLog.IsEnabled = true;
            BtnHelp.IsEnabled = true;

            AppendBotMessage($"Welcome, {_user.Name}! How can I assist you today? 🛡", "#00FF88");
            AppendBotMessage(
                "Ask me about: CYBERSECURITY | PHISHING | PASSWORD | SCAM | PRIVACY | BROWSING | SUSPICIOUS LINKS | REPORT\n" +
                "You can also manage TASKS, play the QUIZ, or view your ACTIVITY LOG.\n" +
                "Type 'help' to see everything I can do. Type 'exit' to quit.",
                "#58A6FF");

            MessageInput.Focus();
        }

        // Send message
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

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                AppendUserMessage(input);
                AppendBotMessage($"Goodbye, {_user.Name}! Stay safe online! 🛡", "#00FF88");
                SendButton.IsEnabled = false;
                MessageInput.IsEnabled = false;
                return;
            }

            AppendUserMessage(input);
            StartProcessingBar();

            Dispatcher.BeginInvoke(new Action(() =>
            {
                string clean = System.Text.RegularExpressions.Regex.Replace(input, @"[!?]+$", "").Trim();
                string response = _chatbot.ProcessInput(clean, _user);
                AppendBotMessage(response, "#00FF88");
            }), DispatcherPriority.Background);
        }

        // Quick topic buttons (existing)
        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;
            MessageInput.Text = btn.Tag.ToString();
            Send_Click(null, null);
        }

        // Side buttons (Part 3)
        private void SideButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null || _user == null) return;
            string tag = btn.Tag.ToString();

            // If Add Task button, we need to prompt the user for description.
            if (tag == "add task - ")
            {
                // We'll simulate typing "add task - " and put focus in input.
                MessageInput.Text = "add task - ";
                MessageInput.Focus();
                MessageInput.CaretIndex = MessageInput.Text.Length;
                return;
            }

            // For other buttons, we process directly.
            // Special case: Complete/Delete need a keyword, we'll prompt if needed.
            if (tag.StartsWith("mark complete ") || tag.StartsWith("delete task "))
            {
                MessageInput.Text = tag;
                MessageInput.Focus();
                MessageInput.CaretIndex = MessageInput.Text.Length;
                return;
            }

            // Otherwise just send the command as if typed.
            string response = _chatbot.ProcessInput(tag, _user);
            AppendBotMessage(response, "#00FF88");
        }

        // Chat display helpers
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
            StartTypingAnimation(tb, text, col);
        }

        private void StartTypingAnimation(TextBlock target, string message, Color textColour)
        {
            _typeTimer?.Stop();
            _activeTextBlock = target;
            _pendingMessage = message;
            _pendingCharIndex = 0;
            _typeTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(12) };
            _typeTimer.Tick += (s, e) =>
            {
                if (_pendingCharIndex < _pendingMessage.Length)
                {
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

        private void StartProcessingBar()
        {
            ProcessingBar.Value = 0;
            ProcessingBar.Visibility = Visibility.Visible;
            _progressTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(25) };
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

        private void ScrollToBottom()
        {
            ChatScroller.ScrollToBottom();
        }
    }
}
using System;
using System.Collections.Generic;

namespace CybersecurityChatbot_GUI
{
    // ============================================================
    // FILE: Chatbot.cs
    // PURPOSE: Core bot logic. All long text content (topics, tips,
    //          sentiments, follow-ups) lives in ChatbotContent.cs to
    //          keep this file short and focused purely on FLOW —
    //          every Part 2 + Part 3 feature is fully intact.
    // ============================================================

    public interface IChatbot
    {
        string ProcessInput(string input, User user);
    }

    public class Chatbot : IChatbot
    {
        // PART 2 fields
        private static readonly Random _random = new Random();
        private string _currentTopic = string.Empty;

        // PART 3 fields — Task Assistant, Quiz, NLP, Activity Log
        private readonly DatabaseHelper _db = new DatabaseHelper();
        private readonly QuizGame _quiz = new QuizGame();
        private readonly NlpMatcher _nlp = new NlpMatcher();
        private readonly ActivityLog _log = new ActivityLog();
        private bool _awaitingReminderForNewTask = false;

        // Interest-capture phrases (Question 5 — Memory & Recall)
        private static readonly string[] InterestPhrases =
        {
            "interested in", "i like", "i love", "i care about", "concerned about",
            "want to learn about", "want to know about", "fascinated by", "worried about", "focused on"
        };

        // ============================================================
        // PROCESS INPUT — main entry point, called by the GUI
        // ============================================================
        public string ProcessInput(string input, User user)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string lower = input.ToLowerInvariant().Trim();
            user.ConversationHistory.Add(input);

            // --- PART 3: quiz answer takes top priority ---
            if (_quiz.IsActive && !_quiz.IsFinished())
                return HandleQuizAnswer(input, user);

            // --- PART 3: decline reminder offer ---
            if (_awaitingReminderForNewTask && (lower == "no" || lower == "not now" || lower == "skip"))
            {
                _awaitingReminderForNewTask = false;
                return $"No problem, {user.Name}! You can add a reminder later by typing 'remind me in X days'.";
            }

            // --- PART 3: NLP intent detection (tasks, quiz, log) ---
            Intent intent = _nlp.DetectIntent(lower);
            if (intent != Intent.Unknown)
                return HandleIntent(intent, input, lower, user);

            // --- PART 2: memory capture ---
            string memoryReply = TryCaptureInterest(input, lower, user);
            if (memoryReply != null) return memoryReply;

            // --- PART 2: sentiment detection ---
            string sentimentReply = TryDetectSentiment(lower, user);
            if (sentimentReply != null) return sentimentReply;

            // --- PART 2: follow-up flow ---
            if (IsFollowUpPhrase(lower))
                return string.IsNullOrEmpty(_currentTopic)
                    ? $"I'd love to go deeper, {user.Name}! Try: cybersecurity, phishing, password, scam, privacy, browsing, suspicious links, or report."
                    : GetFollowUp(_currentTopic, user);

            // --- PART 2: random tip requests ---
            string tipReply = TryRandomTip(lower);
            if (tipReply != null) return tipReply;

            // --- PART 2: help / greetings ---
            string greetingReply = TryGreeting(lower, user);
            if (greetingReply != null) return greetingReply;

            // --- PART 2: topic keyword matching + numeric sub-menu ---
            string topicReply = TryTopicMatch(lower, user);
            if (topicReply != null) return topicReply;

            // --- Default fallback (Question 7 — error handling) ---
            return $"I'm not sure I understand that, {user.Name}. Can you try rephrasing?\n" +
                   "Type 'help' to see all available topics.";
        }

        // ============================================================
        // PART 3 HANDLERS
        // ============================================================
        private string HandleQuizAnswer(string input, User user)
        {
            var (wasCorrect, feedback) = _quiz.SubmitAnswer(input);
            _log.AddEntry($"Quiz answer: '{input}' ({(wasCorrect ? "correct" : "incorrect")})");

            if (_quiz.IsFinished())
            {
                _log.AddEntry($"Quiz completed - {_quiz.Score}/{_quiz.TotalQuestions}");
                return feedback + "\n\n" + _quiz.GetFinalResultMessage(user.Name);
            }
            return feedback + "\n\n" + _quiz.FormatQuestion(_quiz.GetCurrentQuestion(), _quiz.CurrentIndex + 1);
        }

        private string HandleIntent(Intent intent, string input, string lower, User user)
        {
            switch (intent)
            {
                case Intent.StartQuiz:
                    var first = _quiz.StartQuiz();
                    _log.AddEntry("Quiz started");
                    return $"🎮 {_quiz.TotalQuestions} questions, mixed multiple-choice/true-false, {user.Name}!\n\n" +
                           _quiz.FormatQuestion(first, 1);

                case Intent.ShowLog:
                    return lower.Contains("full") ? _log.GetFullLogFormatted() : _log.GetRecentEntriesFormatted();

                case Intent.SetReminder:
                    int? days = _nlp.ExtractDayCount(lower);
                    if (days == null) return "How many days from now should I remind you? (e.g. 'remind me in 3 days')";
                    var date = DateTime.Now.AddDays(days.Value);
                    _awaitingReminderForNewTask = false;
                    if (_db.SetReminderOnLatestTask(date))
                    {
                        _log.AddEntry($"Reminder set for {date:dd MMM yyyy}");
                        return $"Got it! I'll remind you in {days.Value} day(s) — on {date:dddd, dd MMMM yyyy}.";
                    }
                    return "I couldn't set that reminder. Try adding a task first.";

                case Intent.CompleteTask:
                    string completeTitle = _nlp.ExtractTaskTitle(input, lower);
                    if (_db.MarkTaskCompletedByTitle(completeTitle))
                    {
                        _log.AddEntry($"Task completed: '{completeTitle}'");
                        return $"Nicely done, {user.Name}! Marked as completed. 🎉";
                    }
                    return "Couldn't find a matching task. Type 'show tasks' to see your list.";

                case Intent.DeleteTask:
                    string deleteTitle = _nlp.ExtractTaskTitle(input, lower);
                    if (_db.DeleteTaskByTitle(deleteTitle))
                    {
                        _log.AddEntry($"Task deleted: '{deleteTitle}'");
                        return "That task has been deleted.";
                    }
                    return "Couldn't find a matching task. Type 'show tasks' to see your list.";

                case Intent.AddTask:
                    string title = _nlp.ExtractTaskTitle(input, lower);
                    if (string.IsNullOrWhiteSpace(title))
                        return $"Sure, {user.Name}! e.g. 'Add task - Review privacy settings'.";
                    string desc = $"{title} to help keep your accounts and data secure.";
                    if (_db.AddTask(title, desc, null))
                    {
                        _log.AddEntry($"Task added: '{title}'");
                        _awaitingReminderForNewTask = true;
                        return $"Task added with the description \"{desc}\" Would you like a reminder? (e.g. 'remind me in 3 days', or 'no')";
                    }
                    return "Couldn't save that task — check your MySQL connection.";

                case Intent.ViewTasks:
                    var tasks = _db.GetAllTasks();
                    if (tasks.Count == 0)
                        return $"No tasks yet, {user.Name}. Try 'Add task - Enable two-factor authentication'!";
                    string list = $"📋 Your cybersecurity tasks, {user.Name}:\n\n";
                    foreach (var t in tasks)
                    {
                        string status = t.IsCompleted ? "✅ Completed" : "🔲 Pending";
                        string rem = t.ReminderDate.HasValue ? $" | Reminder: {t.ReminderDate:dd MMM yyyy}" : "";
                        list += $"• {t.Title} — {status}{rem}\n";
                    }
                    return list;

                default:
                    return null;
            }
        }

        // ============================================================
        // PART 2 HANDLERS
        // ============================================================
        private string TryCaptureInterest(string input, string lower, User user)
        {
            foreach (string phrase in InterestPhrases)
            {
                if (!lower.Contains(phrase)) continue;
                int idx = lower.IndexOf(phrase) + phrase.Length;
                string topic = input.Substring(idx).Trim(' ', '.', '!', '?', ',');
                if (string.IsNullOrWhiteSpace(topic)) continue;
                user.FavouriteTopic = topic;
                return $"Great! I'll remember that you're interested in {topic}, {user.Name}. Feel free to ask me anything about it — or type 'help'!";
            }
            return null;
        }

        private string TryDetectSentiment(string lower, User user)
        {
            foreach (var s in ChatbotContent.Sentiments)
                foreach (string key in s.keys)
                    if (lower.Contains(key))
                        return $"I hear you, {user.Name}. {s.empathy}\n\n💡 {s.tip}";
            return null;
        }

        private bool IsFollowUpPhrase(string lower) =>
            lower.Contains("tell me more") || lower.Contains("another tip") || lower.Contains("explain more") ||
            lower.Contains("give me more") || lower.Contains("more info") || lower.Contains("elaborate") ||
            lower.Contains("go on") || lower.Contains("keep going") || lower.Contains("what else") || lower.Contains("continue");

        private string TryRandomTip(string lower)
        {
            if (lower.Contains("phishing tip")) { _currentTopic = "phishing"; return GetRandomResponse("phishing tip"); }
            if (lower.Contains("password tip")) { _currentTopic = "password"; return GetRandomResponse("password tip"); }
            if (lower.Contains("scam tip")) { _currentTopic = "scam"; return GetRandomResponse("scam tip"); }
            if (lower.Contains("safety tip") || lower.Contains("general tip") || lower.Contains("give me a tip") || lower.Contains("random tip"))
                return GetRandomResponse("general safety tip");
            return null;
        }

        private string TryGreeting(string lower, User user)
        {
            if (lower == "help" || lower.Contains("what can you do") || lower.Contains("topics") || lower.Contains("menu"))
                return "Here's everything I can help with, " + user.Name + ":\n\n" +
                       "🔐 CYBERSECURITY | 🎣 PHISHING | 🔑 PASSWORD | 💀 SCAM | 🕵 PRIVACY | 🌐 BROWSING | 🔗 SUSPICIOUS | 📋 REPORT\n\n" +
                       "💡 TIPS: 'phishing tip' | 'password tip' | 'scam tip' | 'safety tip'\n" +
                       "🔄 FOLLOW-UP: 'tell me more'\n" +
                       "✅ TASKS: 'add task - [desc]' | 'show tasks' | 'mark [task] complete' | 'delete task [name]'\n" +
                       "⏰ REMINDERS: 'remind me in 3 days'\n" +
                       "🎮 QUIZ: 'start quiz'\n" +
                       "📜 LOG: 'show activity log'\n\nType 'exit' to quit.";

            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey") || lower == "sup")
                return $"Hello again, {user.Name}! 👋 Type 'help' to see all topics.";
            if (lower.Contains("how are you"))
                return $"Running at full security capacity, {user.Name}! How can I help?";
            if (lower.Contains("thank"))
                return $"You're very welcome, {user.Name}! Stay safe out there. 🛡";
            if (lower.Contains("who are you") || lower.Contains("what are you"))
                return $"I'm your Cybersecurity Awareness Chatbot, {user.Name}! Type 'help' to see what I can do.";
            return null;
        }

        private string TryTopicMatch(string lower, User user)
        {
            string memoryPrompt = string.IsNullOrEmpty(user.FavouriteTopic) ? "" :
                $"\n\n💭 As someone interested in {user.FavouriteTopic}, this might connect too.";

            foreach (var topic in ChatbotContent.Topics)
            {
                if (!lower.Contains(topic.Key)) continue;
                _currentTopic = topic.Key;
                _log.AddEntry($"Topic discussed: '{topic.Key}' (NLP keyword match)");
                var data = topic.Value;
                string response = $"{data.intro}\n\n";
                foreach (string opt in data.opts) response += opt + "\n";
                return response + "\nType 1, 2, or 3 for more detail." + memoryPrompt;
            }

            if (int.TryParse(lower, out int choice) && choice >= 1 && choice <= 3 &&
                !string.IsNullOrEmpty(_currentTopic) && ChatbotContent.Topics.ContainsKey(_currentTopic))
            {
                var det = ChatbotContent.Topics[_currentTopic].det;
                if (choice <= det.Length)
                    return string.Join(" ", det[choice - 1]) +
                           $"\n\nAnything else, {user.Name}? Type 'help' or 'tell me more'." + memoryPrompt;
            }
            return null;
        }

        // ============================================================
        // SHARED HELPERS
        // ============================================================
        private string GetRandomResponse(string category)
        {
            if (!ChatbotContent.RandomResponses.ContainsKey(category))
                return "Try 'phishing tip', 'password tip', 'scam tip', or 'safety tip'.";
            var pool = ChatbotContent.RandomResponses[category];
            return pool[_random.Next(pool.Length)];
        }

        private string GetFollowUp(string topic, User user)
        {
            var followUps = ChatbotContent.BuildFollowUps(user.Name, GetRandomResponse);
            if (!followUps.ContainsKey(topic)) return GetRandomResponse("general safety tip");
            var options = followUps[topic];
            return $"Here's more on {topic}, {user.Name}:\n\n" + options[_random.Next(options.Length)];
        }
    }
}
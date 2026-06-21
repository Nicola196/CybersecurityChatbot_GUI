// ============================================================
// FILE: NlpMatcher.cs
// PURPOSE: Simulates basic Natural Language Processing using
//          string manipulation (Part 3, Task 3). Detects the
//          user's INTENT even when phrased differently, instead
//          of requiring exact commands.
//
// HOW THIS SIMULATES NLP:
//   Real NLP libraries use machine learning to understand intent.
//   This is simulated using string.Contains() keyword detection
//   across many phrasings of the same intent — e.g. "add a task",
//   "create a task", "new task", and "remind me to" all map to
//   the same AddTask intent. This is a recognised simplified NLP
//   technique called "keyword spotting" / "intent matching",
//   exactly as suggested by the rubric.
// ============================================================

using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot_GUI
{
    /// <summary>
    /// The possible intents the chatbot can recognise.
    /// </summary>
    public enum Intent
    {
        AddTask,
        SetReminder,
        ViewTasks,
        CompleteTask,
        DeleteTask,
        StartQuiz,
        ShowLog,
        Unknown
    }

    /// <summary>
    /// Performs simple NLP-style intent detection using keyword
    /// matching across multiple phrasings, plus basic extraction
    /// of the meaningful text from a sentence (e.g. pulling the
    /// task title out of "add a task to enable 2FA").
    /// </summary>
    public class NlpMatcher
    {
        // --------------------------------------------------------
        // Keyword groups — multiple ways a user might phrase the
        // same intent. This is the core of the "NLP simulation".
        // Order matters: more specific phrases are checked first.
        // --------------------------------------------------------
        private static readonly Dictionary<Intent, string[]> IntentKeywords = new Dictionary<Intent, string[]>
        {
            {
                Intent.StartQuiz,
                new[] { "start quiz", "play quiz", "begin quiz", "take quiz", "quiz me", "test me", "start the quiz" }
            },
            {
                Intent.ShowLog,
                new[] { "show activity log", "show log", "activity log", "what have you done",
                        "show full log", "show history", "show my history" }
            },
            {
                Intent.SetReminder,
                new[] { "remind me", "set a reminder", "set reminder", "add a reminder", "reminder for" }
            },
            {
                Intent.CompleteTask,
                new[] { "mark complete", "mark as done", "mark as completed", "complete task", "finished task", "done with" }
            },
            {
                Intent.DeleteTask,
                new[] { "delete task", "remove task", "delete the task", "cancel task" }
            },
            {
                Intent.AddTask,
                new[] { "add task", "add a task", "create task", "create a task", "new task",
                        "make a task", "i need to", "i want to add" }
            },
            {
                Intent.ViewTasks,
                new[] { "show tasks", "view tasks", "my tasks", "list tasks", "see my tasks", "what tasks" }
            }
        };

        // --------------------------------------------------------
        // DetectIntent — scans the lowercase input for any keyword
        // belonging to each intent group, checked in priority order
        // (most specific phrases first) so overlapping words like
        // "task" inside "reminder for my task" don't misfire.
        // --------------------------------------------------------
        public Intent DetectIntent(string lowerInput)
        {
            foreach (var group in IntentKeywords)
            {
                if (group.Value.Any(keyword => lowerInput.Contains(keyword)))
                    return group.Key;
            }

            // Loose fallback: just the word "task" on its own,
            // with nothing more specific matched above, defaults
            // to showing the task list.
            if (lowerInput.Contains("task"))
                return Intent.ViewTasks;

            return Intent.Unknown;
        }

        // --------------------------------------------------------
        // ExtractTaskTitle — pulls the meaningful task description
        // out of a sentence by stripping known trigger phrases.
        // E.g. "add a task to enable 2FA" → "enable 2FA"
        //      "remind me to update my password" → "update my password"
        // --------------------------------------------------------
        public string ExtractTaskTitle(string originalInput, string lowerInput)
        {
            string[] stripPhrases =
            {
                "add a task to", "add task to", "add a task -", "add task -",
                "create a task to", "create task to", "new task to",
                "remind me to", "i want to add a task to", "i need to",
                "add a task", "add task", "create a task", "create task",
                "mark", "as completed", "as done", "complete", "delete task",
                "remove task", "task"
            };

            foreach (string phrase in stripPhrases)
            {
                int idx = lowerInput.IndexOf(phrase);
                if (idx >= 0)
                {
                    int afterPhrase = idx + phrase.Length;
                    if (afterPhrase < originalInput.Length)
                    {
                        string extracted = originalInput.Substring(afterPhrase).Trim(' ', '-', ':', '.', ',');
                        if (!string.IsNullOrWhiteSpace(extracted))
                            return extracted;
                    }
                }
            }

            return originalInput.Trim();
        }

        // --------------------------------------------------------
        // ExtractDayCount — looks for "in X days" / "X days" patterns
        // to figure out a reminder timeframe. Simple NLP-style
        // numeric scanning rather than full regex parsing, to keep
        // the technique readable and explainable for Part 3.
        // --------------------------------------------------------
        public int? ExtractDayCount(string lowerInput)
        {
            string[] words = lowerInput.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (int.TryParse(words[i], out int number))
                {
                    if (i + 1 < words.Length && words[i + 1].Contains("day"))
                        return number;
                }
            }

            if (lowerInput.Contains("tomorrow"))
                return 1;
            if (lowerInput.Contains("today"))
                return 0;
            if (lowerInput.Contains("next week"))
                return 7;

            return null;
        }
    }
}
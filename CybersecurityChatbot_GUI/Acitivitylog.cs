// ============================================================
// FILE: ActivityLog.cs
// PURPOSE: Records actions the chatbot has taken during the
//          session (Part 3, Task 4 — Activity Log Feature).
//          Displayed when the user types "show activity log"
//          or "what have you done for me?"
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbot_GUI
{
    /// <summary>
    /// One entry in the activity log: a short description and
    /// the exact time the action happened.
    /// </summary>
    public class ActivityEntry
    {
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Stores and formats the chatbot's running activity log.
    /// Kept as its own class so logging logic doesn't clutter
    /// Chatbot.cs — single responsibility principle.
    /// </summary>
    public class ActivityLog
    {
        // --------------------------------------------------------
        // FIELD: the underlying list of all recorded actions
        // --------------------------------------------------------
        private readonly List<ActivityEntry> _entries = new List<ActivityEntry>();

        // --------------------------------------------------------
        // AddEntry — records a new action with the current time
        // --------------------------------------------------------
        public void AddEntry(string description)
        {
            _entries.Add(new ActivityEntry
            {
                Description = description,
                Timestamp = DateTime.Now
            });
        }

        // --------------------------------------------------------
        // GetRecentEntriesFormatted — returns the last 5-10 actions
        // as a numbered, readable list with short timestamps.
        // --------------------------------------------------------
        public string GetRecentEntriesFormatted(int count = 10)
        {
            if (_entries.Count == 0)
                return "No actions have been logged yet this session. Try adding a task, taking the quiz, or asking about a topic!";

            var recent = _entries
                .Skip(Math.Max(0, _entries.Count - count))
                .Reverse()
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine(" Here's a summary of recent actions:\n");

            int number = 1;
            foreach (var entry in recent)
            {
                sb.AppendLine($"{number}. {entry.Description} ({entry.Timestamp:HH:mm:ss})");
                number++;
            }

            if (_entries.Count > count)
                sb.AppendLine($"\n...and {_entries.Count - count} earlier action(s). Type 'show full log' to see everything.");

            return sb.ToString().TrimEnd();
        }

        // --------------------------------------------------------
        // GetFullLogFormatted — returns the COMPLETE history
        // --------------------------------------------------------
        public string GetFullLogFormatted()
        {
            if (_entries.Count == 0)
                return "No actions have been logged yet this session.";

            var sb = new StringBuilder();
            sb.AppendLine($" Full activity log ({_entries.Count} action(s) total):\n");

            int number = 1;
            foreach (var entry in _entries)
            {
                sb.AppendLine($"{number}. {entry.Description} ({entry.Timestamp:HH:mm:ss})");
                number++;
            }

            return sb.ToString().TrimEnd();
        }

        // --------------------------------------------------------
        // Count — total number of actions logged this session
        // --------------------------------------------------------
        public int Count => _entries.Count;
    }
}
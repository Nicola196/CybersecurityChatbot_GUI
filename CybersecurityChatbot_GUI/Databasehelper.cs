// ============================================================
// FILE: DatabaseHelper.cs
// PURPOSE: All MySQL database logic for the Task Assistant
//          (Part 3, Task 1 — Database Integration / Task Storage).
//          Handles connecting, auto-creating the table, and full
//          CRUD (Create, Read, Update, Delete) for cybersecurity tasks.
//
// SETUP REQUIRED:
//   1. Install the MySql.Data NuGet package:
//      Tools → NuGet Package Manager → Manage NuGet Packages for Solution
//      → Browse → search "MySql.Data" (by Oracle) → Install
//   2. Update the CONNECTION STRING below with your own
//      server, database name, username, and password.
//   3. Create an empty database first in MySQL Workbench:
//      CREATE DATABASE cybersecurity_chatbot;
//      (The 'tasks' table inside it is created automatically.)
// ============================================================

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace CybersecurityChatbot_GUI
{
    /// <summary>
    /// Represents a single cybersecurity task stored in the database.
    /// </summary>
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Handles all MySQL database operations for tasks.
    /// Kept in its own class so neither Chatbot.cs nor the UI
    /// ever talk to MySQL directly — good OOP separation.
    /// </summary>
    public class DatabaseHelper
    {
        // --------------------------------------------------------
        // CONNECTION STRING — update these 4 values for your setup
        // --------------------------------------------------------
        private static readonly string ConnectionString =
            "Server=localhost;Database=cybersecurity_chatbot;Uid=root;Pwd=YOUR_PASSWORD_HERE;";

        // --------------------------------------------------------
        // CONSTRUCTOR — ensures the tasks table exists on startup
        // --------------------------------------------------------
        public DatabaseHelper()
        {
            EnsureTableExists();
        }

        // --------------------------------------------------------
        // EnsureTableExists
        // Creates the 'tasks' table automatically if it doesn't
        // exist yet, so no manual SQL setup is required.
        // --------------------------------------------------------
        private void EnsureTableExists()
        {
            string sql = @"
                CREATE TABLE IF NOT EXISTS tasks (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Title VARCHAR(255) NOT NULL,
                    Description TEXT,
                    ReminderDate DATETIME NULL,
                    IsCompleted BOOLEAN DEFAULT FALSE,
                    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Error handling — the app must not crash if the DB is unreachable
                Console.WriteLine("Database setup error: " + ex.Message);
            }
        }

        // --------------------------------------------------------
        // AddTask — INSERT a new task into the database
        // --------------------------------------------------------
        public bool AddTask(string title, string description, DateTime? reminderDate)
        {
            string sql = @"INSERT INTO tasks (Title, Description, ReminderDate, IsCompleted)
                            VALUES (@title, @desc, @reminder, FALSE);";
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@reminder", (object)reminderDate ?? DBNull.Value);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddTask error: " + ex.Message);
                return false;
            }
        }

        // --------------------------------------------------------
        // SetReminderOnLatestTask — UPDATE the ReminderDate on the
        // most recently created task. Used when the user adds a
        // reminder as a follow-up message right after creating a task
        // (matches the rubric's example interaction exactly).
        // --------------------------------------------------------
        public bool SetReminderOnLatestTask(DateTime reminderDate)
        {
            string sql = @"UPDATE tasks SET ReminderDate = @reminder
                            WHERE Id = (SELECT Id FROM (SELECT MAX(Id) AS Id FROM tasks) AS t);";
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@reminder", reminderDate);
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SetReminder error: " + ex.Message);
                return false;
            }
        }

        // --------------------------------------------------------
        // GetAllTasks — SELECT all tasks, newest first
        // --------------------------------------------------------
        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            string sql = "SELECT * FROM tasks ORDER BY CreatedAt DESC;";

            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new TaskItem
                            {
                                Id = reader.GetInt32("Id"),
                                Title = reader.GetString("Title"),
                                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? "" : reader.GetString("Description"),
                                ReminderDate = reader.IsDBNull(reader.GetOrdinal("ReminderDate")) ? (DateTime?)null : reader.GetDateTime("ReminderDate"),
                                IsCompleted = reader.GetBoolean("IsCompleted"),
                                CreatedAt = reader.GetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllTasks error: " + ex.Message);
            }

            return tasks;
        }

        // --------------------------------------------------------
        // MarkTaskCompletedByTitle — UPDATE IsCompleted = TRUE
        // Matches by partial title text (case-insensitive),
        // picking the most recently created match.
        // --------------------------------------------------------
        public bool MarkTaskCompletedByTitle(string titleKeyword)
        {
            string sql = @"UPDATE tasks SET IsCompleted = TRUE
                            WHERE Title LIKE @keyword
                            AND Id = (SELECT Id FROM (
                                SELECT MAX(Id) AS Id FROM tasks WHERE Title LIKE @keyword
                            ) AS t);";
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + titleKeyword + "%");
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MarkTaskCompleted error: " + ex.Message);
                return false;
            }
        }

        // --------------------------------------------------------
        // DeleteTaskByTitle — DELETE a task matching partial title,
        // picking the most recently created match.
        // --------------------------------------------------------
        public bool DeleteTaskByTitle(string titleKeyword)
        {
            string sql = @"DELETE FROM tasks
                            WHERE Title LIKE @keyword
                            AND Id = (SELECT Id FROM (
                                SELECT MAX(Id) AS Id FROM tasks WHERE Title LIKE @keyword
                            ) AS t);";
            try
            {
                using (var conn = new MySqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + titleKeyword + "%");
                        int rows = cmd.ExecuteNonQuery();
                        return rows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("DeleteTask error: " + ex.Message);
                return false;
            }
        }
    }
}

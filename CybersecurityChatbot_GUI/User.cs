// ============================================================
// FILE: User.cs
// PURPOSE: Holds all information about the person chatting with
//          the bot — identity, memory/recall, and conversation
//          history. Used throughout Chatbot.cs to personalise
//          every response.
// ============================================================

using System.Collections.Generic;

namespace CybersecurityChatbot_GUI
{
    /// <summary>
    /// Represents the current user of the chatbot session.
    /// </summary>
    public class User
    {
        // -------------------------------------------------------
        // PROPERTY: Name
        // Stores the user's name for personalised replies.
        // -------------------------------------------------------
        public string Name { get; set; }

        // -------------------------------------------------------
        // PROPERTY: IsExiting
        // Flag to signal the chat loop should stop.
        // -------------------------------------------------------
        public bool IsExiting { get; set; }

        // -------------------------------------------------------
        // PROPERTY: FavouriteTopic  (Memory & Recall — Part 2)
        // Stores a cybersecurity topic the user expressed interest in.
        // e.g. "I'm interested in privacy" → FavouriteTopic = "privacy"
        // -------------------------------------------------------
        public string FavouriteTopic { get; set; }

        // -------------------------------------------------------
        // PROPERTY: ConversationHistory  (Memory & Recall — Part 2)
        // Keeps a log of what the user has typed so the bot can
        // reference earlier parts of the conversation.
        // -------------------------------------------------------
        public List<string> ConversationHistory { get; set; }

        // -------------------------------------------------------
        // CONSTRUCTOR
        // -------------------------------------------------------
        public User(string name)
        {
            Name = name;
            IsExiting = false;
            FavouriteTopic = string.Empty;
            ConversationHistory = new List<string>();
        }
    }
}
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    /// <summary>
    /// Holds all information about the person chatting with the bot.
    /// Extended for Part 2 with memory/recall properties.
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
        // PROPERTY: FavouriteTopic  (NEW - Memory & Recall)
        // Stores a cybersecurity topic the user expressed interest in.
        // e.g. "I'm interested in privacy" → FavouriteTopic = "privacy"
        // -------------------------------------------------------
        public string FavouriteTopic { get; set; }

        // -------------------------------------------------------
        // PROPERTY: ConversationHistory  (NEW - Memory & Recall)
        // Keeps a log of what the user has mentioned so the bot
        // can reference earlier parts of the conversation.
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
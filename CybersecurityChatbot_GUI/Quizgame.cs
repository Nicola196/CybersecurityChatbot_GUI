// ============================================================
// FILE: QuizGame.cs
// PURPOSE: Cybersecurity knowledge mini-game (Part 3, Task 2).
//          12 questions, mixed multiple-choice and true/false,
//          shown one at a time, with immediate feedback, a
//          running score, and a final score-based message.
// ============================================================

using System;
using System.Collections.Generic;

namespace CybersecurityChatbot_GUI
{
    /// <summary>
    /// One quiz question. Works for both formats:
    /// - Multiple choice: Options has 4 entries (A-D)
    /// - True/False: Options has 2 entries (True, False)
    /// </summary>
    public class QuizQuestion
    {
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }   // e.g. "C" or "True"
        public string Explanation { get; set; }
    }

    /// <summary>
    /// Manages quiz state: the question bank, current question index,
    /// running score, and whether a quiz is currently in progress.
    /// Kept separate from Chatbot.cs — single responsibility principle.
    /// </summary>
    public class QuizGame
    {
        // --------------------------------------------------------
        // FIELDS
        // --------------------------------------------------------
        private readonly List<QuizQuestion> _questions;
        private readonly Random _random = new Random();

        public bool IsActive { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Score { get; private set; }
        public int TotalQuestions => _questions.Count;

        // --------------------------------------------------------
        // CONSTRUCTOR — builds the 12-question bank
        // --------------------------------------------------------
        public QuizGame()
        {
            _questions = BuildQuestionBank();
            IsActive = false;
            CurrentIndex = 0;
            Score = 0;
        }

        // --------------------------------------------------------
        // BuildQuestionBank — 12 questions covering phishing,
        // password safety, safe browsing, and social engineering.
        // Mix of multiple-choice (A-D) and True/False as required.
        // --------------------------------------------------------
        private List<QuizQuestion> BuildQuestionBank()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it" },
                    CorrectAnswer = "C",
                    Explanation = "Correct answer: C. Reporting phishing emails helps your provider block the sender and protects others too."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: A strong password should be at least 12 characters long.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "True. Passwords under 12 characters can be cracked by modern brute-force tools in hours or even minutes."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is the safest way to check if a link is genuine?",
                    Options = new List<string> { "A) Click it and see what happens", "B) Hover over it to preview the real URL", "C) Forward it to a friend to test", "D) Trust it if it has a padlock icon" },
                    CorrectAnswer = "B",
                    Explanation = "Correct answer: B. Hovering reveals the true destination URL before you click — the padlock alone does NOT guarantee safety."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: You should use the same strong password across multiple accounts so it's easier to remember.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "False. Reusing passwords means one breach compromises every account that shares it — always use unique passwords."
                },
                new QuizQuestion
                {
                    QuestionText = "What is 'social engineering' in cybersecurity?",
                    Options = new List<string> { "A) Building secure networks", "B) Manipulating people into revealing information", "C) A type of antivirus software", "D) Designing user interfaces" },
                    CorrectAnswer = "B",
                    Explanation = "Correct answer: B. Social engineering exploits human psychology rather than technical flaws — often easier than hacking systems directly."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: Two-factor authentication (2FA) means you only need a password to log in.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "False. 2FA requires a SECOND form of verification (like a phone code) in addition to your password — that's what makes it powerful."
                },
                new QuizQuestion
                {
                    QuestionText = "What does HTTPS in a website address indicate?",
                    Options = new List<string> { "A) The site is 100% safe", "B) The connection is encrypted", "C) The site is government-owned", "D) The site has no ads" },
                    CorrectAnswer = "B",
                    Explanation = "Correct answer: B. HTTPS only encrypts the CONNECTION — it does not guarantee the website itself is trustworthy."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: Public Wi-Fi is always safe to use for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "False. Public Wi-Fi can be intercepted by attackers. Use a VPN or mobile data for anything sensitive like banking."
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is a common sign of a phishing email?",
                    Options = new List<string> { "A) Personalised greeting with correct spelling", "B) A sense of urgency demanding immediate action", "C) Coming from a known colleague's verified address", "D) No links or attachments at all" },
                    CorrectAnswer = "B",
                    Explanation = "Correct answer: B. Urgency and pressure tactics are classic phishing red flags designed to stop you from thinking critically."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: Ransomware encrypts your files and demands payment to unlock them.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "True. Ransomware locks your files with encryption, and a clean recent backup is the best protection against it."
                },
                new QuizQuestion
                {
                    QuestionText = "What should you do immediately if you suspect you've been scammed financially?",
                    Options = new List<string> { "A) Wait a few days to see what happens", "B) Contact your bank's fraud line immediately", "C) Post about it on social media first", "D) Confront the scammer directly" },
                    CorrectAnswer = "B",
                    Explanation = "Correct answer: B. Acting fast by contacting your bank gives the best chance of reversing or limiting fraudulent transactions."
                },
                new QuizQuestion
                {
                    QuestionText = "True or False: A password manager is a safe way to store and generate unique passwords.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "True. Password managers like Bitwarden generate and securely store complex, unique passwords so you only remember one master password."
                }
            };
        }

        // --------------------------------------------------------
        // StartQuiz — resets state and begins a new quiz attempt
        // --------------------------------------------------------
        public QuizQuestion StartQuiz()
        {
            IsActive = true;
            CurrentIndex = 0;
            Score = 0;
            return _questions[CurrentIndex];
        }

        // --------------------------------------------------------
        // SubmitAnswer — checks the user's answer against the
        // current question, updates the score, and advances.
        // Returns a tuple: (wasCorrect, explanationText)
        // --------------------------------------------------------
        public (bool wasCorrect, string feedback) SubmitAnswer(string userAnswer)
        {
            if (!IsActive || CurrentIndex >= _questions.Count)
                return (false, "No active question right now. Type 'start quiz' to begin!");

            var question = _questions[CurrentIndex];

            // Accept "A", "a)", "a", "true", "TRUE", etc.
            string clean = userAnswer.Trim().ToUpperInvariant().Replace(")", "");

            bool isCorrect = clean.Equals(question.CorrectAnswer.ToUpperInvariant());

            if (isCorrect)
                Score++;

            CurrentIndex++;

            string feedback = (isCorrect ? " Correct! " : " Incorrect. ") + question.Explanation;
            return (isCorrect, feedback);
        }

        // --------------------------------------------------------
        // GetCurrentQuestion — returns the question currently in play
        // --------------------------------------------------------
        public QuizQuestion GetCurrentQuestion()
        {
            if (CurrentIndex < _questions.Count)
                return _questions[CurrentIndex];
            return null;
        }

        // --------------------------------------------------------
        // IsFinished — true once all questions have been answered
        // --------------------------------------------------------
        public bool IsFinished()
        {
            return CurrentIndex >= _questions.Count;
        }

        // --------------------------------------------------------
        // GetFinalResultMessage — builds the end-of-quiz summary
        // with a score-based encouragement message, exactly as
        // the rubric describes.
        // --------------------------------------------------------
        public string GetFinalResultMessage(string userName)
        {
            IsActive = false;
            double percentage = (double)Score / TotalQuestions * 100;

            string encouragement;
            if (percentage >= 80)
                encouragement = $" Great job! You're a cybersecurity pro, {userName}!";
            else if (percentage >= 50)
                encouragement = $" Good effort, {userName}! Keep learning to stay even safer online.";
            else
                encouragement = $" Keep learning to stay safe online, {userName}!";

            return $"Quiz complete! You scored {Score} out of {TotalQuestions} ({percentage:F0}%).\n\n{encouragement}\n\n" +
                   "Type 'start quiz' to try again, or 'help' to see other topics.";
        }

        // --------------------------------------------------------
        // FormatQuestion — builds the display text for a question,
        // showing one question at a time as required.
        // --------------------------------------------------------
        public string FormatQuestion(QuizQuestion q, int questionNumber)
        {
            string text = $" Question {questionNumber} of {TotalQuestions}:\n{q.QuestionText}\n\n";
            foreach (string opt in q.Options)
                text += opt + "\n";

            text += q.Options.Count == 2
                ? "\nType 'True' or 'False' to answer."
                : "\nType A, B, C, or D to answer.";

            return text;
        }
    }
}
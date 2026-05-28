# Cybersecurity Awareness Chatbot — GUI (Part 2)

A **WPF (Windows Presentation Foundation)** desktop application that educates users about cybersecurity threats and safe online practices. Built in C# using .NET Framework, this is the GUI expansion of a console-based chatbot — featuring animated chat bubbles, sentiment detection, keyword recognition, memory recall, and random response variation.

---

## Preview

```
 __        __   _                           _
 \ \      / /__| | ___  ___  _ __ ___   ___| |
  \ \ /\ / / _ \ |/ __/ _ \| '_ ` _ \ / _ \ |
   \ V  V /  __/ | (_| (_) | | | | | |  __/_|
    \_/\_/ \___|_|\___\___/|_| |_| |_|\___(_)
         ★  Cybersecurity Awareness Chatbot  ★
```

---

##  Project Structure

```
CybersecurityChatbot_GUI/
│
├── MainWindow.xaml          # WPF UI layout — chat window, buttons, progress bar
├── MainWindow.xaml.cs       # UI code-behind — animations, event handlers, voice
├── Chatbot.cs               # All bot logic — topics, sentiment, memory, random tips
├── User.cs                  # User data model — name, favourite topic, chat history
├── Audio.wav                # Voice greeting played on app launch
└── README.md                # This file
```

---

## Features

###  GUI Design
- Dark cybersecurity-themed interface (dark navy background, green/cyan/blue accents)
- ASCII art banner rendered in Courier New monospace font at the top of the window
- Animated **character-by-character typing effect** for all bot responses (using `DispatcherTimer`)
- Animated **progress bar** that runs before each bot response
- Chat bubble layout — user messages appear right-aligned in blue, bot messages left-aligned in green
- Voice greeting plays automatically on launch via `System.Media.SoundPlayer`
- Six **quick-action topic buttons** for one-click topic access
- Name entry panel on first launch; collapses once the user enters their name

###  Keyword Recognition (8 Topics)
Each topic has an introduction and three numbered sub-options with detailed answers:

| Topic | What it covers |
|-------|---------------|
| **Cybersecurity** | What it is, why it matters, how to protect yourself |
| **Phishing** | Spotting fake emails, what to do, why they're convincing |
| **Password** | Strong passwords, reuse dangers, how hackers steal them |
| **Scam** | Common SA scams, how to avoid them, what to do if scammed |
| **Privacy** | Why it matters, how to protect it, social media risks |
| **Browsing** | Safe browsing habits, HTTPS explained, dangerous sites |
| **Suspicious Links** | How to identify, what happens if clicked, how to check safely |
| **Report** | Where to report in SA, how to report phishing, what info to provide |

###  Random Responses
For tip requests, the bot randomly selects from a pool of responses to keep interactions varied:
- `phishing tip` — 8 different phishing tips
- `password tip` — 8 different password tips
- `scam tip` — 5 different scam tips
- `safety tip` / `give me a tip` — 8 different general safety tips

### Conversation Flow
The bot maintains context across messages. Say any of these after discussing a topic to get deeper information without restarting:
> `tell me more` · `another tip` · `explain more` · `give me more` · `more info` · `elaborate` · `go on` · `keep going` · `what else` · `continue`

### Memory & Recall
The bot remembers user details during the session:
- Captures your interest when you say things like *"I'm interested in privacy"* or *"I'm worried about scams"*
- References your stored interest in later responses: *"As someone interested in privacy, you might want to explore..."*
- Maintains a `ConversationHistory` list throughout the session

### Sentiment Detection (8 Emotions)
The bot detects emotional tone and responds empathetically before automatically providing a relevant tip — no second input required:

| Emotion Keywords | Bot Response |
|-----------------|--------------|
| worried, scared, anxious, nervous | Reassurance + immediate safety tip |
| frustrated, angry, annoyed, fed up | Validation + actionable control tip |
| confused, lost, overwhelmed | Simplification + beginner-friendly tip |
| curious, want to know, wondering | Encouragement + discovery tip |
| happy, excited, fantastic | Positive reinforcement + habit tip |
| sad, unhappy, depressed, upset | Empathy + gentle encouragement |
| stressed, exhausted, can't cope | Understanding + priority tip |
| bored, free time, killing time | Engagement + productive challenge |

###  Error Handling
- Empty name input is caught and highlighted with a red border warning
- Unrecognised input returns a polite, helpful default message
- Missing `Audio.wav` is caught silently — the app never crashes
- All UI operations use `DispatcherTimer` / `Dispatcher.BeginInvoke` to prevent thread-blocking

---

##  Getting Started

### Prerequisites
- **Visual Studio 2019 or later**
- **.NET Framework 4.7.2 or later** (NOT .NET Core / .NET 5/6/7/8)
- Windows OS (required for WPF and `System.Media.SoundPlayer`)

### Setup Steps

**1. Create the project**
```
File → New → Project → WPF App (.NET Framework)
Name: CybersecurityChatbot_GUI
```

**2. Add the files**

Replace or add the following files in your project:
- `MainWindow.xaml` — replace the default content
- `MainWindow.xaml.cs` — replace the default content
- `Chatbot.cs` — add as a new class file
- `User.cs` — add as a new class file

**3. Fix the namespace**

Ensure all files use the same namespace. Open each file and confirm the top reads:
```csharp
namespace CybersecurityChatbot_GUI
```

Also confirm `App.xaml` contains:
```xml
StartupUri="MainWindow.xaml"
```

**4. Add the voice greeting**
- Right-click the project in Solution Explorer → **Add → Existing Item**
- Select your `Audio.wav` file
- Click the file in Solution Explorer, press **F4** to open Properties
- Set **Build Action** = `Content`
- Set **Copy to Output Directory** = `Copy always`

**5. Build and run**
```
Press F5  or  Debug → Start Debugging
```

---

## Example Interactions

```
You:  phishing
Bot:  Phishing uses fake messages to trick you into revealing personal information...
      1. How do I spot a phishing email?
      2. What should I do if I receive one?
      3. What makes phishing attacks so convincing?
      Type 1, 2, or 3 to get detailed information.

You:  1
Bot:  Watch for these red flags: the sender's email looks slightly wrong...

You:  tell me more
Bot:  Here's more on phishing: Vishing (voice phishing) is surging in South Africa...

You:  I'm worried about online scams
Bot:  I hear you. It's completely understandable to feel that way...
       Here's a tip: Never share your OTP with anyone...

You:  I'm interested in privacy
Bot:  Great! I'll remember that you're interested in privacy...

You:  cybersecurity
Bot:  Cybersecurity is the practice of protecting computers...
       As someone interested in privacy, you might want to explore how this connects...

You:  password tip
Bot:   PASSWORD TIP: Use Bitwarden — it's completely free...  [randomly selected]

You:  exit
Bot:  Goodbye! Stay safe online! 
```

---

##  Architecture & OOP Design

```
┌─────────────────────┐     calls      ┌──────────────────────────┐
│   MainWindow.xaml   │ ─────────────► │   IChatbot (interface)   │
│   (View / UI)       │                └──────────┬───────────────┘
│                     │                           │ implements
│  - Chat bubbles     │                ┌──────────▼───────────────┐
│  - Typing animation │                │   Chatbot (class)        │
│  - Progress bar     │                │                          │
│  - Voice greeting   │   reads/writes │  - Topics (Dictionary)   │
│  - Quick buttons    │ ◄────────────► │  - Sentiments (List)     │
└─────────────────────┘                │  - RandomResponses       │
                                       │  - ProcessInput()        │
┌─────────────────────┐                │  - GetFollowUp()         │
│   User (class)      │ ◄────────────► │  - GetRandomResponse()   │
│                     │                └──────────────────────────┘
│  - Name             │
│  - FavouriteTopic   │
│  - IsExiting        │
│  - ConversationHistory│
└─────────────────────┘
```

### Design Decisions
- **`IChatbot` interface** — allows the bot implementation to be swapped without changing the UI
- **`Dictionary<string, (...)>`** — O(1) topic lookup; clean, readable data structure
- **`List<(string[], string, string)>`** — tuples keep sentiment data compact and co-located
- **`DispatcherTimer`** — keeps all animations on the UI thread without blocking it
- **`User` class with properties** — encapsulates all session state cleanly

---

## 🇿🇦 South Africa–Specific Content

This chatbot includes locally relevant information:
- SABRIC fraud reporting: **sabric.co.za** · Toll-free: **0800 222 040**
- SAPS Cybercrime Centre: **saps.gov.za**
- Bank fraud lines: FNB · ABSA · Standard Bank · Nedbank · Capitec
- SMS scam reporting: forward to **7726** on Vodacom, MTN, Cell C, Telkom
- POPIA (Protection of Personal Information Act) rights explained
- SA-specific scam types: SIM swap fraud, SARS smishing, pig butchering crypto scams

---

## Licence

This project was developed for academic purposes as part of a Programming coursework assessment.  
**© The Independent Institute of Education (Pty) Ltd 2026**

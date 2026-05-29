using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    // ============================================================
    // FILE: Chatbot.cs
    // PURPOSE: All bot logic — keyword recognition, random responses,
    //          sentiment detection, memory recall, conversation flow.
    //          PART 2 — fully expanded for 100% marks.
    // ============================================================

    public interface IChatbot
    {
        string ProcessInput(string input, User user);
    }

    public class Chatbot : IChatbot
    {
        // ============================================================
        // FIELDS
        // ============================================================

        // Used for random response selection (Question 3)
        private static readonly Random _random = new Random();

        // Tracks the last topic so follow-up phrases work (Question 4)
        private string _currentTopic = string.Empty;

        // ============================================================
        // SENTIMENT DETECTION  (Question 6)
        // Each entry: trigger keywords | empathetic reply | automatic tip.
        // The tip fires immediately — user does NOT need to type again.
        // ============================================================
        private static readonly List<(string[] keys, string empathy, string tip)> Sentiments =
            new List<(string[], string, string)>
        {
            (
                new[] { "worried", "scared", "afraid", "fear", "panicking", "terrified", "anxious", "nervous" },
                "It's completely understandable to feel that way — cyber threats are very real and scammers can be extremely convincing.",
                "Here's something that will help: Never click links in unsolicited emails or text messages. " +
                "Instead, open your browser and type the official website address yourself. " +
                "Also enable two-factor authentication on your email — it is the single most powerful protection you have."
            ),
            (
                new[] { "frustrated", "angry", "annoyed", "mad", "fed up", "furious", "irritated", "rage", "livid" },
                "I completely hear you — dealing with cyber threats and scammers is genuinely infuriating, and your frustration is 100% valid.",
                "Here's a way to take back control: enable two-factor authentication (2FA) on every important account. " +
                "Even if a hacker steals your password through a breach or phishing attack, they still cannot log in without your phone. " +
                "It takes five minutes to set up and stops the vast majority of account takeovers."
            ),
            (
                new[] { "confused", "lost", "dont understand", "don't understand", "not sure", "overwhelmed", "puzzled", "unclear", "what does" },
                "No worries at all — cybersecurity uses a lot of jargon and it can feel like a foreign language at first. Let me break it down simply.",
                "Start with just two things: (1) Use a different, strong password for every account — a password manager like Bitwarden (free) makes this easy. " +
                "(2) Install a reputable free antivirus like Malwarebytes. " +
                "Those two steps alone protect you from the majority of everyday cyber threats."
            ),
            (
                new[] { "curious", "want to know", "wondering", "interested to", "tell me about", "what is", "how does", "can you explain" },
                "I love the curiosity — that mindset is genuinely your best cyber defence!",
                "A great starting exercise: go to HaveIBeenPwned.com and enter your email address. " +
                "It will show you every known data breach your email appeared in. " +
                "If any of those sites share a password you still use today, change it immediately."
            ),
            (
                new[] { "happy", "great", "good mood", "excited", "fantastic", "joyful", "wonderful", "cheerful", "feeling good", "loving it" },
                "That is amazing to hear! Your positive energy is infectious — let's put it to good use.",
                "Fun cybersecurity habit to start today: set up a password manager (Bitwarden is completely free). " +
                "You only need to remember one strong master password and it generates and stores unique, complex passwords for every site you use. " +
                "Your future self will thank you!"
            ),
            (
                new[] { "sad", "unhappy", "feeling down", "depressed", "upset", "miserable", "heartbroken", "crying", "not okay", "terrible" },
                "I'm really sorry you're feeling that way. Please be kind to yourself — difficult days happen to everyone.",
                "Remember: you don't need to be a tech expert to stay safe online. " +
                "Small, simple habits make a huge difference — lock your screen when you step away, keep your apps updated, and never share passwords or OTPs with anyone, even people you trust. " +
                "I'm here to help you through it step by step."
            ),
            (
                new[] { "stressed", "pressure", "burnout", "exhausted", "tired", "drained", "cant cope", "can't cope" },
                "Cybersecurity on top of everything else in life can feel like a lot. You don't have to figure it all out at once.",
                "The most important thing right now: make sure your email account has a strong, unique password and two-factor authentication enabled. " +
                "Your email is the master key to everything else — if a hacker controls it, they can reset every other account. " +
                "Secure that one account first, then we can tackle the rest together."
            ),
            (
                new[] { "bored", "nothing to do", "free time", "killing time", "just browsing" },
                "Well, you've come to the right place! Let's make this time count.",
                "Here's a productive cybersecurity challenge: audit your top 5 most important accounts today. " +
                "Check that each one has a unique password and 2FA enabled. " +
                "It takes about 20 minutes and dramatically improves your online safety."
            )
        };

        // ============================================================
        // TOPICS  (Question 2 — Keyword Recognition)
        // 8 topics, each with rich intro + 3 detailed sub-answers.
        // ============================================================
        private static readonly Dictionary<string, (string intro, string[] opts, string[][] det)> Topics =
            new Dictionary<string, (string, string[], string[][])>
        {
            // ----------------------------------------------------------
            {
                "cybersecurity",
                (
                    "Cybersecurity is the practice of protecting computers, networks, programs, and data from digital attacks, damage, or unauthorised access.",
                    new[]
                    {
                        "1. Why is cybersecurity important?",
                        "2. What are the main types of cyber threats?",
                        "3. How can I protect myself?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "Cybersecurity is critical because nearly every aspect of modern life is connected to the internet — banking, healthcare, communication, and shopping. " +
                            "South Africa has one of the highest cybercrime rates in Africa, with losses estimated at over R2.2 billion per year. " +
                            "A single successful attack can drain your bank account, steal your identity, or lock your business out of its own systems. " +
                            "For individuals, it protects your personal data, financial assets, and privacy. " +
                            "For businesses, it protects customer trust, legal compliance, and operational continuity."
                        },
                        new[]
                        {
                            "The main cyber threats you face every day include:\n" +
                            "• PHISHING — Fake emails or messages tricking you into giving away passwords or clicking harmful links.\n" +
                            "• MALWARE — Malicious software (viruses, spyware, trojans) that infects your device to steal data or cause damage.\n" +
                            "• RANSOMWARE — A type of malware that encrypts all your files and demands payment to unlock them.\n" +
                            "• MAN-IN-THE-MIDDLE — Attackers secretly intercept communication between you and a website (common on public Wi-Fi).\n" +
                            "• SOCIAL ENGINEERING — Manipulating people psychologically into breaking security rules, e.g. impersonating IT support.\n" +
                            "• CREDENTIAL STUFFING — Using leaked username/password combinations from one breach to access your other accounts.\n" +
                            "• ZERO-DAY EXPLOITS — Attacks targeting software vulnerabilities that developers haven't discovered or patched yet."
                        },
                        new[]
                        {
                            "Here is a practical protection checklist:\n" +
                            " Use strong, unique passwords for every account (use a password manager).\n" +
                            " Enable two-factor authentication (2FA) on all important accounts.\n" +
                            " Keep your operating system, browser, and all apps updated.\n" +
                            " Install reputable antivirus software (e.g. Malwarebytes, Windows Defender).\n" +
                            " Use a firewall — Windows has one built in; make sure it's enabled.\n" +
                            " Back up important files regularly to an external drive or cloud service.\n" +
                            " Be sceptical — if something feels wrong, trust your instincts and verify before clicking.\n" +
                            " Use a VPN on public Wi-Fi to encrypt your internet traffic."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "phishing",
                (
                    "Phishing is a cyberattack where criminals send fraudulent messages — usually emails — disguised as trustworthy sources to trick you into revealing sensitive information or clicking harmful links.",
                    new[]
                    {
                        "1. How do I spot a phishing email?",
                        "2. What should I do if I receive one?",
                        "3. What makes phishing attacks so convincing?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "Watch for these red flags in any email or message:\n" +
                            "• SENDER ADDRESS — Look carefully: 'support@paypa1.com' (number 1 instead of L) or 'noreply@fnb-secure.co' are fake.\n" +
                            "• URGENCY & THREATS — 'Your account will be closed in 24 hours!' is designed to make you panic and act without thinking.\n" +
                            "• GENERIC GREETINGS — 'Dear Customer' or 'Dear User' instead of your actual name.\n" +
                            "• SPELLING & GRAMMAR — Poor writing, awkward phrasing, or strange punctuation are common in phishing emails.\n" +
                            "• SUSPICIOUS LINKS — Hover over any link (don't click) to see the real URL. If it doesn't match the sender's company, it's fake.\n" +
                            "• UNEXPECTED ATTACHMENTS — Especially .zip, .exe, .docm files you didn't ask for.\n" +
                            "• REQUESTS FOR SENSITIVE INFO — Legitimate banks and companies NEVER ask for your password, OTP, or full card number by email."
                        },
                        new[]
                        {
                            "If you receive a suspicious email, follow these steps in order:\n" +
                            "1. DO NOT click any links or download any attachments.\n" +
                            "2. DO NOT reply to the email — even to say 'wrong person'.\n" +
                            "3. Report it: In Gmail → three dots → 'Report phishing'. In Outlook → 'Report' → 'Report Phishing'.\n" +
                            "4. Delete the email from your inbox AND your trash folder.\n" +
                            "5. If you DID click a link: change your password for that account immediately, run a full antivirus scan, check your bank statements for unusual activity, and enable 2FA right away.\n" +
                            "6. Warn colleagues or family if the email impersonated a shared service — they may have received it too.\n" +
                            "7. Report it to the South African Cybercrime Centre or SABRIC if it involved financial fraud."
                        },
                        new[]
                        {
                            "Modern phishing attacks are frighteningly convincing because attackers:\n" +
                            "• Copy real logos, layouts, fonts, and colour schemes of legitimate companies perfectly.\n" +
                            "• Use your real name, employer, or recent purchases — sourced from social media or previous data breaches.\n" +
                            "• Create domains that look almost identical to real ones (typosquatting): 'arnazon.com' vs 'amazon.com'.\n" +
                            "• Set up fake HTTPS websites with a padlock icon — many people wrongly believe the padlock means the site is safe.\n" +
                            "• Use psychological manipulation — urgency, fear, reward, or authority — so you react emotionally before thinking critically.\n" +
                            "• Conduct 'spear phishing' — highly targeted attacks using personal research about you specifically.\n" +
                            "• Use 'vishing' (voice phishing) — calling you and impersonating your bank, SARS, or a tech support agent."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "password",
                (
                    "Passwords are the primary key protecting your online accounts. Weak or reused passwords are the leading cause of account takeovers worldwide.",
                    new[]
                    {
                        "1. What makes a strong password?",
                        "2. Why is reusing passwords so dangerous?",
                        "3. How do hackers steal passwords?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "A strong password has these characteristics:\n" +
                            "• LENGTH — At least 12 characters; longer is always stronger. 16+ is ideal.\n" +
                            "• COMPLEXITY — Mix of uppercase letters, lowercase letters, numbers, and symbols (! @ # $ % ^ &).\n" +
                            "• NO PERSONAL INFO — Avoid your name, birthdate, pet's name, or any info someone could find on social media.\n" +
                            "• NO DICTIONARY WORDS — Simple words are cracked instantly by brute-force tools.\n" +
                            "• PASSPHRASES — A random phrase like 'Coffee!Rain42Desk' is both strong AND easier to remember than 'P@ssw0rd'.\n" +
                            "• UNIQUENESS — Every single account needs its own password. No exceptions.\n" +
                            "• PASSWORD MANAGER — Use Bitwarden (free), 1Password, or KeePass to generate and store complex passwords. You only remember one master password."
                        },
                        new[]
                        {
                            "Reusing passwords is one of the most dangerous things you can do online. Here's why:\n" +
                            "• Data breaches happen constantly — millions of username and password combinations are leaked every year from hacked websites.\n" +
                            "• Attackers use 'credential stuffing' — they take your leaked email and password and automatically try them on hundreds of other sites.\n" +
                            "• If your password for a small forum is the same as your banking or email password, one breach compromises everything.\n" +
                            "• Your email account is especially critical — if hackers access it, they can reset the password on every other account you own.\n" +
                            "• Check if your email has been in a known breach at HaveIBeenPwned.com — if it has and you reuse passwords, act immediately.\n" +
                            "• Solution: A password manager generates and remembers unique, complex passwords for every site. You only need to remember one strong master password."
                        },
                        new[]
                        {
                            "Hackers use multiple techniques to steal your passwords:\n" +
                            "• BRUTE FORCE — Software automatically tries millions of password combinations per second until it finds the right one. Short or simple passwords fall in seconds.\n" +
                            "• DICTIONARY ATTACKS — A variation of brute force using common words, phrases, and known passwords from previous leaks.\n" +
                            "• PHISHING — You're tricked into typing your password on a fake website that looks identical to the real one.\n" +
                            "• KEYLOGGERS — Malware secretly installed on your device records every key you press and sends it to the attacker.\n" +
                            "• DATA BREACHES — Hackers break into a company's servers and steal their entire user database, including passwords.\n" +
                            "• SHOULDER SURFING — Someone physically watches you type your password in a public place.\n" +
                            "• MAN-IN-THE-MIDDLE — On unsecured Wi-Fi, attackers intercept the data you send, including login credentials.\n" +
                            "• CREDENTIAL STUFFING — Using username/password pairs stolen from one breach to try logging into other services."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "scam",
                (
                    "Online scams are fraudulent schemes where criminals deceive you to steal your money, personal information, or both.",
                    new[]
                    {
                        "1. What are common online scams in South Africa?",
                        "2. How do I avoid being scammed?",
                        "3. What do I do if I've been scammed?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "These are the most common scams targeting South Africans right now:\n" +
                            "• ADVANCE-FEE FRAUD (419 Scam) — 'Send us R500 to release your R50,000 prize/inheritance.' Once you pay, they disappear or ask for more.\n" +
                            "• ROMANCE SCAMS — Fake online relationships built over weeks or months before the 'partner' invents a crisis and asks for money.\n" +
                            "• FAKE ONLINE SHOPS — Convincing websites selling products that never arrive, often advertised on social media.\n" +
                            "• TECH SUPPORT SCAMS — Pop-ups or calls claiming 'your computer is infected, call this number.' They gain remote access and charge fees.\n" +
                            "• INVESTMENT & CRYPTO SCAMS — Promises of guaranteed high returns. Pig butchering scams build trust over time before stealing everything.\n" +
                            "• JOB SCAMS — Fake job offers requiring you to pay for 'training materials' or that ask for your banking details upfront.\n" +
                            "• SMISHING — Fake SMS messages claiming to be from SARS, FNB, SASSA, or courier companies, asking you to click a link.\n" +
                            "• LOTTERY SCAMS — 'You've won!' — you never entered, and 'winning' requires paying fees first."
                        },
                        new[]
                        {
                            "Protect yourself from scams with these rules:\n" +
                            "• If it sounds too good to be true, it absolutely is. Guaranteed returns, free money, and unexpected prizes are always scams.\n" +
                            "• NEVER send money to someone you've only met online, no matter how long you've been talking.\n" +
                            "• NEVER share your OTP (one-time password) with anyone — not even someone claiming to be from your bank. Banks will NEVER ask for it.\n" +
                            "• NEVER pay upfront fees to receive a prize, inheritance, or job offer.\n" +
                            "• Research before you buy — check seller reviews, look up the company on the CIPC website (cipc.co.za), and search the name + 'scam'.\n" +
                            "• Verify unexpected calls by hanging up and calling the organisation back on their official number from their official website.\n" +
                            "• Use secure payment methods — never pay via EFT to unknown individuals; use PayPal or credit cards where possible.\n" +
                            "• Trust your instincts — if something feels wrong, it probably is. Slow down and verify before acting."
                        },
                        new[]
                        {
                            "If you have been scammed, take these steps immediately:\n" +
                            "1. STOP all communication with the scammer immediately — block them on all platforms.\n" +
                            "2. CONTACT YOUR BANK — Call the fraud line on the back of your card right away if money was transferred. Banks can sometimes reverse transactions if reported quickly.\n" +
                            "3. REPORT TO SAPS — File a case at your nearest police station. Ask for a case number. Go to saps.gov.za for more information.\n" +
                            "4. REPORT TO SABRIC — The South African Banking Risk Information Centre at sabric.co.za handles banking fraud reports.\n" +
                            "5. REPORT TO THE SOUTH AFRICAN CYBERCRIME CENTRE — Contact them through saps.gov.za.\n" +
                            "6. SECURE YOUR ACCOUNTS — Change passwords on any accounts the scammer may have accessed. Enable 2FA immediately.\n" +
                            "7. PRESERVE EVIDENCE — Screenshot all conversations, save email headers, and note dates and amounts — you'll need these for the police report.\n" +
                            "8. WARN OTHERS — Report fake profiles and shops on the platform and warn your contacts."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "privacy",
                (
                    "Online privacy means controlling who can access your personal information, how it is used, and what digital footprint you leave behind on the internet.",
                    new[]
                    {
                        "1. Why does online privacy matter?",
                        "2. How do I protect my privacy online?",
                        "3. What are the privacy risks on social media?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "Your personal data is one of the most valuable things you own in the digital age:\n" +
                            "• Companies collect and sell your browsing habits, location data, purchases, and interests to advertisers — often without clear consent.\n" +
                            "• Cybercriminals use personal information for identity theft — opening credit accounts, loans, or even mortgages in your name.\n" +
                            "• Stalkers and abusers can use your online information to track your physical location and routines.\n" +
                            "• Once data is online it is almost impossible to fully remove — every photo, comment, and post can persist indefinitely.\n" +
                            "• Under South Africa's POPIA (Protection of Personal Information Act), you have legal rights over your data — companies must tell you what they collect, why, and must delete it on your request.\n" +
                            "• A privacy breach can lead to financial loss, reputational damage, blackmail, discrimination, and emotional harm.\n" +
                            "• Your data in the wrong hands can be used to craft highly convincing personalised phishing and scam attempts against you."
                        },
                        new[]
                        {
                            "Practical steps to protect your privacy right now:\n" +
                            "• USE A VPN — Especially on public Wi-Fi. A VPN encrypts your internet traffic so it cannot be intercepted or monitored.\n" +
                            "• BROWSER SETTINGS — Use private/incognito mode for sensitive browsing. Install uBlock Origin to block tracking scripts.\n" +
                            "• APP PERMISSIONS — Regularly review which apps have access to your camera, microphone, location, and contacts. Revoke everything you don't need.\n" +
                            "• CHECK YOUR DIGITAL FOOTPRINT — Google your own name to see what's publicly visible. Adjust privacy settings on any results you control.\n" +
                            "• REVIEW GOOGLE ACTIVITY — Visit myactivity.google.com to see and delete what Google has recorded about you.\n" +
                            "• USE STRONG EMAIL PRIVACY — Consider an email alias service (like SimpleLogin) for signing up to websites so your real email stays private.\n" +
                            "• READ PRIVACY POLICIES — Before signing up for any service, check how they use and store your data.\n" +
                            "• POPIA RIGHTS — You can request that South African companies show you what data they hold and demand it be deleted."
                        },
                        new[]
                        {
                            "Social media is one of the biggest privacy risks most people ignore:\n" +
                            "• OVERSHARING LOCATION — Posting real-time check-ins or 'I'm on holiday!' tells criminals your home is empty.\n" +
                            "• PUBLIC PROFILES — Attackers harvest your name, employer, school, family members, and interests from public profiles for social engineering.\n" +
                            "• FRIEND REQUESTS FROM STRANGERS — Fake accounts collect your personal data or run romance scams. Only connect with people you know.\n" +
                            "• QUIZZES AND GAMES — 'What is your superhero name? Use your pet's name and birth month!' — these harvest your security question answers.\n" +
                            "• TAGGED PHOTOS — Photos reveal your location, daily routine, home address, and who you spend time with.\n" +
                            "• THIRD-PARTY APPS — Apps connected to your social accounts ('Log in with Facebook') can access and sell your data.\n" +
                            "• PRIVACY SETTINGS — Set your profile to private. Review your friend/follower list regularly. Limit who can see past posts.\n" +
                            "• THINK BEFORE POSTING — Ask yourself: Would I be comfortable if my employer, family, or a stranger saw this?"
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "browsing",
                (
                    "Safe browsing is the practice of using the internet in a way that protects your device, personal data, and privacy from online threats.",
                    new[]
                    {
                        "1. How do I browse the internet safely?",
                        "2. What is HTTPS and why does it matter?",
                        "3. How do I recognise and avoid dangerous websites?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "Follow these safe browsing habits every time you go online:\n" +
                            "• KEEP BROWSERS UPDATED — Browser updates patch security vulnerabilities that attackers actively exploit. Enable automatic updates.\n" +
                            "• USE A REPUTABLE AD-BLOCKER — Install uBlock Origin (free). It blocks malicious ads, tracking scripts, and pop-ups that can infect your device.\n" +
                            "• AVOID POP-UP DOWNLOADS — Legitimate software is never pushed at you through pop-up ads. Always download from official websites.\n" +
                            "• USE DNS SECURITY — Change your DNS to Cloudflare (1.1.1.1) or Google (8.8.8.8) for faster browsing with built-in malicious site blocking.\n" +
                            "• USE A VPN — On any network you don't control (coffee shops, airports, hotels), a VPN encrypts your traffic so it can't be intercepted.\n" +
                            "• CHECK DOWNLOADS — Before opening any downloaded file, right-click and scan with your antivirus. Better yet, check it at VirusTotal.com first.\n" +
                            "• LOG OUT — Always log out of banking, email, and shopping sites when you're done, especially on shared or public computers.\n" +
                            "• PRIVATE MODE — Use incognito/private browsing when using shared computers so your session data isn't saved."
                        },
                        new[]
                        {
                            "HTTPS (HyperText Transfer Protocol Secure) is the secure version of HTTP — the protocol your browser uses to communicate with websites:\n" +
                            "• ENCRYPTION — HTTPS encrypts the data travelling between your browser and the website. Without it, anyone on the same network can read your data.\n" +
                            "• THE PADLOCK ICON — When you see a padlock in the address bar, the connection to that site is encrypted. Click it to view the security certificate.\n" +
                            "• IMPORTANT WARNING — The padlock only means the CONNECTION is encrypted. It does NOT guarantee the website itself is safe or legitimate. Many phishing sites now use HTTPS too.\n" +
                            "• HTTP SITES — Never enter passwords, personal information, or payment details on any site without HTTPS. Look for 'https://' at the start of the URL.\n" +
                            "• CERTIFICATE ERRORS — If your browser shows a security certificate warning, do NOT proceed. The site may be fake or compromised.\n" +
                            "• HSTS — Some sites use HTTP Strict Transport Security, forcing browsers to always use HTTPS. This provides additional protection against downgrade attacks.\n" +
                            "• CHECK THE FULL URL — Always look at the complete address bar, not just the padlock: 'https://paypal.scam-site.com' has HTTPS but is still dangerous."
                        },
                        new[]
                        {
                            "Know the warning signs of a dangerous website:\n" +
                            "• MISSPELLED DOMAINS — 'gooogle.com', 'arnazon.com', 'faceb00k.com' — attackers register typos of popular sites (typosquatting).\n" +
                            "• MISMATCHED URL — The link says 'FNB Bank' but the URL shows something completely different. Always check the address bar.\n" +
                            "• EXCESSIVE POP-UPS — Legitimate websites don't bombard you with pop-ups, especially ones claiming your device is infected.\n" +
                            "• IMMEDIATE DOWNLOAD PROMPTS — A site that immediately tries to download something without your action is almost certainly malicious.\n" +
                            "• REQUESTS FOR PERSONAL INFO ON ARRIVAL — Any site that asks for your login details the moment you land, without you initiating it, is suspicious.\n" +
                            "• SUSPICIOUS REDIRECTS — You clicked one link but ended up somewhere completely different.\n" +
                            "• CHECK BEFORE VISITING — Paste any suspicious URL into VirusTotal.com or URLVoid.com before opening it.\n" +
                            "• USE GOOGLE SAFE BROWSING — Visit safebrowsing.google.com/safebrowsing/report_phish to check or report suspicious sites."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "suspicious",
                (
                    "Suspicious links are URLs crafted to look harmless or trustworthy but actually lead to malicious websites designed to steal data, install malware, or commit fraud.",
                    new[]
                    {
                        "1. How do I identify a suspicious link?",
                        "2. What can happen if I click a malicious link?",
                        "3. How do I check a link safely before clicking?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "Learn to spot suspicious links before you click:\n" +
                            "• MISSPELLED DOMAINS — 'paypa1.com', 'g00gle.com', 'arnazon.com' — one character changed to fool you at a glance.\n" +
                            "• URL SHORTENERS — bit.ly, tinyurl, or ow.ly links hide the real destination completely. Be very cautious with these.\n" +
                            "• RANDOM STRINGS — Long URLs full of random characters (e.g. 'xkdf83ksd.xyz/q?id=98aks') are typical of malicious redirects.\n" +
                            "• DOMAIN MISMATCH — The display text says 'Click here for FNB' but hovering shows a completely unrelated domain.\n" +
                            "• WRONG TOP-LEVEL DOMAIN — 'sars.gov.za.fake-site.com' — the real domain here is 'fake-site.com', not 'sars.gov.za'.\n" +
                            "• UNSOLICITED MESSAGES — Links arriving in emails, SMS, or WhatsApp from unknown senders or unexpected contexts should always be treated with suspicion.\n" +
                            "• HTTP (NOT HTTPS) — Especially for login pages. Never enter credentials on a non-HTTPS site.\n" +
                            "• HOVER FIRST — On desktop, hover over any link to see the real URL displayed in the browser's status bar before clicking."
                        },
                        new[]
                        {
                            "Clicking a malicious link can have serious consequences:\n" +
                            "• MALWARE INSTALLATION — A 'drive-by download' can silently install viruses, spyware, or ransomware just by visiting the page (especially if your browser is outdated).\n" +
                            "• CREDENTIAL THEFT — You're taken to a convincing fake login page. You enter your username and password, which are sent directly to the attacker.\n" +
                            "• COOKIE HIJACKING — The malicious site steals your browser session cookies, giving attackers access to accounts you're already logged into without needing your password.\n" +
                            "• RANSOMWARE — Your files become encrypted and a payment demand appears. Without a backup, you may lose everything.\n" +
                            "• DEVICE TAKEOVER — Remote Access Trojans (RATs) give attackers complete control of your computer, including your camera and microphone.\n" +
                            "• FINANCIAL FRAUD — Fake banking sites capture your account details and OTPs in real time, draining your account immediately.\n" +
                            "• IDENTITY THEFT — Collected personal information is used to open accounts, apply for loans, or commit fraud in your name.\n" +
                            "• What to do if you clicked: disconnect from Wi-Fi, run a full antivirus scan immediately, change all passwords from a safe device, and contact your bank."
                        },
                        new[]
                        {
                            "Check any suspicious link safely using these methods:\n" +
                            "• HOVER (DESKTOP) — On desktop, hover your mouse over the link. The real URL appears in the status bar at the bottom of the browser window.\n" +
                            "• VIRUSTOTAL.COM — Copy the URL and paste it into VirusTotal.com. It scans the link against 70+ security engines simultaneously and reports if any flag it as malicious.\n" +
                            "• URLVOID.COM — Another link scanner that checks against multiple blacklists and reputation services.\n" +
                            "• GOOGLE SAFE BROWSING — Visit transparencyreport.google.com/safe-browsing/search and paste the URL to check Google's safety assessment.\n" +
                            "• EXPAND SHORT URLs — Use CheckShortURL.com or unshorten.it to reveal what a shortened link actually points to before visiting.\n" +
                            "• WHOIS LOOKUP — Visit whois.domaintools.com to see when a domain was registered. Scam sites are often registered just days or weeks ago.\n" +
                            "• NEVER COPY-PASTE INTO BROWSER IF SUSPICIOUS — Paste it into a text editor first to see the full URL clearly.\n" +
                            "• ON MOBILE — Long-press a link to preview the URL before tapping."
                        }
                    }
                )
            },

            // ----------------------------------------------------------
            {
                "report",
                (
                    "Reporting cybercrime is a crucial step in stopping attackers from targeting more victims. In South Africa, you have several official channels to report digital crimes.",
                    new[]
                    {
                        "1. Where do I report cybercrime in South Africa?",
                        "2. How do I report phishing emails and scam messages?",
                        "3. What information should I have ready when reporting?"
                    },
                    new[]
                    {
                        new[]
                        {
                            "South Africa has several official reporting channels for cybercrime:\n" +
                            "• SAPS CYBERCRIME UNIT — Report to your nearest SAPS station or contact the SAPS Cybercrime Centre. Visit saps.gov.za for contact details. Always get a case number.\n" +
                            "• SABRIC — The South African Banking Risk Information Centre handles banking fraud. Visit sabric.co.za or call 0800 222 040 (toll-free).\n" +
                            "• FNB Fraud Line: 087 575 9444 | ABSA: 0800 111 155 | Standard Bank: 0800 020 600 | Nedbank: 0800 110 929 | Capitec: 0860 10 20 43\n" +
                            "• SOUTH AFRICAN RESERVE BANK — For forex-related financial fraud: sarb.co.za\n" +
                            "• ICASA — For telecommunications fraud (smishing via SMS): icasa.org.za\n" +
                            "• INTERPOL — For international cybercrime: interpol.int/en/Crimes/Cybercrime\n" +
                            "• CONSUMER GOODS AND SERVICES OMBUD — For online shopping scams: cgso.org.za\n" +
                            "• NATIONAL CONSUMER COMMISSION — ncc.gov.za for consumer protection issues including online scams."
                        },
                        new[]
                        {
                            "Report phishing and scam messages through these channels:\n" +
                            "• GMAIL — Open the suspicious email, click the three dots (⋮) in the top right, and select 'Report phishing'. Google will review and block the sender.\n" +
                            "• OUTLOOK/HOTMAIL — Select the email, click 'Report' in the toolbar, then choose 'Report Phishing'. Microsoft will investigate.\n" +
                            "• APPLE MAIL — Forward the email to reportphishing@apple.com.\n" +
                            "• FORWARD TO AUTHORITIES — Forward phishing emails to phishing@sabric.co.za (South Africa) or phishing-report@us-cert.gov (international).\n" +
                            "• SMISHING (SMS SCAMS) — Forward suspicious SMS messages to 7726 (SPAM) — this works on most South African networks (Vodacom, MTN, Cell C, Telkom).\n" +
                            "• WHATSAPP SCAMS — Open the chat, tap the contact name, scroll down and tap 'Report'. Also block the number.\n" +
                            "• SOCIAL MEDIA SCAMS — Use the 'Report' function on Facebook, Instagram, Twitter/X, or TikTok. Report fake accounts, scam pages, and fraudulent ads.\n" +
                            "• FAKE WEBSITES — Report to Google at safebrowsing.google.com/safebrowsing/report_phish."
                        },
                        new[]
                        {
                            "When reporting cybercrime, have this information ready:\n" +
                            "• SCREENSHOTS — Take screenshots of all fraudulent messages, emails, pop-ups, or websites. Capture the full screen including the URL bar.\n" +
                            "• EMAIL HEADERS — For email scams, include the full email headers (in Gmail: three dots → 'Show original'). These contain routing information that helps trace the attacker.\n" +
                            "• URLS AND DOMAINS — Note the exact URLs or domain names of any suspicious websites you encountered.\n" +
                            "• DATES AND TIMES — Record exactly when each incident occurred, including time zone.\n" +
                            "• FINANCIAL DETAILS — If money was stolen: bank names, account numbers, reference numbers, transaction amounts, and dates of all transactions.\n" +
                            "• COMMUNICATION RECORDS — Save all chat logs, emails, or call records from the attacker.\n" +
                            "• YOUR PERSONAL DETAILS — Your full name, ID number, contact number, and address for the official report.\n" +
                            "• DO NOT DELETE EVIDENCE — Even if content is disturbing, preserve all evidence until after you've reported it and received a case number."
                        }
                    }
                )
            }
        };

        // ============================================================
        // RANDOM RESPONSES  (Question 3)
        // Multiple responses per category — one selected randomly.
        // Each category has at least 6 options for good variation.
        // ============================================================
        private static readonly Dictionary<string, string[]> RandomResponses =
            new Dictionary<string, string[]>
        {
            {
                "phishing tip",
                new[]
                {
                    " PHISHING TIP: Be extremely cautious of emails creating urgency — 'Your account will be suspended in 24 hours!' Scammers use panic to bypass your critical thinking. Take a breath, close the email, and go directly to the official website.",
                    " PHISHING TIP: Always verify the sender's email address character by character. Fraudsters register domains like 'support@paypa1.com' (number 1 instead of L) or 'fnb-secure@mail-fnb.co' — convincing at a glance but fake.",
                    " PHISHING TIP: Legitimate organisations will NEVER ask for your password, OTP, PIN, or full credit card number via email or SMS. If any message asks for these, it is a scam — no exceptions.",
                    " PHISHING TIP: When in doubt about a link, open a new browser tab and type the official website address yourself. Never follow a link in an unexpected email, even if it looks perfect.",
                    " PHISHING TIP: Hover over any link in an email (on desktop) before clicking. The real destination URL appears in the bottom status bar. If it doesn't match the expected site, don't click.",
                    " PHISHING TIP: Watch for generic greetings like 'Dear Customer' or 'Dear User' in emails claiming to be from your bank. Your real bank knows your name and uses it.",
                    " PHISHING TIP: Spear phishing uses your real name, employer, and recent activity (gathered from social media) to create highly personalised fake emails. Limit what you share publicly online.",
                    " PHISHING TIP: Vishing (voice phishing) is surging in South Africa. Scammers call claiming to be from your bank or SARS. Hang up and call back on the official number from the organisation's website — never the number they give you."
                }
            },
            {
                "password tip",
                new[]
                {
                    " PASSWORD TIP: Use Bitwarden — it's completely free, open-source, and generates complex unique passwords for every site. You only remember one strong master password. It's the single most impactful security upgrade most people can make.",
                    " PASSWORD TIP: Enable two-factor authentication (2FA) on every account that offers it — especially email, banking, and social media. Even if your password is stolen, attackers cannot log in without your phone.",
                    " PASSWORD TIP: Build passphrases, not passwords. 'Coffee!Rain42Desk' is far stronger than 'P@ssw0rd123' and much easier to remember. Use four random words with numbers and symbols between them.",
                    " PASSWORD TIP: Check if your accounts have been compromised — visit HaveIBeenPwned.com and enter your email address. If your email appears in a breach, change that password everywhere you used it immediately.",
                    " PASSWORD TIP: Never store passwords in a plain text file, sticky note, or your browser's basic save function. Use a dedicated encrypted password manager with a strong master password and 2FA on the manager itself.",
                    " PASSWORD TIP: Your email password is your most critical password — it's the master key to every account. If attackers control your email, they can reset every other account. Make it long, unique, and protect it with 2FA.",
                    " PASSWORD TIP: Change passwords immediately if you hear a service you use has been breached — even if you haven't been notified yet. Don't wait for the company to contact you.",
                    " PASSWORD TIP: Use a hardware security key (like a YubiKey) as your second factor for highest-security accounts. Unlike SMS codes, hardware keys cannot be stolen remotely or intercepted."
                }
            },
            {
                "general safety tip",
                new[]
                {
                    " SAFETY TIP: Keep ALL your software updated — operating system, browser, apps, and antivirus. The majority of successful cyberattacks exploit known vulnerabilities that updates already fix. Enable automatic updates.",
                    " SAFETY TIP: Never use public Wi-Fi for banking, shopping, or anything sensitive without a VPN. On open networks, everything you send is visible to anyone with basic tools. A VPN encrypts your traffic completely.",
                    " SAFETY TIP: Back up your important files using the 3-2-1 rule: 3 copies of your data, on 2 different types of storage, with 1 copy stored offsite or in the cloud. Ransomware is powerless against a clean, recent backup.",
                    " SAFETY TIP: Lock your screen every time you leave your device — even for a minute. Use Windows Key + L (Windows) or Control + Command + Q (Mac). It takes one second and prevents physical access attacks.",
                    " SAFETY TIP: Review your app permissions monthly. Go to your phone's Settings → Apps → Permissions and revoke camera, microphone, location, and contact access from any app that doesn't genuinely need it.",
                    " SAFETY TIP: Be careful what you plug into your computer. USB drives found in parking lots or given as freebies at events are a known attack method. Malware can install automatically when you plug them in.",
                    " SAFETY TIP: Enable 'Find My Device' on your phone and laptop. If your device is lost or stolen, you can remotely locate, lock, or wipe it to prevent your data from falling into the wrong hands.",
                    " SAFETY TIP: Use a credit card (not debit) for online shopping where possible. Credit cards offer far better fraud protection and dispute resolution. You're spending the bank's money, not yours, while a dispute is resolved."
                }
            },
            {
                "scam tip",
                new[]
                {
                    " SCAM TIP: Never share your OTP (one-time password) with anyone — not even someone claiming to be from your bank. Your bank will NEVER ask for it. If someone calls asking for your OTP, hang up immediately.",
                    " SCAM TIP: Before buying from an unfamiliar online shop, search the business name plus 'scam' or 'review' and check the CIPC register at cipc.co.za to verify they are a legitimate registered company.",
                    " SCAM TIP: Pig butchering scams are growing in South Africa — scammers build a fake romantic or friendly relationship over weeks before convincing you to invest in a fraudulent crypto platform. Never invest based on someone you met online.",
                    " SCAM TIP: If someone contacts you claiming you've won a prize, inherited money, or been randomly selected for an opportunity — and asks you to pay any fee to claim it — it is always a scam.",
                    " SCAM TIP: Job scams are common. Legitimate employers never ask you to pay for training materials, background checks, or equipment upfront. They also never ask for your banking details before you've signed a contract."
                }
            }
        };

        // ============================================================
        // INTEREST PHRASES  (Question 5 — Memory & Recall)
        // ============================================================
        private static readonly string[] InterestPhrases =
        {
            "interested in", "i like", "i love", "i care about",
            "concerned about", "want to learn about", "want to know about",
            "fascinated by", "worried about", "focused "
        };

        // ============================================================
        // PROCESS INPUT  — main entry point called by the GUI
        // Returns a string response that the GUI displays.
        // ============================================================
        public string ProcessInput(string input, User user)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string lower = input.ToLowerInvariant().Trim();

            // Add to conversation history (Memory & Recall)
            user.ConversationHistory.Add(input);

            // ----------------------------------------------------------
            // STEP 1: Interest/memory capture  (Question 5)
            // "I'm interested in privacy" → stores user.FavouriteTopic
            // ----------------------------------------------------------
            foreach (string phrase in InterestPhrases)
            {
                if (lower.Contains(phrase))
                {
                    int idx = lower.IndexOf(phrase) + phrase.Length;
                    string topic = input.Substring(idx).Trim(' ', '.', '!', '?', ',');
                    if (!string.IsNullOrWhiteSpace(topic))
                    {
                        user.FavouriteTopic = topic;
                        return $"Great! I'll remember that you're interested in {topic}, {user.Name}. " +
                               $"It's a crucial part of staying safe online. " +
                               $"Feel free to ask me anything about it — or type 'help' to see all topics!";
                    }
                }
            }

            // ----------------------------------------------------------
            // STEP 2: Sentiment detection  (Question 6)
            // Empathetic response + automatic tip — no second prompt.
            // ----------------------------------------------------------
            foreach (var sentiment in Sentiments)
            {
                foreach (string key in sentiment.keys)
                {
                    if (lower.Contains(key))
                    {
                        string name = user.Name;
                        return $"I hear you, {name}. {sentiment.empathy}\n\n💡 {sentiment.tip}";
                    }
                }
            }

            // ----------------------------------------------------------
            // STEP 3: Conversation flow — follow-up phrases  (Question 4)
            // ----------------------------------------------------------
            if (lower.Contains("tell me more") || lower.Contains("another tip") ||
                lower.Contains("explain more") || lower.Contains("give me more") ||
                lower.Contains("more info") || lower.Contains("elaborate") ||
                lower.Contains("go on") || lower.Contains("keep going") ||
                lower.Contains("what else") || lower.Contains("continue"))
            {
                if (!string.IsNullOrEmpty(_currentTopic))
                    return GetFollowUp(_currentTopic, user);
                return $"I'd love to go deeper, {user.Name}! Which topic would you like more on?\n" +
                       "Try: cybersecurity, phishing, password, scam, privacy, browsing, suspicious links, or report.";
            }

            // ----------------------------------------------------------
            // STEP 4: Random tip requests  (Question 3)
            // ----------------------------------------------------------
            if (lower.Contains("phishing tip") || lower.Contains("tip about phishing"))
            {
                _currentTopic = "phishing";
                return GetRandomResponse("phishing tip");
            }
            if (lower.Contains("password tip") || lower.Contains("tip about password"))
            {
                _currentTopic = "password";
                return GetRandomResponse("password tip");
            }
            if (lower.Contains("scam tip") || lower.Contains("tip about scam"))
            {
                _currentTopic = "scam";
                return GetRandomResponse("scam tip");
            }
            if (lower.Contains("safety tip") || lower.Contains("general tip") ||
                lower.Contains("give me a tip") || lower.Contains("random tip"))
            {
                return GetRandomResponse("general safety tip");
            }

            // ----------------------------------------------------------
            // STEP 5: Help / greetings
            // ----------------------------------------------------------
            if (lower == "help" || lower.Contains("what can you do") || lower.Contains("topics") || lower.Contains("menu"))
            {
                return "Here is everything I can help you with, " + user.Name + ":\n\n" +
                       " CYBERSECURITY  — What it is and how to protect yourself\n" +
                       " PHISHING       — Spot, handle, and report fake emails\n" +
                       " PASSWORD       — Strong passwords and safe habits\n" +
                       " SCAM           — Common scams and how to avoid them\n" +
                       " PRIVACY        — Protect your personal data online\n" +
                       " BROWSING       — Safe browsing and HTTPS explained\n" +
                       " SUSPICIOUS     — Identify and check dangerous links\n" +
                       " REPORT         — How and where to report cybercrime in SA\n\n" +
                       " TIPS: 'phishing tip' | 'password tip' | 'scam tip' | 'safety tip'\n" +
                       " FOLLOW-UP: 'tell me more' | 'another tip' | 'explain more'\n" +
                       "Type 'exit' to quit.";
            }

            if (lower.Contains("hello") || lower.Contains("hi") || lower.Contains("hey") || lower == "sup")
                return $"Hello again, {user.Name}!  How can I help you stay safe online today? Type 'help' to see all topics.";

            if (lower.Contains("how are you"))
                return $"I'm running at full security capacity, {user.Name}! Always alert, always protecting. How can I help you?";

            if (lower.Contains("thank"))
                return $"You are very welcome, {user.Name}! Stay vigilant and stay safe out there.  Knowledge is your best defence.";

            if (lower.Contains("who are you") || lower.Contains("what are you"))
                return $"I'm your Cybersecurity Awareness Chatbot, {user.Name}! I'm here to help you understand online threats and teach you how to protect yourself. Type 'help' to see what I can do.";

            // ----------------------------------------------------------
            // STEP 6: Memory recall prompt  (Question 5)
            // ----------------------------------------------------------
            string memoryPrompt = string.Empty;
            if (!string.IsNullOrEmpty(user.FavouriteTopic))
                memoryPrompt = $"\n\n As someone interested in {user.FavouriteTopic}, you might want to explore how this topic connects to that as well.";

            // ----------------------------------------------------------
            // STEP 7: Keyword topic matching  (Question 2)
            // ----------------------------------------------------------
            foreach (var topic in Topics)
            {
                if (!lower.Contains(topic.Key))
                    continue;

                _currentTopic = topic.Key;
                var data = topic.Value;

                string response = $"{data.intro}\n\n";
                foreach (string opt in data.opts)
                    response += opt + "\n";
                response += "\nType 1, 2, or 3 to get detailed information.";
                response += memoryPrompt;
                return response;
            }

            // Numeric sub-menu reply
            if (int.TryParse(lower, out int choice) && choice >= 1 && choice <= 3)
            {
                if (!string.IsNullOrEmpty(_currentTopic) && Topics.ContainsKey(_currentTopic))
                {
                    var det = Topics[_currentTopic].det;
                    if (choice <= det.Length)
                    {
                        string result = string.Join(" ", det[choice - 1]);
                        result += $"\n\nAnything else, {user.Name}? Type 'help' to see all topics, or 'tell me more' for further detail.";
                        result += memoryPrompt;
                        return result;
                    }
                }
                return $"Please ask about a topic first (e.g. type 'phishing'), then choose 1, 2, or 3.";
            }

            // ----------------------------------------------------------
            // STEP 8: Default error handling  (Question 7)
            // ----------------------------------------------------------
            return $"I'm not sure I understand that, {user.Name}. Can you try rephrasing?\n" +
                   "Type 'help' to see all available topics, or ask about: cybersecurity, phishing, password, scam, privacy, browsing, suspicious links, or report.";
        }

        // ============================================================
        // GetRandomResponse — picks randomly from a response pool
        // ============================================================
        private string GetRandomResponse(string category)
        {
            if (!RandomResponses.ContainsKey(category))
                return "I don't have tips for that category yet. Try 'phishing tip', 'password tip', 'scam tip', or 'safety tip'.";

            string[] pool = RandomResponses[category];
            return pool[_random.Next(pool.Length)];
        }

        // ============================================================
        // GetFollowUp — deeper info on the current topic (Question 4)
        // ============================================================
        private string GetFollowUp(string topic, User user)
        {
            var followUps = new Dictionary<string, string[]>
            {
                {
                    "phishing",
                    new[]
                    {
                        GetRandomResponse("phishing tip"),
                        $"Here's something important, {user.Name}: Spear phishing is a targeted attack where criminals research you specifically — using your name, employer, colleagues, and recent activity gathered from social media — to craft an email you're far more likely to believe. Limit what you share publicly.",
                        $"Vishing (voice phishing) is surging in South Africa, {user.Name}. Attackers call pretending to be from FNB, ABSA, SARS, or even the police. They may already know your name and ID number. Always hang up and call back on the official number from the organisation's website.",
                        "Smishing (SMS phishing) uses fake SMS messages that appear to come from SARS, courier companies, or your bank — often with a link to a convincing fake site. Standard Bank, FNB, and SARS have all been impersonated. Forward suspicious SMSes to 7726 on any SA network."
                    }
                },
                {
                    "password",
                    new[]
                    {
                        GetRandomResponse("password tip"),
                        $"Consider this, {user.Name}: Your email password is the most critical one you own. Whoever controls your email can reset every other account — banking, social media, shopping, everything. Give your email password extra length (16+ characters), make it unique, and protect it with 2FA.",
                        "Most password managers — Bitwarden, 1Password, KeePass — have a built-in security audit feature. It flags passwords that are reused, weak, or already found in known data breaches. Run an audit today and work through the results.",
                        "Hardware security keys (like a YubiKey, starting from about R400) provide the strongest possible second factor. Unlike SMS codes, they cannot be intercepted remotely, phished, or transferred by a criminal. Banks and high-security accounts increasingly support them."
                    }
                },
                {
                    "scam",
                    new[]
                    {
                        GetRandomResponse("scam tip"),
                        $"Pig butchering scams are rapidly growing in South Africa, {user.Name}. Criminals spend weeks or months building a fake friendship or romance online, then gradually introduce you to a 'highly profitable' crypto investment platform they control. Once you invest, they vanish with everything.",
                        "Job scams are especially prevalent on LinkedIn, Facebook, and WhatsApp in South Africa. Signs of a fake job offer: it's unsolicited, the salary is unusually high, they don't ask for a CV, and they ask for banking details or payment for 'materials' before you've started.",
                        "SIM swap fraud is a major SA problem. Criminals contact your mobile network pretending to be you and have your number transferred to a new SIM they control. They can then receive your bank OTPs. If you suddenly lose mobile signal, contact your network provider immediately."
                    }
                },
                {
                    "privacy",
                    new[]
                    {
                        $"Did you know, {user.Name}? Under South Africa's POPIA (Protection of Personal Information Act), you have the legal right to ask any SA company what personal data they hold about you, and to demand they delete it. Email their information officer to exercise this right.",
                        "Check your Google privacy settings at myactivity.google.com — you'll see a detailed log of your searches, YouTube history, location data, and more. You can delete specific entries or set automatic deletion after 3 months.",
                        "Use an email alias service like SimpleLogin (free tier available) or Apple's 'Hide My Email'. Instead of giving your real email to every website, you use a unique alias. If one site sells your data or gets breached, you simply delete that alias.",
                        "Review your browser extensions — some popular free extensions are sold to data brokers or malicious actors who then use them to track everything you do online. Only install extensions from verified publishers with many reviews."
                    }
                },
                {
                    "browsing",
                    new[]
                    {
                        $"One of the best free upgrades you can make, {user.Name}: install the uBlock Origin extension in Chrome or Firefox. It blocks malicious ads, tracking scripts, and pop-up malware installers. Millions of malware infections happen through malicious advertisements on legitimate websites.",
                        "Important nuance: the padlock (HTTPS) only means the connection between your browser and the site is encrypted. It does NOT mean the site is safe or legitimate. Phishing sites routinely use HTTPS. Always verify the full domain name in the address bar.",
                        "Change your DNS provider for faster, safer browsing. Go to your network settings and set your DNS to 1.1.1.1 (Cloudflare) or 8.8.8.8 (Google). Cloudflare also offers 1.1.1.2 which blocks known malware domains automatically.",
                        "Browser fingerprinting is a tracking technique that identifies you without cookies — using your screen resolution, fonts, browser plugins, and hardware specs. Tools like Firefox with the uBlock Origin + Privacy Badger combination help reduce this."
                    }
                },
                {
                    "cybersecurity",
                    new[]
                    {
                        $"An important concept, {user.Name}: Zero-day vulnerabilities are software flaws that developers don't know about yet. Attackers discover and exploit them before a patch exists. The only real protection is keeping all software updated the moment patches release — minimising your exposure window.",
                        "Social engineering is the human element of cybercrime — manipulating people psychologically rather than hacking systems technically. It's often easier to trick a person than to crack a firewall. Healthy scepticism and verification habits are your best defence.",
                        GetRandomResponse("general safety tip"),
                        "The cybersecurity principle of 'least privilege' means only giving apps, users, and programs the exact permissions they need — nothing more. Apply this to your own life: don't run as administrator daily, review app permissions regularly, and share only what's necessary."
                    }
                },
                {
                    "suspicious",
                    new[]
                    {
                        $"A critical tool, {user.Name}: VirusTotal.com lets you paste any URL or upload any file and scans it against 70+ security engines simultaneously. It's free, takes seconds, and could save you from a devastating malware infection.",
                        "Typosquatting is when criminals register domain names that are one character off from legitimate sites — 'arnazon.com', 'gooogle.com', 'paypa1.com'. They count on you not looking carefully. Always read the full URL in the address bar before entering any information.",
                        "URL shorteners (bit.ly, tinyurl) hide the real destination of a link. Before clicking a shortened link, paste it into CheckShortURL.com or unshorten.it to reveal where it actually leads. Never click shortened links in unsolicited messages.",
                        "Drive-by downloads are malware infections that happen just by visiting a malicious webpage — no clicking required, especially on outdated browsers. Keep your browser updated and install uBlock Origin to block the malicious ad networks that distribute these attacks."
                    }
                },
                {
                    "report",
                    new[]
                    {
                        $"Remember, {user.Name}: when reporting to SAPS, always request a case number. This is important for insurance claims, bank disputes, and follow-up investigations. Do not leave without it.",
                        "If you were financially defrauded online, contact your bank's fraud line within the same day — the sooner you report, the higher the chance of reversing the transaction. FNB: 087 575 9444 | ABSA: 0800 111 155 | Standard Bank: 0800 020 600.",
                        "Preserve ALL digital evidence before reporting — do not delete messages, emails, or transaction records. Take screenshots with timestamps visible. In email clients, save the raw email with headers (Gmail: three dots → 'Show original') as this contains routing data investigators need.",
                        $"You can also report cybercrime incidents to the SABRIC (South African Banking Risk Information Centre) at sabric.co.za or their toll-free number 0800 222 040. They coordinate with banks and law enforcement to track and stop fraud syndicates."
                    }
                }
            };

            if (followUps.ContainsKey(topic))
            {
                string[] options = followUps[topic];
                return $"Here's more on {topic}, {user.Name}:\n\n" + options[_random.Next(options.Length)];
            }

            return GetRandomResponse("general safety tip");
        }
    }
}
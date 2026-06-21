using System;
using System.Collections.Generic;

namespace CybersecurityChatbot_GUI
{
    // ============================================================
    // FILE: ChatbotContent.cs
    // PURPOSE: Holds all the LONG text content (topics, tips,
    //          sentiments, follow-ups) separately from Chatbot.cs.
    //          Splitting data from logic keeps Chatbot.cs short
    //          and readable while keeping every Part 2 + Part 3
    //          feature fully intact.
    // ============================================================
    public static class ChatbotContent
    {
        // ============================================================
        // SENTIMENTS — keywords | empathy | tip (Question 6)
        // ============================================================
        public static readonly List<(string[] keys, string empathy, string tip)> Sentiments = new List<(string[], string, string)>
        {
            (new[]{"worried","scared","afraid","fear","panicking","terrified","anxious","nervous"},
             "It's completely understandable to feel that way — scammers can be very convincing.",
             "Never click links in unsolicited emails. Type the official site yourself, and enable 2FA on your email."),

            (new[]{"frustrated","angry","annoyed","mad","fed up","furious","irritated","rage","livid"},
             "I hear you — dealing with cyber threats is genuinely infuriating.",
             "Enable two-factor authentication (2FA) everywhere. Even a stolen password can't log in without your phone."),

            (new[]{"confused","lost","dont understand","don't understand","not sure","overwhelmed","puzzled","unclear","what does"},
             "No worries — cybersecurity jargon can feel like a foreign language at first.",
             "Start simple: use a password manager (Bitwarden, free) and install a reputable antivirus like Malwarebytes."),

            (new[]{"curious","want to know","wondering","interested to","tell me about","what is","how does","can you explain"},
             "I love the curiosity — that's genuinely your best cyber defence!",
             "Try HaveIBeenPwned.com with your email — it shows every known breach you've been part of."),

            (new[]{"happy","great","good mood","excited","fantastic","joyful","wonderful","cheerful","feeling good","loving it"},
             "That's amazing to hear! Let's put that energy to good use.",
             "Set up a free password manager today — one master password, unique passwords everywhere else."),

            (new[]{"sad","unhappy","feeling down","depressed","upset","miserable","heartbroken","crying","not okay","terrible"},
             "I'm sorry you're feeling that way. Please be kind to yourself.",
             "Small habits help: lock your screen, keep apps updated, never share passwords or OTPs — even with people you trust."),

            (new[]{"stressed","pressure","burnout","exhausted","tired","drained","cant cope","can't cope"},
             "That's a lot to carry. You don't need to fix everything at once.",
             "Just secure your email first — strong password + 2FA. It's the master key to everything else."),

            (new[]{"bored","nothing to do","free time","killing time","just browsing"},
             "Well, you've come to the right place!",
             "Try this: audit your top 5 accounts today for unique passwords and 2FA. Takes 20 minutes, huge payoff.")
        };

        // ============================================================
        // TOPICS — intro | 3 options | 3 detailed answers (Question 2)
        // ============================================================
        public static readonly Dictionary<string, (string intro, string[] opts, string[][] det)> Topics =
            new Dictionary<string, (string, string[], string[][])>
        {
            { "cybersecurity", (
                "Cybersecurity protects computers, networks, and data from digital attacks and unauthorised access.",
                new[]{"1. Why is it important?","2. What are the main threats?","3. How can I protect myself?"},
                new[]{
                    new[]{"SA has one of Africa's highest cybercrime rates (R2.2bn+ lost yearly). Attacks can drain accounts, steal identities, or lock businesses out of their own systems."},
                    new[]{"PHISHING (fake messages), MALWARE (harmful software), RANSOMWARE (locks files for payment), MAN-IN-THE-MIDDLE (intercepted Wi-Fi traffic), SOCIAL ENGINEERING (psychological manipulation), CREDENTIAL STUFFING (reused leaked passwords), ZERO-DAY EXPLOITS (unpatched flaws)."},
                    new[]{"Use unique passwords + a password manager. Enable 2FA everywhere. Keep software updated. Use antivirus + firewall. Back up files. Stay sceptical of urgent requests. Use a VPN on public Wi-Fi."}
                })},
            { "phishing", (
                "Phishing uses fraudulent messages disguised as trustworthy sources to steal information or spread malware.",
                new[]{"1. How do I spot one?","2. What do I do if I get one?","3. Why are they so convincing?"},
                new[]{
                    new[]{"Red flags: wrong sender address (paypa1.com), urgency/threats, generic greetings, poor spelling, mismatched links, unexpected attachments, requests for passwords/OTPs."},
                    new[]{"Don't click links or attachments. Don't reply. Report it (Gmail/Outlook 'Report Phishing'). If clicked: change passwords, scan for malware, check statements, enable 2FA, report to SABRIC if financial."},
                    new[]{"Attackers copy real logos perfectly, use your real name from breaches, register near-identical domains (typosquatting), use HTTPS padlocks to look safe, and exploit urgency/fear. Spear phishing and vishing (voice calls) target you specifically."}
                })},
            { "password", (
                "Passwords are your first line of defence — weak or reused ones cause most account takeovers.",
                new[]{"1. What makes one strong?","2. Why not reuse them?","3. How are they stolen?"},
                new[]{
                    new[]{"12+ characters, mixed case + numbers + symbols, no personal info, no dictionary words. Use passphrases like 'Coffee!Rain42Desk'. Use a password manager (Bitwarden, 1Password, KeePass) — one master password covers everything."},
                    new[]{"Breaches leak millions of passwords yearly. 'Credential stuffing' tries leaked passwords on your other accounts. One reused password can expose your email, banking, everything. Check HaveIBeenPwned.com."},
                    new[]{"Brute force (guessing millions/sec), dictionary attacks, phishing (fake login pages), keyloggers, data breaches, shoulder surfing, man-in-the-middle on unsecured Wi-Fi, credential stuffing."}
                })},
            { "scam", (
                "Online scams deceive you into giving up money or personal information.",
                new[]{"1. Common SA scams?","2. How do I avoid them?","3. What if I've been scammed?"},
                new[]{
                    new[]{"Advance-fee fraud, romance scams, fake online shops, tech support scams, crypto/investment scams, job scams, smishing (fake SMS from banks/SARS), lottery scams."},
                    new[]{"If it sounds too good to be true, it is. Never send money to strangers online. Never share an OTP. Research sellers (CIPC register). Call organisations back on official numbers. Trust your instincts."},
                    new[]{"Stop contact with the scammer. Call your bank's fraud line immediately. Report to SAPS (get a case number) and SABRIC (0800 222 040). Secure your accounts. Screenshot everything as evidence."}
                })},
            { "privacy", (
                "Online privacy means controlling who accesses your personal information and how it's used.",
                new[]{"1. Why does it matter?","2. How do I protect it?","3. Social media risks?"},
                new[]{
                    new[]{"Your data fuels identity theft, stalking, and targeted scams. Once online, it's nearly impossible to fully remove. POPIA gives South Africans the right to know and delete what companies hold on them."},
                    new[]{"Use a VPN on public Wi-Fi. Use incognito mode + uBlock Origin. Review app permissions regularly. Check myactivity.google.com. Use email aliases (SimpleLogin). Read privacy policies."},
                    new[]{"Oversharing location reveals when you're away from home. Public profiles get harvested for social engineering. Fake friend requests run scams. Quizzes harvest security answers. Set profiles to private."}
                })},
            { "browsing", (
                "Safe browsing protects your device and data from online threats.",
                new[]{"1. How do I browse safely?","2. What does HTTPS mean?","3. How do I spot dangerous sites?"},
                new[]{
                    new[]{"Keep your browser updated. Install uBlock Origin. Avoid pop-up downloads. Use secure DNS (1.1.1.1). Use a VPN on untrusted networks. Scan downloads before opening. Log out of sensitive sites when done."},
                    new[]{"HTTPS encrypts the connection between you and the site — but does NOT guarantee the site is legitimate. Phishing sites use HTTPS too. Always check the full domain name, not just the padlock."},
                    new[]{"Misspelled domains, mismatched link text vs URL, excessive pop-ups, sites demanding downloads immediately, sites requesting login info unprompted, and unexpected redirects are all red flags."}
                })},
            { "suspicious", (
                "Suspicious links look harmless but lead to malicious sites designed to steal data or install malware.",
                new[]{"1. How do I spot one?","2. What happens if I click it?","3. How do I check it safely?"},
                new[]{
                    new[]{"Misspelled domains, URL shorteners hiding destinations, random character strings, mismatched display text, wrong top-level domains, links from unsolicited messages, and non-HTTPS login pages."},
                    new[]{"Drive-by malware downloads, fake login pages stealing credentials, cookie hijacking, ransomware, full device takeover via RATs, financial fraud, and identity theft."},
                    new[]{"Hover to preview the real URL. Paste into VirusTotal.com or URLVoid.com. Expand shortened links with CheckShortURL.com. Check domain age with a WHOIS lookup. On mobile, long-press to preview."}
                })},
            { "report", (
                "Reporting cybercrime helps stop attackers from targeting more victims.",
                new[]{"1. Where do I report in SA?","2. How do I report phishing/scams?","3. What info should I prepare?"},
                new[]{
                    new[]{"SAPS Cybercrime Centre (saps.gov.za), SABRIC (sabric.co.za, 0800 222 040), your bank's fraud line, ICASA for SMS fraud, and the Consumer Goods and Services Ombud for shopping scams."},
                    new[]{"Gmail/Outlook have 'Report Phishing' buttons. Forward SMS scams to 7726. Use the 'Report' function on WhatsApp and social media. Report fake sites to Google Safe Browsing."},
                    new[]{"Screenshots, email headers, URLs involved, exact dates/times, financial transaction details, all communication records, and your own ID/contact details for the official report."}
                })}
        };

        // ============================================================
        // RANDOM RESPONSE POOLS (Question 3)
        // ============================================================
        public static readonly Dictionary<string, string[]> RandomResponses = new Dictionary<string, string[]>
        {
            { "phishing tip", new[]{
                " Be wary of urgency — 'Your account will be suspended!' is designed to make you act before thinking.",
                " Check the sender's email character by character — 'paypa1.com' (with a '1') is a classic fake.",
                " Legit companies never ask for your password, OTP, or full card number by email or SMS.",
                " Type the official website yourself rather than clicking a link in an unexpected email.",
                " Hover over links before clicking — the real URL shows in the bottom status bar.",
                " 'Dear Customer' instead of your name is a red flag — your real bank knows who you are.",
                " Spear phishing uses info from your social media to craft a personalised fake email.",
                " Vishing (phone scams) is rising in SA — always call back on the official number, not the one given to you." } },

            { "password tip", new[]{
                " Use Bitwarden — free, generates unique passwords, you only remember one master password.",
                " Enable 2FA on every account that offers it, especially email and banking.",
                " Build passphrases like 'Coffee!Rain42Desk' — stronger and easier to remember than 'P@ssw0rd'.",
                " Check HaveIBeenPwned.com — if your email's been breached, change that password everywhere.",
                " Never store passwords in plain text — use an encrypted password manager.",
                " Your email password is the master key to everything — make it long, unique, and 2FA-protected.",
                " Change passwords immediately when you hear of a breach, even before you're notified.",
                " Hardware keys (YubiKey) can't be phished remotely like SMS codes can." } },

            { "general safety tip", new[]{
                " Keep all software updated — most attacks exploit flaws that updates already fixed.",
                " Never bank or shop on public Wi-Fi without a VPN.",
                " Follow the 3-2-1 backup rule: 3 copies, 2 storage types, 1 offsite.",
                " Lock your screen every time you step away, even for a minute.",
                " Review app permissions monthly — revoke camera/mic/location access you don't need.",
                " Never plug in unknown USB drives — a classic malware delivery method.",
                " Enable 'Find My Device' so you can lock or wipe a lost device remotely.",
                " Use a credit card over debit for online shopping — better fraud protection." } },

            { "scam tip", new[]{
                " Never share your OTP — your bank will NEVER ask for it over the phone.",
                " Check unfamiliar shops on the CIPC register (cipc.co.za) before buying.",
                " Pig butchering scams build fake trust over weeks before pushing a crypto 'investment'.",
                " Any prize that requires a fee to claim is always a scam.",
                " Legit employers never ask you to pay for training materials upfront." } }
        };

        // ============================================================
        // FOLLOW-UP CONTENT (Question 4) — extra detail per topic
        // ============================================================
        public static Dictionary<string, string[]> BuildFollowUps(string userName, Func<string, string> randomTip)
        {
            return new Dictionary<string, string[]>
            {
                { "phishing", new[]{ randomTip("phishing tip"),
                    $"Spear phishing researches you specifically using your name, employer, and social media activity, {userName}.",
                    "Vishing callers may already know your name and ID number — always hang up and call the official number back.",
                    "Smishing fakes SMS from SARS/banks. Forward suspicious SMS to 7726 on any SA network." } },

                { "password", new[]{ randomTip("password tip"),
                    $"Your email password is the most critical one you own, {userName} — protect it with extra length and 2FA.",
                    "Most password managers can audit your saved passwords for reuse or known breaches — run one today.",
                    "Hardware security keys (from ~R400) are nearly impossible to phish or intercept remotely." } },

                { "scam", new[]{ randomTip("scam tip"),
                    $"Pig butchering scams build fake friendships over months before the crypto 'investment' pitch, {userName}.",
                    "Job scams on LinkedIn/WhatsApp often skip the CV process and ask for upfront payment — a major red flag.",
                    "SIM swap fraud lets criminals receive your OTPs — if you suddenly lose signal, call your network immediately." } },

                { "privacy", new[]{
                    $"Under POPIA, you can legally demand any SA company show or delete your personal data, {userName}.",
                    "myactivity.google.com shows your full search/location history — you can delete it or set auto-deletion.",
                    "Email alias services (SimpleLogin) let you sign up without exposing your real address.",
                    "Some free browser extensions sell your browsing data — only install from verified publishers." } },

                { "browsing", new[]{
                    $"uBlock Origin blocks most malicious ads and tracking scripts — install it, {userName}.",
                    "HTTPS encrypts the connection only — it doesn't mean the site itself is trustworthy.",
                    "Cloudflare DNS (1.1.1.1) blocks many known malware domains automatically.",
                    "Browser fingerprinting tracks you without cookies — privacy-focused extensions help reduce this." } },

                { "cybersecurity", new[]{
                    $"Zero-day flaws are unknown to developers until exploited — fast updates minimise your exposure, {userName}.",
                    "Social engineering targets people, not systems — scepticism is your best defence.",
                    randomTip("general safety tip"),
                    "The 'least privilege' principle: only give apps/programs the access they truly need." } },

                { "suspicious", new[]{
                    $"VirusTotal.com scans any link or file against 70+ security engines in seconds, {userName}.",
                    "Typosquatting domains ('arnazon.com') count on you not reading carefully.",
                    "URL shorteners hide destinations — expand them with CheckShortURL.com first.",
                    "Drive-by downloads infect you just by visiting a page — keep your browser updated." } },

                { "report", new[]{
                    $"Always request a case number from SAPS, {userName} — you'll need it for banks and insurance.",
                    "Report financial fraud to your bank's fraud line the same day for the best chance of reversal.",
                    "Preserve all evidence (screenshots, headers) before reporting — don't delete anything.",
                    "SABRIC (0800 222 040) coordinates with banks and police on fraud syndicates." } }
            };
        }
    }
}
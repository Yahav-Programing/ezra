namespace ezra
{
    using Microsoft.Web.WebView2.WinForms;
    using Microsoft.Web.WebView2.Core;
    using System.Text.Json;

    public partial class Form1 : Form
    {
        private OllamaService _ollamaService;
        Font chatfont = new Font("Segoe UI", 10);
        bool games = false;
        private WebView2 webView;
        private List<AlarmWindow> openAlarmWindows = new List<AlarmWindow>();

        public Form1()
        {
            InitializeComponent();
            _ollamaService = new OllamaService();
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void pnlMusic_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void pnllogo_Paint(object sender, PaintEventArgs e)
        {
        }


        private async void Form1_Load(object sender, EventArgs e)
        {
            _ollamaService.SetModel("gemma4:e2b");

            // Initialize WebView2
            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = Path.Combine(Path.GetTempPath(), "WebView2UserDataFolder")
                }
            };

            // Remove old controls and add WebView2
            this.Controls.Clear();
            this.Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

            // Setup web message receiver for chat interaction
            webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

            // Load the chat.html file
            string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chat.html");
            if (File.Exists(htmlPath))
            {
                string htmlContent = File.ReadAllText(htmlPath);
                webView.CoreWebView2.NavigateToString(htmlContent);

                // Wait for page to fully load
                await Task.Delay(1000);

                // Send greeting messages
                await SendMessageToWebAsync("שלום!", isUser: false);
                await Task.Delay(300);
                await SendMessageToWebAsync("איך אני יכול לעזור?", isUser: false);
            }
            else
            {
                webView.CoreWebView2.NavigateToString("<h1 style='color: red;'>chat.html not found</h1>");
            }
        }

        private void WebView_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string message = e.WebMessageAsJson;
                Console.WriteLine($"WebMessageReceived: {message}");

                // Parse and handle different message types
                if (message.Contains("userMessage"))
                {
                    HandleWebMessage(message);
                }
                else if (message.Contains("launchApp"))
                {
                    HandleLaunchApp(message);
                }
                else if (message.Contains("openPopup"))
                {
                    HandleOpenPopup(message);
                }
                else if (message.Contains("askGames"))
                {
                    HandleAskGames();
                }
                else if (message.Contains("getJoke"))
                {
                    HandleGetJoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebView_WebMessageReceived: {ex.Message}");
            }
        }

        private async void HandleWebMessage(string jsonMessage)
        {
            try
            {
                Console.WriteLine($"HandleWebMessage called with: {jsonMessage}");

                // Parse the JSON to get the message text
                using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(jsonMessage))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("text", out var textElement))
                    {
                        string userText = textElement.GetString();
                        Console.WriteLine($"User message: {userText}");

                        // Get AI response
                        string response = await _ollamaService.GetResponseAsync(userText);

                        // Hide typing indicator and show response
                        await webView.CoreWebView2.ExecuteScriptAsync("hideTypingIndicator();");

                        // Check for commands in the response
                        await ProcessAIResponse(response);

                        // Re-enable input
                        await webView.CoreWebView2.ExecuteScriptAsync("sendBtn.disabled = false; userInput.focus();");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleWebMessage: {ex.Message}");
            }
        }

        private async Task ProcessAIResponse(string response)
        {
            Console.WriteLine($"\n=== ProcessAIResponse ===");
            Console.WriteLine($"Raw response: {response}");

            // Extract and process commands from AI response
            // Look for /clock {HH:MM} and /game {2048 or ghost} patterns

            // Check for /clock command
            System.Text.RegularExpressions.Regex clockRegex = new System.Text.RegularExpressions.Regex(@"/clock\s*{\s*(\d{1,2}):(\d{2})\s*}");
            var clockMatch = clockRegex.Match(response);

            Console.WriteLine($"Clock regex match: {clockMatch.Success}");

            if (clockMatch.Success)
            {
                string hour = clockMatch.Groups[1].Value;
                string minute = clockMatch.Groups[2].Value;

                Console.WriteLine($"? Clock command found! Hour: {hour}, Minute: {minute}");

                // Remove the command from the response
                string displayText = clockRegex.Replace(response, "").Trim();
                if (!string.IsNullOrEmpty(displayText))
                {
                    Console.WriteLine($"Display text: {displayText}");
                    await SendMessageToWebAsync(displayText, isUser: false);
                }

                // Open the alarm popup with the time
                Console.WriteLine($"Opening alarm popup...");
                OpenAlarmPopupWithTime(hour, minute);
                return;
            }

            // Check for /game command
            System.Text.RegularExpressions.Regex gameRegex = new System.Text.RegularExpressions.Regex(@"/game\s*{\s*(2048|ghost)\s*}");
            var gameMatch = gameRegex.Match(response);

            Console.WriteLine($"Game regex match: {gameMatch.Success}");

            if (gameMatch.Success)
            {
                string game = gameMatch.Groups[1].Value.ToLower();

                Console.WriteLine($"? Game command found! Game: {game}");

                // Remove the command from the response
                string displayText = gameRegex.Replace(response, "").Trim();
                if (!string.IsNullOrEmpty(displayText))
                {
                    Console.WriteLine($"Display text: {displayText}");
                    await SendMessageToWebAsync(displayText, isUser: false);
                }

                // Launch the game
                if (game == "2048")
                {
                    Launcher.RunApp(@"Data\2048\2048.exe");
                }
                else if (game == "ghost")
                {
                    Launcher.RunApp(@"Data\client-ghost-game\client-side ghost game.exe");
                }
                return;
            }

            Console.WriteLine("No commands found, displaying response as-is");
            // Default: just display the response
            await SendMessageToWebAsync(response, isUser: false);
        }

        private void OpenAlarmPopupWithTime(string hour, string minute)
        {
            try
            {
                Console.WriteLine($"Opening alarm popup with time: {hour}:{minute}");

                // Create and show the alarm window
                AlarmWindow alarmWindow = new AlarmWindow(hour, minute);
                openAlarmWindows.Add(alarmWindow);

                alarmWindow.FormClosed += (s, e) =>
                {
                    // Remove from list when window is closed
                    openAlarmWindows.Remove(alarmWindow);
                };

                alarmWindow.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening alarm popup: {ex.Message}");
            }
        }

        private void HandleLaunchApp(string jsonMessage)
        {
            try
            {
                using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(jsonMessage))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("app", out var appElement))
                    {
                        string app = appElement.GetString();
                        Console.WriteLine($"Launching app: {app}");

                        if (app == "alarm")
                        {
                            Launcher.RunApp(@"Data\Clock\AlarmClock.exe");
                        }
                        else if (app == "spotify")
                        {
                            // Try to launch Spotify
                            try
                            {
                                Launcher.RunApp("spotify");
                            }
                            catch
                            {
                                // If that fails, try the full path
                                try
                                {
                                    string spotifyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                        @"Spotify\Spotify.exe");
                                    if (File.Exists(spotifyPath))
                                    {
                                        Launcher.RunApp(spotifyPath);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Could not launch Spotify: {ex.Message}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleLaunchApp: {ex.Message}");
            }
        }

        private void HandleOpenPopup(string jsonMessage)
        {
            try
            {
                using (System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(jsonMessage))
                {
                    var root = doc.RootElement;
                    if (root.TryGetProperty("url", out var urlElement))
                    {
                        string popupUrl = urlElement.GetString();
                        Console.WriteLine($"Opening popup: {popupUrl}");

                        // Get the base directory where the HTML files are located
                        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                        string htmlPath = Path.Combine(baseDir, popupUrl);

                        // If the file doesn't exist at the base path, try looking for it
                        if (!File.Exists(htmlPath))
                        {
                            // Try just the filename in the base directory
                            string fileName = Path.GetFileName(popupUrl);
                            htmlPath = Path.Combine(baseDir, fileName);
                        }

                        // Create and show a new form with WebView2 for the popup
                        OpenAlarmPopupWindow(popupUrl, htmlPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleOpenPopup: {ex.Message}");
            }
        }

        private void OpenAlarmPopupWindow(string popupUrl, string htmlPath)
        {
            try
            {
                // Create a new form for the popup window
                Form popupForm = new Form
                {
                    Text = "? Alarm",
                    Width = 500,
                    Height = 650,
                    StartPosition = FormStartPosition.CenterScreen,
                    ShowInTaskbar = true,
                    TopMost = true,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = true
                };

                // Create WebView2 for the popup
                WebView2 popupWebView = new WebView2
                {
                    Dock = DockStyle.Fill,
                    CreationProperties = new CoreWebView2CreationProperties
                    {
                        UserDataFolder = Path.Combine(Path.GetTempPath(), "WebView2AlarmPopup")
                    }
                };

                popupForm.Controls.Add(popupWebView);

                // Initialize WebView2 asynchronously
                popupWebView.EnsureCoreWebView2Async(null).ContinueWith(async _ =>
                {
                    try
                    {
                        // Set up message receiver for the popup
                        popupWebView.CoreWebView2.WebMessageReceived += (sender, e) =>
                        {
                            string message = e.WebMessageAsJson;
                            Console.WriteLine($"Popup Message: {message}");

                            // Handle popup messages
                            if (message.Contains("closeCountdown"))
                            {
                                popupForm.Invoke((MethodInvoker)(() => popupForm.Close()));
                            }
                        };

                        // Load the HTML file
                        if (File.Exists(htmlPath))
                        {
                            string htmlContent = File.ReadAllText(htmlPath);
                            popupWebView.CoreWebView2.NavigateToString(htmlContent);
                        }
                        else
                        {
                            // If file not found, try loading from URL query string parameters
                            string queryString = popupUrl.Contains('?') ? popupUrl.Substring(popupUrl.IndexOf('?')) : "";
                            if (!string.IsNullOrEmpty(queryString))
                            {
                                // Try to find clock-popup.html in the application directory
                                string alarmHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clock-popup.html");
                                if (File.Exists(alarmHtmlPath))
                                {
                                    string htmlContent = File.ReadAllText(alarmHtmlPath);
                                    popupWebView.CoreWebView2.NavigateToString(htmlContent);
                                }
                                else
                                {
                                    popupWebView.CoreWebView2.NavigateToString("<h1 style='color: red;'>clock-popup.html not found</h1>");
                                }
                            }
                            else
                            {
                                popupWebView.CoreWebView2.NavigateToString("<h1 style='color: red;'>Popup file not found</h1>");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error initializing popup WebView2: {ex.Message}");
                    }
                });

                // Show the popup form
                popupForm.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening alarm popup window: {ex.Message}");
            }
        }

        private async void HandleAskGames()
        {
            try
            {
                await webView.CoreWebView2.ExecuteScriptAsync("hideTypingIndicator();");
                await SendMessageToWebAsync("במה תרצה לשחק?", isUser: false);
                await SendMessageToWebAsync("1 - תופסת רוח", isUser: false);
                await SendMessageToWebAsync("2 - 2048", isUser: false);
                await webView.CoreWebView2.ExecuteScriptAsync("sendBtn.disabled = false; userInput.focus();");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleAskGames: {ex.Message}");
            }
        }

        private async void HandleGetJoke()
        {
            try
            {
                // Try to load a joke from the jokes.txt file
                string jokesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\jokes.txt");

                if (File.Exists(jokesPath))
                {
                    string[] jokes = File.ReadAllLines(jokesPath);
                    if (jokes.Length > 0)
                    {
                        Random random = new Random();
                        string randomJoke = jokes[random.Next(jokes.Length)];

                        await webView.CoreWebView2.ExecuteScriptAsync("hideTypingIndicator();");
                        await SendMessageToWebAsync(randomJoke, isUser: false);
                    }
                }
                else
                {
                    await webView.CoreWebView2.ExecuteScriptAsync("hideTypingIndicator();");
                    await SendMessageToWebAsync("סורי, לא מצאתי קבצי בדיחות ??", isUser: false);
                }

                await webView.CoreWebView2.ExecuteScriptAsync("sendBtn.disabled = false; userInput.focus();");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleGetJoke: {ex.Message}");
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }

        private void AddChat(string chatText, Color chatColor, bool isAI = true)
        {
        }

        private async void btnclock_Click(object sender, EventArgs e)
        {
            await SendMessageToWebAsync("תעיר אותי בבקשה", isUser: true);
            Launcher.RunApp(@"Data\Clock\AlarmClock.exe");
        }

        private async void btnplay_Click(object sender, EventArgs e)
        {
            await SendMessageToWebAsync("בא לי לשחק", isUser: true);
            await SendMessageToWebAsync("במה תרצו לשחק?\n1 - תופסת רוח\n2 - 2048", isUser: false);
            games = true;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string userInput = textBox.Text.Trim();

            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            await SendMessageToWebAsync(userInput, isUser: true);
            textBox.Clear();

            if (games && userInput == "1")
            {
                await SendMessageToWebAsync("תופסת רוח - משחק כיף!", isUser: false);
                Launcher.RunApp(@"Data\client-ghost-game\client-side ghost game.exe");
                games = false;
            }
            else if (games && userInput == "2")
            {
                await SendMessageToWebAsync("בואו נשחק 2048!", isUser: false);
                Launcher.RunApp(@"Data\2048\2048.exe");
                games = false;
            }
            else
            {
                await SendMessageToWebAsync("אני חושב על התשובה...", isUser: false);
                string response = await _ollamaService.GetResponseAsync(userInput);
                await SendMessageToWebAsync(response, isUser: false);
            }
        }

        private async Task SendMessageToWebAsync(string message, bool isUser = true)
        {
            try
            {
                if (webView?.CoreWebView2 == null)
                {
                    Console.WriteLine("WebView2 is not initialized");
                    return;
                }

                // Properly escape the message for JavaScript
                string escapedMessage = System.Text.Json.JsonSerializer.Serialize(message);
                string script = $"addMessage({escapedMessage}, {(isUser ? "true" : "false")});";

                Console.WriteLine($"Executing script: {script}");

                string result = await webView.CoreWebView2.ExecuteScriptAsync(script);
                Console.WriteLine($"Script result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.BackColor = ControlPaint.Light(button.BackColor, 0.2f);
            }
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                if (button == btnplay)
                    button.BackColor = Color.FromArgb(76, 175, 255);
                else if (button == btnmuzic)
                    button.BackColor = Color.FromArgb(165, 105, 255);
                else if (button == btnclock)
                    button.BackColor = Color.FromArgb(76, 220, 110);
                else if (button == btnsend)
                    button.BackColor = Color.FromArgb(255, 107, 107);
            }
        }
    }
}

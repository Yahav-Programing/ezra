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

                        // Send response back to UI
                        await SendMessageToWebAsync(response, isUser: false);

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ezra
{
    public partial class Form1 : Form
    {
        private sealed class AlarmRequest
        {
            public AlarmRequest(int hour, int minute, string label)
            {
                Hour = hour;
                Minute = minute;
                Label = label;
            }

            public int Hour { get; }
            public int Minute { get; }
            public string Label { get; }
        }

        private readonly OllamaService _ollamaService;
        private readonly Font chatfont = new Font("Segoe UI", 10);
        private bool games = false;
        private WebView2 webView = null!;
        private AlarmRequest? _pendingAlarm;
        private bool _initialGreetingSent;

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

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = Path.Combine(Path.GetTempPath(), "EzraWebView2UserData")
                }
            };

            webView.NavigationCompleted += MainWebView_NavigationCompleted;

            Controls.Clear();
            Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(null);

            webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;
            webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

            NavigateToAppPage("chat.html");
        }

        private async void MainWebView_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess || webView?.CoreWebView2 == null)
            {
                return;
            }

            if (IsCurrentPage("chat.html") && !_initialGreetingSent)
            {
                _initialGreetingSent = true;
                await Task.Delay(250);
                await SendMessageToWebAsync("שלום!", isUser: false);
                await Task.Delay(250);
                await SendMessageToWebAsync("איך אני יכול לעזור?", isUser: false);
                await RestoreChatInputAsync();
            }

            if (IsCurrentPage("clock.html") && _pendingAlarm != null)
            {
                await Task.Delay(150);
                await DeliverPendingAlarmAsync();
            }
        }

        private async void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            if (webView?.CoreWebView2 == null)
            {
                return;
            }

            var deferral = e.GetDeferral();

            try
            {
                ClockForm popupForm = new ClockForm();
                await popupForm.InitializeAsync(webView.CoreWebView2.Environment);

                e.NewWindow = popupForm.CoreWebView;
                e.Handled = true;

                popupForm.Show(this);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening popup window: {ex.Message}");
            }
            finally
            {
                deferral.Complete();
            }
        }

        private async void WebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(e.WebMessageAsJson);
                JsonElement root = doc.RootElement;

                if (root.ValueKind != JsonValueKind.Object)
                {
                    return;
                }

                string messageType = GetString(root, "type");

                switch (messageType)
                {
                    case "userMessage":
                        await HandleUserMessageAsync(GetString(root, "text"));
                        break;

                    case "launchApp":
                        HandleLaunchApp(root);
                        break;

                    case "launchGame":
                        HandleLaunchGame(root);
                        break;

                    case "askGames":
                        await HandleAskGamesAsync();
                        break;

                    case "getJoke":
                        await HandleGetJokeAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WebView_WebMessageReceived: {ex.Message}");
            }
        }

        private async Task HandleUserMessageAsync(string userText)
        {
            if (string.IsNullOrWhiteSpace(userText))
            {
                await RestoreChatInputAsync();
                return;
            }

            try
            {
                string response = await _ollamaService.GetResponseAsync(userText);
                await HideTypingIndicatorAsync();
                await ProcessAIResponseAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleUserMessageAsync: {ex.Message}");
                await HideTypingIndicatorAsync();
                await SendMessageToWebAsync("Sorry, something went wrong while I was thinking.", isUser: false);
            }
            finally
            {
                await RestoreChatInputAsync();
            }
        }

        private async Task ProcessAIResponseAsync(string response)
        {
            Regex clockRegex = new Regex(@"/clock\s*{\s*(\d{1,2}):(\d{2})\s*}", RegexOptions.IgnoreCase);
            Match clockMatch = clockRegex.Match(response);

            if (clockMatch.Success)
            {
                string displayText = clockRegex.Replace(response, string.Empty).Trim();
                if (!string.IsNullOrEmpty(displayText))
                {
                    await SendMessageToWebAsync(displayText, isUser: false);
                }

                if (int.TryParse(clockMatch.Groups[1].Value, out int hour) &&
                    int.TryParse(clockMatch.Groups[2].Value, out int minute))
                {
                    await SendAlarmToClockAsync(hour, minute, "Alarm from Ezra");
                }

                return;
            }

            Regex gameRegex = new Regex(@"/game\s*{\s*([^}]+)\s*}", RegexOptions.IgnoreCase);
            Match gameMatch = gameRegex.Match(response);

            if (gameMatch.Success)
            {
                string displayText = gameRegex.Replace(response, string.Empty).Trim();
                if (!string.IsNullOrEmpty(displayText))
                {
                    await SendMessageToWebAsync(displayText, isUser: false);
                }

                LaunchGameByKey(gameMatch.Groups[1].Value);
                return;
            }

            await SendMessageToWebAsync(response, isUser: false);
        }

        private async Task SendAlarmToClockAsync(int hour, int minute, string label)
        {
            if (hour < 0 || hour > 23 || minute < 0 || minute > 59)
            {
                return;
            }

            _pendingAlarm = new AlarmRequest(hour, minute, label);

            if (IsCurrentPage("clock.html"))
            {
                await DeliverPendingAlarmAsync();
                return;
            }

            NavigateToAppPage("clock.html");
        }

        private async Task DeliverPendingAlarmAsync()
        {
            if (webView?.CoreWebView2 == null || _pendingAlarm == null)
            {
                return;
            }

            AlarmRequest alarm = _pendingAlarm;
            string payloadJson = JsonSerializer.Serialize(new
            {
                type = "setAlarm",
                hour = alarm.Hour,
                minute = alarm.Minute,
                label = alarm.Label
            });

            await webView.CoreWebView2.ExecuteScriptAsync(
                $"if (window.receiveExternalAlarm) window.receiveExternalAlarm({payloadJson});");

            _pendingAlarm = null;
        }

        private void HandleLaunchApp(JsonElement root)
        {
            string app = GetString(root, "app").ToLowerInvariant();

            if (app == "alarm")
            {
                NavigateToAppPage("clock.html");
                return;
            }

            if (app == "spotify")
            {
                try
                {
                    Launcher.RunApp("spotify");
                    return;
                }
                catch
                {
                }

                string spotifyPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Spotify\Spotify.exe");

                if (File.Exists(spotifyPath))
                {
                    Launcher.RunApp(spotifyPath);
                }
            }
        }

        private void HandleLaunchGame(JsonElement root)
        {
            string key = GetString(root, "id");
            if (string.IsNullOrWhiteSpace(key))
            {
                key = GetString(root, "game");
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                key = GetString(root, "name");
            }

            LaunchGameByKey(key);
        }

        private async Task HandleAskGamesAsync()
        {
            try
            {
                await HideTypingIndicatorAsync();

                var payload = new
                {
                    type = "gamesList",
                    games = new[]
                    {
                        new { id = "ghost", label = "Ghost Game", description = "A spooky quick game." },
                        new { id = "2048", label = "2048", description = "Merge numbers and reach 2048." }
                    }
                };

                if (webView?.CoreWebView2 != null)
                {
                    webView.CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(payload));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleAskGamesAsync: {ex.Message}");
            }
            finally
            {
                await RestoreChatInputAsync();
            }
        }

        private async Task HandleGetJokeAsync()
        {
            try
            {
                await HideTypingIndicatorAsync();

                string jokesPath = ResolveAppFilePath(Path.Combine("Data", "jokes.txt"));

                if (File.Exists(jokesPath))
                {
                    string[] jokes = File.ReadAllLines(jokesPath);

                    if (jokes.Length > 0)
                    {
                        Random random = new Random();
                        string randomJoke = jokes[random.Next(jokes.Length)];
                        await SendMessageToWebAsync(randomJoke, isUser: false);
                    }
                    else
                    {
                        await SendMessageToWebAsync("לא מצאתי בדיחות בקובץ.", isUser: false);
                    }
                }
                else
                {
                    await SendMessageToWebAsync("סליחה, לא מצאתי את קובץ הבדיחות.", isUser: false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HandleGetJokeAsync: {ex.Message}");
            }
            finally
            {
                await RestoreChatInputAsync();
            }
        }

        private void LaunchGameByKey(string key)
        {
            string normalized = key.Trim().ToLowerInvariant();

            if (normalized.Contains("2048"))
            {
                LaunchResolvedApp(Path.Combine("Data", "2048", "2048.exe"));
                return;
            }

            if (normalized.Contains("ghost"))
            {
                LaunchResolvedApp(Path.Combine("Data", "client-ghost-game", "client-side ghost game.exe"));
            }
        }

        private void LaunchResolvedApp(string relativePath)
        {
            string resolvedPath = ResolveAppFilePath(relativePath);

            if (File.Exists(resolvedPath))
            {
                Launcher.RunApp(resolvedPath);
                return;
            }

            MessageBox.Show($"Could not find:\n{relativePath}", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void NavigateToAppPage(string fileName)
        {
            if (webView == null)
            {
                return;
            }

            string filePath = ResolveAppFilePath(fileName);

            if (!File.Exists(filePath))
            {
                string escapedPath = WebUtility.HtmlEncode(filePath);
                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.NavigateToString($"<h1 style='color:red;font-family:Segoe UI'>Missing file: {escapedPath}</h1>");
                }

                return;
            }

            Uri targetUri = new Uri(filePath);

            if (webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(targetUri.AbsoluteUri);
            }
            else
            {
                webView.Source = targetUri;
            }
        }

        private bool IsCurrentPage(string fileName)
        {
            if (webView?.Source == null)
            {
                return false;
            }

            string localPath = webView.Source.IsFile
                ? webView.Source.LocalPath
                : webView.Source.AbsolutePath;

            return string.Equals(Path.GetFileName(localPath), fileName, StringComparison.OrdinalIgnoreCase);
        }

        private string ResolveAppFilePath(string relativePath)
        {
            if (Path.IsPathRooted(relativePath) && File.Exists(relativePath))
            {
                return relativePath;
            }

            foreach (string root in EnumerateSearchRoots())
            {
                string candidate = Path.Combine(root, relativePath);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
        }

        private IEnumerable<string> EnumerateSearchRoots()
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string start in new[] { AppDomain.CurrentDomain.BaseDirectory, Environment.CurrentDirectory })
            {
                if (string.IsNullOrWhiteSpace(start) || !Directory.Exists(start))
                {
                    continue;
                }

                string? current = Path.GetFullPath(start);

                for (int depth = 0; depth < 6 && !string.IsNullOrWhiteSpace(current); depth++)
                {
                    if (Directory.Exists(current) && seen.Add(current))
                    {
                        yield return current;
                    }

                    current = Directory.GetParent(current)?.FullName;
                }
            }
        }

        private static string GetString(JsonElement root, string propertyName)
        {
            if (root.TryGetProperty(propertyName, out JsonElement value) && value.ValueKind == JsonValueKind.String)
            {
                return value.GetString() ?? string.Empty;
            }

            return string.Empty;
        }

        private async Task SendMessageToWebAsync(string message, bool isUser = true)
        {
            try
            {
                if (webView?.CoreWebView2 == null)
                {
                    return;
                }

                string escapedMessage = JsonSerializer.Serialize(message);
                string script = $@"
(() => {{
    if (typeof addMessage !== 'function') {{
        return false;
    }}

    addMessage({escapedMessage}, {(isUser ? "true" : "false")});
    return true;
}})();";

                await webView.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async Task HideTypingIndicatorAsync()
        {
            if (webView?.CoreWebView2 == null)
            {
                return;
            }

            await webView.CoreWebView2.ExecuteScriptAsync(@"
if (typeof hideTypingIndicator === 'function') {
    hideTypingIndicator();
}");
        }

        private async Task RestoreChatInputAsync()
        {
            if (webView?.CoreWebView2 == null)
            {
                return;
            }

            await webView.CoreWebView2.ExecuteScriptAsync(@"
if (typeof sendBtn !== 'undefined' && sendBtn) {
    sendBtn.disabled = false;
}
if (typeof userInput !== 'undefined' && userInput && typeof userInput.focus === 'function') {
    userInput.focus();
}");
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
            NavigateToAppPage("clock.html");
        }

        private async void btnplay_Click(object sender, EventArgs e)
        {
            await SendMessageToWebAsync("בא לי לשחק", isUser: true);
            await HandleAskGamesAsync();
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
                LaunchGameByKey("ghost");
                games = false;
            }
            else if (games && userInput == "2")
            {
                await SendMessageToWebAsync("בואו נשחק 2048!", isUser: false);
                LaunchGameByKey("2048");
                games = false;
            }
            else
            {
                await SendMessageToWebAsync("אני חושב על התשובה...", isUser: false);
                string response = await _ollamaService.GetResponseAsync(userInput);
                await SendMessageToWebAsync(response, isUser: false);
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

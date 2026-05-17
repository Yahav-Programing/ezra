using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ezra
{
    public class OllamaService
    {
        public sealed class OllamaResponse
        {
            public OllamaResponse(string rawResponse, string responseText, string thoughtText)
            {
                RawResponse = rawResponse;
                ResponseText = responseText;
                ThoughtText = thoughtText;
            }

            public string RawResponse { get; }
            public string ResponseText { get; }
            public string ThoughtText { get; }
            public bool HasThought => !string.IsNullOrWhiteSpace(ThoughtText);
        }

        private static readonly Regex ThoughtBlockRegex = new Regex(
            @"^\s*(?:<\|channel\|>|<\|channel>|<channel\|>)\s*thought\s*\r?\n(?<thought>.*?)(?:<\|channel\|>|<\|channel>|<channel\|>)(?<answer>[\s\S]*)$",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        private readonly HttpClient _httpClient;
        private readonly string _ollamaUrl;
        private string _modelName;
        private string _systemPrompt;
        private bool _thinkingEnabled;

        public OllamaService(string modelName = "gemma4:e2b", string ollamaUrl = "http://localhost:11434")
        {
            _modelName = modelName;
            _ollamaUrl = ollamaUrl.TrimEnd('/');
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            _systemPrompt =
                "You are a helpful, friendly, and witty AI assistant. your name is עזרא You write concise and to-the-point answers." +
                "you can use this commands for doing things you can do /game {2048 or ghost} - for opening one of the games also /clock {time HH:MM} - for seting an alarm";
        }

        public void SetModel(string modelName)
        {
            if (!string.IsNullOrWhiteSpace(modelName))
            {
                _modelName = modelName;
            }
        }

        public string GetCurrentModel()
        {
            return _modelName;
        }

        public string GetOllamaUrl()
        {
            return _ollamaUrl;
        }

        public void SetSystemPrompt(string systemPrompt)
        {
            if (!string.IsNullOrWhiteSpace(systemPrompt))
            {
                _systemPrompt = systemPrompt;
            }
        }

        public string GetSystemPrompt()
        {
            return _systemPrompt;
        }

        public void SetThinkingEnabled(bool enabled)
        {
            _thinkingEnabled = enabled;
        }

        public bool IsThinkingEnabled()
        {
            return _thinkingEnabled;
        }

        public async Task<bool> IsOllamaAvailableAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"{_ollamaUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            OllamaResponse response = await GetDetailedResponseAsync(userMessage);
            return response.ResponseText;
        }

        public async Task<OllamaResponse> GetDetailedResponseAsync(string userMessage)
        {
            try
            {
                bool isAvailable = await IsOllamaAvailableAsync();
                if (!isAvailable)
                {
                    return CreatePlainResponse($"❌ Ollama לא זמין על {_ollamaUrl}. אנא התחל את Ollama ותנסה שוב.");
                }

                var requestBody = new
                {
                    model = _modelName,
                    prompt = userMessage,
                    system = BuildSystemPrompt(),
                    stream = false
                };

                StringContent jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{_ollamaUrl}/api/generate",
                    jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return CreatePlainResponse($"❌ המודל '{_modelName}' לא נמצא. בחר מודל אחר עם ollama pull {_modelName}");
                    }

                    return CreatePlainResponse($"❌ שגיאה: {response.StatusCode} - {response.ReasonPhrase}");
                }

                string responseContent = await response.Content.ReadAsStringAsync();

                using JsonDocument jsonDoc = JsonDocument.Parse(responseContent);
                if (jsonDoc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                {
                    string responseText = responseElement.GetString()?.Trim() ?? "לא קיבלתי תשובה";
                    return ParseModelResponse(responseText);
                }

                return CreatePlainResponse("שגיאה בעיבוד התשובה");
            }
            catch (HttpRequestException ex)
            {
                return CreatePlainResponse($"❌ שגיאה בחיבור: {ex.Message}\nודא ש-Ollama פועל: ollama serve");
            }
            catch (JsonException ex)
            {
                return CreatePlainResponse($"❌ שגיאה בעיבוד JSON: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return CreatePlainResponse("❌ התשובה התמה - המודל לוקח יותר מדי זמן. נסה שוב.");
            }
            catch (Exception ex)
            {
                return CreatePlainResponse($"❌ שגיאה לא צפויה: {ex.Message}");
            }
        }

        private string BuildSystemPrompt()
        {
            return _thinkingEnabled ? "<|think|>\n" + _systemPrompt : _systemPrompt;
        }

        private static OllamaResponse CreatePlainResponse(string text)
        {
            string safeText = text?.Trim() ?? string.Empty;
            return new OllamaResponse(safeText, safeText, string.Empty);
        }

        private static OllamaResponse ParseModelResponse(string rawResponse)
        {
            string safeText = rawResponse?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(safeText))
            {
                return new OllamaResponse(string.Empty, "לא קיבלתי תשובה", string.Empty);
            }

            Match match = ThoughtBlockRegex.Match(safeText);
            if (!match.Success)
            {
                return new OllamaResponse(safeText, safeText, string.Empty);
            }

            string thought = match.Groups["thought"].Value.Trim();
            string answer = match.Groups["answer"].Value.Trim();

            if (string.IsNullOrWhiteSpace(answer))
            {
                answer = "לא קיבלתי תשובה";
            }

            return new OllamaResponse(safeText, answer, thought);
        }
    }
}

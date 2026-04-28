using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ezra
{
    public class OllamaService
    {
        private HttpClient _httpClient;
        private string _modelName;
        private readonly string _ollamaUrl;
        private string _systemPrompt;

        public OllamaService(string modelName = "gemma4:e2b", string ollamaUrl = "http://localhost:11434")
        {
            _modelName = modelName;
            _ollamaUrl = ollamaUrl.TrimEnd('/');
            _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };
            // Default system prompt
            _systemPrompt = "You are a helpful, friendly, and witty AI assistant. your name is עזרא You write concise and to-the-point answers." +
                "you can use this commands for doing things you can do /game {2048 or ghost} - for opening one of the games also /clock {time HH:MM} - for setting an alarm ";
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

        public async Task<bool> IsOllamaAvailableAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_ollamaUrl}/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            try
            {
                // Check if Ollama is running
                var isAvailable = await IsOllamaAvailableAsync();
                if (!isAvailable)
                {
                    return $"❌ Ollama לא זמין על {_ollamaUrl}. אנא התחל את Ollama ותנסה שוב.";
                }

                var requestBody = new
                {
                    model = _modelName,
                    prompt = userMessage,
                    system = _systemPrompt,
                    stream = false
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(requestBody),
                    Encoding.UTF8,
                    "application/json");

                var response = await _httpClient.PostAsync(
                    $"{_ollamaUrl}/api/generate",
                    jsonContent);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return $"❌ המודל '{_modelName}' לא נמצא. בחר מודל אחר עם ollama pull {_modelName}";
                    }
                    return $"❌ שגיאה: {response.StatusCode} - {response.ReasonPhrase}";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                using (JsonDocument jsonDoc = JsonDocument.Parse(responseContent))
                {
                    if (jsonDoc.RootElement.TryGetProperty("response", out JsonElement responseElement))
                    {
                        var responseText = responseElement.GetString();
                        return responseText?.Trim() ?? "לא קיבלתי תשובה";
                    }
                }

                return "שגיאה בעיבוד התשובה";
            }
            catch (HttpRequestException ex)
            {
                return $"❌ שגיאה בחיבור: {ex.Message}\nודא ש-Ollama פועל: ollama serve";
            }
            catch (JsonException ex)
            {
                return $"❌ שגיאה בעיבוד JSON: {ex.Message}";
            }
            catch (TaskCanceledException)
            {
                return "❌ התשובה התמה - המודל לוקח יותר מדי זמן. נסה שוב.";
            }
            catch (Exception ex)
            {
                return $"❌ שגיאה לא צפויה: {ex.Message}";
            }
        }
    }
}

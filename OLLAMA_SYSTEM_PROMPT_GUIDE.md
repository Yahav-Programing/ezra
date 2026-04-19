# How to Use System Prompts with Ollama

## What is a System Prompt?

A **system prompt** is an initial instruction that sets the behavior and personality of the AI model. It tells Ollama how to behave, what style to use, and what rules to follow.

---

## How It Works in Your App

### Default System Prompt

Your OllamaService now includes a default Hebrew system prompt:

```csharp
_systemPrompt = "אתה עוזר AI עזרה מועיל, ידידותי וערמומי. אתה כותב תשובות קצרות וקולעות. תמיד תשובות בעברית.";
```

This translates to: "You are a helpful, friendly, and clever AI assistant. You write short and concise answers. Always answer in Hebrew."

---

## Methods Available

### 1. Set a Custom System Prompt

```csharp
// In your Form1.cs or anywhere you use OllamaService:
_ollamaService.SetSystemPrompt("You are a professional coding assistant. Always provide code examples.");
```

### 2. Get the Current System Prompt

```csharp
string currentPrompt = _ollamaService.GetSystemPrompt();
Console.WriteLine("Current prompt: " + currentPrompt);
```

---

## Example System Prompts

### For a Hebrew Assistant (Default)
```csharp
_ollamaService.SetSystemPrompt(
    "אתה עוזר AI עברי חכם וידידותי. " +
    "תשובותיך צריכות להיות קצרות ותקציביות. " +
    "תמיד השב בעברית טהורה."
);
```

### For a Technical Expert
```csharp
_ollamaService.SetSystemPrompt(
    "You are an expert software developer. " +
    "Provide detailed code examples and explanations. " +
    "Use best practices and modern patterns."
);
```

### For a Creative Writer
```csharp
_ollamaService.SetSystemPrompt(
    "You are a creative storyteller. " +
    "Write engaging, imaginative responses. " +
    "Use vivid descriptions and interesting metaphors."
);
```

### For a Math Tutor
```csharp
_ollamaService.SetSystemPrompt(
    "You are a patient math tutor. " +
    "Explain concepts step-by-step. " +
    "Help students understand, not just get answers."
);
```

### For a Translator
```csharp
_ollamaService.SetSystemPrompt(
    "You are a professional translator. " +
    "Translate accurately and maintain context. " +
    "Provide cultural notes when relevant."
);
```

---

## How to Implement in Your App

### Option 1: Set at Startup (in Form1_Load)

```csharp
private void Form1_Load(object sender, EventArgs e)
{
    _ollamaService.SetModel("gemma3:4b");

    // Set your custom system prompt
    _ollamaService.SetSystemPrompt(
        "אתה עוזר עברי חכם ומועיל. " +
        "תשובותיך צריכות להיות קצרות וברורות."
    );

    // ... rest of your code
}
```

### Option 2: Add a Theme-Based System Prompt

```csharp
private void HandleThemeCommand(string command)
{
    if (command.Contains("Quiet", StringComparison.OrdinalIgnoreCase))
    {
        _themeManager.SetTheme(ThemeManager.Theme.Quiet);
        ApplyTheme(ThemeManager.Theme.Quiet);

        // Set system prompt for quiet theme
        _ollamaService.SetSystemPrompt(
            "אתה עוזר שקט וענוד. דיבור דקיק וקצר בלבד."
        );

        AddChat("? ערכת הנושא שונתה ל Quiet", Color.FromArgb(200, 200, 255), isAI: true);
    }
    // ... other themes
}
```

### Option 3: Add a System Prompt Command

```csharp
else if (userInput.StartsWith("סט סיסטם") || userInput.StartsWith("set system"))
{
    string prompt = userInput.Replace("סט סיסטם", "").Replace("set system", "").Trim();

    if (!string.IsNullOrEmpty(prompt))
    {
        _ollamaService.SetSystemPrompt(prompt);
        AddChat($"? סיסטם פרומפט עודכן", Color.FromArgb(200, 200, 255), isAI: true);
        UpdateStatus("System Prompt Updated", Color.FromArgb(76, 175, 255));
    }
}
```

---

## System Prompt Examples by Use Case

### 1. For Games/Fun
```csharp
"אתה AI משעשע וקומי. " +
"תשובותיך צריכות להיות הומוריסטיות ובעלות גחמים חכמים."
```

### 2. For Learning
```csharp
"אתה מורה סבלני. " +
"הסבר בפשטות, תן דוגמאות, וודא הבנה."
```

### 3. For Business
```csharp
"אתה??עסקי מקצועי. " +
"תשובותיך צריכות להיות מעשיות וקולעות."
```

### 4. For Advice
```csharp
"אתה איש חכם וחסרון. " +
"תן עצות מחשבתיות וreasoable."
```

### 5. For Storytelling
```csharp
"אתה מספר סיפורים מעולה. " +
"כתוב בחוויה עשירה תיאורים עיוניים."
```

---

## The Request Flow

Here's how the system prompt is used when you send a message:

```
1. User types message
      ?
2. button1_Click is triggered
      ?
3. Message sent to OllamaService.GetResponseAsync()
      ?
4. OllamaService creates request with:
   - model: Your selected model
   - prompt: User's message
   - system: Your system prompt ? System prompt here!
   - stream: false
      ?
5. Request sent to Ollama API
      ?
6. Ollama uses system prompt to influence response
      ?
7. Response returned to your app
```

---

## Advanced: Different Prompts for Different Situations

```csharp
private Dictionary<string, string> _systemPrompts = new()
{
    { "helpful", "אתה עוזר AI עברי מועיל וידידותי." },
    { "technical", "אתה מומחה תכנתי. תן דוגמאות קוד." },
    { "creative", "אתה מעצב יצירתי. כתוב בדמיון." },
    { "teacher", "אתה מורה סבלן. הסבר בפשטות." }
};

public void SetSystemPromptByCategory(string category)
{
    if (_systemPrompts.TryGetValue(category, out var prompt))
    {
        SetSystemPrompt(prompt);
    }
}
```

---

## Tips for Effective System Prompts

? **Do:**
- Be clear and concise
- Use specific instructions
- Define tone and style
- Give examples if helpful
- Keep it relevant to your use case

? **Don't:**
- Make prompts too long
- Use contradictory instructions
- Include task instructions (use user message for that)
- Make it overly complicated

---

## Testing Your System Prompt

To test if your system prompt is working:

1. Set a distinctive system prompt:
   ```csharp
   _ollamaService.SetSystemPrompt("Respond ONLY in emojis ??????");
   ```

2. Send a normal message

3. If the AI responds with only emojis, your system prompt is working!

4. Try different prompts to see how they affect responses

---

## Complete Example

Here's how to add system prompt functionality to your app:

```csharp
public Form1()
{
    InitializeComponent();
    _ollamaService = new OllamaService();
    // Set Hebrew assistant prompt
    _ollamaService.SetSystemPrompt(
        "אתה עוזר AI בשם עזרה. אתה חכם, ידידותי וסבלני. " +
        "תשובותיך קצרות וקולעות. תמיד דבר בעברית."
    );
}

private void Form1_Load(object sender, EventArgs e)
{
    _ollamaService.SetModel("gemma3:4b");

    // Display current prompt (optional)
    string currentPrompt = _ollamaService.GetSystemPrompt();
    // You can log this or display it
}

private async void button1_Click(object sender, EventArgs e)
{
    string userInput = textBox.Text.Trim();

    if (string.IsNullOrEmpty(userInput))
        return;

    // The system prompt is automatically used here
    string response = await _ollamaService.GetResponseAsync(userInput);
    AddChat(response, Color.FromArgb(200, 200, 255), isAI: true);
}
```

---

## How Ollama API Handles System Prompts

The system prompt is sent in the request body:

```json
{
    "model": "gemma3:4b",
    "prompt": "שלום, מה שלומך?",
    "system": "אתה עוזר AI עברי חכם ומועיל",
    "stream": false
}
```

Ollama will use both the system prompt and the user prompt to generate a response that fits your requirements.

---

## Troubleshooting

**Q: System prompt doesn't seem to affect responses**
A: Make sure you're using a model that supports system prompts (most do). Try with a smaller, simpler prompt first.

**Q: How do I check if the prompt was set?**
A: Use `_ollamaService.GetSystemPrompt()` to verify the current prompt.

**Q: Can I change the system prompt mid-conversation?**
A: Yes! Call `SetSystemPrompt()` anytime. New messages will use the new prompt.

**Q: Does the system prompt affect performance?**
A: Slightly, longer prompts take a bit more processing, but it's negligible.

---

## Summary

Your `OllamaService` now has:
? Default Hebrew system prompt
? `SetSystemPrompt()` method to change it
? `GetSystemPrompt()` method to read it
? Automatic inclusion in all API requests

Use this to customize AI behavior for your application!

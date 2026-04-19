# System Prompt Quick Reference

## What Changed

Your `OllamaService` now supports system prompts! ?

---

## The Three Key Changes

### 1. Added System Prompt Field
```csharp
private string _systemPrompt;
```

### 2. Initialized with Default Prompt
```csharp
_systemPrompt = "אתה עוזר AI עזרה מועיל, ידידותי וערמומי. אתה כותב תשובות קצרות וקולעות. תמיד תשובות בעברית.";
```

### 3. Included System Prompt in API Request
```csharp
var requestBody = new
{
    model = _modelName,
    prompt = userMessage,
    system = _systemPrompt,  // ? System prompt here!
    stream = false
};
```

---

## Two New Methods

### Set System Prompt
```csharp
_ollamaService.SetSystemPrompt("Your custom prompt here");
```

### Get System Prompt
```csharp
string prompt = _ollamaService.GetSystemPrompt();
```

---

## How It Works

```
User Message + System Prompt ? Ollama API ? Influenced Response
```

---

## Quick Examples

### Hebrew Assistant (Current Default)
```csharp
"אתה עוזר AI עזרה מועיל, ידידותי וערמומי. אתה כותב תשובות קצרות וקולעות. תמיד תשובות בעברית."
```

### Technical Helper
```csharp
_ollamaService.SetSystemPrompt(
    "You are a coding expert. Always provide code examples and explain the logic clearly."
);
```

### Casual Friend
```csharp
_ollamaService.SetSystemPrompt(
    "You are a casual and friendly AI. Keep answers conversational and fun. Use emojis occasionally."
);
```

### Formal Assistant
```csharp
_ollamaService.SetSystemPrompt(
    "You are a formal and professional assistant. Keep responses concise and business-appropriate."
);
```

---

## Where to Use It

### In Form1_Load (Startup)
```csharp
private void Form1_Load(object sender, EventArgs e)
{
    _ollamaService.SetModel("gemma3:4b");

    // Set your custom system prompt
    _ollamaService.SetSystemPrompt("Your system prompt here");

    // ... rest of code
}
```

### Change Based on User Input
```csharp
else if (userInput.Equals("mode technical"))
{
    _ollamaService.SetSystemPrompt("You are a coding expert...");
    AddChat("? Switched to technical mode", ...);
}
```

### Change Based on Theme
```csharp
private void HandleThemeCommand(string command)
{
    if (command.Contains("Quiet"))
    {
        _ollamaService.SetSystemPrompt("Be quiet and brief...");
    }
    else if (command.Contains("Colorful"))
    {
        _ollamaService.SetSystemPrompt("Be creative and expressive...");
    }
}
```

---

## Tips

? **Keep it clear** - Be specific about what you want
? **Short is better** - Concise prompts work better
? **Test it** - Try different prompts to see what works
? **Change anytime** - You can switch prompts mid-conversation

---

## Test It

1. Set an obvious prompt:
```csharp
_ollamaService.SetSystemPrompt("Respond ONLY in one emoji ??");
```

2. Send a message and see if Ollama follows the instruction

3. If yes, your system prompt is working! ??

---

## Current Default

**Hebrew:** "אתה עוזר AI עזרה מועיל, ידידותי וערמומי. אתה כותב תשובות קצרות וקולעות. תמיד תשובות בעברית."

**English (commented):** "You are a helpful, friendly, and witty AI assistant. your name is עזרא..."

---

## Build Status

? **Build Successful** - Everything compiles correctly

---

## Full Documentation

See: **OLLAMA_SYSTEM_PROMPT_GUIDE.md** for comprehensive guide

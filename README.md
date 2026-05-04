# Ezra - AI Assistant Application 
  
Ezra is a modern Windows Forms AI assistant application built with .NET 7  
  
## Features  
AI Chat Interface - Conversational AI powered by Ollama  
Modern Dark Theme UI  
Responsive Design  
Games Integration  
Clock Application 
  
## Installation  
  
Prerequisites:  
Windows OS  
.NET 7 or later  
Ollama installed locally  
  
Setup:  
1. Download the latest release  
2. Install Ollama:
```bash
irm https://ollama.com/install.ps1 | iex  
```
3. Install model:
```bash
ollama pull gemma4:e2b  
```
4. Start service:
```bash
ollama serve  
```
5. Launch Ezra 
  
## Project Structure  
  
Form1.cs - Main chat interface backend
chat.html - main chat interface
Form1.Designer.cs - UI layout  
OllamaService.cs - Ollama API integration  
Program.cs - Application entry point  
ClockForm.cs - Clock/Alarm application ( mostly WebView2 )
clock.html + clock-popup.html - clock interface
Data/ - Game executables and resources - the code for the games is not shared the .exe are in the releases
  
## Technology Stack  
  
Framework: .NET 7 Windows Forms  
Language: C# 11.0  
AI Integration: Ollama REST API  
UI: Modern Windows Forms with dark theme 
  
## UI Features  
  
Dark Theme - RGB(25, 25, 35) base color  
Color-Coded Buttons:  
Games: Blue RGB(76, 175, 255)  
Music: Purple RGB(165, 105, 255)  
Clock: Green RGB(76, 220, 110)  
Send: Red RGB(255, 107, 107)  
Chat Bubbles align left for AI, right for user messages  
Responsive layout adapts to window resizing 
  
## Usage  
  
1. Start ollama serve before launching  
2. Open Ezra application  
3. Type messages and press send button  
4. Click games button for game options  
5. Click clock button for alarm settings 
  
## Configuration  
  
Default Ollama Settings:  
URL: http://localhost:11434  
Model: gemma4:e2b
Timeout: 10 minutes 
  
## Supported Models  
  
gemma4:e2b - default, fast  
gemma2  
llama2  
mistral  
neural-chat 
  
## Troubleshooting  
  
If Ollama is not available:  
- Ensure ollama serve is running  
note - in some cases ollama is running in the background 
- Check http://localhost:11434
- Verify model is installed with:
```bash
ollama list  
```
  
If model not found:  
- Run: ollama pull gemma4:e2b
```bash
ollama pull gemma4:e2b
```
  
## Authors  
  
Yahav - https://github.com/Yahav-Programing  
  
## License  
  
This project is using the MIT License - see the LICENSE file for details.
  
## Links  
  
Ollama Official: https://ollama.com  
Ollama Models: https://ollama.com/library  
Note: This application requires Ollama installed and running locally 

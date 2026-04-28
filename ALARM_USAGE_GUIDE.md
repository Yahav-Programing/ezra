# Alarm Clock Usage Guide

## Quick Start

### Method 1: Using the Clock Button
1. Click the ? **שעון מעורר** (Clock) button in the chat interface
2. A clock view will appear with:
   - Current time display
   - Alarm setting panel
   - List of existing alarms

3. Set the time:
   - Enter hours (00-23)
   - Enter minutes (00-59)
   - Add a label (optional)

4. Click **Set Alarm** button

5. The alarm will trigger at the specified time with:
   - Visual pulse effect
   - Beeping sound
   - Snooze/Dismiss options

### Method 2: Ask the AI
Simply tell the AI to set an alarm:

**Examples:**
- "Set an alarm for 7:30 AM"
- "Wake me up at 06:00"
- "I need an alarm at 14:30"
- "Set a reminder alarm for 9:15"

The AI will respond with a command like:
```
I'll set that alarm for you. /clock {07:30}
```

The system will automatically:
- Parse the command
- Open the clock interface
- Set the alarm
- Show you the alarm was created

## Command Format Reference

### For AI Responses
The AI can use these commands in their responses:

#### Alarm Command
```
/clock {HH:MM}
```
- **HH**: Hour (00-23, 24-hour format)
- **MM**: Minutes (00-59)

Examples:
- `/clock {07:30}` - Sets alarm for 7:30 AM
- `/clock {14:45}` - Sets alarm for 2:45 PM
- `/clock {23:00}` - Sets alarm for 11:00 PM

#### Game Command
```
/game {2048|ghost}
```
- **2048** - Opens the 2048 game
- **ghost** - Opens the Ghost game

Examples:
- `Let's play! /game {2048}`
- `I'll launch it for you. /game {ghost}`

## Features

### Alarm Management
- ? Set multiple alarms
- ? Enable/Disable alarms individually
- ? Delete alarms
- ? Custom labels for alarms
- ? Snooze for 5 minutes
- ? Instant dismiss

### Display & Sound
- ?? Live clock with hours:minutes:seconds
- ?? Current date display
- ?? Beeping sound when alarm triggers
- ?? Pulse animation effect
- ?? Dark/Light theme toggle

### Data Persistence
- Alarms are saved in browser storage
- Alarms persist even after closing the app
- Automatic cleanup when dismissed

## Time Format

### Valid Input Examples
```
Hour: 0-23 (24-hour format)
  00 = Midnight (12:00 AM)
  06 = 6:00 AM
  12 = Noon (12:00 PM)
  18 = 6:00 PM
  23 = 11:00 PM

Minutes: 00-59
  00 = Top of the hour
  15 = Quarter past
  30 = Half past
  45 = Quarter to next hour
```

## Complete Examples

### Example 1: Set Alarm via Chat
```
User: "Please set an alarm for tomorrow morning at 6:30"
AI: "I'll set an alarm for 6:30 AM for you. /clock {06:30}"
System: Opens clock, sets alarm for 06:30
```

### Example 2: Set Multiple Alarms
```
Click Clock button
? Set alarm at 06:00
? Set alarm at 13:00
? Set alarm at 21:00

All three alarms will trigger at their respective times
```

### Example 3: Manage Alarms
```
Click Clock button
? See list of existing alarms
? Toggle alarms on/off
? Delete unwanted alarms
? Snooze active alarms
```

## Troubleshooting

### Issue: Alarm doesn't sound
**Solution:**
- Check if your browser allows audio playback
- Make sure the alarm is enabled (check the toggle)
- Check browser volume/system volume
- Check for browser notification permissions

### Issue: Time format not accepted
**Solution:**
- Use 24-hour format (0-23 for hours)
- Use valid minutes (0-59)
- Format must be HH:MM (with leading zeros recommended)

### Issue: AI doesn't generate alarm commands
**Solution:**
- Ask more clearly: "Set alarm" instead of "remind me"
- Try phrases like: "wake me up", "set an alarm", "alarm for"
- The AI responds with `/clock {HH:MM}` format

### Issue: Alarms disappear after refresh
**Solution:**
- Check browser settings for localStorage
- Ensure cookies/storage are not blocked
- Try a different browser if problem persists

## Tips & Tricks

### Quick Alarms
- Clock button is always visible for quick access
- Don't need to wait for AI responses
- Direct interface is fastest way to set alarms

### Using AI for Natural Language
- "Wake me up at 8"
- "Set a 5PM reminder"
- "Alarm at half past three"
- "Morning wake-up call at 6:30"

### Multiple Alarms
- Set alarms for different activities
- Use meaningful labels (Work, Gym, Meeting, etc.)
- Disable alarms you don't need temporarily

### Combining Features
- Set alarm via AI chat
- While waiting, use chat for other tasks
- Clock opens automatically
- Alarms trigger while using other features

## Notes
- Alarms work best in the foreground (app focused)
- Background alarms depend on browser notification settings
- 24-hour format is used throughout (00:00-23:59)
- All times are in local timezone

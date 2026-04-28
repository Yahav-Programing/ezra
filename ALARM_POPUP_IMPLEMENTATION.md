# Alarm Popup Window Implementation

## Overview

The alarm clock now opens a **separate popup window** when an alarm triggers, instead of showing an inline alert. This provides a more prominent notification and better user experience.

## Features

### Popup Window Behavior
- ? Opens as a separate window when alarm triggers
- ? Stays on top of other windows (TopMost)
- ? Shows alarm time, label, and countdown
- ? Displays in center of screen
- ? Non-resizable for consistent UI
- ? Shows in taskbar for visibility
- ? Automatically closes when dismissed

### Alarm Controls in Popup
- **Dismiss** - Stop alarm immediately
- **Snooze 5 min** - Delay alarm by 5 minutes
- **Cancel Alarm** - Remove alarm completely

### Visual Features
- ? Emoji animation in popup center
- ?? Beeping sound via Web Audio API
- ?? Pulse animation on circular progress
- ? Particle effects background
- ?? Countdown progress ring
- ?? Clear time display

## How It Works

### Trigger Flow

```
1. Clock checks time every second
   ?
2. Alarm time matches current time
   ?
3. openAlarmPopup() called
   ?
4. JavaScript sends 'openPopup' message to C#
   ?
5. C# HandleOpenPopup() creates new window
   ?
6. WebView2 loads clock-popup.html
   ?
7. Popup window displays with alarm
```

### Communication

```
JavaScript (chat.html)
    ? postMessage
C# Backend (Form1.cs)
    ? OpenAlarmPopupWindow()
New Window with WebView2
    ?
clock-popup.html rendered
    ?
User interacts (Dismiss/Snooze/Cancel)
    ?
postMessage back to C#
```

## Technical Implementation

### Frontend Changes (chat.html)

**New Function**: `openAlarmPopup(alarm)`
```javascript
function openAlarmPopup(alarm){
  const popupUrl = 'clock-popup.html?hour=' + alarm.hour + 
                   '&minute=' + alarm.minute + 
                   '&label=' + encodeURIComponent(alarm.label);

  if(window.chrome && window.chrome.webview){
    window.chrome.webview.postMessage({
      type: 'openPopup',
      url: popupUrl,
      title: alarm.label,
      hour: alarm.hour,
      minute: alarm.minute
    });
  }
}
```

**Message Handling**:
- Listens for `alarmDismissed`, `alarmSnoozed`, `alarmCancelled` from popup
- Updates alarm state in localStorage
- Creates snooze alarm if needed

### Backend Changes (Form1.cs)

**New Method**: `HandleOpenPopup()`
- Parses the openPopup message
- Extracts popup URL and parameters
- Calls OpenAlarmPopupWindow()

**New Method**: `OpenAlarmPopupWindow()`
- Creates new Windows Form
- Adds WebView2 control
- Loads clock-popup.html
- Sets window properties (TopMost, size, position)
- Handles inter-window communication

### Popup HTML (clock-popup.html)

**Features**:
- Parses alarm data from URL parameters
- Displays countdown timer
- Shows circular progress ring
- Generates beeping sound
- Handles user actions (Dismiss/Snooze/Cancel)
- Communicates back to main window via postMessage

## Window Properties

```csharp
// Popup form configuration
- Title: "? Alarm"
- Width: 500px
- Height: 650px
- Position: CenterScreen
- TopMost: true (stays on top)
- FormBorderStyle: FixedDialog (can't resize)
- MinimizeBox: true (can minimize)
- MaximizeBox: false (can't maximize)
- ShowInTaskbar: true (visible in taskbar)
```

## Data Flow for Alarm Trigger

### 1. Alarm Set in Chat
```
User sets alarm for 07:30
?
Stored in localStorage
?
checkAlarmsInterval() monitoring
```

### 2. Time Reaches 07:30
```
updateClock() runs every second
?
Finds matching alarm in localStorage
?
currentRingingAlarmId set
?
openAlarmPopup(alarm) called
```

### 3. Popup Opens
```
JavaScript sends openPopup message
?
Form1.HandleOpenPopup() receives it
?
OpenAlarmPopupWindow() creates Form
?
WebView2 loads clock-popup.html with params
?
Popup displays countdown and sounds
```

### 4. User Action
```
User clicks Dismiss/Snooze/Cancel
?
Popup sends message back to main window
?
Main window updates alarm state
?
Popup closes automatically
```

## URL Parameter Format

The popup receives alarm data via URL query string:

```
clock-popup.html?hour=7&minute=30&label=Wake%20up
```

Parsed by clock-popup.html:
```javascript
function getParams(){
  const p=new URLSearchParams(window.location.search);
  return{
    hour  : parseInt(p.get('hour') ?? 7, 10),
    minute: parseInt(p.get('minute') ?? 0, 10),
    label : p.get('label') ?? 'Alarm'
  };
}
```

## Message Types

### From chat.html to C#
```json
{
  "type": "openPopup",
  "url": "clock-popup.html?hour=7&minute=30&label=Wake%20up",
  "title": "Wake up",
  "hour": 7,
  "minute": 30
}
```

### From clock-popup.html to C#
```json
// When alarm triggers
{"type": "alarmRinging", "label": "Wake up", "hour": 7, "minute": 30}

// When user dismisses
{"type": "alarmDismissed", "label": "Wake up", "hour": 7, "minute": 30}

// When user snoozes
{"type": "alarmSnoozed", "hour": 7, "minute": 35, "label": "Wake up"}

// When user cancels
{"type": "alarmCancelled", "label": "Wake up", "hour": 7, "minute": 30}

// When closing popup
{"type": "closeCountdown"}
```

## Alarm Actions

### Dismiss
- **Effect**: Stops alarm immediately
- **Storage**: Removes alarm (won't repeat)
- **Window**: Closes popup
- **Notification**: None

### Snooze 5 min
- **Effect**: Delays alarm by 5 minutes
- **Storage**: Creates new alarm 5 min later
- **Window**: Closes popup
- **Notification**: Shows "snoozed" in label

### Cancel Alarm
- **Effect**: Stops alarm before trigger
- **Storage**: Removes alarm completely
- **Window**: Closes popup
- **Notification**: None

## User Experience Flow

### Scenario: Morning Wake-up

1. **Night before**: User sets alarm for 07:00 via clock button
   - Alarm stored in localStorage
   - Returns to chat interface

2. **07:00 AM**: Alarm triggers
   - Main window remains in background
   - **Popup window appears** in center of screen
   - Beeping sound starts
   - Countdown shows "Time's up!"
   - Buttons: Dismiss, Snooze 5 min, Cancel

3. **User chooses action**:
   - **Dismiss**: Alarm stops, window closes, can't repeat
   - **Snooze**: New alarm set for 07:05, window closes
   - **Cancel**: Alarm removed, window closes

## Features

### Sound Generation
- **Type**: Web Audio API (no files needed)
- **Frequency**: 880 Hz (musical note A5)
- **Duration**: 0.45s per beep
- **Interval**: 900ms between beeps
- **Volume**: 0.5 (50% amplitude)

### Visual Elements
- **Progress Ring**: SVG circle showing time remaining
- **Countdown**: Large time display
- **Emoji**: Bouncing ? when ringing
- **Particles**: Animated background
- **Pulse**: Red color when ringing
- **Ripple**: Button click animation

### Responsive Design
- Works on different screen sizes
- Popup size: 500x650px (optimal for visibility)
- Mobile-friendly if needed
- Touch-friendly buttons

## Error Handling

### If HTML File Not Found
- Displays error message in popup
- Allows user to close window
- Main application continues running

### If WebView2 Fails
- Catches exception and logs
- Prevents main app crash
- User can dismiss notification

### If Popup Closed Without Action
- Alarm continues beeping (in popup)
- Can be reopened by taskbar click
- Message repeats until dismissed

## Browser Requirements

- **Windows 10+** (for WebView2)
- **Modern WebKit engine** (for Web Audio API)
- **JavaScript ES6+** support
- **LocalStorage** support

## Testing Recommendations

1. **Set alarm near current time** to test quickly
2. **Verify popup opens** as separate window
3. **Test all buttons**:
   - Dismiss closes popup and stops sound
   - Snooze creates alarm 5 min later
   - Cancel removes alarm
4. **Test sound** - should beep repeatedly
5. **Test countdown** - should decrease to 0:00
6. **Test on background** - click main window, popup stays visible

## Known Limitations

- Popup uses same theme as main window
- No notification on dismissed alarms
- Snooze always 5 minutes (hardcoded)
- Alarms stored locally only (no cloud sync)
- Popup shows in same taskbar (not separate alert)

## Future Enhancements

- [ ] Custom snooze duration
- [ ] Sound selection (multiple alarm tones)
- [ ] Sleep timer functionality
- [ ] Alarm repeat/recurrence
- [ ] Custom colors for different alarms
- [ ] Notification center integration
- [ ] Multiple alarm support per time
- [ ] Accessibility improvements

## Troubleshooting

### Popup doesn't appear
- Check if clock-popup.html exists in app directory
- Verify WebView2 is installed
- Check browser console for errors

### Sound doesn't play
- Check system volume
- Verify Web Audio API is supported
- Check browser autoplay settings

### Popup closes immediately
- Check for exceptions in Form1.cs
- Verify 'closeCountdown' message not sent early
- Check message parsing

### Alarm doesn't trigger
- Verify time format is HH:MM (24-hour)
- Check if alarm is enabled in list
- Verify system time is correct

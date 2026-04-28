namespace ezra
{
    using Microsoft.Web.WebView2.WinForms;
    using Microsoft.Web.WebView2.Core;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public class AlarmWindow : Form
    {
        private WebView2 alarmWebView;
        private string alarmHour;
        private string alarmMinute;

        public AlarmWindow(string hour, string minute)
        {
            alarmHour = hour;
            alarmMinute = minute;

            // Setup form properties
            this.Text = "? Alarm Clock";
            this.Width = 500;
            this.Height = 700;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ShowInTaskbar = true;
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = true;

            // Create WebView2
            alarmWebView = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = new CoreWebView2CreationProperties
                {
                    UserDataFolder = Path.Combine(Path.GetTempPath(), "WebView2AlarmPopup_" + Guid.NewGuid().ToString())
                }
            };

            this.Controls.Add(alarmWebView);
            this.Load += AlarmWindow_Load;
        }

        private async void AlarmWindow_Load(object sender, EventArgs e)
        {
            try
            {
                await alarmWebView.EnsureCoreWebView2Async(null);

                // Load the clock-popup.html file
                string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clock-popup.html");

                if (File.Exists(htmlPath))
                {
                    string htmlContent = File.ReadAllText(htmlPath);
                    alarmWebView.CoreWebView2.NavigateToString(htmlContent);

                    // Wait for the page to fully load
                    await Task.Delay(1000);

                    // Set the alarm time
                    await SetAlarmTime(alarmHour, alarmMinute);
                }
                else
                {
                    alarmWebView.CoreWebView2.NavigateToString("<h1>Error: clock-popup.html not found</h1>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AlarmWindow_Load: {ex.Message}");
            }
        }

        public async Task SetAlarmTime(string hour, string minute)
        {
            try
            {
                if (alarmWebView?.CoreWebView2 == null)
                {
                    Console.WriteLine("WebView2 not initialized yet");
                    return;
                }

                // Call the JavaScript function to set the alarm
                string script = $@"
console.log('Setting alarm time: {hour}:{minute}');
if(typeof window.setAlarmTime === 'function'){{
    window.setAlarmTime({hour}, {minute}, 'Alarm from Ezra');
    console.log('Alarm time set successfully');
}} else {{
    console.error('setAlarmTime function not found');
}}
";
                string result = await alarmWebView.CoreWebView2.ExecuteScriptAsync(script);
                Console.WriteLine($"Script result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting alarm time: {ex.Message}");
            }
        }
    }
}

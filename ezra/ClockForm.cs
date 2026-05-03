using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ezra
{
    public class ClockForm : Form
    {
        private WebView2? webView;

        public ClockForm()
        {
            Text = "עזרא - שעון";
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(460, 760);
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
        }

        public CoreWebView2 CoreWebView
        {
            get
            {
                if (webView?.CoreWebView2 == null)
                {
                    throw new InvalidOperationException("ClockForm WebView2 is not initialized.");
                }

                return webView.CoreWebView2;
            }
        }

        public async Task InitializeAsync(CoreWebView2Environment? environment = null)
        {
            if (webView?.CoreWebView2 != null)
            {
                return;
            }

            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = environment == null
                    ? new CoreWebView2CreationProperties
                    {
                        UserDataFolder = Path.Combine(Path.GetTempPath(), "EzraClockUserData")
                    }
                    : null
            };

            Controls.Add(webView);

            await webView.EnsureCoreWebView2Async(environment);

            webView.CoreWebView2.WindowCloseRequested += (_, _) =>
            {
                if (IsDisposed)
                {
                    return;
                }

                if (IsHandleCreated)
                {
                    BeginInvoke(new Action(Close));
                }
                else
                {
                    Close();
                }
            };

            FormClosed += (_, _) => webView.Dispose();
        }
    }
}

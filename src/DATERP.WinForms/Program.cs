using System;
using System.Windows.Forms;

namespace DATERP.WinForms;

static class Program
{
    // URL of the running DATERP.Web server
    private const string WebUrl = "http://localhost:5223";

    [STAThread]
    static void Main(string[] args)
    {
        System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

        bool autoTestMode = args != null && System.Linq.Enumerable.Contains(args, "--auto-test");

        // Run WinForms with WebView2 pointing to the running Web server
        var form = new MainForm(WebUrl, autoTestMode);
        System.Windows.Forms.Application.Run(form);
    }
}
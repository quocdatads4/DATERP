using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;

namespace DATERP.WinForms;

public partial class MainForm : Form
{
    private readonly string _startUrl;
    private readonly bool _autoTestMode;

    // P/Invoke Declaration
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    private System.ComponentModel.IContainer? components = null;

    public MainForm(string startUrl, bool autoTestMode = false)
    {
        _startUrl = startUrl;
        _autoTestMode = autoTestMode;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1024, 768);
        this.Text = "DATERP Desktop";
        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

        // 1. Create WebView FIRST so it can be captured
        var webView = new WebView2
        {
            Dock = DockStyle.Fill
        };

        // 2. Create MenuStrip
        var menuStrip = new MenuStrip();
        var examMenu = new ToolStripMenuItem("Exam");
        var submitItem = new ToolStripMenuItem("Nộp Bài (Submit & Grade)", null, async (s, e) =>
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn nộp bài không?", "Xác nhận nộp bài", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    // A. Collect Files từ folder DATAcademyTemplates
                    string downloadFolder = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "DATAcademyTemplates");

                    var files = new System.Collections.Generic.List<string>();
                    if (System.IO.Directory.Exists(downloadFolder))
                    {
                        var directory = new System.IO.DirectoryInfo(downloadFolder);
                        foreach (var file in directory.GetFiles("*.docx"))
                        {
                            if (file.CreationTime > DateTime.Now.AddHours(-24))
                            {
                                files.Add(file.FullName);
                            }
                        }
                    }

                    if (files.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy file bài làm nào trong 24h qua.", "Thông báo");
                        return;
                    }

                    // B. Lấy GradingConfig từ API (Mock: Hardcode 6 Tasks cho Project 1)
                    // Trong tương lai: Gọi API examTask.getList() để lấy GradingConfig
                    var taskConfigs = new System.Collections.Generic.List<string?>
                    {
                        "{\"Type\": \"Property\", \"Key\": \"Category\", \"Value\": \"dinosaur\", \"Match\": \"Contains\"}",
                        "{\"Type\": \"ParagraphFormat\", \"ReferenceParagraphOrder\": 1, \"TargetParagraphOrder\": 2, \"HeadingText\": \"When and Where Dinosaurs Lived\"}",
                        "{\"Type\": \"TableSort\", \"HeadingText\": \"Geologic eras\", \"SortColumns\": [{\"ColIndex\": 1, \"Order\": \"Ascending\"}]}",
                        "{\"Type\": \"ListLevel\", \"TargetText\": \"Developed a narrow eyebrow\", \"Level\": 2}",
                        "{\"Type\": \"3DModel\", \"Wrapping\": \"Square\"}",
                        "{\"Type\": \"ArtisticEffect\", \"EffectName\": \"PencilSketch\", \"ContextText\": \"Theropod\"}"
                    };

                    // C. Grade từng file
                    var gradingService = new WinFormsGradingService();
                    var allResults = new System.Collections.Generic.List<TaskGradingResult>();

                    foreach (var filePath in files)
                    {
                        var results = gradingService.GradeProject(filePath, taskConfigs);
                        allResults.AddRange(results);
                    }

                    // D. Tính điểm tổng
                    int passedTasks = allResults.Count(r => r.Passed);
                    int totalTasks = allResults.Count;
                    int score = totalTasks > 0 ? (passedTasks * 1000) / totalTasks : 0;
                    bool isPassed = score >= 700;

                    // E. Hiển thị kết quả chi tiết
                    var detailBuilder = new System.Text.StringBuilder();
                    detailBuilder.AppendLine($"=== KẾT QUẢ CHẤM ĐIỂM ===");
                    detailBuilder.AppendLine($"Điểm: {score}/1000 ({passedTasks}/{totalTasks} task)");
                    detailBuilder.AppendLine($"Kết quả: {(isPassed ? "ĐẬU" : "RỚT")}");
                    detailBuilder.AppendLine();
                    detailBuilder.AppendLine("Chi tiết từng Task:");
                    foreach (var r in allResults)
                    {
                        string status = r.Passed ? "✓" : "✗";
                        detailBuilder.AppendLine($"  Task {r.TaskOrder}: {status} {r.Reason}");
                    }

                    MessageBox.Show(detailBuilder.ToString(), "Kết quả chấm điểm", MessageBoxButtons.OK,
                        isPassed ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    // F. Submit Score to API
                    Guid examListId = Guid.Empty;
                    string submitDto = $"{{\"examListId\": \"{examListId}\", \"totalScore\": {score}, \"isPassed\": {isPassed.ToString().ToLower()}}}";
                    string jsSubmit = $"daterp.examination.examination.examSubmission.submitScore({submitDto});";
                    await webView.ExecuteScriptAsync(jsSubmit);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi nộp bài: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        });
        examMenu.DropDownItems.Add(submitItem);
        menuStrip.Items.Add(examMenu);

        // 3. Add Controls (Menu MUST be added to form)
        this.MainMenuStrip = menuStrip;
        Controls.Add(menuStrip); // Add MenuStrip
        Controls.Add(webView);   // Add WebView

        // Layout Config
        // MenuStrip uses Dock=Top by default. WebView Dock=Fill will fill the rest.
        // Controls.Add order matters for Z-order but Docking handles layout. 
        // Adding WebView second means it is top in Z-order list? 
        // Actually for Dock=Fill to respect Dock=Top, Dock=Top must be added *last* in some frameworks, or *first* in others.
        // In WinForms: Controls added LAST are at the START of the Z-order (Top).
        // Dock order: Controls at the END of the collection are docked FIRST.
        // So we want MenuStrip to be docked first (Top). So it should be at the End of Controls collection?
        // Let's use BringToFront/SendToBack to be safe.
        // Or simpler: Just set webView.Top logic manually if Docking fails, but Docking is standard.
        // Correct WinForms Docking:
        // menuStrip.Dock = Top. webView.Dock = Fill.
        // Add webView. Add menuStrip. (MenuStrip is last added -> First in Z-Order -> Docked First?)
        // Let's try explicit BringToFront.
        menuStrip.BringToFront();


        this.Load += async (s, e) =>
        {
            EnsureDATAcademyTemplatesFolder();

            await webView.EnsureCoreWebView2Async();
            webView.Source = new Uri(_startUrl);

            // Handle navigation to resize window for ExamTaking
            webView.SourceChanged += (sender, args) =>
            {
                var currentUrl = webView.Source?.AbsolutePath ?? "";
                if (currentUrl.StartsWith("/Examination/ExamTaking", StringComparison.OrdinalIgnoreCase))
                {
                    var screen = Screen.FromControl(this);
                    var workingArea = screen.WorkingArea;
                    int newWidth = workingArea.Width;
                    int newHeight = (int)(workingArea.Height * 0.25);
                    int newX = workingArea.Left;
                    int newY = workingArea.Bottom - newHeight;

                    this.WindowState = FormWindowState.Normal;
                    this.SetBounds(newX, newY, newWidth, newHeight);
                    this.TopMost = true;
                }
                else if (currentUrl.StartsWith("/Examination/ExamSubjects", StringComparison.OrdinalIgnoreCase))
                {
                    this.TopMost = false;
                    this.WindowState = FormWindowState.Maximized;
                }
            };

            webView.CoreWebView2.DownloadStarting += (s, e) =>
            {
                e.Handled = true;
                var downloadOperation = e.DownloadOperation;
                string originalName = System.IO.Path.GetFileNameWithoutExtension(downloadOperation.ResultFilePath);
                string extension = System.IO.Path.GetExtension(downloadOperation.ResultFilePath);
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string fileName = $"{originalName}_{timestamp}{extension}";

                string downloadFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "DATAcademyTemplates");

                if (!System.IO.Directory.Exists(downloadFolder))
                    System.IO.Directory.CreateDirectory(downloadFolder);

                string destinationPath = System.IO.Path.Combine(downloadFolder, fileName);
                e.ResultFilePath = destinationPath;

                downloadOperation.StateChanged += (opSender, opArgs) =>
                {
                    if (downloadOperation.State == Microsoft.Web.WebView2.Core.CoreWebView2DownloadState.Completed)
                    {
                        string projectId = "";
                        try
                        {
                            var uri = new Uri(downloadOperation.Uri);
                            var query = uri.Query.TrimStart('?');
                            foreach (var part in query.Split('&'))
                            {
                                var kv = part.Split('=');
                                if (kv.Length == 2 && kv[0].Equals("projectId", StringComparison.OrdinalIgnoreCase))
                                {
                                    projectId = kv[1];
                                    break;
                                }
                            }
                        }
                        catch { }

                        try
                        {
                            this.Invoke((MethodInvoker)async delegate
                            {
                                string sessionId = "";
                                if (!string.IsNullOrEmpty(projectId))
                                {
                                    try
                                    {
                                        string startDtStr = $"{{\"examProjectId\": \"{projectId}\"}}";
                                        string js = $"return daterp.examination.examination.examSession.startSession({startDtStr});";
                                        string resultJson = await webView.ExecuteScriptAsync(js);
                                        sessionId = resultJson.Trim('"');
                                    }
                                    catch (Exception) { }
                                }

                                var wordProcesses = System.Diagnostics.Process.GetProcessesByName("WINWORD");
                                foreach (var proc in wordProcesses)
                                {
                                    try { proc.Kill(); proc.WaitForExit(3000); } catch { }
                                }

                                var process = new System.Diagnostics.Process
                                {
                                    StartInfo = new System.Diagnostics.ProcessStartInfo(destinationPath) { UseShellExecute = true }
                                };
                                process.Start();

                                System.Threading.Tasks.Task.Run(async () =>
                                {
                                    int retries = 0;
                                    IntPtr mainWindowHandle = IntPtr.Zero;
                                    while (retries < 20)
                                    {
                                        await System.Threading.Tasks.Task.Delay(500);
                                        var procs = System.Diagnostics.Process.GetProcessesByName("WINWORD");
                                        if (procs.Length > 0)
                                        {
                                            var wp = procs[0];
                                            wp.Refresh();
                                            if (wp.MainWindowHandle != IntPtr.Zero)
                                            {
                                                mainWindowHandle = wp.MainWindowHandle;
                                                break;
                                            }
                                        }
                                        retries++;
                                    }

                                    if (mainWindowHandle != IntPtr.Zero)
                                    {
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            var screen = Screen.FromControl(this);
                                            var workingArea = screen.WorkingArea;
                                            SetWindowPos(mainWindowHandle, IntPtr.Zero, workingArea.Left, workingArea.Top, workingArea.Width, (int)(workingArea.Height * 0.75), 0x0040);
                                        });

                                        var activeWord = System.Diagnostics.Process.GetProcessesByName("WINWORD");
                                        if (activeWord.Length > 0)
                                        {
                                            activeWord[0].WaitForExit();
                                            if (!string.IsNullOrEmpty(sessionId) && sessionId != "null")
                                            {
                                                this.Invoke((MethodInvoker)async delegate
                                                {
                                                    string safePath = destinationPath.Replace("\\", "\\\\");
                                                    string completeDto = $"{{\"sessionId\": \"{sessionId}\", \"filePath\": \"{safePath}\"}}";
                                                    string jsComplete = $"daterp.examination.examination.examSession.completeSession({completeDto});";
                                                    await webView.ExecuteScriptAsync(jsComplete);
                                                });
                                            }
                                        }
                                    }
                                });
                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                };
            };

            webView.NavigationCompleted += async (sender, args) =>
            {
                if (!args.IsSuccess) return;
                var currentUrl = webView.Source.ToString();

                // Auto Login
                if (currentUrl.Contains("/Account/Login", StringComparison.OrdinalIgnoreCase))
                {
                    string script = @"(function() { setTimeout(() => {
                        var emailInput = document.querySelector('input[name=""LoginInput.UserNameOrEmailAddress""]');
                        var passwordInput = document.querySelector('input[name=""LoginInput.Password""]');
                        var loginBtn = document.querySelector('button[value=""Login""]');
                        if (emailInput && passwordInput && loginBtn) {
                            emailInput.value = 'student@datacademy.edu.vn';
                            emailInput.dispatchEvent(new Event('input', { bubbles: true }));
                            emailInput.dispatchEvent(new Event('change', { bubbles: true }));
                            passwordInput.value = 'Student@123';
                            passwordInput.dispatchEvent(new Event('input', { bubbles: true }));
                            passwordInput.dispatchEvent(new Event('change', { bubbles: true }));
                            setTimeout(() => { loginBtn.click(); }, 500);
                        }
                    }, 500); })();";
                    await webView.ExecuteScriptAsync(script);
                }

                if (_autoTestMode)
                {
                    var path = new Uri(currentUrl).AbsolutePath;
                    if (path == "/" || path == "/Home" || path == "/Index" || path.Contains("/student/dashboard", StringComparison.OrdinalIgnoreCase))
                    {
                        await System.Threading.Tasks.Task.Delay(1000);
                        await webView.ExecuteScriptAsync(@"document.querySelector('#MenuItem_DATERP_ExamSimulation')?.click();");
                    }
                    else if (currentUrl.Contains("/Examination/ExamSubjects", StringComparison.OrdinalIgnoreCase))
                    {
                        await System.Threading.Tasks.Task.Delay(1000);
                        await webView.ExecuteScriptAsync(@"document.querySelector('a[href*=""subjectCode=WORD2019""]')?.click();");
                    }
                    else if (currentUrl.Contains("/Examination/ExamLists", StringComparison.OrdinalIgnoreCase) && currentUrl.Contains("WORD2019", StringComparison.OrdinalIgnoreCase))
                    {
                        await System.Threading.Tasks.Task.Delay(1000);
                        await webView.ExecuteScriptAsync(@"document.querySelector('a[href*=""/Examination/ExamTaking""]')?.click();");
                    }
                }
            };
        };
    }

    private void EnsureDATAcademyTemplatesFolder()
    {
        try
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string templatesPath = System.IO.Path.Combine(documentsPath, "DATAcademyTemplates");
            if (!System.IO.Directory.Exists(templatesPath)) System.IO.Directory.CreateDirectory(templatesPath);
        }
        catch { }
    }
}

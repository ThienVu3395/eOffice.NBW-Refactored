using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Configuration;

namespace Clone_KySoDienTu.Helpers
{
    public static class HtmlToPdfConverter
    {
        private static readonly string ChromePath =
            ConfigurationManager.AppSettings["HtmlToPdf:ChromePath"] ??
            @"C:\Program Files\Google\Chrome\Application\chrome.exe";

        private static readonly string WorkDir =
            ConfigurationManager.AppSettings["HtmlToPdf:WorkDir"] ??
            Path.Combine(Path.GetTempPath(), "Html2PdfWork");

        private static readonly int TimeoutMs = 30000;

        public static byte[] ConvertHtmlString(string html, string persistOutputTo = null)
        {
            Directory.CreateDirectory(WorkDir);

            var htmlPath = Path.Combine(WorkDir, "input_" + Guid.NewGuid().ToString("N") + ".html");
            var pdfPath = Path.Combine(WorkDir, "output_" + Guid.NewGuid().ToString("N") + ".pdf");
            var tmpProfile = Path.Combine(WorkDir, "_tmpProfile");

            try
            {
                File.WriteAllText(htmlPath, html, Encoding.UTF8);

                // Luôn dùng file:///... để Chrome hiểu đúng đường dẫn local
                var fileUrl = "file:///" + htmlPath.Replace('\\', '/');

                // Tắt header/footer: --no-pdf-header-footer (mới)
                // Kèm --print-to-pdf-no-header như fallback (cũ)
                //var args =
                //    "--headless --disable-gpu --no-sandbox --disable-extensions " +
                //    $"--print-to-pdf=\"{pdfPath}\" " +
                //    "--no-pdf-header-footer --print-to-pdf-no-header " +
                //    $"\"{fileUrl}\"";

                // dùng headless cũ cho Chrome ≤109
                var args =
                    "--headless " +
                    "--disable-gpu --no-sandbox --disable-extensions " +
                    "--disable-dev-shm-usage --no-first-run --no-default-browser-check " +
                    "--no-pdf-header-footer --print-to-pdf-no-header " +
                    $"--user-data-dir=\"{tmpProfile}\" " +                     // dùng profile tạm
                    $"--print-to-pdf=\"{pdfPath}\" " +                        // xuất PDF
                    $"\"{fileUrl}\"";                                         // file:///...html

                var psi = new ProcessStartInfo
                {
                    FileName = ChromePath, // chrome.exe hoặc msedge.exe
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using (var p = Process.Start(psi))
                {
                    if (!p.WaitForExit(TimeoutMs))
                    {
                        try { p.Kill(); } catch { /* ignore */ }
                        throw new TimeoutException("Chrome headless timed out.");
                    }

                    // Nếu Chrome trả non-zero code hoặc không tạo file => ném lỗi có log
                    if (!File.Exists(pdfPath) || new FileInfo(pdfPath).Length < 10)
                    {
                        var stderr = p.StandardError.ReadToEnd();
                        var stdout = p.StandardOutput.ReadToEnd();
                        throw new InvalidOperationException(
                            "Không tạo được PDF. Kiểm tra ChromePath/flags/template.\n" +
                            "STDERR: " + stderr + "\nSTDOUT: " + stdout
                        );
                    }
                }

                var bytes = File.ReadAllBytes(pdfPath);

                if (!string.IsNullOrWhiteSpace(persistOutputTo))
                {
                    var dir = Path.GetDirectoryName(persistOutputTo);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.Copy(pdfPath, persistOutputTo, true);
                }

                return bytes;
            }
            finally
            {
                TryDel(htmlPath);
                TryDel(pdfPath);
            }
        }

        private static void TryDel(string p) { try { if (File.Exists(p)) File.Delete(p); } catch { } }
    }
}
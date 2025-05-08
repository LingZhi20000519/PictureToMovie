using System.Diagnostics;

namespace PictureToMovie
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string imageFolderPath = @"C:\LZ\米哈喵呀\尚未发布\AI绘图图片\崩铁\流萤";
            string audioFilePath = @"C:\LZ\米哈喵呀\尚未发布\FreshFocus.mp3";
            string outputPath = @"C:\LZ\output.mp4";

            try
            {
                // 创建临时目录存放有序图片
                string tempImageDir = Path.Combine(Path.GetTempPath(), "VideoGenerator");
                Directory.CreateDirectory(tempImageDir);

                // 随机选择并拷贝图片到临时目录
                var random = new Random();
                var selectedImages = Directory.GetFiles(imageFolderPath, "*.png")
                    .OrderBy(_ => random.Next())
                    .Take(15)
                    .ToList();

                // 按顺序重命名图片为img001.png格式
                for (int i = 0; i < selectedImages.Count; i++)
                {
                    string destPath = Path.Combine(tempImageDir, $"img{i + 1:000}.png");
                    File.Copy(selectedImages[i], destPath);
                }

                // 构建FFmpeg命令
                string ffmpegArgs = $"-framerate 1/3 " +       // 每3秒一帧
                                    $"-pattern_type sequence " +
                                    $"-i \"{tempImageDir}\\img%03d.png\" " +
                                    $"-i \"{audioFilePath}\" " +
                                    $"-t 45 " +                // 总时长45秒
                                    $"-map 0:v " +
                                    $"-map 1:a " +
                                    $"-c:v libx264 " +
                                    $"-vf \"fps=25,format=yuv420p\" " +
                                    $"-c:a aac " +
                                    $"-shortest " +
                                    $"-y " +                   // 覆盖输出文件
                                    $"\"{outputPath}\"";

                // 执行FFmpeg命令
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "ffmpeg";
                    process.StartInfo.Arguments = ffmpegArgs;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardError = true;

                    Console.WriteLine("开始生成视频...");
                    process.Start();
                    string errorOutput = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception($"FFmpeg错误：\n{errorOutput}");
                    }
                }

                Console.WriteLine($"视频已生成：{outputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成失败：{ex.Message}");
            }
            finally
            {
                // 清理临时文件（可选）
                // Directory.Delete(tempImageDir, true);
            }
        }
    }
}

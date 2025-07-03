using System.Diagnostics;
using System.Text;

namespace PictureToMovie
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 图片所在的文件夹的位置
            string imageFolderPath = @"D:\File\米哈喵呀\图片\鸣潮\秧秧";
            // 背景音乐mp3文件的位置
            string audioFilePath = @"D:\File\米哈喵呀\音乐\BackgroundMusic.mp3";
            // 视频输出文件夹的位置
            string outputFolderPath = @"D:\File\米哈喵呀\视频\尚未发布\";
            // 生成的视频数量
            int videoCount = 3;
            // 获取图片文件夹下所有png图片的文件路径
            string[] pngFiles = Directory.GetFiles(imageFolderPath, "*.png", SearchOption.TopDirectoryOnly);
            // ffmpeg使用的 临时txt文件的存放位置
            string tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");

            for (int i = 1; i <= videoCount; i++)
            {
                // 获取的图片数量
                int pictureCount = 10;

                // 正确组合输出路径
                string output = Path.Combine(outputFolderPath, $"video_{i}.mp4");

                // 清空temp.txt文件内容
                File.WriteAllText(tempFilePath, "");

                // 从 pngFiles 中任意取 pictureCount 张图片
                Random random = new Random();
                string[] selectedPngFiles = pngFiles.OrderBy(x => random.Next()).Take(pictureCount).ToArray();

                // 把 selectedPngFiles 写入 temp.txt
                using (StreamWriter writer = new StreamWriter(tempFilePath, false, Encoding.Default))
                {
                    for (int j = 0; j < selectedPngFiles.Length; j++)
                    {
                        string formattedPath = selectedPngFiles[j].Replace(@"\", "/");
                        writer.WriteLine($"file \'{formattedPath}\'");
                        if (j < selectedPngFiles.Length - 1)
                        {
                            writer.WriteLine("duration 3");
                        }
                    }
                }

                // 正确构建FFmpeg参数（不再包含"ffmpeg"命令名）
                string ffmpegArguments = $@"-y -f concat -safe 0 -i ""{tempFilePath}"" -i ""{audioFilePath}"" -vf ""scale=1080:1920:force_original_aspect_ratio=decrease,pad=1080:1920:(ow-iw)/2:(oh-ih)/2"" -fps_mode cfr -r 30 -c:v libx264 -preset medium -pix_fmt yuv420p -c:a aac -shortest ""{output}""";

                // 执行ffmpeg命令
                try
                {  
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "ffmpeg"; // 命令名单独设置
                        process.StartInfo.Arguments = ffmpegArguments; // 只包含参数
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardError = true;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.Start();

                        // 捕获输出和错误（用于调试）
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            throw new Exception($"FFmpeg执行失败");
                        }

                        Console.WriteLine($"成功生成视频: {output}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"生成视频时出错: {ex.Message}");
                    Console.WriteLine($"FFmpeg参数: ffmpeg {ffmpegArguments}"); // 输出完整命令用于调试
                }
            }
        }
    }
}
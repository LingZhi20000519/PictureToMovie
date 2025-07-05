using System.Diagnostics;
using System.Text;

namespace PictureToMovie
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 生成的视频数量
            int videoCount = 10;
            // 图片所在的文件夹的位置
            string imageFolderPath = @"D:\File\米哈喵呀\图片\明日方舟\阿米娅";
            // 背景音乐mp3文件的位置
            string audioFilePath = @"D:\File\米哈喵呀\音乐\BackgroundMusic.mp3";
            // 输出视频的文件夹的位置
            string outputFolderPath = @"D:\File\米哈喵呀\视频\尚未发布\";
            // 输出视频的前缀名
            string videoName = System.IO.Path.GetFileName(imageFolderPath);
            // 视频结尾固定求赞图片的位置
            string endPicturePath = @"D:\File\米哈喵呀\图片\其他\结尾.png";


            // 获取图片文件夹下所有png图片的文件路径
            string[] pngFiles = Directory.GetFiles(imageFolderPath, "*.png", SearchOption.TopDirectoryOnly);
            // ffmpeg使用的 临时txt文件的存放位置
            string tempFilePath = Path.Combine(Directory.GetCurrentDirectory(), "temp.txt");

            for (int i = 1; i <= videoCount; i++)
            {
                // 获取的图片数量
                int pictureCount = 10;

                // 正确组合输出路径
                string output = Path.Combine(outputFolderPath, $"{videoName}美图_{i}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.mp4");

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
                        writer.WriteLine("duration 3");

                    }
                    writer.WriteLine($"file \'{endPicturePath}\'");
                }

                // 正确构建FFmpeg参数（不再包含"ffmpeg"命令名）
                string ffmpegArguments = $@"-y -f concat -safe 0 -i ""{tempFilePath}"" -i ""{audioFilePath}"" -vf ""scale=1080:1920:force_original_aspect_ratio=decrease,pad=1080:1920:(ow-iw)/2:(oh-ih)/2"" -fps_mode cfr -r 30 -c:v libx264 -preset medium -pix_fmt yuv420p -c:a aac -shortest ""{output}""";

                // 执行ffmpeg命令
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = "ffmpeg";
                        process.StartInfo.Arguments = ffmpegArguments;
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.RedirectStandardError = true;

                        Console.WriteLine($"开始生成第{i}个视频");
                        process.Start();
                        string errorOutput = process.StandardError.ReadToEnd();
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            throw new Exception($"FFmpeg错误：\n{errorOutput}");
                        }
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
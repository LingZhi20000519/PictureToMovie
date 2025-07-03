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
            }
        }
    }
}

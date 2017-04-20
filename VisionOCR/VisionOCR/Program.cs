using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionOCR
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                var cognitiveService = new ImageToTextInterpreter
                {
                    ImageFilePath = @"C:\Vision\驾驶证2.jpg",
                    SubscriptionKey = "da1f92f0-4cd3-4dfc-8122-b1727a061cdb"
                };

                var results = await cognitiveService.ConvertImageToStreamAndExtractText();

                OutputToConsole(results);
            }).Wait();
        }
        private static void OutputToConsole(OcrResults results)
        {
            Console.WriteLine("Interpreted text:");
            Console.ForegroundColor = ConsoleColor.Yellow;

            foreach (var region in results.Regions)
            {
                foreach (var line in region.Lines)
                {
                    Console.WriteLine(string.Join(" ", line.Words.Select(w => w.Text)));
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
    public class ImageToTextInterpreter
    {
        public string ImageFilePath { get; set; }

        public string SubscriptionKey { get; set; }

        const string UNKNOWN_LANGUAGE = "unk";

        public async Task<OcrResults> ConvertImageToStreamAndExtractText()
        {
            var visionServiceClient = new VisionServiceClient(SubscriptionKey);
            //use imageMemoryStream.Seek(0, SeekOrigin.Begin); after copy stream from image
            using (Stream imageFileStream = File.OpenRead(ImageFilePath))
            {
                imageFileStream.Seek(0, SeekOrigin.Begin);
                return await visionServiceClient.RecognizeTextAsync(imageFileStream, UNKNOWN_LANGUAGE);
            }
        }
    }
}

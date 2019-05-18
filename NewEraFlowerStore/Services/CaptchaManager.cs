// csharp file that performs as a captcha manager

#region Using Directives
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

using NewEraFlowerStore.Areas.Identity.Data;
using NewEraFlowerStore.Extensions;
#endregion Using Directives

namespace NewEraFlowerStore.Services
{
    /// <summary>
    /// Extending from the class <see cref="ICaptchaManager"/>, the class <see cref="CaptchaManager"/> create/verify the captcha code.
    /// </summary>
    public class CaptchaManager : ICaptchaManager
    {
        private const string fileFormat = "image/jpeg";
        private static List<char> availableCharacters;
        private static readonly ConcurrentDictionary<string, DateTime> concurrentDictionary = new ConcurrentDictionary<string, DateTime>(); // use a thread-safe collection of key/value pairs that can be accessed by multiple threads concurrently to prevent violent crack

        public CaptchaManager()
        {
            availableCharacters = new List<char>();

            //digits 0 and 1 will not be used in captcha
            for (var character = '0'; character <= '9'; character++)
            {
                if (character == '0' || character == '1')
                    continue;

                availableCharacters.Add(character);
            } // end for

            // uppercase letters "I" and "O" will not be used in captcha
            for (var character = 'A'; character < 'Z'; character++)
            {
                if (character == 'I' || character == 'O')
                    continue;

                availableCharacters.Add(character);
            } // end for

            // lowercase letters "l" and "o" will not be used in captcha
            for (var character = 'a'; character < 'z'; character++)
            {
                if (character == 'l' || character == 'o')
                    continue;

                availableCharacters.Add(character);
            } // end for
        } // end constructor CaptchaManager

        /// <summary>
        /// Create the captcha code, as an asynchoronous operation.
        /// </summary>
        /// <param name="characterCount">the number of characters</param>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <returns>a <see cref="CaptchaInfo"/> object</returns>
        public async Task<CaptchaInfo> CreateAsync(int characterCount = 4, int width = 85, int height = 40)
        {
            var captchaInfo = new CaptchaInfo { FileFormat = fileFormat };
            var selectedCharacters = new char[characterCount];
            var length = availableCharacters.Count;
            var random = new Random();

            // create the captcha code
            for (var count = 0; count < characterCount; count++)
                selectedCharacters[count] = availableCharacters[random.Next(length)];

            var captcha = string.Join(string.Empty, selectedCharacters);
            var content = $"{captcha}|{DateTime.Now}";

            captchaInfo.Content = await content.EncryptAsync(); // encrypt the captcha info

            var fontNames = new List<string>
            {
                "Arial",
                "Calibri",
                "Comic Sans Ms",
                "Consolas",
                "Georgia",
                "Lucida Sans",
                "Segoe UI",
                "Tahoma",
                "Times New Roman",
                "Trebuchet MS",
                "Tw Cen Mt",
                "Verdana"
            };

            using (var bitmap = new Bitmap(width, height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    // use white as the background colour
                    graphics.Clear(Color.White);
                    AddInterference(random, bitmap, graphics, width / 2, height);

                    // add a silver border
                    var pen = new Pen(Color.Silver);

                    graphics.DrawRectangle(pen, 0, 0, width - 1, height - 1);

                    var x = 1;
                    const int y = 5;
                    var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    var color = Color.FromArgb(random.Next(100, 122), random.Next(100, 122), random.Next(100, 122));

                    foreach (var character in selectedCharacters)
                    {
                        var fontName = fontNames[random.Next(0, fontNames.Count - 1)];
                        var font = new Font(fontName, random.Next(15, 20));

                        using (var brush = new LinearGradientBrush(rectangle, color, color, 90f, true))
                        {
                            brush.SetSigmaBellShape(0.5f);
                            graphics.DrawString(character.ToString(), font, brush, x + random.Next(-2, 2), y + random.Next(-5, 5));
                            x += width / characterCount;
                        }
                    } // end foreach

                    using (var memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, ImageFormat.Jpeg);
                        captchaInfo.ImageData = memoryStream.ToArray();

                        return captchaInfo;
                    }
                }
            }
        } // end method CreateAsync

        /// <summary>
        /// Verify the captcha code, as an asynchoronous operation.
        /// </summary>
        /// <param name="captchaRequest">a <see cref="CaptchaRequest"/> object</param>
        /// <param name="timeOut">period before the timeout</param>
        /// <returns>a <see cref="CaptchaResponse"/> object</returns>
        public async Task<CaptchaResponse> VerifyAsync(CaptchaRequest captchaRequest, int timeOut = 120)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(captchaRequest.Answer))
                    return new CaptchaResponse { Code = 1, Message = "Please enter the captcha." };

                if (string.IsNullOrWhiteSpace(captchaRequest.CaptchaKey))
                    return new CaptchaResponse { Code = 4, Message = "Error! The captcha does not function properly due to something wrong with the cookie required. We recommend you to accept cookies from this site. If you have already done this and the problem remains, try clearing cookies and reloading the page." };

                // captcha can only be used to verify the user's answer once
                if (concurrentDictionary.ContainsKey(captchaRequest.CaptchaKey))
                    return new CaptchaResponse { Code = 3, Message = "Expired captcha." };

                // record the first call
                concurrentDictionary.TryAdd(captchaRequest.CaptchaKey, DateTime.Now);

                // clear expired data
                foreach (var keyValuePair in concurrentDictionary)
                {
                    var hours = (keyValuePair.Value - DateTime.Now).Hours;

                    if (hours > 12)
                        concurrentDictionary.TryRemove(keyValuePair.Key, out var requestDateTime);
                } // end foreach

                var captchaKey = await captchaRequest.CaptchaKey.DecryptAsync(); // DES decrypter
                var temp = captchaKey.Split('|'); //0: captcha, 1: creationDateTime
                var conversionResult = DateTime.TryParse(temp[1], out var creationDateTime);
                var seconds = (DateTime.Now - creationDateTime).TotalSeconds;

                if (!conversionResult || seconds > timeOut)
                    return new CaptchaResponse { Code = 3, Message = "Expired captcha." };

                if (string.Equals(temp[0], captchaRequest.Answer, StringComparison.OrdinalIgnoreCase))
                    return new CaptchaResponse { Code = 0, Message = "Succeeded." };

                return new CaptchaResponse { Code = 2, Message = "Wrong captcha." };
            }
            catch
            {
                return new CaptchaResponse { Code = -1, Message = "Cannot verify the captcha. You may click to change the captcha image." };
            } // end try...catch
        } // end method VerifyAsync

        /// <summary>
        /// Add interferencial curve and disturbance points to a captcha image.
        /// </summary>
        /// <param name="random">a <see cref="Random"/> object</param>
        /// <param name="bitmap">a <see cref="Bitmap"/> object</param>
        /// <param name="graphics">a <see cref="Graphics"/> object</param>
        /// <param name="lineCount">the number of lines</param>
        /// <param name="pointCount">the number of points</param>
        public static void AddInterference(Random random, Bitmap bitmap, Graphics graphics, int lineCount, int pointCount)
        {
            // add interferencial curve
            for (var count = 0; count < lineCount; count++)
            {
                var x1 = random.Next(bitmap.Width);
                var x2 = random.Next(bitmap.Width);
                var y1 = random.Next(bitmap.Height);
                var y2 = random.Next(bitmap.Height);
                var pen = new Pen(Color.FromArgb(random.Next()));

                graphics.DrawLine(pen, x1, y1, x2, y2);
            } // end for

            // add disturbance points
            for (var count = 0; count < pointCount; count++)
            {
                var x = random.Next(bitmap.Width);
                var y = random.Next(bitmap.Height);

                bitmap.SetPixel(x, y, Color.FromArgb(random.Next()));
            } // end for
        } // end method AddInterference
    } // end class CaptchaManager
} // end namespace NewEraFlowerStore.Services
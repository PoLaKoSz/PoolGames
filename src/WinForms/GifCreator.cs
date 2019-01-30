using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CSharpSnooker.WinForms
{
    public class GifCreator
    {
        private readonly static byte[] _appExtensionBuff;
        private readonly static byte[] _graphicExtensionBuff;



        static GifCreator()
        {
            _appExtensionBuff = new byte[]
            {
                 33,  // Extension introducer
                255,  // Application extension
                 11,  // Size of block
                 78,  // N
                 69,  // E
                 84,  // T
                 83,  // S
                 67,  // C
                 65,  // A
                 80,  // P
                 69,  // E
                 50,  // 2
                 46,  // .
                 48,  // 0
                  3,  // Size of block
                  1,  //
                  0,  //
                  0,  //
                  0,  // Block terminator
            };

            _graphicExtensionBuff = new byte[]
            {
                 33,  // Extension introducer
                249,  // Graphic control extension
                  4,  // Size of block
                  9,  // Flags: reserved, disposal method, user input, transparent color
                  2,  // Delay time low byte
                  0,  // Delay time high byte
                255,  // Transparent color index
                  0,  // Block terminator
            };
        }



        public void FromFolder(string folderPath, string fileName)
        {
            var streamWriter = new StreamWriter(fileName + ".gif");
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(streamWriter.BaseStream))
                {
                    string[] filePaths = Directory.GetFiles(folderPath, "*.jpg");

                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        Image image = Image.FromFile(filePaths[i]);

                        Create(i, image, memoryStream, binaryWriter);
                    }

                    binaryWriter.Write(";");
                }
            }
        }

        public void FromImage(List<Image> images, string fileName)
        {
            FromImage(images, images.Count, fileName);
        }

        /// <summary>
        /// Creates a gif from only a fixed amount of pictures.
        /// </summary>
        /// <param name="images">Source for the gif. Non null.</param>
        /// <param name="endIndex">The last picture index in the collection.</param>
        /// <param name="fileName">Absolute or relative file name without the extension.</param>
        public void FromImage(List<Image> images, int endIndex, string fileName)
        {
            StreamWriter streamWriter = new StreamWriter(fileName + ".gif");
            using (var memoryStream = new MemoryStream())
            {
                using (var binaryWriter = new BinaryWriter(streamWriter.BaseStream))
                {
                    for (int i = 0; i < endIndex; i++)
                    {
                        Create(i, images[i], memoryStream, binaryWriter);
                    }

                    binaryWriter.Write(";");
                }
            }
        }


        private void Create(int i, Image image, MemoryStream memoryStream, BinaryWriter binaryWriter)
        {
            image.Save(memoryStream, ImageFormat.Gif);
            byte[] headerBuff = memoryStream.ToArray();

            if (i == 0)
            {
                binaryWriter.Write(headerBuff, 0, 781);
                binaryWriter.Write(_appExtensionBuff, 0, _appExtensionBuff.Length);
            }

            binaryWriter.Write(_graphicExtensionBuff, 0, _graphicExtensionBuff.Length);
            binaryWriter.Write(headerBuff, 789, headerBuff.Length - 790);

            memoryStream.SetLength(0);
        }
    }
}

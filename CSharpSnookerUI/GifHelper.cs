using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace CSharpSnookerUI
{
    public class GifHelper
    {
        public GifHelper() { }

        public static void GenerateGif()
        {
            //Variable declaration
            //StringCollection stringCollection;
            MemoryStream memoryStream;
            BinaryWriter binaryWriter;
            Image image;
            Byte[] buf1;
            Byte[] buf2;
            Byte[] buf3;
            //Variable declaration

            string[] stringCollection = Directory.GetFiles(@"Snapshots", "*.jpg");

            //Response.ContentType = "Image/gif";
            StreamWriter sw = new StreamWriter(@"Snapshots\Snapshots.gif");
            memoryStream = new MemoryStream();
            buf2 = new Byte[19];
            buf3 = new Byte[8];
            buf2[0] = 33;  //extension introducer
            buf2[1] = 255; //application extension
            buf2[2] = 11;  //size of block
            buf2[3] = 78;  //N
            buf2[4] = 69;  //E
            buf2[5] = 84;  //T
            buf2[6] = 83;  //S
            buf2[7] = 67;  //C
            buf2[8] = 65;  //A
            buf2[9] = 80;  //P
            buf2[10] = 69; //E
            buf2[11] = 50; //2
            buf2[12] = 46; //.
            buf2[13] = 48; //0
            buf2[14] = 3;  //Size of block
            buf2[15] = 1;  //
            buf2[16] = 0;  //
            buf2[17] = 0;  //
            buf2[18] = 0;  //Block terminator
            buf3[0] = 33;  //Extension introducer
            buf3[1] = 249; //Graphic control extension
            buf3[2] = 4;   //Size of block
            buf3[3] = 9;   //Flags: reserved, disposal method, user input, transparent color
            buf3[4] = 2;  //Delay time low byte
            buf3[5] = 0;   //Delay time high byte
            buf3[6] = 255; //Transparent color index
            buf3[7] = 0;   //Block terminator
            binaryWriter = new BinaryWriter(sw.BaseStream);
            for (int picCount = 0; picCount < stringCollection.Length; picCount++)
            {
                image = Bitmap.FromFile(stringCollection[picCount]);
                image.Save(memoryStream, ImageFormat.Gif);
                buf1 = memoryStream.ToArray();

                if (picCount == 0)
                {
                    //only write these the first time....
                    binaryWriter.Write(buf1, 0, 781); //Header & global color table
                    binaryWriter.Write(buf2, 0, 19); //Application extension
                }

                binaryWriter.Write(buf3, 0, 8); //Graphic extension
                binaryWriter.Write(buf1, 789, buf1.Length - 790); //Image data

                if (picCount == stringCollection.Length - 1)
                {
                    //only write this one the last time....
                    binaryWriter.Write(";"); //Image terminator
                }

                memoryStream.SetLength(0);
            }
            binaryWriter.Close();
            //Response.End();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace SwordAndStone.ClientNative
{
    public interface IScreenshot
    {
        void SaveScreenshot();
    }
    public class Screenshot : IScreenshot
    {
        public GameWindow d_GameWindow;
        public string SavePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        public void SaveScreenshot()
        {
            using (Bitmap bmp = GrabScreenshot())
            {
                string time = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string filename = Path.Combine(SavePath, time + ".png");
                bmp.Save(filename);
            }
        }
        // Returns a System.Drawing.Bitmap with the contents of the current framebuffer
        public Bitmap GrabScreenshot()
        {
            Bitmap bmp = new Bitmap(d_GameWindow.ClientSize.X, d_GameWindow.ClientSize.Y);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(new Rectangle(0, 0, d_GameWindow.ClientSize.X, d_GameWindow.ClientSize.Y), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, d_GameWindow.ClientSize.X, d_GameWindow.ClientSize.Y, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
    }
}

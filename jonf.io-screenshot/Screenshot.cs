using System.Drawing;
using System.Drawing.Imaging;    
using System.Windows.Forms;      

namespace jonf.io_screenshot
{
    class Screenshot
    {
        Screen[] _Screens;
        public Screenshot(Screen[] screens)
        {
            _Screens = screens;
        }


        public Bitmap TakeScreenhot(int screenIndex)
        {

            Bitmap _Screenshot = new Bitmap(_Screens[screenIndex].Bounds.Width,
                                  _Screens[screenIndex].Bounds.Height,
                                  PixelFormat.Format32bppArgb);
                                                           
            Graphics _Gfx = Graphics.FromImage(_Screenshot);

            
            _Gfx.CopyFromScreen(_Screens[screenIndex].Bounds.X,
                                        _Screens[screenIndex].Bounds.Y,
                                        0,
                                        0,
                                        _Screens[screenIndex].Bounds.Size,
                                        CopyPixelOperation.SourceCopy);
            return _Screenshot;
        }


        public void SaveScreenshot(string name, Bitmap screenshot)
        {
            screenshot.Save(name + ".bmp");  
        }
    }
}

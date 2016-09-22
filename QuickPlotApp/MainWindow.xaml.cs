using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace QuickPlotApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Init();
        }

        public void Init()
        {
            //this.browser.NavigateToString(QuickPlot.Scatter.sample());
            var html = QuickPlot.Scatter.drawFromCsv(QuickPlot.Utilities.csvReaderConfig.create("../../sample.csv"));
            this.browser.NavigateToString(html);
        }

        private void ScreenShot()
        {
            var imgScreen = new Image
            {
                Width = (int)browser.ActualWidth,
                Height = (int)browser.ActualHeight,
                Source = new DrawingImage(VisualTreeHelper.GetDrawing(browser))
            };
            //var tmpFileName = System.IO.Path.GetTempFileName();
            var tmpFileName = "../../temp.bmp";
            using (var fs = new FileStream(tmpFileName, FileMode.Create))
            {
                var visual = new DrawingVisual();
                var content = visual.RenderOpen();
                content.DrawImage(
                    imgScreen.Source,
                    new Rect(new Size(imgScreen.Width, imgScreen.Height))
                );
                content.Close();

                var rtb = new RenderTargetBitmap(
                    (int)imgScreen.Width,
                    (int)imgScreen.Height,
                    96d, 96d, PixelFormats.Default
                );
                rtb.Render(visual);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(fs);

                BitmapSource bmp = new WriteableBitmap(encoder.Frames[0]);
                Clipboard.SetImage(bmp);
                bmp.Freeze();
            }
        } 

        private void browser_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.C:
                    //if((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.None)
                    //{
                        // Ctrl+C
                        ScreenShot();
                    //}
                    break;
                default:
                    break;
            }
        }
    }
}

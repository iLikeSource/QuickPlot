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
using System.Collections.ObjectModel;

namespace QuickPlotApp
{

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DataSources { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this.DataSources;

            Init();
        }

        public void Init()
        {
            //var model = QuickPlot.Model.t.fromFile("../../sample.csv");
            //var html = model.draw();
            //this.browser.NavigateToString(html);
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

        /// <summary>
        /// ドロップ処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Drop(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if ( files != null ) {
                if(files.Length > 0 ) {
                    // 現状では最初のファイルのみ
                    var model = QuickPlot.Model.t.fromFile(files[0]);
                    var html = model.draw();
                    this.textBox.Text = QuickPlot.DataSources.DataSources.toJson(model.dataSources);
                    this.browser.NavigateToString(html);
                }
            }
        }

        /// <summary>
        /// ドラッグ中の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewDragOver(object sender, DragEventArgs e)
        {
            if ( e.Data.GetDataPresent(DataFormats.FileDrop, true) ) {
                e.Effects = DragDropEffects.Copy;
            } else {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }
    }
}

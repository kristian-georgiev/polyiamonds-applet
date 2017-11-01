using Svg2Xaml;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Polyiamonds_Applet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowLogic : Window
    {
        private string _selectedShapeNameToSpawn;
        private object _movingObject;
        private double _firstXPos, _firstYPos;

        public MainWindowLogic()
        {
            InitializeComponent();
            ImageBrush ib = new ImageBrush();
            string dir = System.IO.Path.GetDirectoryName((System.IO.Directory.GetCurrentDirectory()));
            ib.ImageSource = new BitmapImage(new Uri(dir + @"\images\triangular_grid_large.png"));
            drawingBoard.Background = ib;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SelectShape(object sender, RoutedEventArgs e)
        {
            int tileType = int.Parse(((Button)sender).Tag.ToString());
            _selectedShapeNameToSpawn = GetImageToSpawn(tileType);
        }

        private void MouseClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            string result = null;
            if (_selectedShapeNameToSpawn == null || _movingObject != null)
            {
                return;
            }

            string dir = System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            Image bodyImage = new Image
            {
                Source = new BitmapImage(new Uri(dir + @"\images\" + _selectedShapeNameToSpawn))
            };

            bodyImage.AllowDrop = true;
            bodyImage.PreviewMouseLeftButtonDown += this.CanvasObjectLeftClick;
            bodyImage.PreviewMouseMove += this.CanvasObjectMouseMove;
            bodyImage.PreviewMouseLeftButtonUp += this.PreviewCanvasObjectLeftButtonUp;
            bodyImage.MouseRightButtonDown += this.CanvasObjectRightClick;

            Point mousePosition = Mouse.GetPosition(drawingBoard);
            double xTest = (mousePosition.X - bodyImage.Source.Width / 2);
            double yTest = (mousePosition.Y - bodyImage.Source.Height / 2);

            double XRound = Math.Round(xTest / 22.47);
            double YRound = Math.Round(yTest / 19.47);
            if (result == "tile_slim_1.png" || result == "tile_slim_4.png" || result == "line_2.png")
                Canvas.SetLeft(bodyImage, 22.47 * XRound - 2 + ((YRound + 1) % 2) * 11.235);
            else if (result == "dot_white.png" || result == "dot_black.png")
                Canvas.SetLeft(bodyImage, 22.47 * XRound - 9 + ((YRound) % 2) * 11.235);
            else
                Canvas.SetLeft(bodyImage, 22.47 * XRound - 2 + ((YRound) % 2) * 11.235);
            if (result == "dot_white.png" || result == "dot_black.png")
                Canvas.SetTop(bodyImage, 19.47 * YRound - 8);
            else
                Canvas.SetTop(bodyImage, 19.47 * YRound - 2);

            drawingBoard.Children.Add(bodyImage);
        }

        private void CanvasObjectLeftClick(object sender, MouseButtonEventArgs e)
        {
            // In this event, we get the current mouse position on the control to use it in the MouseMove event.
            Image img = sender as Image;
            Canvas canvas = img.Parent as Canvas;

            _firstXPos = e.GetPosition(img).X;
            _firstYPos = e.GetPosition(img).Y;

            _movingObject = sender;


            // Put the image currently being dragged on top of the others
            int top = Canvas.GetZIndex(img);
            foreach (Image child in canvas.Children)
                if (top < Canvas.GetZIndex(child))
                    top = Canvas.GetZIndex(child);
            Canvas.SetZIndex(img, top + 1);


        }

        private void PreviewCanvasObjectLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string result = null;

            Image img = sender as Image;
            Canvas canvas = img.Parent as Canvas;

            Point mousePosition = Mouse.GetPosition(drawingBoard);
            double xTest = (mousePosition.X - img.Source.Width / 2);
            double yTest = (mousePosition.Y - img.Source.Height / 2);

            double XRound = Math.Round(xTest / 22.47);
            double YRound = Math.Round(yTest / 19.47);
            if (result == "tile_slim_1.png" || result == "tile_slim_4.png" || result == "line_2.png")
                Canvas.SetLeft(img, 22.47 * XRound - 2 + ((YRound + 1) % 2) * 11.235);
            else if (result == "dot_white.png" || result == "dot_black.png")
                Canvas.SetLeft(img, 22.47 * XRound - 9 + ((YRound) % 2) * 11.235);
            else
                Canvas.SetLeft(img, 22.47 * XRound - 2 + ((YRound) % 2) * 11.235);
            if (result == "dot_white.png" || result == "dot_black.png")
                Canvas.SetTop(img, 19.47 * YRound - 8);
            else
                Canvas.SetTop(img, 19.47 * YRound - 2);

            _movingObject = null;

            // Put the image currently being dragged on top of the others
            int top = Canvas.GetZIndex(img);
            foreach (Image child in canvas.Children)
                if (top > Canvas.GetZIndex(child))
                    top = Canvas.GetZIndex(child);
            Canvas.SetZIndex(img, top + 1);



        }

        private void CanvasObjectRightClick(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            drawingBoard.Children.Remove(img);
        }

        private void CanvasObjectMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && sender == _movingObject)
            {
                Image img = sender as Image;
                Canvas canvas = img.Parent as Canvas;

                double newLeft = e.GetPosition(canvas).X - _firstXPos - canvas.Margin.Left;
                // newLeft inside canvas right-border?
                if (newLeft > canvas.Margin.Left + canvas.ActualWidth - img.ActualWidth)
                    newLeft = canvas.Margin.Left + canvas.ActualWidth - img.ActualWidth;
                // newLeft inside canvas left-border?
                else if (newLeft < canvas.Margin.Left)
                    newLeft = canvas.Margin.Left;
                img.SetValue(Canvas.LeftProperty, newLeft);
                double newTop = e.GetPosition(canvas).Y - _firstYPos - canvas.Margin.Top;
                // newTop inside canvas bottom-border?
                if (newTop > canvas.Margin.Top + canvas.ActualHeight - img.ActualHeight)
                    newTop = canvas.Margin.Top + canvas.ActualHeight - img.ActualHeight;
                // newTop inside canvas top-border?
                else if (newTop < canvas.Margin.Top)
                    newTop = canvas.Margin.Top;
                img.SetValue(Canvas.TopProperty, newTop);
            }
        }

        private void CanvasReset(object sender, RoutedEventArgs e)
        {
            drawingBoard.Children.RemoveRange(0, drawingBoard.Children.Count);
        }

        private void CanvasUndo(object sender, RoutedEventArgs e)
        {
            if (drawingBoard.Children.Count > 0)
            {
                drawingBoard.Children.RemoveAt(drawingBoard.Children.Count - 1);
            }
        }

        private void CanvasPrint(object sender, RoutedEventArgs e)
        {
            PrintDialog prnt = new PrintDialog();
            if (prnt.ShowDialog() == true)
            {
                //Size pageSize = new Size(850, 800);
                //drawingBoard.Measure(pageSize);
                //drawingBoard.Arrange(new Rect(0, 0, pageSize.Width, pageSize.Height));
                //if (prnt.ShowDialog() == true)
                prnt.PrintVisual(drawingBoard, "Printing Canvas");

            }
        }



        private string GetImageToSpawn(int tileType)
        {
            string result3 = null;
            switch (tileType)
            {
                case 1:
                    result3 = "tile_slim_1.png";
                    break;

                case 2:
                    result3 = "tile_slim_2.png";
                    break;

                case 3:
                    result3 = "tile_slim_3.png";
                    break;

                case 4:
                    result3 = "tile_slim_4.png";
                    break;

                case 5:
                    result3 = "tile_slim_5.png";
                    break;

                case 6:
                    result3 = "tile_slim_6.png";
                    break;

                case 7:
                    result3 = "line_1.png";
                    break;

                case 8:
                    result3 = "line_2.png";
                    break;

                case 9:
                    result3 = "line_3.png";
                    break;

                case 10:
                    result3 = "dot_white.png";
                    break;

                case 11:
                    result3 = "dot_black.png";
                    break;
            }

            return result3;
        }

    }

}

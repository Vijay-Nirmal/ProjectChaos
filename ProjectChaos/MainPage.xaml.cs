using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ProjectChaos
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Random random = new Random();
        private Timer timer;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void CreateGrid((int NoOfSplit, int MaxValue, int CellSize) gridProp)
        {
            for (int i = 0; i <= gridProp.NoOfSplit; i++)
            {
                var yLine = new Line();
                yLine.Stroke = new SolidColorBrush(Colors.LightGray);
                yLine.X1 = 0;
                yLine.X2 = gridProp.CellSize * gridProp.NoOfSplit;
                yLine.Y1 = gridProp.CellSize * i;
                yLine.Y2 = gridProp.CellSize * i;
                GraphLinesGrid.Children.Add(yLine);
                var yLable = new TextBlock();
                yLable.Text = (((double)i / gridProp.NoOfSplit) * gridProp.MaxValue).ToString();
                Canvas.SetLeft(yLable, 10);
                Canvas.SetTop(yLable, gridProp.CellSize * i - 15);
                YLableCanvas.Children.Add(yLable);
            }

            for (int i = 0; i <= gridProp.NoOfSplit; i++)
            {
                var xLine = new Line();
                xLine.Stroke = new SolidColorBrush(Colors.LightGray);
                xLine.X1 = gridProp.CellSize * i;
                xLine.X2 = gridProp.CellSize * i;
                xLine.Y1 = 0;
                xLine.Y2 = gridProp.CellSize * gridProp.NoOfSplit;
                GraphLinesGrid.Children.Add(xLine);
                var xLable = new TextBlock();
                xLable.Text = (((double)i / gridProp.NoOfSplit) * gridProp.MaxValue).ToString();
                Canvas.SetLeft(xLable, gridProp.CellSize * i - 15);
                Canvas.SetTop(xLable, 0);
                XLableCanvas.Children.Add(xLable);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var ballColors = new List<SolidColorBrush>()
            {
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Blue)
            };
            var gridProp = (NoOfSplit: 10, MaxValue: 100, CellSize: 50);
            var ballProp = (NoOfBallsInColor: 50, Colors: ballColors, Speed: 5, Size: 1);
            var makeTurnParm = (NoOfSplit: gridProp.NoOfSplit, CellSize: gridProp.CellSize, Speed: ballProp.Speed);
            CreateGrid(gridProp);
            CreateBalls(ballProp, gridProp);
            parm = (NoOfSplit: gridProp.NoOfSplit, CellSize: gridProp.CellSize, Speed: ballProp.Speed);
            timer?.Dispose();
            timer = new Timer(MakeNextTurnAsync, makeTurnParm, 50, 50);
        }

        (int NoOfSplit, int CellSize, int Speed) parm = (NoOfSplit: 10, CellSize: 50, Speed: 10);
        List<Ellipse> balls = new List<Ellipse>();

        private async void MakeNextTurnAsync(object state)
        {
            if (balls.Count == 0)
            {
                return;
            }
            foreach (var child in balls.ToList())
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    var direction = random.NextDouble() * 360;
                    direction = Math.PI * direction / 180.0; // Degree to radian 
                    var ballTranslate = child.RenderTransform.TransformPoint(new Point(0, 0));
                    var x = ballTranslate.X;
                    var y = ballTranslate.Y;
                    var x1 = x + (parm.Speed * Math.Cos(direction));
                    var y1 = y + (parm.Speed * Math.Sin(direction));

                    while (x1 < 0 || x1 > (parm.CellSize * parm.NoOfSplit) || y1 < 0 || y1 > (parm.CellSize * parm.NoOfSplit))
                    {
                        direction = random.NextDouble() * 360;
                        direction = Math.PI * direction / 180.0; // Degree to radian 
                        x1 = x + (parm.Speed * Math.Cos(direction));
                        y1 = y + (parm.Speed * Math.Sin(direction));
                    }


                    TranslateTransform myTranslate = new TranslateTransform();
                    myTranslate.X = x1;
                    myTranslate.Y = y1;
                    child.RenderTransform = myTranslate;
                });
            }
        }

        private void CreateBalls((int NoOfBallsInColor, List<SolidColorBrush> Colors, int Speed, int Size) ballProp, (int NoOfSplit, int MaxValue, int CellSize) gridProp)
        {
            foreach (var ballColor in ballProp.Colors)
            {
                for (int i = 0; i < ballProp.NoOfBallsInColor; i++)
                {
                    var x = random.Next(gridProp.CellSize * gridProp.NoOfSplit);
                    var y = random.Next(gridProp.CellSize * gridProp.NoOfSplit);
                    var ballSize = ballProp.Size * gridProp.CellSize * gridProp.NoOfSplit / gridProp.MaxValue;
                    var ball = new Ellipse()
                    {
                        Width = ballSize,
                        Height = ballSize,
                        Fill = ballColor
                    };
                    TranslateTransform myTranslate = new TranslateTransform();
                    myTranslate.X = x;
                    myTranslate.Y = y;
                    ball.RenderTransform = myTranslate;
                    balls.Add(ball);
                    BallsCanvas.Children.Add(ball);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            timer?.Dispose();

            foreach (var child in balls)
            {
                BallsCanvas.Children.Remove(child);
            }
            balls.Clear();
            
            var ballColors = new List<SolidColorBrush>()
            {
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Blue)
            };
            var gridProp = (NoOfSplit: 10, MaxValue: 100, CellSize: 50);
            var ballProp = (NoOfBallsInColor: 50, Colors: ballColors, Speed: 5, Size: 1);
            var makeTurnParm = (NoOfSplit: gridProp.NoOfSplit, CellSize: gridProp.CellSize, Speed: ballProp.Speed);
            CreateBalls(ballProp, gridProp);
            parm = (NoOfSplit: gridProp.NoOfSplit, CellSize: gridProp.CellSize, Speed: ballProp.Speed);
            timer = new Timer(MakeNextTurnAsync, makeTurnParm, 50, 50);
        }
    }
}

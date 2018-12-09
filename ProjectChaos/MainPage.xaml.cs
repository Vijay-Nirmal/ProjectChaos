using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
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
        GridProperty gridProp;
        BallProperty ballProp;
        List<InfluenceProperty> influenceProp = new List<InfluenceProperty>();
        ObservableCollection<SolidColorBrush> ballColorsItemSource = new ObservableCollection<SolidColorBrush>();
        private Timer timer;

        public MainPage()
        {
            this.InitializeComponent();

            AssignInitialValues();
        }

        private void AssignInitialValues()
        {
            ballColorsItemSource.Clear();
            ballColorsItemSource.Add(new SolidColorBrush(Colors.Red));
            ballColorsItemSource.Add(new SolidColorBrush(Colors.Blue));

            gridProp = new GridProperty(noOfSplit: 10, maxValue: 100, cellSize: 50, refreshSpeed: 50);
            ballProp = new BallProperty(noOfBallsInColor: 50, colors: ballColorsItemSource.ToList(), speed: 5, size: 1);
            influenceProp.Add(new InfluenceProperty(ballColor: new SolidColorBrush(Colors.Red), coordinate: new Point(50, 50), strength: 250));

            NoOfSplitTextBox.Text = "10";
            MaxvalueTextBox.Text = "100";
            CellSizeTextBox.Text = "50";
            RefreshSpeedTextBox.Text = "50";
            NoOfBallsInColorTextBox.Text = "50";
            SpeedTextBox.Text = "5";
            SizeTextBox.Text = "1";
        }

        private void CreateGrid()
        {
            RunInUIThread(() =>
            {
                GraphLinesGrid.Children.Clear();
                XLableCanvas.Children.Clear();
                YLableCanvas.Children.Clear();

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
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateGrid();
            CreateBalls();
            StartTimer();
        }

        private void MakeNextTurn(object state)
        {
            RunInUIThread(() =>
            {
                if (BallsCanvas.Children.Count == 0)
                {
                    return;
                }

                foreach (var child in BallsCanvas.Children.ToList())
                {
                    var ballTranslate = child.RenderTransform.TransformPoint(new Point(0, 0));
                    var x = ballTranslate.X;
                    var y = ballTranslate.Y;

                    var direction = random.NextDouble() * 360;
                    direction = Math.PI * direction / 180.0; // Degree to radian 
                    var x1 = x + (ballProp.Speed * Math.Cos(direction));
                    var y1 = y + (ballProp.Speed * Math.Sin(direction));

                    while (x1 < 0 || x1 > (gridProp.CellSize * gridProp.NoOfSplit) || y1 < 0 || y1 > (gridProp.CellSize * gridProp.NoOfSplit))
                    {
                        direction = random.NextDouble() * 360;
                        direction = Math.PI * direction / 180.0; // Degree to radian 
                        x1 = x + (ballProp.Speed * Math.Cos(direction));
                        y1 = y + (ballProp.Speed * Math.Sin(direction));
                    }

                    foreach (var influencePoint in influenceProp.Where(parm => parm.BallColor.Color == ((child as Ellipse).Fill as SolidColorBrush).Color))
                    {
                        var distance = Distance(x, y, influencePoint.Coordinate.X, influencePoint.Coordinate.Y);

                        
                    }

                    TranslateTransform myTranslate = new TranslateTransform();
                    myTranslate.X = x1;
                    myTranslate.Y = y1;
                    child.RenderTransform = myTranslate;
                }
            });
        }

        private void CreateBalls()
        {
            RunInUIThread(() =>
            {
                BallsCanvas.Children.Clear();

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
                        BallsCanvas.Children.Add(ball);
                    }
                }
            });
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            gridProp.NoOfSplit = int.Parse(NoOfSplitTextBox.Text);
            gridProp.CellSize = int.Parse(CellSizeTextBox.Text);
            gridProp.MaxValue = int.Parse(MaxvalueTextBox.Text);
            gridProp.RefreshSpeed = int.Parse(RefreshSpeedTextBox.Text);
            ballProp.Colors = ballColorsItemSource.ToList();
            ballProp.NoOfBallsInColor = int.Parse(NoOfBallsInColorTextBox.Text);
            ballProp.Size = int.Parse(SizeTextBox.Text);
            ballProp.Speed = int.Parse(SpeedTextBox.Text);

            CreateGrid();
            CreateBalls();
        }

        private void StartTimer()
        {
            timer?.Dispose();
            timer = new Timer(MakeNextTurn, null, 50, gridProp.RefreshSpeed);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Dispose();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            timer?.Dispose();

            AssignInitialValues();
            CreateGrid();
            CreateBalls();
            StartTimer();
        }

        private static async void RunInUIThread(Action action)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }

        private void RemnoveButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ballColorsItemSource.RemoveAt(ColorListView.SelectedIndex);
        }

        private void confirmColor_Click(object sender, RoutedEventArgs e)
        {
            ballColorsItemSource.Add(new SolidColorBrush(BallColorPicker.Color));
            BallColorAddButton.Flyout.Hide();
        }

        private void cancelColor_Click(object sender, RoutedEventArgs e)
        {
            BallColorAddButton.Flyout.Hide();
        }

        public double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        }
    }
}

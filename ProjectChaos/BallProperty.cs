using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace ProjectChaos
{
    public class BallProperty
    {
        public int NoOfBallsInColor { get; set; }
        public List<SolidColorBrush> Colors { get; set; }
        public int Speed { get; set; }
        public int Size { get; set; }

        public BallProperty(int noOfBallsInColor, List<SolidColorBrush> colors, int speed, int size)
        {
            this.NoOfBallsInColor = noOfBallsInColor;
            this.Colors = colors;
            this.Speed = speed;
            this.Size = size;
        }
    }
}
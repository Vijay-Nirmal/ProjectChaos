using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace ProjectChaos
{
    internal class InfluenceProperty
    {
        public SolidColorBrush BallColor { get; set; }
        public Point Coordinate { get; set; }
        public int Strength { get; set; }

        public InfluenceProperty(SolidColorBrush ballColor, Point coordinate, int strength)
        {
            this.BallColor = ballColor;
            this.Coordinate = coordinate;
            this.Strength = strength;
        }
    }
}
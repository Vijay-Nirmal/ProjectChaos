namespace ProjectChaos
{
    internal class GridProperty
    {
        public int NoOfSplit { get; set; }
        public int MaxValue { get; set; }
        public int CellSize { get; set; }
        public int RefreshSpeed { get; set; }

        public GridProperty(int noOfSplit, int maxValue, int cellSize, int refreshSpeed)
        {
            this.NoOfSplit = noOfSplit;
            this.MaxValue = maxValue;
            this.CellSize = cellSize;
            this.RefreshSpeed = refreshSpeed;
        }
    }
}
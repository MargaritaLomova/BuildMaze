public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }

    public bool LeftWall { get; set; }
    public bool BottomWall { get; set; }

    public bool Visited { get; set; }
    public bool Finish { get; set; }

    public int DistanceFromStart { get; set; }
}
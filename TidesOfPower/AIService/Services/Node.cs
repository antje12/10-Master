namespace AIService.Services;

public class Node
{
    public int X;
    public int Y;
    public Node? Parent;
    public int Depth;
    public int Cost;
    public double F;


    public Node(
        int x, int y, 
        Node? parent = null, 
        int depth = 0, int cost = 0, double f = 0)
    {
        X = x;
        Y = y;
        Parent = parent;
        Depth = depth;
        Cost = cost;
        F = f;
    }

    public string Key()
    {
        return $"{X}:{Y}";
    }

    public List<Node> Path()
    {
        var current = this;
        var path = new List<Node> {this};
        while (current.Parent != null)
        {
            current = current.Parent;
            path.Add(current);
        }

        return path;
    }

    public override bool Equals(object? obj)
    {
        return obj is Node other && X == other.X && Y == other.Y;
    }
}
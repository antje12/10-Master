namespace AIService.Services;

public class Node
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Node? Parent { get; set; }
    
    public int Depth { get; set; }
    public int Cost { get; set; }
    public double F { get; set; }
    

    public Node(int x, int y, Node? parent = null, int depth = 0, int cost = 0, double f = 0) {
        X = x;
        Y = y;
        Parent = parent;
        Depth = depth;
        Cost = cost;
        F = f;
    }

    public string Key() {
        return $"{X}:{Y}";
    }

    public List<Node> Path() {
        var current = this;
        var path = new List<Node> { this };
        while (current.Parent != null) {
            current = current.Parent;
            path.Add(current);
        }
        path.Reverse(); // Since the path is constructed backwards, we reverse it
        return path;
    }

    public override bool Equals(object obj) {
        return obj is Node other && X == other.X && Y == other.Y;
    }
}
using ClassLibrary.Domain;
using ClassLibrary.GameLogic;

namespace AIService.Services;

public class AStar
{
    public static Node Search(Node agent, Node target, List<Node> obstacles)
    {
        obstacles.Remove(target);
        
        var fringe = new Dictionary<string, Node>();
        var visited = new Dictionary<string, Node>();

        fringe[agent.Key()] = agent;

        while (fringe.Count > 0)
        {
            var node = GetCheapestNode(fringe);
            fringe.Remove(node.Key());
            visited[node.Key()] = node;

            if (NodeCollision(node, target))
            {
                var path = node.Path();
                if (path.Count == 1)
                    return SurvivalSearch(agent, obstacles);
                return path[path.Count - 2];
            }

            var children = ExpandNode(node, target, obstacles, fringe, visited);
            foreach (var child in children)
            {
                fringe[child.Key()] = child;
            }
        }

        // No path to goal found!
        return SurvivalSearch(agent, obstacles);
    }

    private static List<Node> ExpandNode(
        Node node, Node target, List<Node> obstacles,
        Dictionary<string, Node> fringe, Dictionary<string, Node> visited)
    {
        var successors = new List<Node>();
        var children = GetChildren(node, obstacles);

        foreach (var child in children)
        {
            if (visited.ContainsKey(child.Key()))
                continue;

            child.Parent = node;
            child.Depth = node.Depth + 1;
            child.Cost = node.Cost + 1;
            child.F = F(child, target);

            if (fringe.ContainsKey(child.Key()) && child.Cost > fringe[child.Key()].Cost)
                continue;

            successors.Add(child);
        }

        return successors;
    }

    private static List<Node> GetChildren(Node node, List<Node> obstacles)
    {
        var children = new List<Node>();
        AddIfValid(new Node(node.X, node.Y + 10), children, obstacles); // go north
        AddIfValid(new Node(node.X, node.Y - 10), children, obstacles); // go south
        AddIfValid(new Node(node.X + 10, node.Y), children, obstacles); // go east
        AddIfValid(new Node(node.X - 10, node.Y), children, obstacles); // go west
        AddIfValid(new Node(node.X + 10, node.Y + 10), children, obstacles); // go north east
        AddIfValid(new Node(node.X - 10, node.Y + 10), children, obstacles); // go north west
        AddIfValid(new Node(node.X + 10, node.Y - 10), children, obstacles); // go south east
        AddIfValid(new Node(node.X - 10, node.Y - 10), children, obstacles); // go south west
        return children;
    }

    private static void AddIfValid(Node child, List<Node> children, List<Node> obstacles)
    {
        foreach (var obstacle in obstacles)
        {
            if (NodeCollision(child, obstacle))
            {
                return;
            }
        }
        children.Add(child);
    }

    private static bool NodeCollision(Node n1, Node n2)
    {
        return Collide.Circle(
            n1.X, n1.Y, Agent.TypeRadius,
            n2.X, n2.Y, Agent.TypeRadius);
    }

    private static Node GetCheapestNode(Dictionary<string, Node> fringe)
    {
        return fringe.MinBy(x => x.Value.F).Value;
    }

    private static double F(Node node, Node target)
    {
        // Weighted A*
        var weight = 5;
        return G(node) + weight * H(node, target);
    }

    private static int G(Node n)
    {
        // Travel cost
        return n.Cost;
    }

    private static double H(Node node, Node target)
    {
        // Heuristic cost calculation
        return H(node.X, node.Y, target.X, target.Y);
    }

    public static double H(int nodeX, int nodeY, int targetX, int targetY)
    {
        // Heuristic cost calculation
        var dx = nodeX - targetX;
        var dy = nodeY - targetY;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static Node SurvivalSearch(Node node, List<Node> obstacles)
    {
        // Random fallback logic
        var children = GetChildren(node, obstacles);
        return children.OrderBy(x => Guid.NewGuid()).First();
    }
}
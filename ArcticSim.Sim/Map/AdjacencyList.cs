namespace ArcticSim.Sim.Map;

/// <summary>
/// Adjacency list data structure used for constructing the map.
/// </summary>
public class AdjacencyList
{
    private Dictionary<string, Node> _nodes = new();

    /// <summary>
    /// Add a node to the graph.
    /// </summary>
    /// <param name="id">Identifier for the node.</param>
    public void AddNode(string id)
    {
        _nodes[id] = new Node(id);
    }

    /// <summary>
    /// Add an edge to the graph.
    /// </summary>
    /// <param name="nodeId">ID of the "from" node.</param>
    /// <param name="edge">Edge instance leading away from the given node.</param>
    public void AddEdge(string nodeId, Edge edge)
    {
        if (_nodes.TryGetValue(nodeId, out var node))
            node.AddEdge(edge);
    }

    /// <summary>
    /// Remove node from the graph.
    /// </summary>
    /// <param name="id">ID of the node to be removed.</param>
    public void RemoveNode(string id)
    {
        _nodes.Remove(id);
    }

    /// <summary>
    /// Remove edge from the graph.
    /// </summary>
    /// <param name="nodeId">ID of the "from" node.</param>
    /// <param name="edge">The edge instance to be removed.</param>
    public void RemoveEdge(string nodeId, Edge edge)
    {
        if (_nodes.TryGetValue(nodeId, out var node))
            node.RemoveEdge(edge);
    }

    /// <summary>IDs of all nodes in the graph.</summary>
    public IEnumerable<string> NodeIds => _nodes.Keys;

    /// <summary>
    /// Returns a node by its ID.
    /// </summary>
    /// <param name="id">ID of the node to be retrieved.</param>
    /// <returns>A node if one is found with the given ID, otherwise null.</returns>
    public Node? GetNode(string id) => _nodes.GetValueOrDefault(id);

    /// <summary>
    /// Returns edges connected to a node.
    /// </summary>
    /// <param name="nodeId">ID of the query node.</param>
    /// <returns>Edges connected to the given node.</returns>
    public List<Edge> GetEdges(string nodeId) =>
        _nodes.TryGetValue(nodeId, out var node) ? node.Edges : [];

    /// <summary>
    /// Prints out the graph structure of the adjacency list.
    /// </summary>
    public void Print()
    {
        foreach (var (id, node) in _nodes)
        {
            Console.WriteLine($"[{id}]");
            foreach (var edge in node.Edges)
            {
                var status = edge.Passable ? "open" : "iced";
                Console.WriteLine($"  -> {edge.DestinationId} ({edge.Weeks}wks, fuel: {edge.FuelCost}, {status})");
            }
        }
    }
}

namespace ArcticSim.Sim.Map;

/// <summary>
/// Represents a location on the Arctic map (e.g. Baffin Bay, Lancaster Sound).
/// Holds all outgoing edges from this location.
/// </summary>
public class Node(string id)
{
    /// <summary>Outgoing edges from this node to adjacent locations.</summary>
    public List<Edge> Edges = new();

    /// <summary>Unique identifier for this location.</summary>
    public string Id = id;

    /// <summary>Adds an outgoing edge from this node.</summary>
    public void AddEdge(Edge edge)
    {
        Edges.Add(edge);
    }

    /// <summary>Removes an outgoing edge from this node.</summary>
    public void RemoveEdge(Edge edge)
    {
        Edges.Remove(edge);
    }
}

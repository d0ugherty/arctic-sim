namespace ArcticSim.Sim.Interfaces;

public interface IGraph<K, T> where K : notnull
{
    public Dictionary<K, List<IEdge<T>>> Graph { get; }

    public void AddEdge(K key, IEdge<T> edge);
    public void AddNode(K nodeId);
    public void RemoveNode(K nodeId);
    public void RemoveEdge(K key, IEdge<T> edge);
}

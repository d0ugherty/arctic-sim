using ArcticSim.Sim.Map;

namespace ArcticSim.Sim;

/// <summary>
/// Builds the Arctic route graph used by the simulation.
/// </summary>
public static class MapBuilder
{
    /// <summary>
    /// Constructs and returns the Arctic map as an <see cref="AdjacencyList"/>.
    /// Routes run west from Baffin Bay toward McClure Strait (the Northwest Passage),
    /// with ice difficulty increasing the further west you travel.
    /// <para>
    /// Edge parameters: <c>Edge(destination, weeks, fuelCost, winterIceChance)</c>.
    /// <c>winterIceChance</c> is the probability of a route being iced over in winter;
    /// Spring scales to 40% of that value, Autumn to 60%, Summer is always open.
    /// </para>
    /// </summary>
    public static AdjacencyList Build()
    {
        var map = new AdjacencyList();

        map.AddNode("baffin_bay");
        map.AddNode("lancaster_sound");
        map.AddNode("barrow_strait");
        map.AddNode("melville_strait");
        map.AddNode("mcclure_strait");
        map.AddNode("gulf_of_boothia");

        // Going west — getting progressively harder and icier
        map.AddEdge("baffin_bay",       new Edge("lancaster_sound",  weeks: 2, fuelCost:  6, winterIceChance: 0.30));
        map.AddEdge("lancaster_sound",  new Edge("barrow_strait",    weeks: 2, fuelCost:  8, winterIceChance: 0.50));
        map.AddEdge("lancaster_sound",  new Edge("gulf_of_boothia",  weeks: 3, fuelCost: 10, winterIceChance: 0.50));
        map.AddEdge("barrow_strait",    new Edge("melville_strait",  weeks: 2, fuelCost:  8, winterIceChance: 0.70));
        map.AddEdge("melville_strait",  new Edge("mcclure_strait",   weeks: 3, fuelCost: 12, winterIceChance: 0.90));

        // Going east
        map.AddEdge("mcclure_strait",   new Edge("melville_strait",  weeks: 3, fuelCost: 12, winterIceChance: 0.90));
        map.AddEdge("melville_strait",  new Edge("barrow_strait",    weeks: 2, fuelCost:  8, winterIceChance: 0.70));
        map.AddEdge("barrow_strait",    new Edge("lancaster_sound",  weeks: 2, fuelCost:  8, winterIceChance: 0.50));
        map.AddEdge("gulf_of_boothia",  new Edge("lancaster_sound",  weeks: 3, fuelCost: 10, winterIceChance: 0.50));
        map.AddEdge("lancaster_sound",  new Edge("baffin_bay",       weeks: 2, fuelCost:  6, winterIceChance: 0.30));

        return map;
    }
}

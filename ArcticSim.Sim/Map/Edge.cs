namespace ArcticSim.Sim.Map;

/// <summary>
/// Represents a navigable route between two locations on the Arctic map.
/// Ice passability is re-evaluated each turn based on <see cref="WinterIceChance"/> and the current season.
/// </summary>
/// <param name="destinationId">ID of the destination node.</param>
/// <param name="weeks">Travel time in weeks.</param>
/// <param name="fuelCost">Coal consumed making this crossing.</param>
/// <param name="winterIceChance">
/// Probability (0–1) that this route is iced over in winter.
/// Spring uses 40% of this value; Autumn uses 60%; Summer is always open.
/// </param>
public record Edge(string destinationId, int weeks = 2, double fuelCost = 8.0, double winterIceChance = 0.5)
{
    /// <summary>ID of the destination node.</summary>
    public string DestinationId { get; set; } = destinationId;

    /// <summary>Coal consumed making this crossing.</summary>
    public double FuelCost { get; set; } = fuelCost;

    /// <summary>Travel time in weeks.</summary>
    public int Weeks { get; set; } = weeks;

    /// <summary>Whether this route is currently navigable. Updated each turn by <see cref="GameEngine"/>.</summary>
    public bool Passable { get; set; } = true;

    /// <summary>
    /// Probability (0–1) that this route is iced over in winter.
    /// Spring uses 40% of this value; Autumn uses 60%; Summer is always open.
    /// </summary>
    public double WinterIceChance { get; init; } = winterIceChance;
}

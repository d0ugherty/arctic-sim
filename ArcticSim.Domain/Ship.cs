namespace ArcticSim.Domain;

/// <summary>
/// Represents the expedition's ship. Holds the ship's supplies (food and fuel).
/// </summary>
public class Ship
{
    /// <summary>The ship's current stores of food and fuel.</summary>
    public Supplies Supplies = new Supplies();
}

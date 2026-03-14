namespace ArcticSim.Domain;

/// <summary>
/// Tracks the ship's consumable resources: food for the crew and fuel (coal) for the engines.
/// Both drain passively over time and additionally when specific events or actions occur.
/// Reaching zero for either is a lose condition.
/// </summary>
public class Supplies
{
    /// <summary>
    /// Remaining coal in tonnes. Consumed per nautical leg based on route fuel cost.
    /// Reaching zero leaves the ship icebound.
    /// </summary>
    public double Fuel { get; set; } = 100.0;

    /// <summary>
    /// Remaining food stores. Consumed at <c>0.15 × crew size</c> per week at full rations
    /// (60% of that when overwintering). Reaching zero starves the crew.
    /// </summary>
    public double Food { get; set; } = 200.0;
}

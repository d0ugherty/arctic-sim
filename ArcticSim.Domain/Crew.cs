namespace ArcticSim.Domain;

/// <summary>
/// Represents the ship's crew. Tracks headcount, physical health, and morale.
/// Morale at zero triggers a mutiny; health at zero ends the expedition to disease.
/// </summary>
public class Crew
{
    /// <summary>Number of crew members remaining.</summary>
    public int Size { get; set; } = 24;

    /// <summary>
    /// Collective health of the crew (0–100).
    /// Reduced by disease events such as scurvy. At zero, the expedition is lost.
    /// </summary>
    public double Health { get; set; } = 100.0;

    /// <summary>
    /// Collective morale of the crew (0–100).
    /// Reduced by overwintering and adverse events. At zero, the crew mutinies.
    /// </summary>
    public double Morale { get; set; } = 100.0;
}

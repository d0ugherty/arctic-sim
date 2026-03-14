using ArcticSim.Domain;
using ArcticSim.Sim.Map;

namespace ArcticSim.Sim;

/// <summary>
/// Holds all mutable state for a running game: the map, ship, crew, and current time.
/// Passed into <see cref="GameEngine"/> which mutates it each turn.
/// </summary>
public class GameState(AdjacencyList map, string startNode)
{
    /// <summary>The Arctic route graph.</summary>
    public AdjacencyList Map { get; init; } = map;

    /// <summary>ID of the node the ship is currently at.</summary>
    public string CurrentNodeId { get; set; } = startNode;

    /// <summary>The player's ship, including its supplies.</summary>
    public Ship Ship { get; init; } = new();

    /// <summary>The expedition crew.</summary>
    public Crew Crew { get; init; } = new();

    /// <summary>Current week of the year (1–52).</summary>
    public int Week { get; set; } = 1;

    /// <summary>Current calendar year.</summary>
    public int Year { get; set; } = 1845;

    /// <summary>True while the ship is locked in for winter.</summary>
    public bool IsOverwintering { get; set; } = false;

    /// <summary>True when a win or lose condition has been reached.</summary>
    public bool GameOver { get; set; } = false;

    /// <summary>Human-readable description of how the game ended.</summary>
    public string? GameOverReason { get; set; }

    /// <summary>
    /// Current season derived from <see cref="Week"/>.
    /// Winter: weeks 1–13, Spring: 14–26, Summer: 27–39, Autumn: 40–52.
    /// </summary>
    public string Season => Week switch
    {
        >= 1 and <= 13  => "Winter",
        >= 14 and <= 26 => "Spring",
        >= 27 and <= 39 => "Summer",
        _               => "Autumn"
    };
}

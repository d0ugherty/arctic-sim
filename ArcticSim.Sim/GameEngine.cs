namespace ArcticSim.Sim;

/// <summary>
/// Drives the simulation: resolves player actions, drains resources,
/// updates ice conditions, fires random events, and checks win/lose conditions.
/// After each action, check <see cref="PendingMessages"/> for messages to display.
/// </summary>
public class GameEngine
{
    private readonly GameState _state;
    private readonly Random _rng = new();

    /// <summary>Food consumed per crew member per week at full rations.</summary>
    private const double FoodPerCrewPerWeek = 0.15;

    /// <summary>
    /// Messages queued by the engine (ice state changes, random events) for the
    /// CLI to display. Callers should drain and clear this after each action.
    /// </summary>
    public List<string> PendingMessages { get; } = new();

    /// <summary>
    /// Initialises the engine and sets initial ice conditions based on the
    /// starting week/season in <paramref name="state"/>.
    /// </summary>
    public GameEngine(GameState state)
    {
        _state = state;
        UpdateIceConditions();
    }

    /// <summary>
    /// Attempts to sail the ship to <paramref name="destinationId"/>.
    /// Consumes fuel and food proportional to the route's cost and travel time.
    /// </summary>
    /// <returns>
    /// <c>(true, message)</c> on success; <c>(false, reason)</c> if the route
    /// doesn't exist or is currently iced over.
    /// </returns>
    public (bool success, string message) MoveTo(string destinationId)
    {
        var edge = _state.Map.GetEdges(_state.CurrentNodeId)
            .FirstOrDefault(e => e.DestinationId == destinationId);

        if (edge == null)
            return (false, "No route to that location.");

        if (!edge.Passable)
            return (false, "That route is blocked by ice.");

        _state.Ship.Supplies.Fuel -= edge.FuelCost;
        _state.Ship.Supplies.Food -= _state.Crew.Size * FoodPerCrewPerWeek * edge.Weeks;
        _state.Week += edge.Weeks;
        _state.CurrentNodeId = destinationId;

        NormalizeWeek();
        UpdateIceConditions();
        CheckWinLose();
        return (true, $"You sail to {destinationId.Replace('_', ' ')}.");
    }

    /// <summary>
    /// Locks the ship in for winter and skips time forward to the following summer (week 27).
    /// Food is consumed at 60% rations for the duration. Morale takes a hit.
    /// </summary>
    /// <returns>A message describing the outcome.</returns>
    public string Overwinter()
    {
        int weeksUntilSummer = (_state.Week <= 26)
            ? 27 - _state.Week
            : 52 - _state.Week + 27;

        _state.Ship.Supplies.Food -= _state.Crew.Size * FoodPerCrewPerWeek * 0.6 * weeksUntilSummer;
        _state.Crew.Morale -= 10;
        _state.Week = 27;
        _state.Year++;

        UpdateIceConditions();
        CheckWinLose();
        return "You overwinter. The ice retreats with the spring thaw.";
    }

    /// <summary>
    /// Advances time by one week in place: consumes food, re-evaluates ice,
    /// and may trigger a random event.
    /// </summary>
    /// <returns>An event message, or a default "uneventful" message.</returns>
    public string Wait()
    {
        _state.Ship.Supplies.Food -= _state.Crew.Size * FoodPerCrewPerWeek;
        _state.Week++;
        NormalizeWeek();

        UpdateIceConditions();
        var eventMessage = TriggerRandomEvent();
        CheckWinLose();
        return eventMessage ?? "The week passes uneventfully.";
    }

    /// <summary>
    /// Re-evaluates passability for every edge in the map based on the current season.
    /// Any routes that open or close are added to <see cref="PendingMessages"/>.
    /// </summary>
    private void UpdateIceConditions()
    {
        foreach (var nodeId in _state.Map.NodeIds)
        {
            foreach (var edge in _state.Map.GetEdges(nodeId))
            {
                bool wasPassable = edge.Passable;
                edge.Passable = RollPassable(edge);

                if (edge.Passable != wasPassable)
                {
                    var from   = nodeId.Replace('_', ' ');
                    var to     = edge.DestinationId.Replace('_', ' ');
                    var status = edge.Passable ? "opened" : "iced over";
                    PendingMessages.Add($"[ICE] {from} → {to} has {status}.");
                }
            }
        }
    }

    /// <summary>
    /// Rolls whether a route is passable given the current season and the
    /// edge's <see cref="Edge.WinterIceChance"/>.
    /// </summary>
    private bool RollPassable(Map.Edge edge)
    {
        double iceChance = _state.Season switch
        {
            "Summer" => 0.0,
            "Autumn" => edge.WinterIceChance * 0.6,
            "Spring" => edge.WinterIceChance * 0.4,
            "Winter" => edge.WinterIceChance,
            _        => 0.0
        };
        return _rng.NextDouble() >= iceChance;
    }

    /// <summary>Wraps <see cref="GameState.Week"/> and increments the year at week 53.</summary>
    private void NormalizeWeek()
    {
        if (_state.Week <= 52) return;
        _state.Week -= 52;
        _state.Year++;
    }

    /// <summary>
    /// 20% chance each turn to fire a random event that affects supplies,
    /// crew health, or morale.
    /// </summary>
    /// <returns>The event message, or <c>null</c> if no event fired.</returns>
    private string? TriggerRandomEvent()
    {
        if (_rng.NextDouble() > 0.20)
            return null;

        var events = new (string message, Action effect)[]
        {
            ("A blizzard pins you down. Rations run short.",      () => _state.Ship.Supplies.Food -= 10),
            ("Scurvy breaks out among the crew.",                 () => _state.Crew.Health -= 10),
            ("A fierce gale damages the boiler.",                 () => _state.Ship.Supplies.Fuel -= 8),
            ("The crew grows restless in the ice.",               () => _state.Crew.Morale -= 8),
            ("A polar bear sighting lifts the crew's spirits.",   () => _state.Crew.Morale += 5),
            ("Local Inuit share knowledge of the passage ahead.", () => _state.Crew.Morale += 8),
        };

        var (message, effect) = events[_rng.Next(events.Length)];
        effect();
        return $"EVENT: {message}";
    }

    /// <summary>Sets <see cref="GameState.GameOver"/> if a terminal condition is met.</summary>
    private void CheckWinLose()
    {
        if (_state.CurrentNodeId == "mcclure_strait")
        {
            _state.GameOver = true;
            _state.GameOverReason = "You navigated the Northwest Passage. Victory!";
        }
        else if (_state.Ship.Supplies.Food <= 0)
        {
            _state.GameOver = true;
            _state.GameOverReason = "Your crew has starved. The expedition is lost.";
        }
        else if (_state.Ship.Supplies.Fuel <= 0)
        {
            _state.GameOver = true;
            _state.GameOverReason = "Out of coal, your ship is locked in ice forever.";
        }
        else if (_state.Crew.Morale <= 0)
        {
            _state.GameOver = true;
            _state.GameOverReason = "The crew mutinied. The expedition is over.";
        }
        else if (_state.Crew.Health <= 0)
        {
            _state.GameOver = true;
            _state.GameOverReason = "Disease has claimed the last of your crew.";
        }
    }
}

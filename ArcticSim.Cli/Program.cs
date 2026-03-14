using ArcticSim.Sim;

var map = MapBuilder.Build();
var state = new GameState(map, "baffin_bay");
var engine = new GameEngine(state);

Console.WriteLine("=== ARCTIC EXPEDITION ===");
Console.WriteLine("Year: 1845. Your mission: find the Northwest Passage.");
Console.WriteLine("Press Enter to begin...");
Console.ReadLine();

while (!state.GameOver)
{
    FlushMessages(engine);
    RenderState(state);
    var input = GetInput(state);
    var result = HandleInput(input, state, engine);
    Console.WriteLine();
    if (result != null)
        Console.WriteLine(result);
}

FlushMessages(engine);
Console.WriteLine();
Console.WriteLine(state.GameOverReason);

static void FlushMessages(GameEngine engine)
{
    if (engine.PendingMessages.Count == 0) return;
    foreach (var msg in engine.PendingMessages)
        Console.WriteLine(msg);
    engine.PendingMessages.Clear();
}

static void RenderState(GameState state)
{
    Console.WriteLine();
    Console.WriteLine(new string('-', 50));
    Console.WriteLine($"  {state.CurrentNodeId.Replace('_', ' ').ToUpper()}");
    Console.WriteLine($"  Week {state.Week}, {state.Season} {state.Year}");
    Console.WriteLine($"  Food: {state.Ship.Supplies.Food,6:F1}  |  Fuel: {state.Ship.Supplies.Fuel,6:F1}");
    Console.WriteLine($"  Crew: {state.Crew.Size,4}         |  Health: {state.Crew.Health,4:F0}  |  Morale: {state.Crew.Morale,4:F0}");
    Console.WriteLine(new string('-', 50));
}

static string GetInput(GameState state)
{
    var edges    = state.Map.GetEdges(state.CurrentNodeId);
    var passable = edges.Where(e => e.Passable).ToList();
    var blocked  = edges.Where(e => !e.Passable).ToList();

    Console.WriteLine("\nWhat do you do?");

    int i = 1;
    foreach (var edge in passable)
    {
        var dest = edge.DestinationId.Replace('_', ' ');
        Console.WriteLine($"  {i++}. Sail to {dest} ({edge.Weeks} wks, fuel: {edge.FuelCost:F0})");
    }
    foreach (var edge in blocked)
    {
        Console.WriteLine($"      [ICED] {edge.DestinationId.Replace('_', ' ')}");
    }

    Console.WriteLine($"  {i++}. Overwinter here (skip to next summer)");
    Console.WriteLine($"  {i}.   Wait one week");

    Console.Write("\n> ");
    return Console.ReadLine() ?? "";
}

static string? HandleInput(string input, GameState state, GameEngine engine)
{
    var edges    = state.Map.GetEdges(state.CurrentNodeId);
    var passable = edges.Where(e => e.Passable).ToList();

    if (!int.TryParse(input.Trim(), out int choice))
        return "Please enter a number.";

    if (choice >= 1 && choice <= passable.Count)
    {
        var (_, message) = engine.MoveTo(passable[choice - 1].DestinationId);
        return message;
    }
    else if (choice == passable.Count + 1)
    {
        return engine.Overwinter();
    }
    else if (choice == passable.Count + 2)
    {
        return engine.Wait();
    }

    return "Invalid choice.";
}

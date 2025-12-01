
class Player
{
    private readonly Random rng = new();
    public string Name;
    public bool IsAI { get; private set; } = false;
    private static readonly string[] aiNames = ["AI_Lukas", "AI_Vena", "AI_Morgan", "AI_Samuel", "AI_Alex"];
    public int Score = 0;
    public int Money = 12399;
    private IAbilityProduct? currentAbility;
    public List<IProduct> Inventory = [];
    public Board Board;
    public Shop? Shop { get; set; }

    public readonly ControlScheme ControlScheme;
    public string[] ValidKeys => [.. ControlScheme.Keys];

    public Player(string name, ControlScheme controlScheme)
    {
        this.ControlScheme = controlScheme;
        this.Name = name;
        this.Board = new();
    }
    public Player(bool isAI)
    {
        if (isAI == false) throw new ArgumentException("Use other constructor for non-AI players");
        this.IsAI = true;

        this.ControlScheme = []; // Empty control scheme for AI
        this.Name = aiNames[rng.Next(aiNames.Length)];
        this.Board = new();
    }

    public void Tick(string key)
    {
        // TODO: polymorphism
        if (IsAI)
        {
            bool noOp = rng.NextDouble() < 0.8;
            if (!noOp)
            {
                Input[] possibleInputs = Enum.GetValues<Input>();
                Board.Move(possibleInputs[rng.Next(possibleInputs.Length)]);
            }
        }
        else
        {
            Input? input = ValidKeys.Contains(key) ? ControlScheme[key] : null;
            if (input is Input moveInput) Board.Move(moveInput);
        }

        Board.Tick();
        TickInventory();

        // Every tick it will empty the boards local score and money buffers into the player's total
        this.Score += Board.ScoreBuffer;
        this.Money += Board.MoneyBuffer;

        if (Board.HasLost)
        {
            // TODO: not like this...
            Console.WriteLine($"{Name} has lost the game!");
            Environment.Exit(0); // TODO: implement proper game over handling
        }
    }

    public void UseAbility()
    {
        if (currentAbility?.Cooldown == 0)
        {
            currentAbility.Use(); // TODO: implement
        }
    }

    public void AddToInventory(IProduct product)
    {
        if (product is IAbilityProduct temp)
        {
            currentAbility = temp;
        }
        else
        {
            Inventory.Add(product);
            product.Use();
        }
    }

    public void TickInventory()
    {
        foreach (IProduct product in Inventory)
        {
            if (product is ITemporaryProduct temp)
            {
                temp.UpdateLifetime();
                if (temp.lifetime == 0)
                {
                    temp.Disable();
                }
            }
        }
        if ((currentAbility != null) && (currentAbility.Cooldown > 0)) currentAbility.Cooldown--;
    }
}
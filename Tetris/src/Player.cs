
class Player
{
    private static readonly Random rng = new();
    public string Name;
    public bool IsAI { get; protected set; } = false;
    private static readonly string[] aiNames = ["AI_Lukas", "AI_Vena", "AI_Morgan", "AI_Samuel", "AI_Alex"];
    public int Score = 0;
    public int Money = 200;
    private IAbilityProduct? currentAbility;
    public List<IProduct> Inventory = [];
    // KRAV 4:
    // 1: Objektkomposition
    // 2: Vi använder objektkomposition för att ge varje spelare en egen spelplan.
    //     Boards Move() och Tick() metoder anropas från Player klassen. Samt läser
    //     player-klassen Boardens publika ScoreBuffer och MoneyBuffer fält. 
    // 3: Konceptuellt är det rimligt att en spelar har koll på sitt eget bräde,
    //     och programetiskt är det smidigt eftersom spelaren agerar identifierar 
    //     till sitt eget bräde när det gäller interaktion med shoppen. Board är
    //     väldigt inkapslad och interagerar endast med spelaren via "kommandon"
    //     som inte behöver rapportera upp något samt publika buffrar som tömmer 
    //     sig själva på board änden.
    public Board Board;
    public Shop? Shop { get; set; }// TODO make non-nullable
    private long lastTick = 0;
    private long inventoryTickRate = GameState.GameplayDuration;

    public readonly ControlScheme ControlScheme;
    // KRAV 3:
    // 1: Computed property.
    // 2: Vi använder konceptet för att dynamiskt få en lista av giltiga tangenter.
    // 3: När vi kontrollerar inputs så används denna egenskap för att verifiera 
    //     om en tangent är giltig så switchen eller dicionary access inte throwar.
    public string[] ValidKeys => [.. ControlScheme.Keys];

    public Player(string name, ControlScheme controlScheme)
    {
        this.ControlScheme = controlScheme;
        this.Name = name;
        this.Board = new();
    }
    // KRAV 2:
    // 1: Overloading av konstruktorer
    // 2: Det finns två konstruktorer för att kunna instansiera en AI-spelare 
    //     utan att behöva hitta på låtsasdata vid varje implementation.
    // 3: Det är smidigt och rimligt att ha denna skyddade konstruktor som en 
    //     subklass anropar för att skapa AI-spelare. Det ger en mer explicit 
    //     och tydlig implementation som inte kräver onödig data som dessutom 
    //     kan behöva skapas på olika platser.
    protected Player(bool isAI)
    {
        if (isAI == false) throw new ArgumentException("Use other constructor for non-AI players");
        this.IsAI = true;

        this.ControlScheme = []; // Empty control scheme for AI
        this.Name = aiNames[rng.Next(aiNames.Length)];
        this.Board = new();
    }

    protected virtual Input? GetInput(string key) => null;

    public virtual void Tick(string key)
    {
        Input? input = GetInput(key);
        if (input is Input moveInput) Board.Move(moveInput);

        // Tick the temporary products every round
        long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (currentTime - lastTick > inventoryTickRate)
        {
            TickInventory();
        }
        lastTick = currentTime;
        Board.Tick();

        // Every tick it will empty the boards local score and 
        //  money buffers into the player's total
        this.Score += Board.ScoreBuffer;
        this.Money += Board.MoneyBuffer;

        if (Board.HasLost)
        {
            // TODO: not like this...
            Console.WriteLine($"{Name} has lost the game!");
            Environment.Exit(0); // TODO: implement proper game over handling
        }
    }

    // TODO: implement
    public void UseAbility()
    {
        if (currentAbility?.Cooldown == 0)
        {
            currentAbility.Use();
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
                temp.Tick();
            }
        }
        if ((currentAbility != null) && (currentAbility.Cooldown > 0)) currentAbility.Cooldown--;
    }
}

class HumanPlayer(string name, ControlScheme controlScheme) : Player(name, controlScheme)
{
    protected override Input? GetInput(string key)
    {
        return ValidKeys.Contains(key) ? ControlScheme[key] : null;
    }
}

class AIPlayer : Player
{
    private readonly Random aiRng = new();

    public AIPlayer() : base(isAI: true) { }

    protected override Input? GetInput(string key)
    {
        bool noOp = aiRng.NextDouble() < 0.8;
        if (!noOp)
        {
            Input[] possibleInputs = Enum.GetValues<Input>();
            return possibleInputs[aiRng.Next(possibleInputs.Length)];
        }
        return null;
    }
}
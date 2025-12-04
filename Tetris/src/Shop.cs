class Shop
{
    private const int ProductsPerRestock = 4;

    private readonly Random rng = new();
    private readonly List<Player> others = [];
    private readonly List<Func<IProduct>> productPool = [];

    public Player Owner { get; }
    public List<IProduct> Products { get; private set; } = [];

    public Shop(Player owner, List<Player> others)
    {
        this.Owner = owner;
        this.others = others;
        this.productPool = [
            () => new SpeedUp(owner, others),
            () => new MoreI(owner, others),
            () => new Tax(owner, others),
            () => new MoreRows(owner),
            () => new SlowDown(owner),
            () => new AddDot(owner),
            () => new MoneyMultiplier(owner),
            () => new LongerPreview(owner),
            () => new SlowMotion(owner),
            () => new SkipOwnCurrent(owner),
            () => new SkipOpponentsCurrent(owner, others),
            () => new MonochromeBoard(owner, others),
        ];

        Restock();
    }

    public void Restock()
    {
        Products = [];
        for (int i = 0; i < ProductsPerRestock; i++)
        {
            Products.Add(productPool[rng.Next(productPool.Count)].Invoke());
        }
    }
}

// KRAV 7:
// 1: Subtypspolymorfism #2
// 2: ProductPool och Products använder typen IProduct, men fylls med olika
//     konkreta subklasser (AddDot, SpeedUp, Tax, osv). När spelaren
//     senare köper en produkt anropas Player.AddToInventory och vid product.Use()
//     anropas rätt Use()-implementation beroende på faktisk produkttyp.
// 3: Detta gör att shoppen kan hantera alla produkter på ett enhetligt sätt
//     utan switch-satser per produkttyp. Vi kan enkelt lägga till nya
//     produkter genom att bara lägga till en ny klass och fabrik i ProductPool.
interface IProduct
{
    public string Name { get; }
    string description { get; }
    double rarity { get; }
    int price { get; }
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }

    void Purchase()
    {
        if (Purchaser.Money >= this.price)
        {
            Purchaser.AddToInventory(this);
            Purchaser.Money -= price;
        }
    }
    public void Use();
}

interface IStaticProduct : IProduct
{
    // Always active

}

interface ITemporaryProduct : IProduct
{
    // Active for set amount of time / rounds
    int Lifetime { get; set; }
    public void Tick()
    {
        Lifetime--;
        if (Lifetime <= 0) Disable();
    }
    public void Disable();
}

interface IAbilityProduct : IProduct
{
    // Can be activated any time but has Cooldown
    public int Cooldown { get; set; }
    public int CooldownTimer { get; set; }
    public int Duration { get; set; }
    public int DurationTimer { get; set; }
    public bool Disabled { get; set; }
    public void Disable();
    public void Tick()
    {
        CooldownTimer--;
        DurationTimer--;
        if (DurationTimer <= 0)
        {
            DurationTimer = 0;
            if (!Disabled) Disable();
        }
        if (CooldownTimer <= 0) { CooldownTimer = 0; }
    }
}

// Future product ideas (placeholder notes, not implemented yet):
// - Communism (temporary): splits income evenly with opponents.
// - ScoreMultiplier (static): multiplies score gains.
// - TetrominoProjection (static): shows landing projection for active piece.
// - CandyCrush (temporary): breaks connected colors.
// - NuclearDrop (temporary): loosens drop collision on the Y-axis.
// - ExtraBoard (temporary): forces opponents to juggle a second board.
// - DisableQuickDrop (ability): blocks opponents from fast dropping.
// - FreezeInput (ability): briefly freezes opponent inputs.

// ================== BUFFS ==================
class AddDot : IStaticProduct
{
    // Buff
    // Have a chance to fill in the gaps 
    // with the new exciting . (dot) monomino!
    public string Name { get; } = "Add Dot";
    public string description { get; } = "Add a dot monomino to your queue so you have a chance of getting it in your queue.";
    public double rarity { get; } = 0.2;
    public int price { get; } = 50;
    public Player Purchaser { get; set; }
    public List<Player> Targets { get; set; }
    public AddDot(Player purchaser)
    {
        this.Purchaser = purchaser;
        this.Targets = [purchaser];
    }

    public void Use()
    {
        Purchaser.Board.PolyominoPool.Add(() => new MonominoDot());
    }
}

class SlowDown : IStaticProduct
{
    // Buff
    // Lowers speed of own board
    public string Name { get; } = "Slow down";
    public string description { get; } = "Slows down how fast your board updates its physics.";
    public double rarity { get; } = 0.15;
    public int price { get; } = 40;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }
    private double multiplier = 1.1;
    public SlowDown(Player purchaser)
    {
        this.Targets = [purchaser];
        this.Purchaser = purchaser;
    }

    public void Use()
    {
        Purchaser.Board.DT = (int)Math.Floor(Purchaser.Board.DT * multiplier);
    }
}

class MoneyMultiplier : IStaticProduct
{
    // Buff
    // Multiplies money
    // Buff
    // Multiplies score
    public string Name { get; } = "Money Multiplier";
    public string description { get; } = "Increase the amount of currency units (cu) you earn.";
    public double rarity { get; } = 0.05;
    public int price { get; } = 200;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }

    public MoneyMultiplier(Player purchaser)
    {
        this.Purchaser = purchaser;
        this.Targets = [purchaser];
    }

    public void Use()
    {
        Purchaser.Board.MoneyMultiplier += 0.1;
    }
}

class MoreRows : IStaticProduct
{
    // Buff
    // Adds more rows to board
    public string Name { get; } = "More Rows";
    public string description { get; } = "Adds extra rows to your board, giving you more time to react.";
    public double rarity { get; } = 0.12;
    public int price { get; } = 80;
    public Player Purchaser { get; set; }
    public List<Player> Targets { get; set; }
    public MoreRows(Player purchaser)
    {
        this.Purchaser = purchaser;
        this.Targets = [purchaser];
    }

    public void Use()
    {
        Purchaser.Board.VisibleHeight += 1;
    }
}

class LongerPreview : IStaticProduct
{
    // Buff
    // Shows more upcoming blocks in queue
    public string Name { get; } = "Bigger Preview";
    public string description { get; } = "Shows more upcoming pieces in your queue.";
    public double rarity { get; } = 0.16;
    public int price { get; } = 70;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }
    public LongerPreview(Player purchaser)
    {
        this.Purchaser = purchaser;
        this.Targets = [purchaser];
    }

    public void Use()
    {
        Purchaser.Board.AddToQueue();
    }
}

class MoreI : ITemporaryProduct
{
    // Buff
    // Gets more I pieces but may result in esoteric I's
    public string Name { get; } = "More I-Tetrominoes";
    public string description { get; } = "Increases frequency of I pieces, may result in esoteric pieces.";
    public double rarity { get; } = 0.08;
    public int price { get; } = 120;
    public int LifeTime { get; } = 5;

    public Player Purchaser { get; set; }
    private List<Func<Polyomino>> polyominoes = [
        () => new TetrominoI(),
        () => new OctominoThiccI(),
        () => new OctominoThiccI(),
        () => new TrominoLowerI(),
    ];

    public int Lifetime { get; set; } = 5;
    public List<Player> Targets { get; set; }
    public MoreI(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }
    public void Use()
    {
        if (Purchaser == null) return;
        Purchaser.Board.PolyominoPool.AddRange(polyominoes);
    }
    public void Disable()
    {
        polyominoes.ForEach(polyomino => Purchaser.Board.PolyominoPool.Remove(polyomino));
    }
}

class SlowMotion : IAbilityProduct
{
    // Buff
    // Halves drop speed of board
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }
    public string Name { get; } = "Slow Motion";
    public string description { get; } = "Halves drop speed";
    public double rarity { get; } = 0.09;
    public int price { get; } = 150;
    public int Cooldown { get; set; } = 10;
    public int CooldownTimer { get; set; } = 0;
    public int Duration { get; set; } = 5;
    public int DurationTimer { get; set; } = 0;
    private double multiplier = 2;
    public bool Disabled { get; set; } = true;

    public SlowMotion(Player purchaser)
    {
        this.Targets = [purchaser];
        this.Purchaser = purchaser;
    }

    public void Use()
    {
        if ((CooldownTimer <= 0) && Disabled)
        {
            Disabled = false;
            Purchaser.Board.DT = (int)Math.Floor(Purchaser.Board.DT * multiplier);
            DurationTimer = Duration;
        }
    }

    public void Disable()
    {
        CooldownTimer = 10;
        Disabled = true;
        Purchaser.Board.DT = (int)Math.Floor(Purchaser.Board.DT / multiplier);
    }
}

class SkipOwnCurrent : IAbilityProduct
{
    // Buff
    // Discard current falling polyomino
    public string Name { get; } = "Skip";
    public string description { get; } = "Discards currently falling polyomino.";
    public double rarity { get; } = 0.11;
    public int price { get; } = 130;
    public int Cooldown { get; set; } = 7;
    public int CooldownTimer { get; set; } = 0;
    public int Duration { get; set; } = 0;
    public int DurationTimer { get; set; } = 0;
    public bool Disabled { get; set; } = true;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }

    public SkipOwnCurrent(Player purchaser)
    {
        this.Targets = [purchaser];
        this.Purchaser = purchaser;
    }

    public void Use()
    {
        if ((CooldownTimer <= 0) && Disabled)
        {
            Disabled = false;
            Purchaser.Board.FallingPolyominoes.RemoveAt(0);
            CooldownTimer = Cooldown;
        }
    }

    public void Disable()
    {
        Disabled = true;
    }
}


// ================== DEBUFFS ==================
class SpeedUp : IStaticProduct
{
    // Debuff
    // Increase DT of opponent
    public SpeedUp(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }
    public List<Player> Targets { get; set; }
    public string Name { get; } = "Speed Up";
    public string description { get; } = "Make your opponents boards apply physics faster.";
    public double rarity { get; } = 0.14;
    public int price { get; } = 90;
    public Player Purchaser { get; set; }

    public void Use()
    {
        Targets.ForEach(target => target.Board.DT = (int)Math.Floor(target.Board.DT * 0.90));
    }
}

class MonochromeBoard : IStaticProduct
{
    // Debuff
    // Make opponents board black & white 
    // (might not pair well if they have with candy crush)
    public string Name { get; } = "MonochromeBoard";
    public string description { get; } = "Turn opponents polyominoes monochrome. It will affect both settled and falling pieces as well as the visible queue.";
    public double rarity { get; } = 0.13;
    public int price { get; } = 95;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }
    public MonochromeBoard(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }

    public void Use()
    {
        Targets.ForEach(target =>
        {
            target.Board.SettledTiles.ForEach(tile => tile.Color = AnsiColor.GrayCode);
            target.Board.FallingPolyominoes.ForEach(polyomino => polyomino.Color = AnsiColor.GrayCode);
            int queueLength = target.Board.Queue.Count;
            List<Polyomino> tempQueue = [];
            for (int i = 0; i < queueLength; i++)
            {
                Polyomino poly = target.Board.Queue.Dequeue();
                poly.Color = AnsiColor.GrayCode;
                tempQueue.Add(poly);
            }
            tempQueue.Reverse();
            tempQueue.ForEach(poly => target.Board.Queue.Enqueue(poly));
        });
    }
}

class Tax : ITemporaryProduct
{
    // Debuff
    // Tax your opponent! YOU ARE THE KING

    public Tax(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }
    public string Name { get; } = "Tax Time!";
    public Player Purchaser { get; set; }
    public List<Player> Targets { get; set; }
    public int Lifetime { get; set; } = 5;

    public string description { get; } = "You will take a part of your opponents money.";
    public double rarity { get; } = 0.06;
    public int price { get; } = 140;
    public int LifeTime { get; } = 4;
    private double taxRate = 0.9;

    public void Use()
    {
        int taxed = 0;
        foreach (var target in Targets)
        {
            int before = target.Money;
            target.Money = (int)Math.Floor(target.Money * taxRate);
            taxed += before - target.Money;
        }
        Purchaser.Money += taxed;
    }
    public void Disable()
    {
        Purchaser.Inventory.Remove(this);
    }
}

class SkipOpponentsCurrent : IAbilityProduct
{
    // Debuff
    // Skip the next piece for your opponent
    public string Name { get; } = "Skip Opponents Piece";
    public string description { get; } = "Discard the currently falling piece of all opponents.";
    public double rarity { get; } = 0.04;
    public int price { get; } = 170;
    public int Cooldown { get; set; } = 8;
    public List<Player> Targets { get; set; }
    public Player Purchaser { get; set; }
    public SkipOpponentsCurrent(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }
    public int CooldownTimer { get; set; } = 0;
    public int Duration { get; set; } = 0;
    public int DurationTimer { get; set; } = 0;
    public bool Disabled { get; set; } = true;

    public void Use()
    {
        if ((CooldownTimer <= 0) && Disabled)
        {
            Disabled = false;
            Targets.ForEach(target => target.Board.FallingPolyominoes.RemoveAt(0));
            CooldownTimer = Cooldown;
        }
    }

    public void Disable()
    {
        Disabled = true;
    }
}

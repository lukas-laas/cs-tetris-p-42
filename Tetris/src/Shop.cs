class Shop
{
    public List<IProduct> Products = [];

    public Player Owner; // The player who buys stuff
    private List<Player> others = [];

    public List<Func<IProduct>> ProductPool = [];

    public Shop(Player owner, List<Player> others)
    {
        this.Owner = owner;
        this.others = others;
        this.ProductPool = [
            () => new SpeedUp(owner, others),
            () => new MoreI(owner, others),
            () => new Tax(owner, others),
            () => new MoreRows(owner),
            () => new SlowDown(owner),
            () => new DotTime(owner),
            () => new MoneyMultiplier(owner),
            () => new LongerPreview(owner),
        ];

        int productsLength = 3;
        Random rng = new();
        this.Products = [];

        for (int i = 0; i < productsLength; i++)
        {
            Products.Add(ProductPool[rng.Next(ProductPool.Count)].Invoke());
        }

    }
}

interface IProduct
{
    string name { get; }
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
    int lifetime { get; set; }
    public void Tick()
    {
        lifetime--;
        if (lifetime <= 0) Disable();
    }
    public void Disable();
}

interface IAbilityProduct : IProduct
{
    // Can be activated any time but has Cooldown
    public int Cooldown { get; set; }
}

// class Communism : ITemporaryProduct
// {
//     // Buff/Debuff
//     // Split income evenly with opponent
//     public string name { get; } = "Communism";
//     public string description { get; } = "Splits income evenly with opponent";
//     public double rarity { get; } = 0.1;
//     public int price { get; } = 100;
//     public int LifeTime { get; } = 3;
//     public void Purchase() { }
// }


// // ==================== BUFFS ===================
class DotTime : IStaticProduct
{
    // Buff
    // Have a chance to fill in the gaps 
    // with the new exciting . (dot) tetromino!
    public string name { get; } = "DotTime";
    public string description { get; } = "Chance to spawn dot tetromino";
    public double rarity { get; } = 0.2;
    public int price { get; } = 50;
    public Player Purchaser { get; set; }
    public List<Player> Targets { get; set; }
    public DotTime(Player purchaser)
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
    public string name { get; } = "Slow down";
    public string description { get; } = "Lowers own drop timer";
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

// class ScoreMultiplier : IStaticProduct
// {
//     // Buff
//     // Multiplies score
//     public string name { get; } = "ScoreMultiplier";
//     public string description { get; } = "Multiplies score";
//     public double rarity { get; } = 0.05;
//     public int price { get; } = 200;
//     public List<Player> Targets { get; set; }
//     public Player Purchaser { get; set; }

//     public ScoreMultiplier(Player purchaser)
//     {
//         this.Purchaser = purchaser;
//         this.Targets = [purchaser];
//     }

//     public void Use()
//     {
//         Purchaser.Board.
//     }
// }

class MoneyMultiplier : IStaticProduct
{
    // Buff
    // Multiplies money
    // Buff
    // Multiplies score
    public string name { get; } = "Score Multiplier";
    public string description { get; } = "Multiplies score";
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
    public string name { get; } = "More Rows";
    public string description { get; } = "Adds extra rows to board";
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

// class TetrominoProjection : IStaticProduct
// {
//     // Buff
//     // Shows where tetromino is going to land
//     public string name { get; } = "TetrominoProjection";
//     public string description { get; } = "Shows landing projection";
//     public double rarity { get; } = 0.18;
//     public int price { get; } = 60;
//     public void Purchase() { }
// }

class LongerPreview : IStaticProduct
{
    // Buff
    // Shows more upcoming blocks in queue
    public string name { get; } = "Longer Preview";
    public string description { get; } = "Shows more upcoming pieces";
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
    public string name { get; } = "More I-Tetrominoes";
    public string description { get; } = "Increases frequency of I pieces, may result in esoteric pieces";
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

    public int lifetime { get; set; } = 5;
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

// class CandyCrush : ITemporaryProduct
// {
//     // Buff
//     // Breaks connecting colors
//     public string name { get; } = "CandyCrush";
//     public string description { get; } = "Breaks connecting colors";
//     public double rarity { get; } = 0.07;
//     public int price { get; } = 110;
//     public int LifeTime { get; } = 4;
//     public void Purchase() { }
// }



// class NuclearDrop : ITemporaryProduct
// {
//     // Buff
//     // Loosened y-axis block collision
//     public string name { get; } = "NuclearDrop";
//     public string description { get; } = "Alters drop collision";
//     public double rarity { get; } = 0.03;
//     public int price { get; } = 250;
//     public int LifeTime { get; } = 3;
//     public void Purchase() { }
// }

// class SlowMotion : IAbilityProduct
// {
//     // Buff
//     // Halves drop speed of board
//     public string name { get; } = "SlowMotion";
//     public string description { get; } = "Halves drop speed";
//     public double rarity { get; } = 0.09;
//     public int price { get; } = 150;
//     public int Cooldown { get; } = 10;
//     public void Purchase() { }
// }

// class Skip : IAbilityProduct
// {
//     // Buff
//     // Discard tetromino
//     public string name { get; } = "Skip";
//     public string description { get; } = "Discard current tetromino";
//     public double rarity { get; } = 0.11;
//     public int price { get; } = 130;
//     public int Cooldown { get; } = 6;
//     public void Purchase() { }
// }


// // ==================== DEBUFFS =================
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
    public string name { get; } = "Speed Up";
    public string description { get; } = "Increases opponents drop rate";
    public double rarity { get; } = 0.14;
    public int price { get; } = 90;
    public Player Purchaser { get; set; }

    public void Use()
    {
        Targets.ForEach(target => target.Board.DT = (int)Math.Floor(target.Board.DT * 0.90));
    }
}

// class MonochromeBoard : IStaticProduct
// {
//     // Debuff
//     // Make opponents board black & white 
//     // (might not pair well if they have with candy crush)
//     public string name { get; } = "MonochromeBoard";
//     public string description { get; } = "Turns opponent board monochrome";
//     public double rarity { get; } = 0.13;
//     public int price { get; } = 95;
//     public void Purchase() { }
// }

class Tax : ITemporaryProduct
{
    // Debuff
    // Tax your opponent! YOU ARE THE KING

    public Tax(Player purchaser, List<Player> targets)
    {
        this.Purchaser = purchaser;
        this.Targets = targets;
    }
    public string name { get; } = "Tax Time!";
    public Player Purchaser { get; set; }
    public List<Player> Targets { get; set; }
    public int lifetime { get; set; } = 5;

    public string description { get; } = "Taxes opponents earnings";
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

// class ExtraBoard : ITemporaryProduct
// {
//     // Debuff
//     // Your opponent will love playing two games at once
//     // but watch out, the money is still counted..

//     // Different pieces on second board
//     // Faster dt as well?
//     public string name { get; } = "ExtraBoard";
//     public string description { get; } = "Adds a second board for opponent";
//     public double rarity { get; } = 0.02;
//     public int price { get; } = 300;
//     public int LifeTime { get; } = 6;
//     public void Purchase() { }
// }

// class SkipOpponentsPiece : IAbilityProduct
// {
//     // Debuff
//     // Skip the next piece for your opponent
//     public string name { get; } = "SkipOpponentsPiece";
//     public string description { get; } = "Skips opponents next piece";
//     public double rarity { get; } = 0.04;
//     public int price { get; } = 170;
//     public int Cooldown { get; } = 8;
//     public void Purchase() { }
// }

// class DisableQuickDrop : IAbilityProduct
// {
//     // Debuff
//     // Make your opponent wait ages for their tetrominoes
//     // to hit the ground
//     public string name { get; } = "DisableQuickDrop";
//     public string description { get; } = "Disables opponents quick drop";
//     public double rarity { get; } = 0.05;
//     public int price { get; } = 160;
//     public int Cooldown { get; } = 9;
//     public void Purchase() { }
// }

// class FreezeInput : IAbilityProduct
// {
//     // Debuff
//     // Freeze opponents input for a couple seconds
//     public string name { get; } = "FreezeInput";
//     public string description { get; } = "Freezes opponents input briefly";
//     public double rarity { get; } = 0.04;
//     public int price { get; } = 180;
//     public int Cooldown { get; } = 12;
//     public void Purchase() { }
// }
class Shop
{
    List<IProduct> products = [];

    // Products
    // Buffs
    // Debuffs

    // Casino? blackjack, slots, roulette osv
}

interface IProduct
{
    string name;
    string description;
    double rarity;
    int price;
    public void Purchase();
}

interface IStaticProduct : IProduct
{
    // Always active
}

interface ITemporaryProduct : IProduct
{
    // Active for set amount of time / rounds
    private int lifetime;
}

interface IAbilityProduct : IProduct
{
    // Can be activated any time but has cooldown
    private int cooldown;
}

class Communism : ITemporaryProduct
{
    // Buff/Debuff
    // Split income evenly with opponent
}


// ==================== BUFFS ===================
class DotTime : IStaticProduct
{
    // Buff
    // Have a chance to fill in the gaps 
    // with the new exciting . (dot) tetromino!
}

class LowerDT : IStaticProduct
{
    // Buff
    // Lowers speed of own board
}

class ScoreMultiplier : IStaticProduct
{
    // Buff
    // Multiplies score
}

class MoneyMultiplier : IStaticProduct
{
    // Buff
    // Multiplies money
}

class MoreRows : IStaticProduct
{
    // Buff
    // Adds more rows to board
}

class TetrominoProjection : IStaticProduct
{
    // Buff
    // Shows where tetromino is going to land
}

class LongerPreview : IStaticProduct
{
    // Buff
    // Shows more upcoming blocks in queue
}

class MoreTetrominoI : ITemporaryProduct
{
    // Buff
    // Gets more I pieces but may result in esoteric I's
}

class CandyCrush : ITemporaryProduct
{
    // Buff
    // Breaks connecting colors
}



class NuclearDrop : ITemporaryProduct
{
    // Buff
    // Loosened y-axis block collision
}

class SlowMotion : IAbilityProduct
{
    // Buff
    // Halves drop speed of board
}

class Skip : IAbilityProduct
{
    // Buff
    // Discard tetromino
}


// ==================== DEBUFFS =================
class IncreaseDT : IStaticProduct
{
    // Debuff
    // Increase DT of opponent
}

class MonochromeBoard : IStaticProduct
{
    // Debuff
    // Make opponents board black & white 
    // (might not pair well if they have with candy crush)
}

class TaxTime : ITemporaryProduct
{
    // Debuff
    // Tax your opponent! YOU ARE THE KING
}

class ExtraBoard : ITemporaryProduct
{
    // Debuff
    // Your opponent will love playing two games at once
    // but watch out, the money is still counted..

    // Different pieces on second board
    // Faster dt as well?
}

class SkipOpponentsPiece : IAbilityProduct
{
    // Debuff
    // Skip the next piece for your opponent
}

class DisableQuickDrop : IAbilityProduct
{
    // Debuff
    // Make your opponent wait ages for their tetrominos
    // to hit the ground
}

class FreezeInput : IAbilityProduct
{
    // Debuff
    // Freeze opponents input for a couple seconds
}
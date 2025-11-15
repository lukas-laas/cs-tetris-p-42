using System.Collections.Generic;

class Shop
{
    List<IProduct> products = new List<IProduct>();

    // Products
    // Buffs
    // Debuffs

    // Casino? blackjack, slots, roulette osv
}

interface IProduct
{
    string name { get; }
    string description { get; }
    double rarity { get; }
    int price { get; }
    void Purchase();
}

interface IStaticProduct : IProduct
{
    // Always active

}

interface ITemporaryProduct : IProduct
{
    // Active for set amount of time / rounds
    int lifetime { get; }
}

interface IAbilityProduct : IProduct
{
    // Can be activated any time but has cooldown
    int cooldown { get; }
}

class Communism : ITemporaryProduct
{
    // Buff/Debuff
    // Split income evenly with opponent
    public string name { get; } = "Communism";
    public string description { get; } = "Splits income evenly with opponent";
    public double rarity { get; } = 0.1;
    public int price { get; } = 100;
    public int lifetime { get; } = 3;
    public void Purchase() { }
}


// ==================== BUFFS ===================
class DotTime : IStaticProduct
{
    // Buff
    // Have a chance to fill in the gaps 
    // with the new exciting . (dot) tetromino!
    public string name { get; } = "DotTime";
    public string description { get; } = "Chance to spawn dot tetromino";
    public double rarity { get; } = 0.2;
    public int price { get; } = 50;
    public void Purchase() { }
}

class LowerDT : IStaticProduct
{
    // Buff
    // Lowers speed of own board
    public string name { get; } = "LowerDT";
    public string description { get; } = "Lowers own drop timer";
    public double rarity { get; } = 0.15;
    public int price { get; } = 40;
    public void Purchase() { }
}

class ScoreMultiplier : IStaticProduct
{
    // Buff
    // Multiplies score
    public string name { get; } = "ScoreMultiplier";
    public string description { get; } = "Multiplies score";
    public double rarity { get; } = 0.05;
    public int price { get; } = 200;
    public void Purchase() { }
}

class MoneyMultiplier : IStaticProduct
{
    // Buff
    // Multiplies money
    public string name { get; } = "MoneyMultiplier";
    public string description { get; } = "Multiplies money earned";
    public double rarity { get; } = 0.05;
    public int price { get; } = 200;
    public void Purchase() { }
}

class MoreRows : IStaticProduct
{
    // Buff
    // Adds more rows to board
    public string name { get; } = "MoreRows";
    public string description { get; } = "Adds extra rows to board";
    public double rarity { get; } = 0.12;
    public int price { get; } = 80;
    public void Purchase() { }
}

class TetrominoProjection : IStaticProduct
{
    // Buff
    // Shows where tetromino is going to land
    public string name { get; } = "TetrominoProjection";
    public string description { get; } = "Shows landing projection";
    public double rarity { get; } = 0.18;
    public int price { get; } = 60;
    public void Purchase() { }
}

class LongerPreview : IStaticProduct
{
    // Buff
    // Shows more upcoming blocks in queue
    public string name { get; } = "LongerPreview";
    public string description { get; } = "Shows more upcoming pieces";
    public double rarity { get; } = 0.16;
    public int price { get; } = 70;
    public void Purchase() { }
}

class MoreTetrominoI : ITemporaryProduct
{
    // Buff
    // Gets more I pieces but may result in esoteric I's
    public string name { get; } = "MoreTetrominoI";
    public string description { get; } = "Increases frequency of I pieces";
    public double rarity { get; } = 0.08;
    public int price { get; } = 120;
    public int lifetime { get; } = 5;
    public void Purchase() { }
}

class CandyCrush : ITemporaryProduct
{
    // Buff
    // Breaks connecting colors
    public string name { get; } = "CandyCrush";
    public string description { get; } = "Breaks connecting colors";
    public double rarity { get; } = 0.07;
    public int price { get; } = 110;
    public int lifetime { get; } = 4;
    public void Purchase() { }
}



class NuclearDrop : ITemporaryProduct
{
    // Buff
    // Loosened y-axis block collision
    public string name { get; } = "NuclearDrop";
    public string description { get; } = "Alters drop collision";
    public double rarity { get; } = 0.03;
    public int price { get; } = 250;
    public int lifetime { get; } = 3;
    public void Purchase() { }
}

class SlowMotion : IAbilityProduct
{
    // Buff
    // Halves drop speed of board
    public string name { get; } = "SlowMotion";
    public string description { get; } = "Halves drop speed";
    public double rarity { get; } = 0.09;
    public int price { get; } = 150;
    public int cooldown { get; } = 10;
    public void Purchase() { }
}

class Skip : IAbilityProduct
{
    // Buff
    // Discard tetromino
    public string name { get; } = "Skip";
    public string description { get; } = "Discard current tetromino";
    public double rarity { get; } = 0.11;
    public int price { get; } = 130;
    public int cooldown { get; } = 6;
    public void Purchase() { }
}


// ==================== DEBUFFS =================
class IncreaseDT : IStaticProduct
{
    // Debuff
    // Increase DT of opponent
    public string name { get; } = "IncreaseDT";
    public string description { get; } = "Increases opponent DT";
    public double rarity { get; } = 0.14;
    public int price { get; } = 90;
    public void Purchase() { }
}

class MonochromeBoard : IStaticProduct
{
    // Debuff
    // Make opponents board black & white 
    // (might not pair well if they have with candy crush)
    public string name { get; } = "MonochromeBoard";
    public string description { get; } = "Turns opponent board monochrome";
    public double rarity { get; } = 0.13;
    public int price { get; } = 95;
    public void Purchase() { }
}

class TaxTime : ITemporaryProduct
{
    // Debuff
    // Tax your opponent! YOU ARE THE KING
    public string name { get; } = "TaxTime";
    public string description { get; } = "Taxes opponent earnings";
    public double rarity { get; } = 0.06;
    public int price { get; } = 140;
    public int lifetime { get; } = 4;
    public void Purchase() { }
}

class ExtraBoard : ITemporaryProduct
{
    // Debuff
    // Your opponent will love playing two games at once
    // but watch out, the money is still counted..

    // Different pieces on second board
    // Faster dt as well?
    public string name { get; } = "ExtraBoard";
    public string description { get; } = "Adds a second board for opponent";
    public double rarity { get; } = 0.02;
    public int price { get; } = 300;
    public int lifetime { get; } = 6;
    public void Purchase() { }
}

class SkipOpponentsPiece : IAbilityProduct
{
    // Debuff
    // Skip the next piece for your opponent
    public string name { get; } = "SkipOpponentsPiece";
    public string description { get; } = "Skips opponents next piece";
    public double rarity { get; } = 0.04;
    public int price { get; } = 170;
    public int cooldown { get; } = 8;
    public void Purchase() { }
}

class DisableQuickDrop : IAbilityProduct
{
    // Debuff
    // Make your opponent wait ages for their tetrominos
    // to hit the ground
    public string name { get; } = "DisableQuickDrop";
    public string description { get; } = "Disables opponents quick drop";
    public double rarity { get; } = 0.05;
    public int price { get; } = 160;
    public int cooldown { get; } = 9;
    public void Purchase() { }
}

class FreezeInput : IAbilityProduct
{
    // Debuff
    // Freeze opponents input for a couple seconds
    public string name { get; } = "FreezeInput";
    public string description { get; } = "Freezes opponents input briefly";
    public double rarity { get; } = 0.04;
    public int price { get; } = 180;
    public int cooldown { get; } = 12;
    public void Purchase() { }
}
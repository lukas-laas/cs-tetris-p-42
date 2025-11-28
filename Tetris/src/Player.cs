
class Player(string name, Board board)
{
    public string Name = name;
    public bool IsAI = false;
    public int Score = 0;
    public int Money = 0;
    private IAbilityProduct? currentAbility;
    public List<IProduct> Inventory = [];
    public Board Board = board;
    public Shop? Shop { get; set; }

    public void Tick(string key)
    {
        Board.Tick(key);
        TickInventory();

        // Every tick it will empty the boards local score and money buffers into the player's total
        this.Score += Board.ScoreBuffer;
        this.Money += Board.MoneyBuffer;
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
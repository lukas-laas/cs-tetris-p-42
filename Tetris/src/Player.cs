class Player
{
    string name;
    bool isAi;
    int score;
    public int Money;
    public List<IProduct> Inventory = [];
    IAbilityProduct? currentAbility;
    public Board Board;
    public Shop? Shop { get; set; }

    public Player(string name, Board board)
    {
        this.Board = board;
        this.name = name;
        isAi = false;
        score = 0;
        Money = 0;
        // currentAbility = new HardDrop(); // TODO: implement
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
class Player
{
    string name;
    bool isAi;
    int score;
    int money;
    List<IProduct> inventory = [];
    IAbilityProduct? currentAbility;

    public Player(string name)
    {
        this.name = name;
        isAi = false;
        score = 0;
        money = 0;
        // currentAbility = new HardDrop(); // TODO: implement
    }

    public void UseAbility()
    {
        if (currentAbility?.Cooldown == 0)
        {
            // currentAbility.Use(); // TODO: implement
        }
    }
}
public enum GameMode
{
    MultiPlayer,
    SinglePlayer
}

public enum EggState 
{
    None, 
    Spawned,
    Player
}

public enum Winner
{
    Player1,
    Player2,
    Loose
}

public enum TileType 
{
    Item0 = 0,
    Item1 = 1,
    Item2 = 2,
    Item3 = 3,
    Item4 = 4,
    Item5 = 5,
    Item6 = 6,
    Item7 = 7,
    Item8 = 8,
    Item9 = 9,
    Item10 = 10,
    Item11 = 11,
    Item12 = 12,
    Item13 = 13,
    Item14 = 14,
    Item15 = 15,

    None
}

public enum PowerUpType 
{
    SwapCard = 0,
    ViewCard = 1,
    AddCombinations = 2,
    DropEgg = 3, 
}

public enum TileState
{
    Unrevealed,
    Revealed,
    Highlighted,
    Player
}
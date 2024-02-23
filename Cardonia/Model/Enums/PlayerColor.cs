namespace Cardonia.Model.Enums;

public enum PlayerColor
{
    RED,
    BLU,
}

public static class PlayerColorOverloading
{
    public static PlayerColor Other(this PlayerColor c) 
    {
        if (c == PlayerColor.RED) return PlayerColor.BLU;
        else return PlayerColor.RED;
    }
}



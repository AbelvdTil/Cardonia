using Cardonia.Model.Cards;
using Cardonia.Model.Enums;

namespace Cardonia.Model.Table;

public class Table
{
    public event Func<Task>? UpdateEvent;

    /// <summary>
    /// Cards on the board.
    /// </summary>
    private IDictionary<PlayerColor, ICollection<(int pos, Card card)>> _boardDict { get; set; } = new Dictionary<PlayerColor, ICollection<(int pos, Card card)>>();

    /// <summary>
    /// Cards in the hand.
    /// </summary>
    private IDictionary<PlayerColor, ICollection<Card>> _handDict { get; set; } = new Dictionary<PlayerColor, ICollection<Card>>();

    /// <summary>
    /// Cards in the deck.
    /// </summary>
    private IDictionary<PlayerColor, int> _deckDict = new Dictionary<PlayerColor, int>();

    /// <summary>
    /// The amount of mana the player has.
    /// </summary>
    private IDictionary<PlayerColor, (int used, int max)> _manaDict = new Dictionary<PlayerColor, (int used, int max)>();

    /// <summary>
    /// Name of player.
    /// </summary>
    private IDictionary<PlayerColor, string> _nameDict = new Dictionary<PlayerColor, string>();

    public PlayerColor ActivePlayer { get; private set; } = PlayerColor.BLU;

    #region dict getters
    public ICollection<(int pos, Card card)> Board(PlayerColor c) => _boardDict[c]; 

    public int Deck (PlayerColor c) => _deckDict[c];

    public ICollection<Card> Hand(PlayerColor c) => _handDict[c];

    public (int used, int max) Mana(PlayerColor c) => _manaDict[c];

    public string Name (PlayerColor c) => _nameDict[c];

    #endregion

    public PlayerColor? JoinTable(string name)
    {
        if (_nameDict.Count > 2) return null;

        PlayerColor color = _nameDict.Count == 0 ? PlayerColor.BLU : PlayerColor.RED;

        _boardDict.Add(color, new List<(int pos, Card card)>());

        _nameDict.Add(color, name);

        return color;
    }

    public void PlayCard(PlayerColor c, Card card, int pos)
    {
        if (!IsActive(c)) return;

        _handDict[c].Remove(card);
        _boardDict[c].Add((pos, card));

        card.WhenPlayed();
    }

    public void NextTurn(PlayerColor c)
    {
        if (!IsActive(c)) return;

        ActivePlayer = ActivePlayer.Other();

        (int _, int max) = Mana(ActivePlayer);

        max++;

        _manaDict[ActivePlayer] = (max, max);
    }

    #region Helper methods

    private bool IsActive(PlayerColor c)
    {
        return ActivePlayer == c;
    }

    public void Synchronize()
    {
        SynchronizeEvent?.Invoke();
    }

    #endregion

}

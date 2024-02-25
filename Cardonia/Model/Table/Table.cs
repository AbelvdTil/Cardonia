using Cardonia.Model.Cards;
using Cardonia.Model.Enums;
using Cardonia.Model.Util;

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
    private IDictionary<PlayerColor, (int current, int max)> _manaDict = new Dictionary<PlayerColor, (int current, int max)>();

    /// <summary>
    /// Name of player.
    /// </summary>
    private IDictionary<PlayerColor, string> _nameDict = new Dictionary<PlayerColor, string>();

    public PlayerColor ActivePlayer { get; private set; } = PlayerColor.BLU;

    #region dict getters
    public ICollection<(int pos, Card card)> Board(PlayerColor c) => _boardDict[c]; 

    public int Deck (PlayerColor c) => _deckDict[c];

    public ICollection<Card> Hand(PlayerColor c) => _handDict[c];

    public (int current, int max) Mana(PlayerColor c) => _manaDict[c];

    public string Name (PlayerColor c) => _nameDict[c];

    public bool FullTable => _nameDict.Count == 2;

    #endregion

    public PlayerColor? JoinTable(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        if (_nameDict.Values.Any(n => n == name))
        {
            Update();

            return _nameDict.FirstOrDefault(n => n.Value == name).Key;
        }

        if (FullTable) return null;

        PlayerColor color = _nameDict.Count == 0 ? PlayerColor.BLU : PlayerColor.RED;

        _boardDict.Add(color, new List<(int pos, Card card)>());
        _handDict.Add(color, new List<Card>());
        _deckDict.Add(color, 60);
        
        _nameDict.Add(color, name);

        if (color == PlayerColor.BLU)
        {
            DrawCard(color, 7);
            _manaDict.Add(color, (1, 1));
        }
        else
        {
            DrawCard(color, 6);
            _manaDict.Add(color, (0, 0));
        }

        Update();

        return color;
    }

    public OpponentInfo? GetOpponentInfo(PlayerColor c)
    {
        PlayerColor oc = c.Other();

        if (!FullTable) return null;

        return new OpponentInfo()
        {
            Name = _nameDict[oc],
            DeckSize = _deckDict[oc],
            HandSize = _handDict[oc].Count,
            Mana = _manaDict[oc],
            Board = _boardDict[oc],
        };
    }

    public void DrawCard(PlayerColor c, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            _deckDict[c]--;
            _handDict[c].Add(new Card()
            {
                Attack = 2,
                Health = 3,
                Name = "Gunter",
                Table = this
            });
        }

        Update();
    }

    #region Card Actions

    public void PlayCard(PlayerColor c, Card? play, Card? sacrafice, int pos)
    {
        if (!IsActive(c) || play is null || sacrafice is null) return;

        (int current, int max) mana = _manaDict[c];

        if (mana.current < play.Cost) return;

        _manaDict[c] = (mana.current - play.Cost, mana.max);

        play.Owner = c;
        play.Table = this;

        _handDict[c].Remove(play);
        _handDict[c].Remove(sacrafice);

        _boardDict[c].Add((pos, play));

        play.OnPlayed();

        Update();
    }

    public void AttackCard(PlayerColor c, int actorPos, int recipientPos)
    {
        GetCard(c, actorPos)?.AttackCard(GetCard(c.Other(), recipientPos));

        Update();
    }

    public void UseCard(PlayerColor c, int actorPos)
    {
        GetCard(c, actorPos)?.OnUse();

        Update();
    }

    public void RemoveCard(PlayerColor c, Card card)
    {
        _boardDict[c].Remove(_boardDict[c].First(b => b.card == card));

        Update();
    }

    #endregion

    public void NextTurn(PlayerColor c)
    {
        if (!IsActive(c)) return;

        ActivePlayer = ActivePlayer.Other();

        (int _, int max) = Mana(ActivePlayer);

        max++;

        max = int.Min(10, max);

        _manaDict[ActivePlayer] = (max, max);

        DrawCard(ActivePlayer, 2);

        foreach(Card card in _boardDict[c.Other()].Select(b => b.card))
        {
            card.IsUsed = false;
        }

        Update();
    }

    #region Helper methods

    private bool IsActive(PlayerColor c)
    {
        return ActivePlayer == c;
    }

    private void Update()
    {
        UpdateEvent?.Invoke();
    }

    private void Reset()
    {
        _boardDict.Clear();
        _handDict.Clear();
        _deckDict.Clear();
        _manaDict.Clear();
        _nameDict.Clear();
    }

    private Card? GetCard(PlayerColor c, int pos)
    {
        (int pos, Card card)? cardpos = _boardDict[c].FirstOrDefault(b => b.pos == pos);
        return cardpos?.card ?? null;
    }

    #endregion
}

public record OpponentInfo
{
    public required string Name {  get; set; }
    public required int DeckSize { get; set; }
    public required int HandSize { get; set; }
    public required (int current, int max) Mana { get; set; }

    public required ICollection<(int pos, Card card)> Board { get; set; } 
}

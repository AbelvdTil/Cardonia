using Cardonia.Model.Cards;

namespace Cardonia.Model.Table;
public class PlayerTable
{
    public string PlayerName { get; set; } = "";

    public int DeckSize { get; set; } = 40;

    public int HandSize { get; set; } = 2;

    public required TableManager TableManager { get; set; }

    public ICollection<(Card card, int pos)> Board { get; } = new List<(Card, int)>();

    public PlayerTable? GetOpponent()
    {
        return TableManager.GetOtherPlayer(this);
    }

    public Card? GetAtPos(int pos)
    {
        return Board
            .Where(b => b.pos == pos)
            .Select(b => b.card)
            .FirstOrDefault();
    }

    public void DrawCard()
    {
        DeckSize--;
        HandSize++;

        TableManager.Synchronizer.Synchronize();
    }

    public void PlayCard(Card card, int pos)
    {
        if (Board.Any(b => b.pos == pos)) return;

        HandSize--;
        Board.Add((card, pos));
        card.Table = this;

        TableManager.Synchronizer.Synchronize();
    }

    public void RemoveCard(Card card)
    {
        if (!Board.Any(b => b.card == card)) return;

        Board.Remove(Board.First(b => b.card == card));

        TableManager.Synchronizer.Synchronize();
    }
}


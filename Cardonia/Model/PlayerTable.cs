namespace Cardonia.Model;
public class PlayerTable
{
    public string PlayerName { get; set; } = "";

    public int DeckSize { get; set; } = 40;

    public int HandSize { get; set; } = 2;

    public TableManager TableManager { get; set; }

    public IDictionary<int, Card> Board { get; } = new Dictionary<int, Card>();

    public Card? GetAtPos(int pos)
    {
        if (Board.TryGetValue(pos, out Card card))
        {
            return card;
        }
        else
        {
            return null;
        }
    }

    public void DrawCard()
    {
        DeckSize--;
        HandSize++;

        TableManager.UpdateView();
    }

    public void PlayCard(int pos) 
    {
        if (!Board.ContainsKey(pos))
        {
            HandSize--;
            Board.Add(pos, new Card()
            {
                Name = "Gunther",
                Attack = 5,
                Health = 2
            });

            TableManager.UpdateView();
        }
    }
}


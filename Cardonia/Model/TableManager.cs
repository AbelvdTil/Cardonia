namespace Cardonia.Model;

public class TableManager
{
    private IDictionary<string, PlayerTable> Players { get; set; } = new Dictionary<string, PlayerTable>();

    private IEnumerable<IRerenderable> Rerenderables { get; set; } = new List<IRerenderable>();

    public void UpdateView()
    {
        foreach(IRerenderable component in Rerenderables)
        {
            component.Rerender();
        }
    }

    public bool Join(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        if (Players.Count() > 2) return false;

        if (Players.ContainsKey(name))
        {
            Players[name] = Players[name];
        }
        else
        {
            Players.Add(name, new PlayerTable()
            {
                PlayerName = name,
                TableManager = this
            });
        }

        UpdateView();

        return true;
    }

    public void Leave(string name)
    {
        if (Players.ContainsKey(name))
        {
            Players.Remove(name);
        }

        UpdateView();
    }


    public void Subscribe(IRerenderable rerenderable)
    {
        Rerenderables = Rerenderables.Append(rerenderable);
    }

    public PlayerTable? GetTable(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;

        if (Players.TryGetValue(name, out PlayerTable table))
        {
            return table;
        }

        return null;
    }

    public PlayerTable? GetEnemyTable(string name)
    {
        return Players.Where(p => p.Key != name).Select(p => p.Value).FirstOrDefault();
    }
}


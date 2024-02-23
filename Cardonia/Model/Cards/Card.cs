using Cardonia.Model.Enums;
using Cardonia.Model.Table;

namespace Cardonia.Model.Cards;

public class Card
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int Attack { get; set; }

    public int Health { get; set; }

    public PlayerTable? Table { get; set; }

    public Card()
    {
        Name = "Gunther";
        Attack = 3;
        Health = 2;
    }

    public void AttackCard(Card recipient)
    {
        recipient.TakeDamage(Attack, DamageType.MELEE);
    }

    public void TakeDamage(int amount, DamageType type)
    {
        if (Table is null) throw new ArgumentNullException("No table set.");

        Health -= int.Min(Health, amount);

        if (Health == 0)
        {
            Table.RemoveCard(this);
        }
    }

    public void WhenPlayed()
    {

    }

    public void WhenActivated()
    {

    }

    public void WhenAttacking()
    {

    }


}

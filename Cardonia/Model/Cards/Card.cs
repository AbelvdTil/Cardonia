using Cardonia.Model.Enums;

namespace Cardonia.Model.Cards;

public class Card
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public int Cost { get; set; } = 2;

    public int Attack { get; set; }

    public int Health { get; set; }

    public bool IsUsed { get; set; } = true;

    public Table.Table Table { get; set; } = default!;

    public PlayerColor Owner { get; set; } = default!;

    public void AttackCard(Card? recipient)
    {
        if (recipient == null) return;

        if (IsUsed) return;

        OnAttacking();

        recipient.TakeDamage(Attack, DamageType.MELEE);

        TakeDamage(recipient.Attack, DamageType.MELEE);

        IsUsed = true;
    }

    public void OnUse()
    {
        if (IsUsed) return;

        Health += 2;
        Attack += 1;

        IsUsed = true;
    }

    public void TakeDamage(int amount, DamageType type)
    {
        if (Table is null) throw new ArgumentNullException("No table set.");

        Health -= int.Min(Health, amount);

        if (Health == 0)
        {
            Table.RemoveCard(Owner, this);
        }
    }

    public void OnPlayed()
    {
        Table.DrawCard(Owner);
    }

    private void OnAttacking()
    {
        Table.DrawCard(Owner);
    }


}

using Godot;
using System;

public partial class CreditManager : Node
{
    [Signal]
    public delegate void CreditUpdatedEventHandler(int credit);

    public int CurrentCredit { get; private set; }

    public override void _Ready()
    {
        IncreaseCredit(GetNode<MetaProgression>("/root/MetaProgression").GetCredit());
        GetNode<GameEvents>("/root/GameEvents").CreditAdded += OnCreditAdded;
    }

    private void IncreaseCredit(int credit)
    {
        CurrentCredit += credit;
        EmitSignal(nameof(CreditUpdated), CurrentCredit);
    }

    public void OnCreditAdded(int credit) => IncreaseCredit(credit);

    public override void _ExitTree() => GetNode<GameEvents>("/root/GameEvents").CreditAdded -= OnCreditAdded;
}

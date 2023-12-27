using System.Collections.Generic;
using Amegakure.Starkane.PubSub;
using Amegakure.Starkane.UI.NonDiegetic;

public class CombatCancel : ICommand
{
    private Dialog dialog;

    public CombatCancel(Dialog dialog)
    { this.dialog = dialog; }

    public string Name => "Cancelar";

    public void Do()
    {
        dialog.Hide();
        Dictionary<string, object> eventData = new();
        EventManager.Instance.Publish(GameEvent.CUTSCENE_COMBAT_CANCEL, eventData);        
    }
}
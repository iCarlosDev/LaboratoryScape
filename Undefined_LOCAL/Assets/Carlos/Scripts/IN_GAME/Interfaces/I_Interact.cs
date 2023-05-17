using EPOOutline;
using TMPro;
using UnityEngine;

public interface I_Interact
{
    public Outlinable outliner { get; set; }

    public void SetOultine(bool shouldActivate);

    public void SetTextInteract(bool shouldShow);
}

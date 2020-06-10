// ================================================================================================================================
// File:        PlayerInteraction.cs
// Description:	Allows the player to interact with objects in the game
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject InteractionAlertIcon; //This icon will be displayed above the player when they are within range of an object they can interact with
    private void Start() { InteractionAlertIcon.SetActive(false); }


}

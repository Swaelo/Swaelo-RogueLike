// ================================================================================================================================
// File:        Player.cs
// Description:	Stores info about the player in a singleton object so other scripts can easily access them
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class Player : MonoBehaviour
{
    //Singleton Instance
    public static Player Instance = null;
    private void Awake() { Instance = this; }

    //Hiding/Unhiding
    public bool Hidden = false;
    public void SetHidden(bool Hide)
    {
        gameObject.SetActive(!Hide);
        Hidden = Hide;
    }

    private void Start()
    {
        SetHidden(true);
    }
}

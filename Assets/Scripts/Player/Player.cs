// ================================================================================================================================
// File:        Player.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class Player : MonoBehaviour
{
    //Singleton Instance
    public static Player Instance = null;
    public void Awake() { Instance = this; }
}

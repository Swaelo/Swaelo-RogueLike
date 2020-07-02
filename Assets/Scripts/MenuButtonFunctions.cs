// ================================================================================================================================
// File:        MenuButtonFunctions.cs
// Description:	
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuButtonFunctions : MonoBehaviour
{
    public void ClickPlayButton()
    {
        SceneManager.LoadScene(1);
    }
}

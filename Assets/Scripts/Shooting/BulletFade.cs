// ================================================================================================================================
// File:        BulletFade.cs
// Description:	Destroys the bullet after a short timer has passed
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class BulletFade : MonoBehaviour
{
    //Sets the bullet to be destroyed after a short amount of time has passed
    public void SetFade(float Timer)
    {
        Invoke("Fade", Timer);
    }

    private void Fade()
    {
        Destroy(gameObject);
    }
}

// ================================================================================================================================
// File:        MuzzleFlash.cs
// Description:	Displays a muzzle flash effect on the players weapon every time they fire a projectile
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public SpriteRenderer WeaponRenderer;   //Sprite renderer used to draw the players weapon
    public Sprite DefaultWeaponSprite;  //Normal weapon sprite
    public Sprite MuzzleFlashWeaponSprite;  //Weapon sprite displayed during muzzle flash effect
    private float MuzzleFlashDuration = 0.025f; //Duration of the muzzle flash effect

    //Triggers the muzzle flash effect
    public void TriggerMuzzleFlash()
    {
        WeaponRenderer.sprite = MuzzleFlashWeaponSprite;
        CancelInvoke();
        Invoke("EndMuzzleFlash", MuzzleFlashDuration);
    }

    //Disables the muzzle flash effect, changing back to the default weapon sprite
    private void EndMuzzleFlash()
    {
        WeaponRenderer.sprite = DefaultWeaponSprite;
    }
}

// ================================================================================================================================
// File:        PlayerWeaponAiming.cs
// Description:	Positions the players weapon so its always aiming towards the crosshair
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerWeaponAiming : MonoBehaviour
{
    public PlayerMovement PlayerMovement;   //Used to check what direction the character is facing
    public GameObject PlayerWeapon; //The players current weapon object
    public SpriteRenderer WeaponSprite; //Renderer used to draw the players weapon
    private float WeaponYOffset = -0.05f;    //Weapon is offset on Y axis to appear in the players hand
    private float IgnoreCursorDistance = 1f;    //Weapon sprite wont get flipped if cursor is this close
    public bool WeaponFlipped = false;  //Tracks when the weapon sprite is being flipped

    public void Update()
    {
        //Get the current direction from the gun to the crosshair, which is where the gun should be aiming
        Vector3 CursorPos = CursorLocation.Instance.Get();
        Vector3 GunToCrosshairDirection = Vector3.Normalize(PlayerWeapon.transform.position - CursorPos);
        WeaponFlipped = GunToCrosshairDirection.x > 0.25f;

        //Flip and reposition the weapon sprite based on which direction the crosshair is
        float PlayerToCursorDistance = Vector3.Distance(transform.position, CursorPos);
        if(PlayerToCursorDistance > IgnoreCursorDistance)
        {
            WeaponSprite.flipX = WeaponFlipped;
            PlayerWeapon.transform.localPosition = new Vector3(PlayerWeapon.transform.localPosition.x, WeaponYOffset, PlayerWeapon.transform.localPosition.z);
        }

        //Map the crosshairs y direction value onto the number range of possible rotation values
        float AimRatio = (GunToCrosshairDirection.y - -1) / (1 - -1);
        float MappedRotation = AimRatio * (-90 - 90) + 90;

        //Aim the weapon towards the crosshair, and set the correct draw order
        PlayerWeapon.transform.rotation = Quaternion.Euler(0f, 0f, WeaponFlipped ? -MappedRotation : MappedRotation);
        WeaponSprite.sortingOrder = PlayerMovement.FacingDirection == Direction.North ? 1 : 3;
    }
}

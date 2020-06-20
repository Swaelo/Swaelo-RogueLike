// ================================================================================================================================
// File:        PlayerShooting.cs
// Description:	Gives the player control of their gun so they can shoot at the enemies
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    //Setup the custom cursor crosshair when the game starts up
    public Texture2D CrosshairTexture;
    private Vector2 CursorOffset = new Vector2(7.5f, 7.5f);
    private void Start() { Cursor.SetCursor(CrosshairTexture, CursorOffset, CursorMode.Auto); }

    public GameObject PlayerGun;   //Players weapon gameobject
    public SpriteRenderer PlayerGunSprite;  //Sprite used to draw the players weapon

    public float GunYOffset = -0.05f;  //Distance the gun is offset from the players location on the Y axis
    private float IgnoreCursorDistance = 1f; //The gun wont change its facing direction if the distance between player and cursor is lower than this value

    //Rotate the gun so its facing towards the players crosshair
    public void Update()
    {
        //Find the direction from the gun to the crosshair to determine which direction the gun should be aiming
        Vector3 CursorPos = CursorLocation.Instance.Get();
        Vector3 GunToCrosshairDirection = Vector3.Normalize(PlayerGun.transform.position - CursorPos);
        bool AimingRight = GunToCrosshairDirection.x > .25f;

        //Flip and reposition the gun sprite based on which direction the crosshair is from it (and as long as the mouse cursor isnt directly on top of the player)
        float PlayerToCursorDistance = Vector3.Distance(transform.position, CursorPos);
        if (PlayerToCursorDistance > IgnoreCursorDistance)
        {
            PlayerGunSprite.flipX = AimingRight;
            PlayerGun.transform.localPosition = new Vector3(PlayerGun.transform.localPosition.x, GunYOffset, PlayerGun.transform.localPosition.z);
        }

        //Map the crosshairs y direction value (which always falls in the range of 1 to -1), onto the range of possible rotation values
        float AimRatio = (GunToCrosshairDirection.y - -1) / (1 - -1);
        float MappedRotation = AimRatio * (-90 - 90) + 90;
        //Point the gun sprite straight towards the crosshair by applying a new quaternion created from this mapped rotation value
        PlayerGun.transform.rotation = Quaternion.Euler(0f, 0f, AimingRight ? -MappedRotation : MappedRotation);

        //Change the gun sprites draw order so it renders behind the player when they're facing northwards
        PlayerGunSprite.sortingOrder = GetComponent<PlayerMove>().FacingDirection == Direction.North ? 1 : 3;
    }
}
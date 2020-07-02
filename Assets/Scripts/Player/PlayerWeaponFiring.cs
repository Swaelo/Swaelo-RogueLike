// ================================================================================================================================
// File:        PlayerWeaponFiring.cs
// Description:	Allows the player to fire their weapon
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;

public class PlayerWeaponFiring : MonoBehaviour
{
    public PlayerWeaponAiming WeaponAiming; //Used to check when the weapon sprite is being flipped
    public GameObject NormalBulletSpawn;    //Bullets spawned here normally
    public GameObject FlippedBulletSpawn;   //Bullets spawned here when weapon is flipped
    public GameObject BulletPrefab; //Bullets fired by the players weapon
    private float FiringCooldown = 0.1f;    //Time between shots
    private bool WeaponHot = false; //Cannot fire while true
    public MuzzleFlash MuzzleFlash; //Used to trigger muzzle flash effect when firing projectiles

    private void Update()
    {
        //Fire another projectile whenever the button is pressed and the weapon is off cooldown
        if (Input.GetMouseButton(0) && !WeaponHot)
            FireBullet();
    }

    //Fires a bullet towards the crosshair
    private void FireBullet()
    {
        //Start the weapon cooldown
        WeaponHot = true;
        Invoke("CoolWeapon", FiringCooldown);

        //Figure out which direction the bullet will be fired, and the position/rotation it should be spawned in with
        Vector3 ShootDirection = Vector3.Normalize(CursorLocation.Instance.Get() - transform.position);
        float ShootAngle = Vector3.Angle(ShootDirection, transform.right);
        Quaternion BulletRotation = Quaternion.Euler(0f, 0f, ShootDirection.y > 0f ? ShootAngle : -ShootAngle);
        Vector3 BulletPosition = WeaponAiming.WeaponFlipped ? FlippedBulletSpawn.transform.position : NormalBulletSpawn.transform.position;

        //Spawn a new bullet in with these values
        GameObject BulletSpawn = Instantiate(BulletPrefab, BulletPosition, BulletRotation);
        BulletSpawn.GetComponent<BulletTravel>().InitializeBullet(ShootDirection);

        //Display the muzzle flash effect
        MuzzleFlash.TriggerMuzzleFlash();
    }

    //Allows the weapon to be fired again
    private void CoolWeapon()
    {
        WeaponHot = false;
    }
}

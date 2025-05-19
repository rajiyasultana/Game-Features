using ShootingSystem;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime;

    public void Shoot(Vector3 direction)
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;
        GameObject projectile = ObjectPooling.Instance.GetProjectile();
        if (projectile != null)
        {
            projectile.transform.position = firePoint.position;
            projectile.GetComponent<Projectile>().Fire(direction);
        }
    }
}
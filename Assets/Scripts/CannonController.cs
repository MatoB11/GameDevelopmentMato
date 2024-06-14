using UnityEngine;

public class CannonController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float detectionRadius = 10f;
    public GameObject projectilePrefab; // Prefab projektilu
    public Transform firePoint; // Miesto, odkiaľ sa bude strieľať
    public float fireRate = 1f; // Rýchlosť streľby
    public float reloadTime = 1.5f; // Čas potrebný na načítanie kanónu
    public GameObject rangeCirclePrefab; // Prefab pre vizualizáciu dostrelu

    private float fireCooldown;
    private bool isReloading = false;
    private Transform currentTarget = null;
    private GameObject currentRangeCircle; // Aktuálna inštancia vizualizácie

    void Update()
    {
        if (currentTarget == null || currentTarget.GetComponent<EnemyHealth>().IsDead())
        {
            currentTarget = FindNearestEnemy();
        }

        if (currentTarget != null)
        {
            RotateTowards(currentTarget);
            if (!isReloading && fireCooldown <= 0f)
            {
                Shoot(currentTarget);
                fireCooldown = 1f / fireRate;
                isReloading = true; // Spustenie načítania po streľbe
                Invoke("FinishReloading", reloadTime); // Zavolať funkciu po uplynutí času načítania
            }
        }

        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SelectCannon();
        }
    }

    Transform FindNearestEnemy()
    {
        EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance && distance <= detectionRadius)
            {
                minDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }

        return nearestEnemy;
    }

    void RotateTowards(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Shoot(Transform target)
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Projectile projectile = projectileInstance.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Seek(target);
        }
    }

    void FinishReloading()
    {
        isReloading = false;
    }

    void SelectCannon()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (currentRangeCircle == null)
                {
                    ShowRange();
                }
                else
                {
                    HideRange();
                }
            }
        }
    }

    void ShowRange()
    {
        currentRangeCircle = Instantiate(rangeCirclePrefab, transform.position, Quaternion.Euler(90, 0, 0));
        float scale = detectionRadius * 2; // Nastavenie správnej veľkosti
        currentRangeCircle.transform.localScale = new Vector3(scale, scale, 1); // Uistite sa, že os Z je 1
        currentRangeCircle.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z); // Presné zarovnanie
        currentRangeCircle.transform.SetParent(transform, false); // Nastavenie rodiča na kanón
    }

    void HideRange()
    {
        if (currentRangeCircle != null)
        {
            Destroy(currentRangeCircle);
        }
    }
}

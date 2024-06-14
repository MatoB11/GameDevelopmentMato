using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform[] path; // Pole s transformami bodov na ceste
    public float speed = 5f; // Rýchlosť pohybu nepriateľa
    public float rotationSpeed = 5f; // Rýchlosť rotácie nepriateľa

    private int targetWaypointIndex = 0; // Index aktuálneho cieľového bodu

    void Start()
    {
        // Nastavíme cieľový waypoint na prvý bod v ceste
        SetNextWaypoint();
    }

    void Update()
    {
        if (path != null && targetWaypointIndex < path.Length)
        {
            // Smer pohybu k cieľovému bodu
            Vector3 direction = path[targetWaypointIndex].position - transform.position;

            // Pohyb nepriateľa smerom k cieľovému bodu
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

            // Otáčanie nepriateľa smerom k cieľovému bodu
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Ak sme blízko k cieľovému bodu, prejdeme na ďalší bod
            if (Vector3.Distance(transform.position, path[targetWaypointIndex].position) < 0.1f)
            {
                SetNextWaypoint();
            }
        }
    }

    void SetNextWaypoint()
    {
        // Ak sme dosiahli koniec cesty, nepriateľ sa zničí
        if (targetWaypointIndex >= path.Length - 1)
        {
            Destroy(gameObject);
            return;
        }

        // Nastavíme ďalší cieľový bod na ceste
        targetWaypointIndex++;
    }
}

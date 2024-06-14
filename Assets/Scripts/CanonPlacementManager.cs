using UnityEngine;
using UnityEngine.UI;

public class CannonPlacementManager : MonoBehaviour
{
    public Image cannonIcon; // Ikona kanónu v UI
    public GameObject cannonPrefab; // Prefab kanónu
    public GameObject cannonPreviewPrefab; // Prefab náhľadu kanónu
    public Transform[] waypoints; // Pole waypointov pozdĺž cesty
    public int cannonCost = 50; // Cena kanónu

    public LayerMask placementLayerMask; // Vrstva pre povolené umiestnenie
    public LayerMask forbiddenLayerMask; // Vrstva pre zakázané umiestnenie

    public Material invalidPlacementMaterial; // Materiál pre neplatné umiestnenie

    private bool isPlacingCannon = false;
    private GameObject cannonPreviewInstance;
    private CoinManager coinManager;
    private MeshRenderer[] previewRenderers;
    private Material[] originalMaterials;

    void Start()
    {
        // Priradenie metódy na kliknutie na ikonu kanónu
        cannonIcon.GetComponent<Button>().onClick.AddListener(OnCannonIconClick);
        coinManager = FindObjectOfType<CoinManager>();
    }

    void Update()
    {
        if (isPlacingCannon)
        {
            UpdateCannonPreview();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlaceCannon();
            }
        }
    }

    void OnCannonIconClick()
    {
        // Aktivovanie režimu umiestňovania kanónu, ak má hráč dostatok coinov
        if (coinManager != null && coinManager.SpendCoins(cannonCost))
        {
            isPlacingCannon = true;
            // Vytvorenie inštancie náhľadu kanónu
            cannonPreviewInstance = Instantiate(cannonPreviewPrefab);
            previewRenderers = cannonPreviewInstance.GetComponentsInChildren<MeshRenderer>();

            // Uloženie pôvodných materiálov
            originalMaterials = new Material[previewRenderers.Length];
            for (int i = 0; i < previewRenderers.Length; i++)
            {
                originalMaterials[i] = previewRenderers[i].material;
            }
        }
    }

    void UpdateCannonPreview()
    {
        if (cannonPreviewInstance == null)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
        {
            // Skontrolovanie, či pozícia nie je na zakázanej vrstve
            if (!Physics.CheckSphere(hit.point, 0.5f, forbiddenLayerMask))
            {
                cannonPreviewInstance.SetActive(true); // Zobraziť náhľad kanónu
                cannonPreviewInstance.transform.position = hit.point;
                SetPreviewMaterial(originalMaterials); // Nastaviť pôvodný materiál
            }
            else
            {
                cannonPreviewInstance.SetActive(true); // Stále zobraziť náhľad kanónu
                cannonPreviewInstance.transform.position = hit.point;
                SetPreviewMaterial(invalidPlacementMaterial); // Nastaviť materiál pre neplatné umiestnenie
            }
        }
        else
        {
            cannonPreviewInstance.SetActive(false); // Skryť náhľad kanónu, ak nie je na platnej vrstve
        }
    }

    void SetPreviewMaterial(Material material)
    {
        foreach (var renderer in previewRenderers)
        {
            renderer.material = material;
        }
    }

    void SetPreviewMaterial(Material[] materials)
    {
        for (int i = 0; i < previewRenderers.Length; i++)
        {
            previewRenderers[i].material = materials[i];
        }
    }

    void TryPlaceCannon()
    {
        if (cannonPreviewInstance == null || !cannonPreviewInstance.activeSelf)
        {
            return;
        }

        Vector3 position = cannonPreviewInstance.transform.position;
        Ray ray = new Ray(position + Vector3.up * 10, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayerMask))
        {
            // Skontrolovanie, či pozícia nie je na zakázanej vrstve
            if (!Physics.CheckSphere(hit.point, 0.5f, forbiddenLayerMask))
            {
                PlaceCannon(hit.point);
            }
        }
    }

    void PlaceCannon(Vector3 position)
    {
        GameObject placedCannon = Instantiate(cannonPrefab, position, Quaternion.identity);
        RotateTowardsNearestWaypoint(placedCannon.transform);

        isPlacingCannon = false; // Deaktivovanie režimu umiestňovania po umiestnení kanónu

        // Zničenie inštancie náhľadu kanónu
        Destroy(cannonPreviewInstance);
    }

    void RotateTowardsNearestWaypoint(Transform cannonTransform)
    {
        Transform nearestWaypoint = null;
        float minDistance = Mathf.Infinity;

        foreach (var waypoint in waypoints)
        {
            float distance = Vector3.Distance(cannonTransform.position, waypoint.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestWaypoint = waypoint;
            }
        }

        if (nearestWaypoint != null)
        {
            Vector3 direction = nearestWaypoint.position - cannonTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            cannonTransform.rotation = Quaternion.Euler(0f, lookRotation.eulerAngles.y, 0f); // Iba otáčanie na osi Y
        }
    }
}

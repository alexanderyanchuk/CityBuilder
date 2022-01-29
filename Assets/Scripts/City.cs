using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class City : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, Building> buildings = new Dictionary<Vector2Int, Building>();

    [SerializeField] private GameObject gridPivot;
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private GameObject buildingGhostGameObject;

    private Collider gridCollider;
    private Renderer gridRenderer;
    private Building buildingGhost;
    private bool isInBuildMode;
    private uint totalPower;
    private Vector3 dragOrigin;

    public UnityEvent<uint> totalPowerChanged;

    private void Start()
    {
        gridCollider = grid.GetComponent<Collider>();
        gridRenderer = grid.GetComponent<Renderer>();
        buildingGhost = buildingGhostGameObject.GetComponent<Building>();

        UpdateGrid();
    }

    private void Update()
    {
        if (!isInBuildMode)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitBuildMode();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (gridCollider.Raycast(ray, out RaycastHit hit, 100.0f))
        {
            CityConfig config = GameManager.instance.config;
            Vector2Int gridPosition = ConvertWorldToGridPosition(hit.point);
            buildingGhostGameObject.transform.localPosition = new Vector3(gridPosition.x * config.cellSize, 0, gridPosition.y * config.cellSize);

            Vector2Int extent = new Vector2Int(buildingGhost.width, buildingGhost.length);
            if (CanBuildAt(gridPosition, extent))
            {
                buildingGhost.mode = Building.Mode.Transparent;

                Vector3 dragDelta = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
                if (Input.GetMouseButtonUp(0) && dragDelta.magnitude < 0.01f)
                {
                    GameObject newBuildingGameObject = Instantiate(buildingPrefab, buildingGhostGameObject.transform.localPosition, buildingGhostGameObject.transform.rotation, transform);
                    Building newBuilding = newBuildingGameObject.GetComponent<Building>();
                    newBuilding.width = buildingGhost.width;
                    newBuilding.length = buildingGhost.length;
                    newBuilding.height = buildingGhost.height;
                    newBuilding.power = buildingGhost.power;
                    newBuilding.showGrid = true;
                    newBuilding.mode = Building.Mode.Normal;
                    buildings.Add(gridPosition, newBuilding);

                    totalPower += newBuilding.power;
                    totalPowerChanged?.Invoke(totalPower);

                    ExitBuildMode();
                }
            }
            else
            {
                buildingGhost.mode = Building.Mode.Blocked;
            }
        }
    }

    private bool CanBuildAt(Vector2Int newBuildingPosition, Vector2Int newBuildingSize)
    {
        CityConfig config = GameManager.instance.config;
        if (newBuildingPosition.x <= 0 || newBuildingPosition.y <= 0 ||
            newBuildingPosition.x + newBuildingSize.x >= config.cityWidth || newBuildingPosition.y + newBuildingSize.y >= config.cityLength)
        {
            return false;
        }

        Vector2Int min = newBuildingPosition;
        Vector2Int max = newBuildingPosition + newBuildingSize;

        foreach ((Vector2Int position, Building building) in buildings)
        {
            Vector2Int buildingBoundsMin = new Vector2Int(position.x, position.y);
            Vector2Int buildingBoundsMax = new Vector2Int(position.x + building.width, position.y + building.length);
            if (min.x <= buildingBoundsMax.x && max.x >= buildingBoundsMin.x && min.y <= buildingBoundsMax.y && max.y >= buildingBoundsMin.y)
            {
                return false;
            }
        }

        return true;
    }

    private void UpdateGrid()
    {
        CityConfig config = GameManager.instance.config;

        ushort width = config.cityWidth;
        ushort length = config.cityLength;
        float cellSize = config.cellSize;

        gridPivot.transform.localPosition = new Vector3(width * cellSize / 2, 0, length * cellSize / 2);
        gridPivot.transform.localScale = new Vector3(width * cellSize, 1, length * cellSize);
        gridRenderer.material.mainTextureScale = new Vector2(width, length);
    }

    public void EnterBuildMode()
    {
        if (isInBuildMode)
        {
            return;
        }

        gridRenderer.material.color = Color.green;
        isInBuildMode = true;
        buildingGhostGameObject.SetActive(true);

        BuildingConfig newBuildingType = GetRandomBuildingType();
        buildingGhost.width = newBuildingType.width;
        buildingGhost.length = newBuildingType.length;
        buildingGhost.height = newBuildingType.height;
        buildingGhost.power = newBuildingType.power;
        buildingGhost.mode = Building.Mode.Transparent;

        foreach (Building building in buildings.Values)
        {
            building.showGrid = true;
            building.mode = Building.Mode.Transparent;
        }
    }

    private void ExitBuildMode()
    {
        gridRenderer.material.color = Color.white;
        isInBuildMode = false;
        buildingGhostGameObject.SetActive(false);

        foreach (Building building in buildings.Values)
        {
            building.showGrid = false;
            building.mode = Building.Mode.Normal;
        }
    }

    private Vector2Int ConvertWorldToGridPosition(Vector3 position)
    {
        CityConfig config = GameManager.instance.config;

        float x = (position.x - transform.position.x) / config.cellSize;
        float y = (position.z - transform.position.z) / config.cellSize;

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    private BuildingConfig GetRandomBuildingType()
    {
        CityConfig config = GameManager.instance.config;

        return config.buildingTypes[Random.Range(0, config.buildingTypes.Length)];
    }
}

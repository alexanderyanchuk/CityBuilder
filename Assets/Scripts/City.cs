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

            if (CanBuildAt(buildingGhost.GetGridRect(gridPosition)))
            {
                buildingGhost.SetMode(Building.Mode.Transparent);

                Vector3 dragDelta = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
                if (Input.GetMouseButtonUp(0) && dragDelta.magnitude < 0.01f)
                {
                    GameObject newBuildingGameObject = Instantiate(buildingPrefab, buildingGhostGameObject.transform.localPosition, buildingGhostGameObject.transform.rotation, transform);
                    Building newBuilding = newBuildingGameObject.GetComponent<Building>();
                    newBuilding.Init(buildingGhost);
                    newBuilding.ShowGrid();
                    newBuilding.SetMode(Building.Mode.Normal);
                    buildings.Add(gridPosition, newBuilding);

                    totalPower += newBuilding.power;
                    totalPowerChanged?.Invoke(totalPower);

                    ExitBuildMode();
                }
            }
            else
            {
                buildingGhost.SetMode(Building.Mode.Blocked);
            }
        }
    }

    private bool CanBuildAt(RectInt newBuildingRect)
    {
        CityConfig config = GameManager.instance.config;
        if (newBuildingRect.xMin <= 0 || newBuildingRect.yMin <= 0 || newBuildingRect.xMax >= config.cityWidth || newBuildingRect.yMax >= config.cityLength)
        {
            return false;
        }

        foreach ((Vector2Int position, Building building) in buildings)
        {
            if (newBuildingRect.Overlaps(building.GetGridRect(position, margin: 1)))
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
        buildingGhost.Init(newBuildingType);
        buildingGhost.SetMode(Building.Mode.Transparent);

        foreach (Building building in buildings.Values)
        {
            building.ShowGrid();
            building.SetMode(Building.Mode.Transparent);
        }
    }

    private void ExitBuildMode()
    {
        gridRenderer.material.color = Color.white;
        isInBuildMode = false;
        buildingGhostGameObject.SetActive(false);

        foreach (Building building in buildings.Values)
        {
            building.HideGrid();
            building.SetMode(Building.Mode.Normal);
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

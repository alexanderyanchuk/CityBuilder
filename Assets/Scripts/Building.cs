using TMPro;
using UnityEngine;
using Zenject;

public class Building : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        Transparent,
        Blocked,
    }

    [Inject] private CityConfig config;

    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject gridPivot;
    [SerializeField] private GameObject grid;
    [SerializeField] private TextMeshProUGUI powerText;

    [SerializeField] private Material cubeMaterial;
    [SerializeField] private Material cubeTransparentMaterial;
    [SerializeField] private Material cubeBlockedMaterial;

    private Renderer cubeRenderer;
    private Renderer gridRenderer;

    public ushort width { get; private set; }
    public ushort length { get; private set; }
    public ushort height { get; private set; }
    public uint power { get; private set; }
    public Mode mode { get; private set; }

    private void Awake()
    {
        cubeRenderer = cube.GetComponent<Renderer>();
        gridRenderer = grid.GetComponent<Renderer>();
        gridRenderer.material = Instantiate(gridRenderer.material);
    }

    public void Init(ushort width, ushort length, ushort height, uint power)
    {
        this.width = width;
        this.length = length;
        this.height = height;
        this.power = power;

        UpdateCube();
        UpdateGrid();
        UpdateUI();
    }

    public void Init(BuildingConfig config)
    {
        Init(config.width, config.length, config.height, config.power);
    }

    public void Init(Building other)
    {
        Init(other.width, other.length, other.height, other.power);
    }

    private void UpdateCube()
    {
        float cellSize = config.cellSize;

        cube.transform.localPosition = new Vector3(width * cellSize / 2, height * cellSize / 2, length * cellSize / 2);
        cube.transform.localScale = new Vector3(width * cellSize, height * cellSize, length * cellSize);
    }

    private void UpdateGrid()
    {
        float cellSize = config.cellSize;

        gridPivot.transform.localPosition = new Vector3(width * cellSize / 2, 0, length * cellSize / 2);
        gridPivot.transform.localScale = new Vector3((width + 2) * cellSize, 1, (length + 2) * cellSize);
        gridRenderer.material.color = Color.red;
        gridRenderer.material.mainTextureScale = new Vector2(width + 2, length + 2);
    }

    private void UpdateUI()
    {
        powerText.text = power.ToString();
    }

    public RectInt GetGridRect(Vector2Int position, int margin = 0)
    {
        return new RectInt(position.x - margin, position.y - margin, width + 2 * margin, length + 2 * margin);
    }

    public void ShowGrid()
    {
        grid.SetActive(true);
    }

    public void HideGrid()
    {
        grid.SetActive(false);
    }

    public void SetMode(Mode mode)
    {
        if (mode == this.mode)
        {
            return;
        }

        switch (mode)
        {
            case Mode.Normal:
                cubeRenderer.material = cubeMaterial;
                break;

            case Mode.Transparent:
                cubeRenderer.material = cubeTransparentMaterial;
                break;

            case Mode.Blocked:
                cubeRenderer.material = cubeBlockedMaterial;
                break;
        }

        this.mode = mode;
    }
}

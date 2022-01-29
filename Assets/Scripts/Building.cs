using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum Mode
    {
        Normal,
        Transparent,
        Blocked,
    }

    private ushort _width;
    private ushort _length;
    private ushort _height;
    private uint _power;
    private bool _showGrid;
    private Mode _mode;

    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject gridPivot;
    [SerializeField] private GameObject grid;
    [SerializeField] private TextMeshProUGUI powerText;

    private Renderer cubeRenderer;
    private Renderer gridRenderer;

    [SerializeField] private Material cubeMaterial;
    [SerializeField] private Material cubeTransparentMaterial;
    [SerializeField] private Material cubeBlockedMaterial;

    public ushort width
    {
        get { return _width; }
        set
        {
            _width = value;
            UpdateCube();
            UpdateGrid();
        }
    }

    public ushort length
    {
        get { return _length; }
        set
        {
            _length = value;
            UpdateCube();
            UpdateGrid();
        }
    }

    public ushort height
    {
        get { return _height; }
        set
        {
            _height = value;
            UpdateCube();
            UpdateGrid();
        }
    }

    public uint power
    {
        get { return _power; }
        set
        {
            _power = value;
            powerText.text = value.ToString();
        }
    }

    public bool showGrid
    {
        get { return _showGrid; }
        set
        {
            _showGrid = value;
            grid.SetActive(value);
        }
    }

    public Mode mode
    {
        get { return _mode; }
        set
        {
            if (_mode == value)
            {
                return;
            }

            _mode = value;
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
        }
    }

    private void Awake()
    {
        cubeRenderer = cube.GetComponent<Renderer>();
        gridRenderer = grid.GetComponent<Renderer>();
        gridRenderer.material = Instantiate(gridRenderer.material);
    }

    private void UpdateCube()
    {
        float cellSize = GameManager.instance.config.cellSize;

        cube.transform.localPosition = new Vector3(_width * cellSize / 2, _height * cellSize / 2, _length * cellSize / 2);
        cube.transform.localScale = new Vector3(_width * cellSize, _height * cellSize, _length * cellSize);
    }

    private void UpdateGrid()
    {
        float cellSize = GameManager.instance.config.cellSize;

        gridPivot.transform.localPosition = new Vector3(_width * cellSize / 2, 0, _length * cellSize / 2);
        gridPivot.transform.localScale = new Vector3((_width + 2) * cellSize, 1, (_length + 2) * cellSize);
        gridRenderer.material.color = Color.red;
        gridRenderer.material.mainTextureScale = new Vector2(width + 2, length + 2);
    }
}

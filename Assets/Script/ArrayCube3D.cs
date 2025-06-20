using UnityEngine;

public class ArrayCube3D : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    [SerializeField] private float _sideCube = 1f;
    [SerializeField] private float _offsetX = 0.2f;
    [SerializeField] private int _arrayX = 5;
    [SerializeField] private float _offsetY = 0.2f;
    [SerializeField] private int _arrayY = 5;
    [SerializeField] private float _offsetZ = 0.2f;
    [SerializeField] private int _arrayZ = 5;
    [SerializeField] private bool _rePosition;
    private Transform[] _allCube;

    private Vector3 FindCenter()
    {
        float x = (_arrayX - 1) * _sideCube + _offsetX * (_arrayX - 1);
        float y = (_arrayY - 1) * _sideCube + _offsetY * (_arrayY - 1);
        float z = (_arrayZ - 1) * _sideCube + _offsetZ * (_arrayZ - 1);
        return new Vector3(x / 2f, y / 2f, z / 2f);
    }

    private int Index(int x, int y, int z) => x + y * _arrayX + z * _arrayX * _arrayY;

    private Vector3 LocalPosition(int x, int y, int z) => new Vector3(x * _sideCube + _offsetX * x, y * _sideCube + _offsetY * y, z * _sideCube + _offsetZ * z);

    private void Generate()
    {
        _allCube = new Transform[_arrayX * _arrayY * _arrayZ];
        Vector3 center = FindCenter();

        for (int z = 0; z < _arrayZ; z++)
        {
            for (int y = 0; y < _arrayY; y++)
            {
                for (int x = 0; x < _arrayX; x++)
                {
                    int indice = Index(x, y, z);
                    _allCube[indice] = Instantiate(_cube, transform.position + LocalPosition(x, y, z) - center, Quaternion.identity, transform);
                }
            }
        }
    }

    private void RePosition()
    {
        Vector3 center = FindCenter();

        for (int z = 0; z < _arrayZ; z++)
        {
            for (int y = 0; y < _arrayY; y++)
            {
                for (int x = 0; x < _arrayX; x++)
                {
                    _allCube[Index(x, y, z)].transform.localPosition = LocalPosition(x, y, z) - center;
                }
            }
        }
    }

    void Start()
    {
        Generate();
    }

    void Update()
    {
        if (_rePosition)
        {
            _rePosition = false;
            RePosition();
        }
    }
}

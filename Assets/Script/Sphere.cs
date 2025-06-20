using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    [SerializeField] private int _index;
    private int _indexLock;
    [SerializeField] private float _radius;
    [SerializeField] private bool _isInside;
    [SerializeField] private bool _isRandomRotation;
    [SerializeField] private bool _isRandomScale;
    [SerializeField] private float _randomScaleMin = 0.5f;
    [SerializeField] private float _randomScaleMax = 1.5f;
    private Transform[] _cubes;

    private void TransformArray()
    {
        foreach (Transform cube in _cubes)
        {
            cube.position = Position();
            cube.rotation = Rotation();
            cube.localScale = Scale();
        }
    }

    private void Generate()
    {
        for (int i = 0; i < _index; i++)
        {
            _cubes[i] = Instantiate(_cube, transform);
        }
    }

    private Vector3 Position()
    {
        if (_isInside)
        {
            return Random.insideUnitSphere * _radius;
        }
        else
        {
            return Random.onUnitSphere * _radius;
        }
    }

    private Quaternion Rotation()
    {
        if (_isRandomRotation)
        {
            return Random.rotation;
        }
        else
        {
            return Quaternion.identity;
        }
    }

    private Vector3 Scale()
    {
        if (_isRandomScale)
        {
            return Vector3.one * Random.Range(_randomScaleMin, _randomScaleMax);
        }
        else
        {
            return Vector3.one;
        }
    }

    private void Initializate()
    {
        _indexLock = _index;
        _cubes = new Transform[_index];
        Generate();
        TransformArray();
    }

    void Awake()
    {
        Initializate();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            if (_index != _indexLock)
            {
                foreach (Transform cube in _cubes)
                {
                    Destroy(cube.gameObject);
                }
                Initializate();
            }
            else
            {
                TransformArray();
            }
        }
    }
}

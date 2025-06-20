using UnityEngine;

public class ManagerMyPoligon : MonoBehaviour
{
    private Camera _camera;
    [Range(3, 32)]
    [SerializeField] private int _segments = 16;
    [Range(-180f, 180f)]
    [SerializeField] private float _rotationSegments = 90f;
    [Range(0.2f, 1.5f)]
    [SerializeField] private float _radius = 0.75f;
    [Range(0.1f, 0.5f)]
    [SerializeField] private float _offset = 0.2f;
    //Diametro della sfera ipotetica sulla quale giaciono i centri dei MyPoligon
    [Range(5f, 15f)]
    [SerializeField] private float _distance = 10f;
    [Range(1, 6)]
    [SerializeField] private int _concentricCircumferences = 3;
    [SerializeField] private MyPoligon _empty;
    [SerializeField] private Material[] _materials = new Material[9];

    private Mesh _poligon;
    private MyPoligon[] _myPoligonCells;

    private Transform _palette;
    private MyPoligon[] _myPoligonPalettes = new MyPoligon[8];

    private MyPoligon _myPoligonSelect;

    private RaycastHit _hit;
    private LayerMask _layerMask = (1 << 6);
    private int _intColorSelect;

    [SerializeField] private bool _recalculate;

    private void CalculateMesh()
    {
        //I vertici che compongono la mesh, 1 al centro + tutti quelli sulla circonferenza (_segments)
        Vector3[] vertices = new Vector3[_segments + 1];
        vertices[0] = Vector3.zero;
        //Calcolo i vertici
        for (int i = 0; i < _segments; i++)
        {
            Quaternion angle = Quaternion.Euler(0f, 0f, (360 / _segments) * i + _rotationSegments);
            vertices[i + 1] = angle * new Vector3(_radius, 0f, 0f);
        }

        //Calcolo i trinagoli
        int[] triangles = new int[_segments * 3];
        for (int i = 0; i < _segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 2] = (i + 2 > _segments) ? 1 : i + 2;
            triangles[i * 3 + 1] = i + 1;
        }

        //Calcolo Mesh
        _poligon = new Mesh();
        _poligon.vertices = vertices;
        _poligon.triangles = triangles;
        _poligon.RecalculateBounds();
        _poligon.name = "MyPoligon";
    }

    private void InstantiateMyPoligon()
    {
        transform.LookAt(_camera.transform);

        _myPoligonCells = new MyPoligon[6 * _concentricCircumferences + 1];
        _myPoligonCells[0] = Instantiate(_empty, transform);

        //CIRCONFERENZE CONCENTRICHE
        //Instazio le circonferenze
        for (int i = 0; i < _concentricCircumferences; i++)
        {
            //Distanza tra i centri dei nuovi MyPoligon rispetto il precedente
            float radius = (_radius * 2 + _offset) * (i + 1) - (_offset * Mathf.Max(0, i)) - (_radius * Mathf.Max(0, i - 1));
            //Angolo che descrive la latitudine inversa (cioè dal polo) sulla quale giaciono i centri dei MyPoligon
            Quaternion arcAngle = Quaternion.Euler(0f, (radius / (_distance * 2f)) * Mathf.Rad2Deg * -1f, 0f);
            //Ruoto la posizione locale sulla latitudine inversa
            Vector3 position = arcAngle * new Vector3(radius, 0f, 0f);
            float deltaAngle = (i % 2 == 0) ? 0f : 30f;
            for (int j = 0; j < 6; j++)
            {
                int index = i * 6 + j + 1;
                _myPoligonCells[index] = Instantiate(_empty, transform);
                //Posizione locale sulla latitudine inversa
                _myPoligonCells[index].transform.localPosition = position;
                Quaternion angle = Quaternion.Euler(0f, 0f, 60f * j + deltaAngle);
                _myPoligonCells[index].transform.localPosition = angle * _myPoligonCells[index].transform.localPosition;
            }
        }

        foreach (MyPoligon myPoligon in _myPoligonCells)
        {
            myPoligon.SetManager(this);
            myPoligon.CreateComponets(true);
            myPoligon.SetComponent(_poligon, _materials[0], _radius, _camera, true, MyPoligon.TypeColor.Color);
        }
    }

    private void CreatePaletteAndSelect()
    {
        GameObject palette = new GameObject("Palette");
        _palette = Instantiate(palette.transform, transform);
        _palette.localPosition = new Vector3 (_distance + _radius * _concentricCircumferences, _radius * 5f + _offset * 1.5f, 0f);
        Destroy(palette);

        Vector3 xPosition = new Vector3(_radius + _offset / 2f, 0f, 0f);
        for (int i = 0; i < 8; i++)
        {
            _myPoligonPalettes[i] = Instantiate(_empty, _palette);
            //Spostamento in X
            _myPoligonPalettes[i].transform.localPosition = xPosition * ((i % 2 == 0) ? 1f : -1f);
            //Spostamento in Z
            _myPoligonPalettes[i].transform.localPosition += new Vector3(0f, ((i % 2 == 0) ? i : i - 1) * -(_radius +_offset), 0f);
            _myPoligonPalettes[i].SetManager(this);
            _myPoligonPalettes[i].CreateComponets(true);
            _myPoligonPalettes[i].SetComponent(_poligon, _materials[i + 1], _radius, _camera, false, MyPoligon.TypeColor.Palette);
        }

        _myPoligonSelect = Instantiate(_empty, _palette);
        _myPoligonSelect.transform.localPosition += new Vector3(0f, 8 * -(_radius + _offset), 0f);
        _myPoligonSelect.SetManager(this);
        _myPoligonSelect.CreateComponets(false);
        _myPoligonSelect.SetComponent(_poligon, _materials[0], _radius, _camera, false, MyPoligon.TypeColor.Select);
    }

    private void Main()
    {
        CalculateMesh();
        InstantiateMyPoligon();
        CreatePaletteAndSelect();
    }

    void Awake()
    {
        _camera = Camera.main;
        Main();
    }

    void Start()
    {
        
    }

    private void Recalculate()
    {
        _recalculate = false;
        
        for (int i = 0; i <  _myPoligonCells.Length; i++)
        {
            Destroy(_myPoligonCells[i].gameObject);
        }
        Destroy(_palette.gameObject);
        _intColorSelect = 0;

        Main();
    }

    private void ChangeColor()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hit, _layerMask))
        {
            MyPoligon myPoligonHit = _hit.collider.gameObject.GetComponent<MyPoligon>();
            switch (myPoligonHit.Type)
            {
                case(MyPoligon.TypeColor.Color):
                    myPoligonHit.SetColor(_materials[_intColorSelect].color);
                    break;
                case (MyPoligon.TypeColor.Palette):
                    //Si può fare più semplice, ma è per esercizio ...
                    Color color = myPoligonHit.GetColor();
                    for(int i = 0; i < _materials.Length; i++)
                    {
                        if (_materials[i].color.Equals(color))
                        {
                            _intColorSelect = i;
                            break;
                        }
                    }
                    _myPoligonSelect.SetColor(_materials[_intColorSelect].GetColor("_BaseColor"));
                    break;
            }
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < _myPoligonCells.Length; i++)
        {
            _myPoligonCells[i].SetColor(_materials[0].color);
        }
        for (int i = 0; i < _myPoligonPalettes.Length; i++)
        {
            _myPoligonPalettes[i].SetColor(_materials[i + 1].color);
        }
        _myPoligonSelect.SetColor(_materials[0].color);
        _intColorSelect = 0;
    }

    void Update()
    {
        if (_recalculate)
        {
            Recalculate();
        }

        if (Input.GetMouseButton(0))
        {
            ChangeColor();
        }

        if (Input.GetMouseButtonDown(1))
        {
            ResetColor();
        }
    }
}

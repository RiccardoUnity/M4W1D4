using UnityEngine;

public class MyPoligon : MonoBehaviour
{
    private ManagerMyPoligon _manager;

    public enum TypeColor
    {
        Color = 0,
        Select = 1,
        Palette = 2
    }

    private bool _lockComponet;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private SphereCollider _sphereCollider;
    private bool _setComponent;
    private Camera _camera;

    private TypeColor _type = TypeColor.Color;
    public TypeColor Type
    {
        get => _type;
        private set => _type = value;
    }

    public void SetManager(ManagerMyPoligon manager)
    {
        _manager = manager;
    }

    public void CreateComponets(bool isPhysic)
    {
        if (_lockComponet)
        {
            Debug.LogWarning("Non puoi creare altri componenti qui");
        }
        else
        {
            _lockComponet = true;
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            if (isPhysic)
            {
                _sphereCollider = gameObject.AddComponent<SphereCollider>();
            }
        }
    }

    public void SetComponent(Mesh mesh, Material material, float radius, Camera camera, bool useCamera, TypeColor type)
    {
        if (_setComponent)
        {
            Debug.LogWarning("Non puoi settare i componenti qui");
        }
        else
        {
            _setComponent = true;
            _meshFilter.mesh = mesh;
            _meshRenderer.material = material;
            if (_sphereCollider != null)
            {
                _sphereCollider.radius = radius;
            }
            _camera = camera;
            if (useCamera)
            {
                transform.LookAt(_camera.transform.position);
            }
            Type = type;
        }
    }

    public Color GetColor()
    {
        //Si può fare più semplice, ma è per esercizio ...
        return _meshRenderer.material.GetColor("_BaseColor");
    }

    public void SetColor(Color color)
    {
        //Si può fare più semplice, ma è per esercizio ...
        _meshRenderer.material.SetColor("_BaseColor", color);
    }

    //public void Unlock()
    //{
    //    _lockComponet = false;
    //    _setComponent = false;
    //}

    void Start()
    {
        
    }
}

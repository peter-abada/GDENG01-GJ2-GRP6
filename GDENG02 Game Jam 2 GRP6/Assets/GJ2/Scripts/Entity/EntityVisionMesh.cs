using JetBrains.Annotations;
using TMPro;
using Unity.Hierarchy;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.UI.Image;

public class EntityVisionMesh : MonoBehaviour
{

    private bool isInViewCone, isInRange, isHidden;
    [SerializeField] public GameObject Entity;
    [SerializeField] public GameObject Player;
    
    [SerializeField] public float detectionRange;
    [SerializeField] private float viewAngle;

    Mesh mesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    Vector3 heightOffset;
    Vector3 origin;
    float fov;
    int rayCount;
    float angle;
    float angleStep;

    public Material UndetectedMaterial;
    public Material DetectedMaterial;

    //Debugging vars
    //public TMP_Text RangeText;
    //public TMP_Text IsHiddenText;
    //public TMP_Text InViewConeText;

    public Quaternion EnemyRotation;
    public Quaternion PlayerRotation;

    void Start()
    {
        if (!Player)
        {
            Debug.LogError($"{name}has no Player gameObject");
        }

        
        heightOffset = new Vector3 (0f, 0.2f, 0f);
        detectionRange = 4.0f;
        viewAngle = 90f;
        rayCount = 25;


        InitializeMesh();

    }

    void Update()
    {
        isInRange = false;
        isHidden = false;
        isInViewCone = false;

        CheckIfInRange();
        CheckIfHidden();
        CheckIfInViewCone();

        //TEST_SimpleMesh();
        DrawVerticesPosition();

        //DisplayOnScreen();
    }

    void CheckIfInRange()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= detectionRange)
        {
            isInRange = (true);
        }
    }
    void CheckIfHidden()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Player.transform.position - transform.position, out hit, Mathf.Infinity))
        {
            isHidden = (true);
        }
    }
    void CheckIfInViewCone()
    {
        Vector3 side1 = Player.transform.position - transform.position;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);
        if (angle <= viewAngle * 0.5f && angle >= -viewAngle * 0.5f)
        {
            isInViewCone = true;
        }
    }

    
    float GetNormal(Vector3 vec)
    {
        return Mathf.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z + vec.z);
    }
    void TEST_SimpleMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[3];
        uv = new Vector2[3];
        triangles = new int[3];


        Vector3 offsetPosition = origin + heightOffset;// + Entity.transform.localPosition;
        Quaternion rotation = transform.rotation;
        Vector3 forward = rotation * Vector3.forward;

        Vector3 edgeDirection = GetNewFlatVectorFromAngle(viewAngle);
        edgeDirection = rotation * edgeDirection;
        Vector3 edgeOfViewCone = (forward * detectionRange) + (edgeDirection * detectionRange);
        Vector3 evc = edgeOfViewCone;
        vertices[0] = rotation * offsetPosition;
        vertices[1] = rotation * (new Vector3(-evc.x, evc.y, evc.z) + offsetPosition);
        vertices[2] = rotation * ( new Vector3(evc.x, evc.y, evc.z) + offsetPosition);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        
    }
    void InitializeMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void ResetMeshData()
    {
        origin = Vector3.zero;
        fov = viewAngle;
        angle = viewAngle / 2f;
        angleStep = fov / rayCount;

        vertices = new Vector3[rayCount + 1 + 1]; // 1 for origin, 1 for ray 0
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];
    }

    void DrawVerticesPosition()
    {
        ResetMeshData();

        Vector3 transformOffset = origin + heightOffset;
        Vector3 targetDirection = Player.transform.forward;
        Vector3 targetPosition = transform.position + targetDirection;
        EnemyRotation = transform.rotation;
        PlayerRotation = Player.transform.rotation;

        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        //Debug.DrawRay(transform.position, Player.transform.forward, Color.green);//player
        //transform.LookAt(Player.transform.forward);

        Vector3 rotation = transform.forward;


        vertices[0] = transformOffset;
        int vertIndex = 1;
        int triangleIndex = 0;
        int layerMask = ~(1 << LayerMask.NameToLayer("Enemy")) & ~(1 << LayerMask.NameToLayer("Player"));

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            Vector3 aimDirection = GetFlatVectorFromAngle(angle);
            RaycastHit hit;


            //Debug.DrawRay(transformOffset + transform.position, aimDirection * detectionRange, Color.red);
            if (Physics.Raycast(origin, aimDirection, out hit, detectionRange, layerMask))
            {
                vertex = hit.point;
            }
            else
            {
                vertex = transformOffset + aimDirection * detectionRange;
            }


                vertices[vertIndex] = vertex;
            //if (i > 0)
            //{
            //    triangles[triangleIndex + 0] = 0;
            //    triangles[triangleIndex + 1] = vertIndex - 1;
            //    triangles[triangleIndex + 2] = vertIndex;
            //    triangleIndex += 3;
            //}


            vertIndex++;
            angle -= angleStep;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    //void DisplayOnScreen()
    //{
    //    if (isInRange)
    //    {
    //        RangeText.text = "In Range";
    //        RangeText.color = Color.green;
    //    }
    //    else
    //    {
    //        RangeText.text = "Not In Range";
    //        RangeText.color = Color.red;
    //    }
    //    if (IsHiddenText)
    //    {
    //        IsHiddenText.text = "Is Hidden";
    //        IsHiddenText.color = Color.red;
    //    }
    //    else
    //    {
    //        IsHiddenText.text = "Is not Hidden";
    //        IsHiddenText.color = Color.green;
    //    }
    //    if (InViewConeText)
    //    {
    //        InViewConeText.text = "In View Cone";
    //        InViewConeText.color = Color.green;
    //    }
    //    else
    //    {
    //        InViewConeText.text = "Not In View Cone";
    //        InViewConeText.color = Color.red;
    //    }
    //}

    Vector2 Get2DVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    Vector3 GetFlatVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
    }
    Vector3 GetNewFlatVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
    }

    float CalculateAngle(Vector3 a, Vector3 b)
    {
        float dotProduct = Vector3.Dot(a, b);
        float magnitudeA = a.magnitude;
        float magnitudeB = b.magnitude;
        float cosTheta = dotProduct / (magnitudeA * magnitudeB);
        float angleInRadians = Mathf.Acos(cosTheta);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        return angleInDegrees;
    }

    public bool GetIsInRange()
    {
        return isInRange;
    }
    public bool GetIsInViewCone()
    {
        return isInViewCone;
    }
    public bool GetIsHidden()
    {
        return isHidden;
    }
}

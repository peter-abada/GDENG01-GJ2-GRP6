using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.UI.Image;

public class EntityVision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private bool isInViewCone, isInRange, isHidden;
    [SerializeField] public float detectionRange;
    [SerializeField] public GameObject player;
    [SerializeField] private float viewAngle;
    
    [SerializeField] Vector3[] arcPoints;
    private LineRenderer lineRenderer;
    private int segments = 5;
    private int arcPointsSize;

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

    //Debugging vars
    public TMP_Text RangeText;
    public TMP_Text IsHiddenText;
    public TMP_Text ViewConeText;

    void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        heightOffset = new Vector3 (0f, 0.2f, 0f);
        detectionRange = 4.0f;
        viewAngle = 45;


        //SimpleMesh();
        //InitializeMesh();

        arcPointsSize = segments + 1;
        arcPoints = new Vector3[arcPointsSize];
        InitializeLineRenderer();
        
        CalculateArcPoints();
    }

    // Update is called once per frame
    void Update()
    {
        isInRange = false;
        isHidden = false;
        isInViewCone = false;

        CheckIfInRange();
        CheckIfHidden();
        CheckIfInViewCone();

        //DrawVerticesPosition();
        DrawArcPointsTemp();
        DisplayOnScreen();
    }

    void CheckIfInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            isInRange = (true);
        }
    }
    void CheckIfHidden()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, Mathf.Infinity))
        {
            isHidden = (true);
        }
    }
    void CheckIfInViewCone()
    {
        Vector3 side1 = player.transform.position - transform.position;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);
        if (angle <= viewAngle * 0.5f && angle >= -viewAngle * 0.5f)
        {
            isInViewCone = true;
        }
    }

    
    void SimpleMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[3];
        uv = new Vector2[3];
        triangles = new int[3];

        float offsetDistance = 0.2f;
        Vector3 offsetPosition = origin + new Vector3(0, 0.5f, 0);
        vertices[0] = offsetPosition;
        vertices[1] = new Vector3(5, 0, 0) + offsetPosition;
        vertices[2] = new Vector3(0, 0, -5) + offsetPosition;

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
        rayCount = 30;
        angle = viewAngle * 0.5f;
        angleStep = fov / rayCount;

        vertices = new Vector3[rayCount + 1 + 1]; // 1 for origin, 1 for ray 0
        uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];
    }

    void DrawVerticesPosition()
    {
        ResetMeshData();

        Vector3 transformOffset = origin + heightOffset;
        vertices[0] = transformOffset;
        int vertIndex = 1;
        int triangleIndex = 0;
        int layerMask = ~(1 << LayerMask.NameToLayer("Enemy")) & ~(1 << LayerMask.NameToLayer("Player"));

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex;

            Vector3 aimDirection = GetFlatVectorFromAngle(angle);
            RaycastHit hit;


            Debug.DrawRay(transformOffset + transform.position,aimDirection * detectionRange, Color.red);
            if (Physics.Raycast(origin, aimDirection, out hit, detectionRange, layerMask))
            {
                vertex = hit.point;
            }
            else
            {
                vertex = transformOffset * detectionRange;
            }


                vertices[vertIndex] = vertex;
            if (i > 0)
            {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertIndex - 1;
                triangles[triangleIndex + 2] = vertIndex;
                triangleIndex += 3;
            }


            vertIndex++;
            angle -= angleStep;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    void CalculateArcPoints()
    {
        float angle = viewAngle * 2f / segments;
        Vector3 offsetPosition = transform.position + new Vector3(0, 0.2f, 0);

        arcPoints[0] = offsetPosition;

        for (int i = 1; i < segments; i++)
        {
            float newAngle = -viewAngle + angle * i;

            Vector3 directionOfPoint = Quaternion.Euler(0, newAngle, 0) * transform.forward * detectionRange;
            arcPoints[i] = offsetPosition + directionOfPoint;
        }

        arcPoints[segments] = offsetPosition;
    }
    void DrawArcPointsTemp()
    {
        float angle = viewAngle * 2f / segments;
        Vector3 offsetPosition = transform.position + new Vector3(0, 0.2f, 0);

        lineRenderer.SetPosition(0, offsetPosition);

        for (int i = 1; i < segments; i++)
        {
            float newAngle = -viewAngle + angle * i;

            Vector3 directionOfPoint = Quaternion.Euler(0, newAngle, 0) * transform.forward * detectionRange;
            lineRenderer.SetPosition(i, offsetPosition + directionOfPoint);
        }

        lineRenderer.SetPosition(segments, offsetPosition);
    }
    void DrawArcPoints()
    {
        for (int i = 0; i < segments; i++)
        {
            lineRenderer.SetPosition(i, arcPoints[i]);
        }
    }

    void DisplayOnScreen()
    {
        if (isInRange)
        {
            RangeText.text = "In Range";
            RangeText.color = Color.green;
        }
        else
        {
            RangeText.text = "Not In Range";
            RangeText.color = Color.red;
        }
        if (IsHiddenText)
        {
            IsHiddenText.text = "Is Hidden";
            IsHiddenText.color = Color.red;
        }
        else
        {
            IsHiddenText.text = "Is not Hidden";
            IsHiddenText.color = Color.green;
        }
        if (ViewConeText)
        {
            ViewConeText.text = "In View Cone";
            ViewConeText.color = Color.green;
        }
        else
        {
            ViewConeText.text = "Not In View Cone";
            ViewConeText.color = Color.red;
        }
    }

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

    void InitializeLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = arcPointsSize;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.red;
    }

}

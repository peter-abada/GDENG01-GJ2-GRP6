using System.Collections;
using TMPro;
using UnityEngine;

public class EntityVisionLineRender : MonoBehaviour
{
    public bool isInViewCone, isInRange, isHidden;
    [SerializeField] public GameObject Player;
    [SerializeField] public float detectionRange;
    [SerializeField] private float viewAngle;

    [SerializeField] Vector3[] arcPoints;
    private LineRenderer lineRenderer;
    private int segments = 20;
    private int arcPointsSize;
    Vector3 heightOffset;
    float lookOffset = -90f;

    void Start()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }

        heightOffset = new Vector3(0f, 1.0f, 0f);
        detectionRange = 4.0f;
        viewAngle = 90;

        arcPointsSize = segments + 1;
        arcPoints = new Vector3[arcPointsSize];
        InitializeLineRenderer();

        
    }

    void Update()
    {
        CalculateArcPoints();

        isInRange = false;
        isHidden = true;
        isInViewCone = false;

        CheckIfInRange();
        
        if (isInRange)
        {
            CheckIfHidden();
            CheckIfInViewCone();
        }

        DrawArcPoints();
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
        //Debug.DrawRay(transform.position + transform.up * 1f , (Player.transform.position - transform.position - transform.up * 1f), Color.blue);
        if (Physics.Raycast(transform.position + transform.up * 1f, (Player.transform.position - transform.position - transform.up * 1f), out hit, detectionRange))
        {
            if (hit.collider.gameObject == Player)
            {
                isHidden = false;
            }
            
        }
        else
        {
            isHidden = true;
        }

    }
    void CheckIfInViewCone()
    {
        Vector3 side1 = Player.transform.position - transform.position;
        Vector3 side2 = transform.forward;
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up) + lookOffset;
        if (angle <= viewAngle && angle >= -viewAngle)
        {
            isInViewCone = true;
        }
    }


    void CalculateArcPoints()
    {
        float angle = viewAngle * 2f / segments;
        Vector3 offsetPosition = transform.position + new Vector3(0, 0.3f, 0);

        arcPoints[0] = offsetPosition;

        for (int i = 1; i < segments; i++)
        {
            float newAngle = -viewAngle + angle * i;
            Vector3 directionOfPoint = Quaternion.Euler(0, newAngle + lookOffset, 0) * transform.forward * detectionRange;
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

            Vector3 directionOfPoint = Quaternion.Euler(0, newAngle + lookOffset, 0) * transform.forward * detectionRange;
            lineRenderer.SetPosition(i, offsetPosition + directionOfPoint);
        }

        lineRenderer.SetPosition(segments, offsetPosition);
    }
    void DrawArcPoints()
    {
        for (int i = 0; i <= segments; i++)
        {
            lineRenderer.SetPosition(i, arcPoints[i]);
        }
    }   

    void InitializeLineRenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = arcPointsSize;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.grey;
            lineRenderer.endColor = Color.grey;
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

    public void SetMaterial(Material material)
    {
        lineRenderer.material = material;
    }
}


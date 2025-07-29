using TMPro;
using UnityEngine;

public class EntityVisionLineRender : MonoBehaviour
{
    private bool isInViewCone, isInRange, isHidden;
    [SerializeField] public GameObject Player;
    [SerializeField] public float detectionRange;
    [SerializeField] private float viewAngle;

    [SerializeField] Vector3[] arcPoints;
    private LineRenderer lineRenderer;
    private int segments = 10;
    private int arcPointsSize;

    //Debugging vars
    public TMP_Text RangeText;
    public TMP_Text IsHiddenText;
    public TMP_Text InViewConeText;

    void Start()
    {
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        
        detectionRange = 4.0f;
        viewAngle = 45;


        //SimpleMesh();
        //InitializeMesh();

        arcPointsSize = segments + 1;
        arcPoints = new Vector3[arcPointsSize];
        InitializeLineRenderer();

        CalculateArcPoints();
    }

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
        if (InViewConeText)
        {
            InViewConeText.text = "In View Cone";
            InViewConeText.color = Color.green;
        }
        else
        {
            InViewConeText.text = "Not In View Cone";
            InViewConeText.color = Color.red;
        }
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

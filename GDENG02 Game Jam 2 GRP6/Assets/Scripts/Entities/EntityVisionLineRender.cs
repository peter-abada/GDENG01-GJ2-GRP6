using System.Collections;
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
    Vector3 heightOffset;

    public float attackCooldown;
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

        heightOffset = new Vector3(0f, 0.5f, 0f);
        detectionRange = 4.0f;
        viewAngle = 45;
        attackCooldown = 3.5f;

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
        isHidden = true;
        isInViewCone = false;

        CheckIfInRange();
        
        if (isInRange)
        {
            CheckIfHidden();
            CheckIfInViewCone();
        }
        ViewConeColor();


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
        Debug.DrawRay(transform.position, (Player.transform.position - transform.position), Color.blue);
        if (Physics.Raycast(transform.position + heightOffset, (Player.transform.position - transform.position) + heightOffset, out hit, detectionRange))
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
        float angle = Vector3.SignedAngle(side1, side2, Vector3.up);
        if (angle <= viewAngle && angle >= -viewAngle)
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
            RangeText.text = "In Ranges";
            RangeText.color = Color.green;
        }
        else
        {
            RangeText.text = "Not In Range";
            RangeText.color = Color.red;
        }
        if (isHidden == true)
        {
            IsHiddenText.text = "Is Hidden";
            IsHiddenText.color = Color.red;
        }
        else if (isHidden == false)
        {
            IsHiddenText.text = "Is not Hidden";
            IsHiddenText.color = Color.green;
        }
        if (isInViewCone)
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
            lineRenderer.startColor = Color.grey;
            lineRenderer.endColor = Color.grey;
        }

    void ViewConeColor()
    {
        if (!isHidden && isInViewCone)
        {
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.material.SetColor("_Color", Color.red * 2f);
        }
        else
        {
            lineRenderer.startColor = Color.grey;
            lineRenderer.endColor = Color.grey;
        }
    }

    IEnumerable AnimateEmmision()
    {
        float time = 0f;
        while(time < duration)
    }
}


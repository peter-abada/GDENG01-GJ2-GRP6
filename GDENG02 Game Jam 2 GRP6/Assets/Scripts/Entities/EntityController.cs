using UnityEngine;

public class EntityController : MonoBehaviour
{
    public GameObject model;
    public AnimController animController;
    public EntityVisionLineRender entityVision;

    private bool isInViewCone, isInRange, isHidden;
    void Start()
    {
        animController = model.GetComponent<AnimController>();
        entityVision = model.GetComponent<EntityVisionLineRender>();

        if (animController == null)
        {
            Debug.Log(transform.name + "animController missing");
        }
        if (entityVision == null)
        {
            Debug.Log(transform.name + "entityVision missing");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        isInRange = entityVision.GetIsInRange();
        isHidden = entityVision.GetIsHidden(); 
        isInViewCone = entityVision.GetIsInViewCone();

        if (isInRange && !isInViewCone || isInRange && isHidden)
        {
            animController.Draw();
        }
        if (!isHidden && isInViewCone)
        {

        }
    }
}

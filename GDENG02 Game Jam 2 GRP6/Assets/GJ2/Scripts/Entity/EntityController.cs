using UnityEngine;

public class EntityController : MonoBehaviour
{
    public GameObject Player;
    AnimController animController;
    EntityVisionLineRender entityVision;

    private bool isInViewCone, isInRange, isHidden;
    public Material undetected;
    public Material cautious;
    public Material detected;

    void Start()
    {
        animController = Player.GetComponent<AnimController>();
        entityVision = Player.GetComponent<EntityVisionLineRender>();
        entityVision.SetMaterial(undetected);
        if (animController == null)
        {
            Debug.LogError($"{name}'s animController is missing");
        }
        if (entityVision == null)
        {
            Debug.LogError($"{name}'s entityVision is missing");
        }
    }

    // Update is called once per frame
    void Update()
    {
        isInRange = entityVision.GetIsInRange();
        isHidden = entityVision.GetIsHidden(); 
        isInViewCone = entityVision.GetIsInViewCone();

        if (isInRange)
        {

            //if (!isInViewCone && isHidden)
            //{
                
            //}
            //else
            if(!isHidden && (isInViewCone))
            {
                animController.ReleaseDraw();
                entityVision.SetMaterial(detected);
            }
            else
            {
                animController.Draw();
                entityVision.SetMaterial(cautious);
            }
        }
        else if (!isInRange)
        {
            animController.Idle();
            entityVision.SetMaterial(undetected);
        }
        
    }
}

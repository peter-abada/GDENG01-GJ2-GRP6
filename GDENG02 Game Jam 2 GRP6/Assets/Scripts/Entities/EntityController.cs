using Unity.VisualScripting;
using UnityEngine;

public class EntityController : MonoBehaviour
{
    public GameObject model;
    public AnimController animController;
    public EntityVisionLineRender entityVision;

    public bool ViewCone, Range, Hidden;
    public Material undetected;
    public Material caution;
    public Material detected;
    void Start()
    {
        //animController = model.GetComponent<AnimController>();
        //entityVision = model.GetComponent<EntityVisionLineRender>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Range = entityVision.GetIsInRange();
        Hidden = entityVision.GetIsHidden();
        ViewCone = entityVision.GetIsInViewCone();

        if (Range)
        {
            animController.Draw();
            entityVision.SetMaterial(caution);
            if (!Hidden && ViewCone)
            {
                animController.ReleaseDraw();
                entityVision.SetMaterial(detected);
            }
            else if (!ViewCone && !Hidden)
            {
                animController.HoldDraw();
            }
            
        }
        else
        {
            animController.Idle();
            entityVision.SetMaterial(undetected);
        }

    }
}

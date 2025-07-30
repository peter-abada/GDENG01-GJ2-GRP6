using UnityEngine;

public class AnimController : MonoBehaviour
{
    public GameObject Model;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        EntityVisionMesh env = Model.GetComponent<EntityVisionMesh>();
    }
    public void Draw()
    {
        animator.SetBool("IsDraw", true);
    }
    public void HoldDraw()
    {
        animator.SetBool("IsHoldDraw", true);
        animator.SetBool("IsReleaseDraw", false);
    }
    public void ReleaseDraw()
    {
        animator.SetBool("IsReleaseDraw", true);
        animator.SetBool("IsHoldDraw", false);
    }
    public void Idle()
    {
        animator.SetBool("IsDraw", false);
        animator.SetBool("IsHoldDraw", false);
        animator.SetBool("IsReleaseDraw", false);
    }

    void FlipBool(string name)
    {
        bool current = animator.GetBool(name);
        animator.SetBool(name, !current);
    }
    void SetBool(string name, bool value)
    {
        animator.SetBool(name, value);
    }
}
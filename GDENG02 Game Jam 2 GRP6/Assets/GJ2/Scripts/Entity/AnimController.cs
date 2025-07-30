using UnityEditor;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public GameObject Player;
    Animator animator;


    void Start()
    {
        if (Player == null)
        {

            Debug.LogError($"{gameObject} has no Player", gameObject);
        }

        animator = GetComponent<Animator>();
        EntityVisionMesh env = Player.GetComponent<EntityVisionMesh>();
        //Player = this.GetComponentInParent<EntityController>().Player;
    }

    public void Idle()
    {
        animator.SetBool("IsDraw", false);
        animator.SetBool("IsHoldDraw", false);
        animator.SetBool("IsReleaseDraw", false);
    }
    public void Draw()
    {
        animator.SetBool("IsDraw", true);
        animator.SetBool("IsHoldDraw", true);
        animator.SetBool("IsReleaseDraw", false);
    }
    public void HoldDraw()
    {
        animator.SetBool("IsHoldDraw", true);
        //animator.SetBool("IsDraw", false);
    }
    public void ReleaseDraw()
    {
        animator.SetBool("IsReleaseDraw", true);
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
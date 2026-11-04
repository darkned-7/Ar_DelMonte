using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartLeafs : MonoBehaviour
{
    public Animator animator;

    public void RestartLeaf()
    {
        animator.SetInteger("Restart", 1);
    }

    public void InitialLeafs()
    {
        animator.SetInteger("Restart", 0);
    }


}

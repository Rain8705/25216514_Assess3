using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorDemo : MonoBehaviour
{
    public Animator pacAnimator;
    public Animator ghostAnimator;
    public Animator pelletAnimator;

    IEnumerator Start()
    {
        if (pacAnimator != null)
        {
            pacAnimator.SetBool("isMoving", true);
            yield return new WaitForSeconds(5f);
            pacAnimator.SetBool("isMoving", false);
            yield return new WaitForSeconds(1f);
            pacAnimator.SetBool("isDead", true);
        }

        if (ghostAnimator != null)
        {
            ghostAnimator.SetBool("isMoving", true);
            yield return new WaitForSeconds(4f);
            ghostAnimator.SetBool("isScared", true);
            yield return new WaitForSeconds(4f);
            ghostAnimator.SetBool("isDead", true);
        }

        if (pelletAnimator != null)
        {
        }
    }
}


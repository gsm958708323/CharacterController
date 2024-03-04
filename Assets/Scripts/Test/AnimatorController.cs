using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public Animator Animator;
    public static int HorizontalMovement = Animator.StringToHash("HorizontalMovement");
    public static int VerticalMovement = Animator.StringToHash("VerticalMovement");
    public static int IsMovement = Animator.StringToHash("IsMovement");
    public static int IsAiming = Animator.StringToHash("IsAiming");

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHorizontalMovement(float value)
    {
        if (value <= 0.01f)
        {
            value = 0;
        }
        Animator.SetFloat(HorizontalMovement, value);
    }
    public void SetVerticalMovement(float value)
    {
        if (value <= 0.01f)
        {
            value = 0;
        }
        Animator.SetFloat(VerticalMovement, value);
    }
}

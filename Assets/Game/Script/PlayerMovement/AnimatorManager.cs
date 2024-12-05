using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator _animator;
    private int _horizontal;
    private int _vertical;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _horizontal = Animator.StringToHash("Horizontal");
        _vertical = Animator.StringToHash("Veritical");
    }


    public void ChangeAnimatorValue(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontalMovement;
        float snappedVerticalMovement;


        #region Snapped Horizontal


        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontalMovement = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontalMovement = 1f;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontalMovement = -0.55f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontalMovement = -1f;
        }
        else
        {
            snappedHorizontalMovement = 0f;
        }


        #endregion
        #region Snapped Vertical


        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVerticalMovement = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVerticalMovement = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVerticalMovement = -0.55f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVerticalMovement = -1f;
        }
        else
        {
            snappedVerticalMovement = 0f;
        }


        #endregion


        if (isSprinting)
        {
            snappedHorizontalMovement = horizontalMovement;
            snappedHorizontalMovement = 2;
        }

        _animator.SetFloat(_horizontal, snappedHorizontalMovement, 0.1f, Time.deltaTime);
        _animator.SetFloat(_vertical, snappedVerticalMovement, 0.1f, Time.deltaTime);
    }


    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        _animator.SetBool("isInteracting", isInteracting);
        _animator.CrossFade(targetAnim, 0.2f);
    }



}

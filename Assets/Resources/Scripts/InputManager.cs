using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class InputManager : MonoBehaviour
{



    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCanceClimbInput;
    private void CheckMovementInput()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertikal = Input.GetAxis("Vertical");

        OnMoveInput(new Vector2(Horizontal, Vertikal));
        
    }

    private void CheckJumpInput()
    {
        bool jump = Input.GetKey(KeyCode.Space);


        if (jump)
        {
            if (OnJumpInput != null) OnJumpInput();
        }

    }

    private void CheckSprintInput()
    {
        bool run = Input.GetKey(KeyCode.LeftShift);
    
        if (run)
        {
            if(OnSprintInput != null) OnSprintInput(true);
        }
        else
        {
            if(OnSprintInput != null) OnSprintInput(false);

        }


    }
    private void CheckSprintCrouchInput() 
    { 
        bool crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (crouch) Debug.Log("Jongkok");
    }

    private void CheckChangePOVInput() 
    {
        bool pov = Input.GetKey(KeyCode.Q);
        if (pov) Debug.Log("Mengganti POV");
    
    }

    private void CheckClimbInput()
    {
        bool Climb = Input.GetKey(KeyCode.E);
        if (Climb) OnClimbInput();
    }

    private void CheckStopClimbInput()
    {
        bool StopClimb = Input.GetKey(KeyCode.C);
        if (StopClimb)
        {
            if(OnCanceClimbInput != null) OnCanceClimbInput();
        }
    }

    private void CheckHitInput()
    {
        bool hit = Input.GetKey(KeyCode.Mouse0);
        if (hit) Debug.Log("Hit");
    }

    private void CheckMenuInput()
    {
        bool menu = Input.GetKey(KeyCode.Escape);
        if (menu) Debug.Log("Main menu");
    }
    void Start()
    {
        
    }

    void Update()
    {
        CheckMovementInput();   
        CheckJumpInput();   
        CheckSprintInput();
        CheckSprintCrouchInput();
        CheckChangePOVInput();
        CheckClimbInput();
        CheckStopClimbInput();
        CheckHitInput();
        CheckMenuInput();
    }
}

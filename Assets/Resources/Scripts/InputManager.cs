using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{



    public Action<Vector2> OnMoveInput;
    public Action<bool> OnSprintInput;
    public Action OnJumpInput;
    public Action OnClimbInput;
    public Action OnCanceClimbInput;
    public Action OnChangePOV;
    public Action OnChangeCrouch;
    public Action OnGlide;

    public Action OnCancelGlide;

    public Action OnPunchInput;
    public Action OnMainMenuInput;



    

    
    private void CheckGlide()
    {
        bool OnGldie = Input.GetKeyDown(KeyCode.G);
        if (OnGldie) if (OnGlide != null) OnGlide();
    }
    private void CheckCancelGlide()
    {
        bool OnGldie = Input.GetKeyDown(KeyCode.C);
        if (OnGldie) if (OnGlide != null) OnCancelGlide();
    }
    private void CheckMovementInput()
    {
        float Horizontal = Input.GetAxis("Horizontal");
        float Vertikal = Input.GetAxis("Vertical");

        OnMoveInput(new Vector2(Horizontal, Vertikal));
        
    }

    private void CheckJumpInput()
    {
        bool jump = Input.GetKeyDown(KeyCode.Space);


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
        bool crouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        if (crouch) {
            OnChangeCrouch();
        } ;
    }

    private void CheckChangePOVInput() 
    {
        bool pov = Input.GetKeyDown(KeyCode.Q);
        if (pov) if (OnChangePOV != null) OnChangePOV(); 
    
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
        bool hit = Input.GetKeyDown(KeyCode.Mouse0);
        if (hit) { 
            OnPunchInput();
        }
    }

    private void CheckMenuInput()
    {
        bool menu = Input.GetKey(KeyCode.Escape);
        if (menu) if(OnMainMenuInput != null) OnMainMenuInput();
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
        CheckGlide();
        CheckCancelGlide();
    }
}

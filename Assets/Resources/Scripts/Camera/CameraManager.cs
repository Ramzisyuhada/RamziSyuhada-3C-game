using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [Header("Camera System")]
    [Tooltip("GameOneject Camera Manager")]
    [SerializeField] public CameraState _CameraState;


    [Header("Camera System")]
    [Tooltip("GameOneject FPSCamera Manager")]

    [SerializeField] CinemachineVirtualCamera _fpsCamera;


    [Header("Camera System")]
    [Tooltip("GameOneject TPSCamera Manager")]

    [SerializeField] CinemachineFreeLook _tpsCamera;

    public Action OnChagePerspective;

    public void SetFpsClampedCamera(bool isClamped, Vector3 playerRotation)
    {


        CinemachinePOV pov = _fpsCamera.GetCinemachineComponent<CinemachinePOV>();
        if (isClamped)

        {
            pov.m_HorizontalAxis.m_Wrap = false;
            pov.m_HorizontalAxis.m_MinValue = playerRotation.y - 45;
            pov.m_HorizontalAxis.m_MaxValue = playerRotation.y + 45;
        }
        else
        {
            pov.m_HorizontalAxis.m_Wrap = true;
            pov.m_HorizontalAxis.m_MinValue = -180;
            pov.m_HorizontalAxis.m_MaxValue = 180;

        }

    }
    

    public void SwitchCamera()
    {


        if (CameraState.ThirdPerson == _CameraState)
        {
            OnChagePerspective();

            _CameraState = CameraState.FirstPerson;
            _fpsCamera.gameObject.SetActive(true);
            _tpsCamera.gameObject.SetActive(false);
        }
        else
        {
            OnChagePerspective();

            _CameraState = CameraState.ThirdPerson;
            _fpsCamera.gameObject.SetActive(false);
            _tpsCamera.gameObject.SetActive(true);

        }
    }


    public void SetTPSFieldOfView(float fieldOfView)
    {
        _tpsCamera.m_Lens.FieldOfView = fieldOfView;
    }
}

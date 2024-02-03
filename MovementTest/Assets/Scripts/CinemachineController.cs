using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{

    public CinemachineVirtualCamera cinemachineVirtualCamera;

    public void MakeCameraSmaller()
    {
        IncreaseCameraArea(5f, 1f);
    }

    public void MakeCameraLarger()
    {
        IncreaseCameraArea(9f, 1f);
    }

    public void IncreaseCameraArea(float targetOrthoSize, float duration)
    {
        StartCoroutine(LerpCameraOrthoSize(targetOrthoSize, duration));
    }

    private IEnumerator LerpCameraOrthoSize(float targetOrthoSize, float duration)
    {
        float startOrthoSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newOrthoSize = Mathf.Lerp(startOrthoSize, targetOrthoSize, time / duration);
            cinemachineVirtualCamera.m_Lens.OrthographicSize = newOrthoSize;
            yield return null;
        }

        cinemachineVirtualCamera.m_Lens.OrthographicSize = targetOrthoSize;
    }

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MakeCameraLarger();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            MakeCameraSmaller();
        }
    }


}

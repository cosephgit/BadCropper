using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// manages the camera and background positions to pan with the player and create parallax effect

public class ParallaxMenuManager : MonoBehaviour
{
    [SerializeField]Transform skyTransform;
    [SerializeField]float skyParaMagnitude = 0.1f;
    [SerializeField]Transform horizonTransform;
    [SerializeField]float horizonParaMagnitude = 0.2f;
    [SerializeField]Camera mainCamera;
    [SerializeField]float cameraParaMagnitude = 0.9f;
    [SerializeField]float screenMoveRate = 0.1f;
    [SerializeField]float screenMoveMagnitude = 10f;
    float timePassed;

    void LateUpdate()
    {
        timePassed += Time.deltaTime;
        float playerX = Mathf.Sin(timePassed * screenMoveRate) * screenMoveMagnitude;
        float skyX = playerX * skyParaMagnitude;
        float horizonX = playerX * horizonParaMagnitude;
        float cameraX = playerX * cameraParaMagnitude;
        Vector3 pos;

        pos = skyTransform.position;
        pos.x = skyX;
        skyTransform.position = pos;

        pos = horizonTransform.position;
        pos.x = horizonX;
        horizonTransform.position = pos;

        pos = mainCamera.transform.position;
        pos.x = cameraX;
        mainCamera.transform.position = pos;
    }
}

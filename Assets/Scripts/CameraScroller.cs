using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] private float cameraScrollSpeed = 5f;
    private void Update()
    {
        transform.Translate(Vector2.right * (Time.deltaTime * cameraScrollSpeed));
    }
}

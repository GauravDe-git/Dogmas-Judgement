using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float _length, _startPos;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallaxEffect;

    private void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(_startPos + dist, transform.position.y);
        if (temp > _startPos + _length)
        {
            _startPos += _length;
        }
        else if (temp < _startPos - _length)
        {
            _startPos -= _length;
        }
    }
}

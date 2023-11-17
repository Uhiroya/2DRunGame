using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScroller : MonoBehaviour
{
    RawImage _image;
    [SerializeField] float _uvSpeed;
    
    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }
    public void ManualUpdate(float deltaTime)
    {
        Move(deltaTime);
    }
    public void Move(float deltaTime)
    {
        var rect = _image.uvRect;
        rect.y += _uvSpeed * deltaTime;
        rect.y %= 1f;
        _image.uvRect = rect;
    }
    public void SetUVSpeed(float uvSpeed)
    {
        _uvSpeed = uvSpeed;
    }
}

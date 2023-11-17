using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundController : MonoBehaviour
{
    RawImage _image;
    [SerializeField] float _uvSpeed;
    
    private void Awake()
    {
        _image = GetComponent<RawImage>();
    }
    public void ManualUpdate(float deltaTime)
    {
        UVScroll(deltaTime);
    }
    public void UVScroll(float deltaTime)
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

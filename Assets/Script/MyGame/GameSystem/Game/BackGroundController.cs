using UnityEngine.UI;

public interface IBackGroundController
{
    public void ManualUpdate(float deltaTime);
    public void SetUVSpeed(float uvSpeed);
}

public class BackGroundController : IBackGroundController
{
    private readonly RawImage _image;
    private float _uvSpeed;

    public BackGroundController(RawImage image)
    {
        _image = image;
    }

    public void ManualUpdate(float deltaTime)
    {
        UVScroll(deltaTime);
    }

    public void SetUVSpeed(float uvSpeed)
    {
        _uvSpeed = uvSpeed;
    }

    private void UVScroll(float deltaTime)
    {
        var rect = _image.uvRect;
        rect.y += _uvSpeed * deltaTime;
        rect.y %= 1f;
        _image.uvRect = rect;
    }
}

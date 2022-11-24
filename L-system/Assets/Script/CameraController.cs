
using UnityEngine;

using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public enum MouseState
    {
        None,
        MidMouseBtn,
        LeftMouseBtn
    }
    private MouseState mMouseState = MouseState.None;
    float _mouseX = 0;
    float _mouseY = 0;
    public float moveSpeed = 1;

    public Slider rotationSlider;
    public float lastValue;
    private void Start()
    {
        if(rotationSlider != null)
        {
            lastValue = rotationSlider.value;
            rotationSlider.onValueChanged.AddListener(delegate { handleSlider(); }) ;
        }
    }

    public void Update()
    {
        CameraMove();
    }

    public void CameraMove()
    {
        if (Input.GetMouseButton(2))
        {
            _mouseX = Input.GetAxis("Mouse X");
            _mouseY = Input.GetAxis("Mouse Y");

            
            Vector3 moveDir = (_mouseX * -transform.right + _mouseY * -transform.up);

            
            //moveDir.y = 0;
            transform.position += moveDir * 0.5f * moveSpeed;
        }
        else if (Input.GetMouseButtonDown(2))
        {
            mMouseState = MouseState.MidMouseBtn;
            Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }
        else if (Input.GetMouseButtonUp(2))
        {
            mMouseState = MouseState.None;
            Debug.Log(GetType() + "mMouseState = " + mMouseState.ToString());
        }

    }

    public void CameraRotation(float angle)
    { 
        
    }

    private void handleSlider()
    {
        Debug.Log(rotationSlider.value);
        float deltaAngle=(rotationSlider.value -lastValue) *360;
        transform.RotateAround(Vector3.zero, Vector3.up, deltaAngle);
        lastValue = rotationSlider.value;
    }

}

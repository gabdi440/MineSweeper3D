using UnityEngine;

public class camerascript : MonoBehaviour
{
    public map Map;
    GameObject camera_GameObject;
    Camera cam;
    Vector2 StartPosition;
    Vector2 initPosition = new Vector2 (-1,-1);
    public float orthoZoomSpeed = 0.5f;
    bool isZooming;

    void Start()
    {
        camera_GameObject = transform.GetChild(0).gameObject;
        cam = camera_GameObject.GetComponent<Camera>();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Map.cam_on) { return; }
        if (Input.touchCount == 0)
        {
            if (isZooming)
            {
                isZooming = false;
            }

            if(initPosition != new Vector2(-1, -1))
            {
                initPosition = new Vector2(-1, -1);
            }

        }
    
        if (Input.touchCount == 1)
        {
            if (!isZooming)
            {
                if (initPosition == new Vector2(-1, -1))
                {
                    initPosition = Input.GetTouch(0).position;
                }

                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {               
                 //   StartPosition = Input.GetTouch(0).position;             
                    Vector2 NewPosition = GetWorldPosition();
                    Vector2 PositionDifference = NewPosition - StartPosition;
                    camera_GameObject.transform.parent.Translate(-PositionDifference);
                    PositionDifference = NewPosition - initPosition;

                    Vector2 pos_differance = Input.GetTouch(0).position;
                    pos_differance.x = (initPosition.x - pos_differance.x);
                    pos_differance.y = (initPosition.y - pos_differance.y);
                    if (pos_differance.x > Screen.width * .2f || pos_differance.x < -Screen.width * .2f || pos_differance.y > Screen.width * .2f || pos_differance.y < -Screen.width * .2f) { Map.clic_void(); }
                    //limits
                    if (camera_GameObject.transform.parent.position.x < Map.map_center.x - ( Map.size.x * 10)) { camera_GameObject.transform.parent.position = new Vector3(Map.map_center.x - (Map.size.x * 10), 1, camera_GameObject.transform.parent.position.z); }
                    if(camera_GameObject.transform.parent.position.x > Map.map_center.x + ( Map.size.x * 10)) { camera_GameObject.transform.parent.position = new Vector3(Map.map_center.x + (Map.size.x * 10), 1, camera_GameObject.transform.parent.position.z); }
                    if(camera_GameObject.transform.parent.position.z < Map.map_center.y - ( Map.size.y * 10)) { camera_GameObject.transform.parent.position = new Vector3(camera_GameObject.transform.parent.position.x, 1, (Map.map_center.x - (Map.size.x * 10))); }
                    if(camera_GameObject.transform.parent.position.z > Map.map_center.y + ( Map.size.y * 10)) { camera_GameObject.transform.parent.position = new Vector3(camera_GameObject.transform.parent.position.x, 1, (Map.map_center.x + (Map.size.x * 10))); }

                }
                StartPosition = GetWorldPosition();
            }
        }
        else if (Input.touchCount == 2)
        {
            isZooming = true;
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            if (cam.orthographic)
            {
                // ... change the orthographic size based on the change in distance between the touches.
                cam.orthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;

                // Make sure the orthographic size never drops below zero.
                cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
               
                //limits
                if (cam.orthographicSize < 45) { cam.orthographicSize = 45; }
                if (cam.orthographicSize > 300) { cam.orthographicSize = 300; }
            }

        }
    }

    Vector2 GetWorldPosition()
    {
        return camera_GameObject.transform.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
    }

    Vector2 GetWorldPositionOfFinger(int FingerIndex)
    {
        return camera_GameObject.GetComponent<Camera>().ScreenToWorldPoint(Input.GetTouch(FingerIndex).position);
    }
}

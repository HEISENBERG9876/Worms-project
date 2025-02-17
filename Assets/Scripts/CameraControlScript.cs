using UnityEngine;
using System.Collections;

public class CameraControlScript : MonoBehaviour
{
    public float zoomSpeed = 20.0f;
    public float dragSpeed = 1.0f;

    private float minCameraX = 0f;
    private float maxCameraX = 80f;
    private float minCameraY = 0f;
    private float maxCameraY = 40f;

    private Vector3 dragOrigin;
    private bool isFollowing = false;

    void FixedUpdate()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        ZoomCamera(scroll);

        DragCamera();

    }

    private void ZoomCamera(float scrollValue)
    {
        Camera.main.fieldOfView -= scrollValue * zoomSpeed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 10f, 120f);
    }

    private void DragCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopFollowing();
            dragOrigin = Input.mousePosition;
            return;
        }

        if (!Input.GetMouseButton(0)) return;

        Vector3 pos = Camera.main.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
        Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);

        Vector3 newPosition = transform.position + move;
        newPosition.x = Mathf.Clamp(newPosition.x, minCameraX, maxCameraX);
        newPosition.y = Mathf.Clamp(newPosition.y, minCameraY, maxCameraY);

        transform.position = newPosition;
    }

    public IEnumerator FollowProjectile(GameObject obj)
    {
        isFollowing = true;
        Vector3 origPosition = transform.position;

        while (obj != null && isFollowing)
        {
            float newX = Mathf.Clamp(obj.transform.position.x, minCameraX, maxCameraX);
            float newY = Mathf.Clamp(obj.transform.position.y, minCameraY, maxCameraY);
            transform.position = new Vector3(newX, newY, transform.position.z);
            yield return null;
        }

        isFollowing = false;
        yield return StartCoroutine(SmoothlyTransitionToPosition(origPosition));
    }

    private IEnumerator SmoothlyTransitionToPosition(Vector3 pos)
    {
        float lerpTime = 0f;
        while (lerpTime < 1.0f)
        {
            lerpTime += Time.deltaTime * 2.0f;
            transform.position = Vector3.Lerp(transform.position, pos, lerpTime);
            yield return null;
        }
    }
    public void StopFollowing()
    {
        isFollowing = false;
    }

    public void SetPosOnGameObject(GameObject obj)
    {
        float newX = Mathf.Clamp(obj.transform.position.x, minCameraX, maxCameraX);
        float newY = Mathf.Clamp(obj.transform.position.y, minCameraY, maxCameraY);
        StartCoroutine(SmoothlyTransitionToPosition(new Vector3(newX, newY, transform.position.z)));
    }



}
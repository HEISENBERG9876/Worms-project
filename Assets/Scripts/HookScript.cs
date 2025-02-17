using UnityEngine;

public class HookScript : UsableItemScript
{
    [SerializeField] GameObject worm;
    [SerializeField] GameObject hookRotationPivot;
    private float rotationSpeed = 0.6f;

    private float maxHookDistance = 20.0f;

    private bool isGrappling = false;
    private Vector3 hookPoint;
    private SpringJoint joint;
    private LineRenderer ropeRenderer;

    void Start()
    {
        ropeRenderer = worm.AddComponent<LineRenderer>();
        ropeRenderer.positionCount = 2;
        ropeRenderer.material = new Material(Shader.Find("Sprites/Default"));
        ropeRenderer.startWidth = 0.1f;
        ropeRenderer.endWidth = 0.1f;
        ropeRenderer.enabled = false;
    }

    public override void Use()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isGrappling)
        {
            Vector3 hookDirection = hookRotationPivot.transform.right;
            RaycastHit hit;

            if (Physics.Raycast(hookRotationPivot.transform.position, hookDirection, out hit, maxHookDistance))
            {
                isGrappling = true;
                hookPoint = hit.point;

                joint = worm.AddComponent<SpringJoint>();

                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = hookPoint;

                float distanceFromHookPoint = Vector3.Distance(transform.position, hookPoint);
                joint.minDistance = 0f;
                joint.maxDistance = 0f;

                joint.spring = 10.0f;
                joint.damper = 10.0f;
                joint.massScale = 5.0f;

                ropeRenderer.enabled = true;
                ropeRenderer.SetPosition(0, transform.position);
                ropeRenderer.SetPosition(1, hookPoint);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space) && isGrappling)
        {
            StopGrapple();
        }
    }

    void StopGrapple()
    {
        Destroy(joint);
        isGrappling = false;

        ropeRenderer.enabled = false;
    }

    void UpdateRopeRenderer()
    {
        ropeRenderer.SetPosition(0, transform.position);
        ropeRenderer.SetPosition(1, hookPoint);
    }

    void Update()
    {
        if (isGrappling)
        {
            UpdateRopeRenderer();
        }
    }

    public override void Rotate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            hookRotationPivot.transform.Rotate(0f, 0f, rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            hookRotationPivot.transform.Rotate(0f, 0f, -rotationSpeed);
        }
    }
}

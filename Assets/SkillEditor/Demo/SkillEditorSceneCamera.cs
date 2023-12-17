using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SkillEditorSceneCamera : MonoBehaviour
{
    //聚焦对象
    [SerializeField]
    public Transform focus = default;
    //距离
    [SerializeField, Range(1f, 20f)]
    float distance = 5f;
    //聚焦半径 追焦
    [SerializeField, Min(0f)]
    float focusRadius = 1f;
    //聚焦中心系数
    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;

    Vector3 focusPoint;

    //轨道相机角度仅仅旋转 x y 轴
    Vector2 orbitAngles = new Vector2(45f, 0f);
    //旋转速度
    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;
    //最大俯仰角度
    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    //自动对其延迟
    [SerializeField, Min(0f)]
    float alignDelay = 5f;
    float lastManualRotationTime;
    Vector3 previousFocusPoint;
    //对其角度
    [SerializeField, Range(0f, 90f)]
    float alignSmoothRange = 45f;
    //可以遮挡相机的层级
    [SerializeField]
    LayerMask obstructionMask = -1;
    //Camera组件
    Camera regularCamera;
    //翻转XY轴
    [SerializeField, Range(-1, 1)]
    int revseX = 1, revseY = -1;
    //重力对其
    Quaternion gravityAlignment = Quaternion.identity;
    Quaternion orbitRotation;
    //锁定光标
    [SerializeField]
    bool loockCouser;

    //向上对其的速度
    //[SerializeField, Min(0f)]
    //float upAlignmentSpeed = 360f;

    Vector3 CameraHalfExtends
    {
        get
        {
            Vector3 halfExtends;
            halfExtends.y = regularCamera.nearClipPlane * Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
            halfExtends.x = halfExtends.y * regularCamera.aspect;
            halfExtends.z = 0f;
            return halfExtends;
        }
    }
        
    void Awake()
    {
        regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
        transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
        Cursor.lockState = loockCouser ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void Update()
    {
    }

    //防止移动中的对象成为焦点
    void LateUpdate()
    {
        previousFocusPoint = focusPoint;
        UpdateFocusPoint();

        //设置聚焦相机旋转
        //Quaternion lookRotation;
        if (ManualRotation() || AutomaticRotation())
        {
            ConstrainAngles();
            //lookRotation = Quaternion.Euler(orbitAngles);
            orbitRotation = Quaternion.Euler(orbitAngles);
        }

        Quaternion lookRotation = gravityAlignment * orbitRotation;

        //绕 x y 轴旋转
        Vector3 lookDirection = lookRotation * Vector3.forward; ;

        //聚焦位置的后distance米
        Vector3 lookPosition = focusPoint - lookDirection * distance;

        Vector3 rectOffset = lookDirection * regularCamera.nearClipPlane;
        Vector3 rectPosition = lookPosition + rectOffset;
        Vector3 castFrom = focus.position;
        Vector3 castLine = rectPosition - castFrom;
        float castDistance = castLine.magnitude;
        Vector3 castDirection = castLine / castDistance;


        //检测是否中间有遮挡
        if (Physics.BoxCast(
            castFrom, CameraHalfExtends, castDirection, out RaycastHit hit,
            lookRotation, castDistance, obstructionMask
        ))
        {
            //变化为障碍物前的位置
            rectPosition = castFrom + castDirection * hit.distance;
            lookPosition = rectPosition - rectOffset;
        }

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    //跟新聚焦点
    void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;

        float t = 1f;
        if (distance > 0.01f && focusCentering > 0f)
        {
            //(1-time)^c
            t = Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime);
        }

        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            //相机与聚焦物体距离大于追焦半径 
            if (distance > focusRadius)
            {
                //进行插值追焦
                //focusPoint = Vector3.Lerp(targetPoint, focusPoint, focusRadius / distance);
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }
    }

    bool ManualRotation()
    {
        Vector2 input = new Vector2(
           revseY * Input.GetAxis("Mouse Y"),
           revseX * Input.GetAxis("Mouse X")
        );
        const float e = 0.001f;
        //如果输入超过某个小 epsilon 值（例如 0.001），则将输入添加到轨道角，并按旋转速度和时间增量进行缩放。再次，我们使其独立于游戏时间。
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        return false;
    }

    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }

    //矫正角度
    void ConstrainAngles()
    {
        orbitAngles.x = Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }

    //自动旋转
    bool AutomaticRotation()
    {
        if (Time.unscaledTime - lastManualRotationTime < alignDelay)
        {
            return false;
        }

        Vector3 alignedDelta =
        Quaternion.Inverse(gravityAlignment) *
        (focusPoint - previousFocusPoint);
        Vector2 movement = new Vector2(alignedDelta.x, alignedDelta.z);

        float movementDeltaSqr = movement.sqrMagnitude;

        //焦点位移非常小
        if (movementDeltaSqr < 0.0001f)
        {
            return false;
        }

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));

        float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));

        float rotationChange = rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);

        if (deltaAbs < alignSmoothRange)
        {
            rotationChange *= deltaAbs / alignSmoothRange;
        }
        else if (180f - deltaAbs < alignSmoothRange)
        {
            rotationChange *= (180f - deltaAbs) / alignSmoothRange;
        }
        orbitAngles.y = Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
        return true;
    }

    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
    }
}
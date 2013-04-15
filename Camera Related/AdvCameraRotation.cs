using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class AdvCameraRotation : MonoBehaviour
{
    /// <summary>
    /// This is the Lazy Cameramans Platforming - Eh.. Camera!
    /// 
    /// Todo: Add Lazy movement  !!  Needs to move only when absolutely necessary.
    /// Todo: Clean Code structure  !!  Readability.
    /// Todo: Add Pathing via Spline Movement  !!  So the camera can freely move and use the animation curves morso.
    /// Todo: Better Null Handling !! We don't want errors everywhere when the user doesn't set up correctly, now do we?
    /// Todo: Unlock from Player !! Allow camera to work perfectly without player references.
    /// Todo: Nice and Easy GUI !! Don't make it look like crap.
    /// Todo: Keybinding !! Nothing set in stone, all needs to be changed for the greater good.
    /// 
    /// </summary>

    public AnimationCurve RotateTime = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
    public int RotAmount;
    private CrunchPlatformColliders _cruncher;
    private DisablePlayer _disablePlayer;
    private float currentAngle;
    public float AllocatedTime;
    private float dT;
    private bool Rotating;
    private GameObject player;
    public Camera camera;

    enum AnimationStates
    {
        
    }

    void OnAwake()
    {
        camera = Camera.main;
    }

    private Matrix4x4 ortho, perspective;
    public float fov = 60f, near = .3f, far = 1000f, orthographicSize = 50f;
    private float aspect;
    private bool orthoOn;

    private float rotationX, rotationY;
    public float lookSpeed, moveSpeed;
    public bool FreeView;

    private void Start()
    {
        _cruncher = GetComponentInChildren<CrunchPlatformColliders>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _disablePlayer = player.GetComponentInChildren<DisablePlayer>();

        aspect = (float)Screen.width / (float)Screen.height;
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov, aspect, near, far);
        camera.projectionMatrix = ortho;
        orthoOn = true;
    }


    private float animStartTime, animLength, animEndTime;
    private float endAngle, startAngle;
    private float animMaxVal;
    private float endTime;



    #region Projection Code

    public static Matrix4x4 MatrixLerp(Matrix4x4 from, Matrix4x4 to, float time)
    {
        Matrix4x4 ret = new Matrix4x4();
        for (int i = 0; i < 16; i++)
            ret[i] = Mathf.Lerp(from[i], to[i], time);
        return ret;
    }



    private IEnumerator LerpFromTo(Matrix4x4 src, Matrix4x4 dest, float duration)
    {
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            camera.projectionMatrix = MatrixLerp(src, dest, (Time.time - startTime) / duration);
            yield return 1;
        }

        camera.projectionMatrix = dest;
    }



    public Coroutine BlendToMatrix(Matrix4x4 targetMatrix, float duration)
    {
        StopAllCoroutines();
        return StartCoroutine(LerpFromTo(camera.projectionMatrix, targetMatrix, duration));
    }

    #endregion

    #region Rotation Code
    private IEnumerator RotateThisThing(float startAngle, float endAngle, float time)
    {
        //-- Start of Rotation--//
        while (dT < time) //Check if running overtime
        {
            Rotating = true;
            dT += Time.deltaTime;
            //Rotate the camera
            currentAngle = Mathf.LerpAngle(startAngle, endAngle,
                                           RotateTime.Evaluate((dT/time)*animLength + animStartTime)/animMaxVal);
                                           //Evaluate the Animation Curve, Time in rotation = Time in animation
            transform.rotation = Quaternion.Euler(0, currentAngle, 0);
            yield return null;
        }
        //-- Declare Rotation Complete--//
        Rotating = false;
        RotationComplete();
    }

    private void setupRotation(float targetAngle, float time)
    {
        //-- Clamp the Angle between 0-360, so that it can't extend from that number--//
        startAngle = Mathf.Repeat(currentAngle, 360);
        endAngle = Mathf.Repeat((targetAngle + currentAngle), 360);
                                //Take the current angle into consideration when rotating
        endTime = time;
        dT = 0.0f;

        //--Get the properties of the animation--//
        animStartTime = RotateTime[0].time;
        animEndTime = RotateTime[RotateTime.length - 1].time;
        animLength = animEndTime - animStartTime;
        //--Get the Maximum Value of the animation curve--//
        animMaxVal = 0;
        for (int ii = 0; ii < RotateTime.length; ++ii)
        {
            if (RotateTime[ii].value > animMaxVal)
            {
                animMaxVal = RotateTime[ii].value;
            }
        }
        //--Initiate Rotation, Disable the characters movement--//
        _cruncher.setPlayerPos();
        _disablePlayer.disable();
        StartCoroutine(RotateThisThing(startAngle, endAngle, time));
    }
    #endregion

    private void FreeViewActive()
    {
        BlendToMatrix(perspective, 1f);
        _disablePlayer.disable();
        rotationX += Input.GetAxis("Mouse X") * lookSpeed;
        rotationY += Input.GetAxis("Mouse Y") * lookSpeed;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical");
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal");
    }

    private void Update()
    {
        startAngle = transform.rotation.y;
        if (FreeView)
            FreeViewActive();

        if (!Rotating)
        {

            if (Input.GetKeyDown(KeyCode.Z))
            {
                setupRotation(RotAmount, AllocatedTime);
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                setupRotation(-RotAmount, AllocatedTime);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            orthoOn = !orthoOn;
            BlendToMatrix(orthoOn ? ortho : perspective, 1f);
        }
    }

    private void RotationComplete()
    {
        _disablePlayer.enable();
        _cruncher = GetComponentInChildren<CrunchPlatformColliders>();
        _cruncher.crunchCollidersToPlayer();
    }

}


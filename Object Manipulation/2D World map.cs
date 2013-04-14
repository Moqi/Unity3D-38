using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
    public float SizeOfWorld = 10f; //Size of World
    public int Spaces; //Spaces on World
    public GameObject Point; //Each point on the world
    public GameObject testb;
    private Vector3 spawnpoint;
    private GameObject _center;
    private GameObject tapToBuild;
    [SerializeField] private GameObject _camera;
    [HideInInspector] public List<GameObject> SOL = new List<GameObject>(); //List Of Created spaces
    [HideInInspector] public List<GameObject> TAK = new List<GameObject>(); //List Of Taken spaces
    [HideInInspector] public float InnerCircle; //Inner Selection
    [HideInInspector] public float OuterCirlce; //Outer Selection
    BuildQueue _buildQ;
    MainCamera _main;

    public World()
    {
        
    }

    // Use this for initialization
    private void Start()
    {
        _buildQ = GetComponent<BuildQueue>();
        _main = _camera.GetComponent<MainCamera>();
        Vector3 thisCenter = transform.position;

        float ang;
        for (int x = 0; x < Spaces; x++)
        {
            if (x >= 0) //Evenly distribute 
                ang = 360f/(Spaces)*x;
            else
                ang = 0.01f; //Starting point.

            Vector3 pos;
            pos.x = thisCenter.x + SizeOfWorld*Mathf.Sin(ang*Mathf.Deg2Rad);
            pos.y = thisCenter.y + SizeOfWorld*Mathf.Cos(ang*Mathf.Deg2Rad);
            pos.z = thisCenter.z;

            GameObject spoint = Instantiate(Point, pos, transform.rotation) as GameObject;
            SOL.Add(spoint);
            if (spoint != null) spoint.transform.parent = transform;
            
        }
       

    }

    private bool _ib = true;

    void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos, int tapCount)
    {
        if (!tapToBuild) return;
        fingerPos = GetWorldPos(fingerPos);
        IntBuild(fingerPos);
    }

    #region Building Objects with tapToBuild

    private GameObject chGO;

    void IntBuild(Vector2 where)
    {
        //--------------------------------------------
        if (_ib){//-----------------No Blueprints laid
        //--------------------------------------------    
            if(CheckFree(GetNearestSpawnPoint(where)))
            {
                //------------------------------------------------------------------
                //Lay the blueprints, unless there's already a building, then abort.
                //------------------------------------------------------------------
                GameObject temp = Instantiate(_buildQ.NextBuild(), GetNearestSpawnPoint(where), Quaternion.identity) as GameObject;
                TAK.Add(temp);
                temp.tag = "BluePrints";
                chGO = temp;
                _ib = false;
                Debug.Log("One");
            }
            else
            {
                Destroy(chGO);
                TAK.Remove(chGO);
                Debug.Log("Two");
            }
        }
        //-------------------------------------
        else//----------Blueprints already laid
        //-------------------------------------
        {
            
            if(CheckFree(GetNearestSpawnPoint(where)) == false)
            {
                //-----------------------------------------------------------------------------------------
                //If the user confirms the blueprints, create the building, otherwise move the blueprints
                //-----------------------------------------------------------------------------------------
                if (GetNearestSpawnPoint(where) == new Vector2(chGO.transform.position.x, chGO.transform.position.y))
                {
                    Build(where);
                    _ib = true;
                    Destroy(chGO);
                    TAK.Remove(chGO);
                    Debug.Log("Three");
                }
            }
            else
            {
                chGO.transform.position = GetNearestSpawnPoint(where);
                chGO.transform.LookAt(Vector3.zero);
                //_ib = true;
                Debug.Log("Four");
            }
        }
    }

    public bool CheckFree(Vector3 targetPos)
    {
        return TAK.All(current => current.transform.position != targetPos);
    }

    void Build(Vector2 thispoint)
    {
        GameObject Occu = Instantiate(_buildQ.NextBuild(), GetNearestSpawnPoint(thispoint), Quaternion.identity) as GameObject;
        TAK.Add(Occu);
        Occu.transform.LookAt(Vector3.zero);
    }

    public Vector2 GetNearestSpawnPoint(Vector2 fingerpos){
    // and finally the actual process for finding the nearest object
        float nearestDistanceSqr = Mathf.Infinity;
        
         // loop through each tagged object, remembering nearest one found
        foreach (GameObject obj in SOL)
        {
            var objectPos = new Vector2(obj.transform.position.x, obj.transform.position.y);
            var distanceSqr = (objectPos - fingerpos).sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                spawnpoint = obj.transform.position;
                nearestDistanceSqr = distanceSqr;
            }
        }

       return spawnpoint;
}

    public static Vector3 GetWorldPos(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // we solve for intersection with z = 0 plane
        float t = -ray.origin.z / ray.direction.z;

        return ray.GetPoint(t);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Point.transform.position, SizeOfWorld);

        foreach (GameObject go in SOL)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(go.transform.position, size: new Vector3(0.5f, 0.5f, 0.5f));
        }
        var xa = 1;
        for (int i = 0; i < SOL.Count; i++)
        {
            if (xa >= SOL.Count)
                xa = 0;
            Gizmos.DrawLine(SOL[i].transform.position, SOL[xa].transform.position);
            xa++;
        }
    }

    void OnEnable()
    {
        // Register to FingerGestures events

        // per-finger gestures
        FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
        FingerGestures.OnFingerStationaryBegin += FingerGestures_OnFingerStationaryBegin;
        FingerGestures.OnFingerStationary += FingerGestures_OnFingerStationary;
        FingerGestures.OnFingerStationaryEnd += FingerGestures_OnFingerStationaryEnd;
        FingerGestures.OnFingerMoveBegin += FingerGestures_OnFingerMoveBegin;
        FingerGestures.OnFingerMove += FingerGestures_OnFingerMove;
        FingerGestures.OnFingerMoveEnd += FingerGestures_OnFingerMoveEnd;
        FingerGestures.OnFingerUp += FingerGestures_OnFingerUp;
        FingerGestures.OnFingerLongPress += FingerGestures_OnFingerLongPress;
        FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
        FingerGestures.OnFingerSwipe += FingerGestures_OnFingerSwipe;
        FingerGestures.OnFingerDragBegin += FingerGestures_OnFingerDragBegin;
        FingerGestures.OnFingerDragMove += FingerGestures_OnFingerDragMove;
        FingerGestures.OnFingerDragEnd += FingerGestures_OnFingerDragEnd;

        // global gestures
        FingerGestures.OnLongPress += FingerGestures_OnLongPress;
        FingerGestures.OnTap += FingerGestures_OnTap;
        FingerGestures.OnSwipe += FingerGestures_OnSwipe;
        FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
        FingerGestures.OnDragMove += FingerGestures_OnDragMove;
        FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
        FingerGestures.OnPinchBegin += FingerGestures_OnPinchBegin;
        FingerGestures.OnPinchMove += FingerGestures_OnPinchMove;
        FingerGestures.OnPinchEnd += FingerGestures_OnPinchEnd;
        FingerGestures.OnRotationBegin += FingerGestures_OnRotationBegin;
        FingerGestures.OnRotationMove += FingerGestures_OnRotationMove;
        FingerGestures.OnRotationEnd += FingerGestures_OnRotationEnd;
        FingerGestures.OnTwoFingerLongPress += FingerGestures_OnTwoFingerLongPress;
        FingerGestures.OnTwoFingerTap += FingerGestures_OnTwoFingerTap;
        FingerGestures.OnTwoFingerSwipe += FingerGestures_OnTwoFingerSwipe;
        FingerGestures.OnTwoFingerDragBegin += FingerGestures_OnTwoFingerDragBegin;
        FingerGestures.OnTwoFingerDragMove += FingerGestures_OnTwoFingerDragMove;
        FingerGestures.OnTwoFingerDragEnd += FingerGestures_OnTwoFingerDragEnd;
    }

    void OnDisable()
    {
        // Unregister FingerGestures events so we will no longer receive notifications after this object is disabled

        // per-finger gestures
        FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
        FingerGestures.OnFingerStationaryBegin -= FingerGestures_OnFingerStationaryBegin;
        FingerGestures.OnFingerStationary -= FingerGestures_OnFingerStationary;
        FingerGestures.OnFingerStationaryEnd -= FingerGestures_OnFingerStationaryEnd;
        FingerGestures.OnFingerMoveBegin -= FingerGestures_OnFingerMoveBegin;
        FingerGestures.OnFingerMove -= FingerGestures_OnFingerMove;
        FingerGestures.OnFingerMoveEnd -= FingerGestures_OnFingerMoveEnd;
        FingerGestures.OnFingerUp -= FingerGestures_OnFingerUp;
        FingerGestures.OnFingerLongPress -= FingerGestures_OnFingerLongPress;
        FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
        FingerGestures.OnFingerSwipe -= FingerGestures_OnFingerSwipe;
        FingerGestures.OnFingerDragBegin -= FingerGestures_OnFingerDragBegin;
        FingerGestures.OnFingerDragMove -= FingerGestures_OnFingerDragMove;
        FingerGestures.OnFingerDragEnd -= FingerGestures_OnFingerDragEnd;

        // global gestures
        FingerGestures.OnLongPress -= FingerGestures_OnLongPress;
        FingerGestures.OnTap -= FingerGestures_OnTap;
        FingerGestures.OnSwipe -= FingerGestures_OnSwipe;
        FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
        FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
        FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
        FingerGestures.OnPinchBegin -= FingerGestures_OnPinchBegin;
        FingerGestures.OnPinchMove -= FingerGestures_OnPinchMove;
        FingerGestures.OnPinchEnd -= FingerGestures_OnPinchEnd;
        FingerGestures.OnRotationBegin -= FingerGestures_OnRotationBegin;
        FingerGestures.OnRotationMove -= FingerGestures_OnRotationMove;
        FingerGestures.OnRotationEnd -= FingerGestures_OnRotationEnd;
        FingerGestures.OnTwoFingerLongPress -= FingerGestures_OnTwoFingerLongPress;
        FingerGestures.OnTwoFingerTap -= FingerGestures_OnTwoFingerTap;
        FingerGestures.OnTwoFingerSwipe -= FingerGestures_OnTwoFingerSwipe;
        FingerGestures.OnTwoFingerDragBegin -= FingerGestures_OnTwoFingerDragBegin;
        FingerGestures.OnTwoFingerDragMove -= FingerGestures_OnTwoFingerDragMove;
        FingerGestures.OnTwoFingerDragEnd -= FingerGestures_OnTwoFingerDragEnd;
    }

    #region Per-Finger Event Callbacks

    void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerMoveBegin(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerMove(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerMoveEnd(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerStationaryBegin(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerStationary(int fingerIndex, Vector2 fingerPos, float elapsedTime)
    {

    }

    void FingerGestures_OnFingerStationaryEnd(int fingerIndex, Vector2 fingerPos, float elapsedTime)
    {

    }

    void FingerGestures_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
    {

    }

    void FingerGestures_OnFingerLongPress(int fingerIndex, Vector2 fingerPos)
    {

    }

    void FingerGestures_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
    {

    }

    void FingerGestures_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
    {

    }

    void FingerGestures_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
    {

    }

    #endregion

    #region Global Gesture Callbacks

    void FingerGestures_OnLongPress(Vector2 fingerPos)
    {

    }

    void FingerGestures_OnTap(Vector2 fingerPos, int tapCount)
    {

    }

    void FingerGestures_OnSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
    {

    }

    void FingerGestures_OnDragBegin(Vector2 fingerPos, Vector2 startPos)
    {

    }

    void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
    {

    }

    void FingerGestures_OnDragEnd(Vector2 fingerPos)
    {

    }

    void FingerGestures_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
    {

    }

    void FingerGestures_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
    {

    }

    void FingerGestures_OnPinchEnd(Vector2 fingerPos1, Vector2 fingerPos2)
    {

    }

    void FingerGestures_OnRotationBegin(Vector2 fingerPos1, Vector2 fingerPos2)
    {

    }

    void FingerGestures_OnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
    {

    }

    void FingerGestures_OnRotationEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
    {

    }

    void FingerGestures_OnTwoFingerLongPress(Vector2 fingerPos)
    {

    }

    void FingerGestures_OnTwoFingerTap(Vector2 fingerPos, int tapCount)
    {

    }

    void FingerGestures_OnTwoFingerSwipe(Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
    {

    }

    void FingerGestures_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
    {

    }

    void FingerGestures_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
    {

    }

    void FingerGestures_OnTwoFingerDragEnd(Vector2 fingerPos)
    {

    }

    #endregion
}
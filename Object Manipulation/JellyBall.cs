using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class JellyBall : MonoBehaviour
{
    public enum Space { _, __ }
    public GameObject Spring;
    public GameObject CenterSpring;
    public Space _;
    public int NumSprings = 21; //Layer density
    public float SkinThickness = 5f; //Space betwen layers
    public int OverallSpring = 2; //Stregnth of spring
    public float Damper = 0.2f; //Resistance to bounce
    public int MaxForce = 5;
    public Space __;
    public int InsideOverallSpring = 2; //Stregnth of spring
    public float InsideDamper = 0.2f; //Resistance to bounce
    public int InsideMaxForce = 5;
    public Space _____;
    public int Radius = 10; //Overall size of Goo
    public Space ___;
    public int MaxBounce = 0; //The Maximum amount of bounce when hitting the limit
    public int Max = 0; //Max limit of the joint
    public int Min = 0; //Min limit of the joint
    public int MinBounce = 0; //Minimum Bounce of the joint
    public Space ____;
    public bool UseGravity;
    private GameObject[] _cachedpos;
    [HideInInspector]
    public List<GameObject> lList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> lList2 = new List<GameObject>();
    
    public Transform target;
    
	// Use this for initialization
	void Start ()
	{
	    Vector3 thisCenter = transform.position; //The players center
	    Vector3 pos;
	    Vector3 posx2;

        // Center/Anchor Point

	    #region Center Spring

	    GameObject springCenter = Instantiate(Spring, thisCenter, transform.rotation) as GameObject;
	    springCenter.transform.parent = transform;
	    var centerJoint = springCenter.AddComponent<SpringJoint>();
	    var centerrid = springCenter.GetComponent<Rigidbody>();
	    centerrid.useGravity = false;
	    centerJoint.connectedBody = rigidbody;
	    springCenter.AddComponent("SpringJoint");
	    springCenter.name = "SpringCenter";
	    centerJoint.maxDistance = 0f;
	    centerJoint.minDistance = 0f;
	    centerJoint.spring = 10f;
	    CenterSpring = springCenter;

	    #endregion

        // Main Joint Settings

	    #region Joint Settings

	    JointDrive layer = new JointDrive();
	    layer.positionSpring = OverallSpring;
	    layer.positionDamper = Damper;
	    layer.maximumForce = MaxForce;
	    layer.mode = JointDriveMode.Position;

        JointDrive insidelayer = new JointDrive();
        insidelayer.positionSpring = InsideOverallSpring;
        insidelayer.positionDamper = InsideDamper;
        insidelayer.maximumForce = InsideMaxForce;
        insidelayer.mode = JointDriveMode.Position;

        JointLimits layerA = new JointLimits();
	    layerA.max = Max;
	    layerA.maxBounce = MaxBounce;
	    layerA.min = Min;
	    layerA.minBounce = MinBounce;

	    #endregion

        // Framework, includes size - spawn freq - initial springs

	    #region Initial Framework
        float ang;
	    for (int x = 0; x < NumSprings; x++)
	    {
	        if (x >= 0) //Evenly distribute 
	            ang = 360/(NumSprings)*x;
	        else
	            ang = 0.001f; //Starting point.

	        pos.x = thisCenter.x + Radius*Mathf.Sin(ang*Mathf.Deg2Rad);
	        pos.y = thisCenter.y + Radius*Mathf.Cos(ang*Mathf.Deg2Rad);
	        pos.z = thisCenter.z;

	        posx2.x = thisCenter.x + ((Radius*SkinThickness)*Mathf.Sin(ang*Mathf.Deg2Rad));
	        posx2.y = thisCenter.y + ((Radius*SkinThickness)*Mathf.Cos(ang*Mathf.Deg2Rad));
	        posx2.z = thisCenter.z;

	        GameObject springInstance = Instantiate(Spring, pos, transform.rotation) as GameObject;
	        lList.Add(springInstance);
	        GameObject springInstanceLayer2 = Instantiate(Spring, posx2, transform.rotation) as GameObject;
	        lList2.Add(springInstanceLayer2);
	        springInstance.transform.parent = transform;
	        springInstanceLayer2.transform.parent = transform;

	        springInstance.tag = "Layer1";
	        springInstanceLayer2.tag = "Layer2";

	        if (springInstance != null) springInstance.AddComponent<ConfigurableJoint>();
	        if (springInstanceLayer2 != null) springInstanceLayer2.AddComponent<ConfigurableJoint>();


	        var rigidDistro = springInstance.GetComponent<Rigidbody>();
	        rigidDistro.useGravity = UseGravity;

	        rigidDistro = springInstanceLayer2.GetComponent<Rigidbody>();
	        rigidDistro.useGravity = UseGravity;

	        //Outside Layer Settings
	        var springDistro = springInstance.GetComponent<ConfigurableJoint>();
	        springDistro.connectedBody = springInstanceLayer2.rigidbody;
	        springDistro.xDrive = layer;
	        springDistro.yDrive = layer;

	        //Inside Layer Settings
	        springDistro = springInstanceLayer2.GetComponent<ConfigurableJoint>();
	        springDistro.connectedBody = springCenter.rigidbody;
	        springDistro.xDrive = insidelayer;
	        springDistro.yDrive = insidelayer;


	    }

	    #endregion

        // Again to crosshatch those spokes.     

	    #region Crosshatch Ball

	    var sz = 1; //These are to always add the link to 1 step ahead of the component
	    var zs = 1;
	    for (int i = 0; i < lList.Count; i++)
	    {
	        if (sz >= NumSprings)
	            sz = 0;
	        var joint = lList[i].AddComponent<ConfigurableJoint>();
	        joint.connectedBody = lList2[sz].rigidbody;
	        joint.xDrive = layer;
	        joint.yDrive = layer;
	        sz++;
	    }

	    for (int a = 0; a < lList2.Count; a++)
	    {
	        if (zs >= NumSprings)
	            zs = 0;
	        var joint = lList2[a].AddComponent<ConfigurableJoint>();
	        joint.connectedBody = lList[zs].rigidbody;
	        joint.xDrive = insidelayer;
	        joint.yDrive = insidelayer;
	        zs++;
	    }

	    #endregion

        // AND THEN AGAIN. To strengthen the layer..  

	    #region Layer the ball

	    var xa = 1; //These are to always add the link to 1 step ahead of the component
	    var ax = 1;
	    for (int i = 0; i < lList.Count; i++)
	    {
	        if (xa >= NumSprings)
	            xa = 0;
	        var joint = lList[i].AddComponent<ConfigurableJoint>();
	        joint.connectedBody = lList[xa].rigidbody;
	        joint.xDrive = layer;
	        joint.yDrive = layer;
	        xa++;
	    }

	    for (int o = 0; o < lList2.Count; o++)
	    {
	        if (ax >= NumSprings)
	            ax = 0;
	        var joint = lList2[o].AddComponent<ConfigurableJoint>();
	        joint.connectedBody = lList2[ax].rigidbody;
	        joint.xDrive = insidelayer;
	        joint.yDrive = insidelayer;
	        ax++;
	    }

	    #endregion

        // Add colliders to the ball.

	    #region Add Colliders

	    foreach (SphereCollider sphereCollider in lList.Select(t => t.AddComponent<SphereCollider>()))
	    {
	        sphereCollider.radius = 0.5f;
	    }

	    #endregion

        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
	}


    // Update is called once per frame
	void Update () {
        
        Vector2[] vertices2D = new Vector2[lList.Count];
        Vector3[] vertices = new Vector3[lList.Count];
        Vector2[] uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices2D[i] = new Vector2(lList[i].transform.position.x, lList[i].transform.position.y);
            vertices[i] = lList[i].transform.position;
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            vertices[i] -= transform.position;
        }

	    Triangulator tr = new Triangulator(vertices2D);

        int[] indices = tr.Triangulate();

	    Mesh msh = new Mesh();
        msh.vertices = vertices;
	    msh.triangles = indices;
        msh.uv = uvs;

        msh.RecalculateNormals();
        msh.RecalculateBounds();

	    MeshFilter filter = GetComponent<MeshFilter>();

	    filter.mesh = msh;

	    target = transform;

        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Debug.DrawLine(Camera.mainCamera.transform.position, transform.position + mesh.vertices[0]);


    }

    void OnDrawGizmos()
    {
        // Gizmos.DrawSphere();
        Gizmos.DrawWireSphere(transform.position, Radius);
        _cachedpos = GameObject.FindGameObjectsWithTag("springs");

        foreach (GameObject go in _cachedpos)
        {     
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(go.transform.position, size: new Vector3(0.5f, 0.5f, 0.5f));
        }

        var xa = 1; //These are to always add the link to 1 step ahead of the component
        var ax = 1;
        for (int i = 0; i < lList2.Count; i++) {
            if (xa >= NumSprings)
                xa = 0;
            Gizmos.DrawLine(lList[i].transform.position, lList2[ax].transform.position);
            Gizmos.DrawLine(lList2[i].transform.position, lList2[xa].transform.position);
            Gizmos.DrawLine(lList2[i].transform.position, transform.position);
            xa++;
        }
        for (int i = 0; i < lList.Count; i++) {
            if (ax >= NumSprings)
                ax = 0;
            Gizmos.DrawLine(lList2[i].transform.position, lList[ax].transform.position);
            Gizmos.DrawLine(lList[i].transform.position, lList[ax].transform.position);
            Gizmos.DrawLine(lList2[i].transform.position, lList[i].transform.position);
           ax++;
        }
    }
}


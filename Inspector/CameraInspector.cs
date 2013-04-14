using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AdvCameraRotation))]
public class CameraInspector : Editor
{

    private AdvCameraRotation _adv;
    private GUIStyle bold = new GUIStyle();

    void Awake()
    {
        _adv = (AdvCameraRotation)target;
        init();
    }

    void OnEnable()
    {

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    void init()
    {
        _adv.lookSpeed = 1f;
        _adv.moveSpeed = 1f;
    }

    private bool foldout;
    private bool Rotationfoldout = true;

    public override void OnInspectorGUI()
    {
        //------------ Styles --------------//
        
        bold = new GUIStyle(GUI.skin.label);
        bold.fontStyle = FontStyle.Bold;

        //----------------------------------//
        GUILayout.Label("Crucial Properties:");
        EditorGUILayout.Separator();
        //------ Top aligned controls ------//
        #region Control Panel

        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        _adv.FreeView = GUILayout.Toggle(_adv.FreeView, "Free Form");
        GUILayout.FlexibleSpace();
        _adv.FreeView = GUILayout.Toggle(_adv.FreeView, "Free Form View");
        GUILayout.FlexibleSpace();
        _adv.FreeView = GUILayout.Toggle(_adv.FreeView, "Free Form View");
        GUILayout.Space(40);
        GUILayout.EndHorizontal();

        #endregion


        if (_adv.FreeView)
        {
            _adv.lookSpeed = EditorGUILayout.FloatField("Mouse Acceleration", _adv.lookSpeed);
            _adv.moveSpeed = EditorGUILayout.FloatField("Keyboard Acceleration",
                                                                     _adv.moveSpeed);
        }
        EditorGUILayout.Separator();

        GUILayout.Box("", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(1) });
        _adv.camera = EditorGUILayout.ObjectField("Camera", _adv.camera, typeof (Camera), false) as Camera;

        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        GUILayout.Label("Action Name", bold);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Primary", bold, GUILayout.Width(105));
        GUILayout.Label("Secondary", bold, GUILayout.Width(110));
        GUILayout.Space(40);
        GUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        Rotationfoldout = EditorGUILayout.Foldout(Rotationfoldout, "Rotation Controls");
        EditorGUILayout.Separator();
        
        if (Rotationfoldout)
        {
            _adv.RotAmount = EditorGUILayout.IntField("Rotation Amount", _adv.RotAmount);
            _adv.AllocatedTime = EditorGUILayout.FloatField("Rotation Speed",
                                                                         _adv.AllocatedTime);
            _adv.RotateTime = EditorGUILayout.CurveField("Rotation Curve", _adv.RotateTime);
        }

        EditorGUILayout.Separator();
        foldout = EditorGUILayout.Foldout(foldout, "Camera Controls");
        EditorGUILayout.Separator();
        
        if(foldout)
        {
            GUILayout.BeginHorizontal();
            _adv.near = EditorGUILayout.FloatField("Near", _adv.near, GUILayout.MinWidth(80), GUILayout.MaxWidth(200));
            _adv.fov = EditorGUILayout.FloatField("Fov", _adv.fov, GUILayout.MinWidth(80), GUILayout.MaxWidth(200));
            _adv.far = EditorGUILayout.FloatField("Far", _adv.far, GUILayout.MinWidth(80), GUILayout.MaxWidth(200));
           

            GUILayout.FlexibleSpace();


            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _adv.orthographicSize = EditorGUILayout.FloatField("Orthographic Size", _adv.orthographicSize);
            GUILayout.EndHorizontal();
        }
        
        if(GUI.changed)
           EditorUtility.SetDirty(_adv);

    }

}

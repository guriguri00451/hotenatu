/*
    fSpyLoader for Unity 2020.3+
    (C)2022 nishiura.hiroshi

    Required Unity package:
    * com.unity.nuget.newtonsoft-json
*/
using UnityEditor;
using UnityEngine;
using System.IO; // File
using Newtonsoft.Json; // JsonConvert
public class fSpyPoint
{
    public float x;
    public float y;
}
public class fSpyMatrix
{
    public float[,] rows;
}
public class fSpyData
{
    public fSpyPoint principalPoint;
    public fSpyMatrix viewTransform;
    public fSpyMatrix cameraTransform;
    public float horizontalFieldOfView;
    public float verticalFieldOfView;
    public fSpyPoint[] vanishingPoints;
    public string[] vanishingPointAxes;
    public float relativeFocalLength;
    public int imageWidth;
    public int imageHeight;
}

public class fSpyLoader : EditorWindow
{
    Camera _camera;
    string[] _toggleMenu = { "Y-up", "Z-up", "x", "y", "z" };
    string[] _rotateMenu = { "0", "90", "180" };
    int _toggleAxis = 0;
    int _rotateX = 0;
    string _log;

    [MenuItem("Window/Custom/fSpy Loader...")]
    static void Init()
    {
        fSpyLoader loader = (fSpyLoader)EditorWindow.GetWindow(typeof(fSpyLoader));
        if (Selection.gameObjects.Length > 0)
        {
            loader._camera = Selection.gameObjects[0].GetComponent<Camera>();
        }
        loader.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        _camera = (Camera)EditorGUILayout.ObjectField("Camera", _camera, typeof(Camera), true);
        if (_camera)
        {
            GUILayout.Label("Toggle Axis");
            _toggleAxis = GUILayout.Toolbar(_toggleAxis, _toggleMenu);
            if (_toggleAxis > 1)
            {
                GUILayout.Label("Rotate X-Axis");
                _rotateX = GUILayout.Toolbar(_rotateX, _rotateMenu);
            }
            GUILayout.Space(5);
            if (GUILayout.Button("Load fSpy data from the json file")) LoadfSpyFile();
            GUILayout.Space(20);
            GUILayout.Label(_log);
        }
        EditorGUILayout.EndVertical();
    }
    void LoadfSpyFile()
    {
        _log = "";

        string path = EditorUtility.OpenFilePanel("Open fSpy json", "", "json");
        if (string.IsNullOrEmpty(path)) return;
        _log += "File:" + path + "\n";

        string text = File.ReadAllText(path);
        fSpyData fspy = JsonConvert.DeserializeObject<fSpyData>(text);

        _log += "vanishingPointAxes: ";
        foreach (string ax in fspy.vanishingPointAxes) _log += "[" + ax + "]";
        _log += "\n";

        float[,] m = fspy.cameraTransform.rows;
        Matrix4x4 m4 = new Matrix4x4();
        for (int i = 0; i < 4; i++) m4.SetRow(i, new Vector4(m[i, 0], m[i, 1], m[i, 2], m[i, 3]));

        Vector3 position = new Vector3(m4[0, 3], m4[1, 3], m4[2, 3]);
        Quaternion rotation = Quaternion.Inverse(m4.rotation);

        if (_toggleAxis > 0)
        {
            switch (_toggleAxis)
            {
                case 1:
                    (position.y,position.z) = (position.z,position.y);
                    (rotation.y,rotation.z) = (rotation.z,rotation.y);
                    rotation *= Quaternion.AngleAxis(90, Vector3.right);
                    break;
                case 2:
                    position.x *= -1;
                    rotation.x *= -1;
                    break;
                case 3:
                    position.y *= -1;
                    rotation.y *= -1;
                    break;
                case 4:
                    position.z *= -1;
                    rotation.z *= -1;
                    break;
            }
            switch (_rotateX)
            {
                case 1:
                    rotation *= Quaternion.AngleAxis(90, Vector3.right);
                    break;
                case 2:
                    rotation *= Quaternion.AngleAxis(180, Vector3.right);
                    break;
            }
        }
        else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "zPositive")
        {
            position.y *= -1;
            rotation.y *= -1;
            rotation *= Quaternion.AngleAxis(180, Vector3.right);
        }
        else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "zNegative")
        {
            position.z *= -1;
            rotation.z *= -1;
        }
        else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "yPositive")
        {
            position.y *= -1;
            rotation.y *= -1;
            rotation *= Quaternion.AngleAxis(180, Vector3.right);
        }
        else if (fspy.vanishingPointAxes[0] == "xNegative" && fspy.vanishingPointAxes[1] == "yNegative")
        {
            position.z *= -1;
            rotation.z *= -1;
        }
        else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "zPositive")
        {
            position.y *= -1;
            rotation.y *= -1;
            rotation *= Quaternion.AngleAxis(180, Vector3.right);
        }
        else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "zNegative")
        {
            position.z *= -1;
            rotation.z *= -1;
        }
        else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "yPositive")
        {
            position.y *= -1;
            rotation.y *= -1;
            rotation *= Quaternion.AngleAxis(180, Vector3.right);
        }
        else if (fspy.vanishingPointAxes[0] == "xPositive" && fspy.vanishingPointAxes[1] == "yNegative")
        {
            position.z *= -1;
            rotation.z *= -1;
        }
        else
        {
            _log += "\nError:\n";
            _log += "Unsupported Vanishing Point Axes.\n";
            return;
        }
        _camera.transform.SetPositionAndRotation(position, rotation);
        _camera.fieldOfView = fspy.verticalFieldOfView * Mathf.Rad2Deg;

        _log += "fieldOfView:" + _camera.fieldOfView + "\n";
        _log += "Quaternion:" + m4.rotation.ToString("F3") + "\n";
        _log += "Position:" + position.ToString("F3") + "\n";
    }
}

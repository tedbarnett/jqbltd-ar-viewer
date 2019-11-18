using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(AudiencePath))]
public class AudiencePathEditor : Editor
{
    private AudiencePath audiencePathTarget;

    private SerializedProperty _looking;
    private SerializedProperty _target;
    private SerializedProperty _damping;

    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new List<T>();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

    public void OnEnable()
    {
        audiencePathTarget = target as AudiencePath;

        _looking = serializedObject.FindProperty("looking");
        _target = serializedObject.FindProperty("target");
        _damping = serializedObject.FindProperty("damping");
    }

    public List<DirectoryInfo> FindAllDirs(string path)
    {
        List<DirectoryInfo> ret = new List<DirectoryInfo>();
        DirectoryInfo f = new DirectoryInfo(path);
        if (f.GetDirectories().Length > 0)
        {
            foreach (var item in f.GetDirectories())
            {
                ret.Add(item);
                if (item.GetDirectories().Length > 0)
                {
                    ret.AddRange(FindAllDirs(item.FullName));
                }
            }
        }
        return ret;

    }

    public void OnSceneGUI()
    {
        if (audiencePathTarget.newPointCreation || audiencePathTarget.oldPointDeleting)
        {
            if (Event.current.type == EventType.MouseMove) SceneView.RepaintAll();
            RaycastHit hit;
            Vector2 mPos = Event.current.mousePosition;
            mPos.y = Screen.height - mPos.y - 40;
            Ray ray = Camera.current.ScreenPointToRay(mPos);

            if (Physics.Raycast(ray, out hit, 3000))
            {
                audiencePathTarget.mousePosition = hit.point;

                if ((Event.current.type == EventType.MouseDown && Event.current.button == 0))
                {
                    // создаём новую точку
                    if (audiencePathTarget.newPointCreation)
                    {
                        audiencePathTarget.AddPoint();
                    }
                    // удаляем старую точку
                    if (audiencePathTarget.oldPointDeleting)
                    {
                        audiencePathTarget.DeletePoint();
                    }
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        AudiencePath walkPath = target as AudiencePath;

        EditorGUILayout.Space();

        GUIStyle boxStyle = new GUIStyle("Box");

        EditorGUILayout.LabelField("Audience Path", boxStyle, GUILayout.ExpandWidth(true));

        EditorGUILayout.Space();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(_looking, new GUIContent("looking"));

        if (_looking.boolValue)
        {
            EditorGUILayout.PropertyField(_target, new GUIContent("target"));
            EditorGUILayout.PropertyField(_damping, new GUIContent("damping"));
        }

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Populate!"))
        {
            if (walkPath.par != null)
            {
                DestroyImmediate(walkPath.par);
            }

            if (walkPath.peoplePrefabs != null && walkPath.peoplePrefabs.Length > 0 && walkPath.peoplePrefabs[0] != null)
            {
                walkPath.SpawnPeople();
            }
        }

        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Remove prefabs"))
        {
            if (walkPath.par != null)
            {
                DestroyImmediate(walkPath.par);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        if (walkPath.peoplePrefabs == null || walkPath.peoplePrefabs.Length == 0 || walkPath.peoplePrefabs[0] == null)
            EditorGUILayout.HelpBox("To create a path must be at least 1 walking object prefab", MessageType.Warning);


        if ((audiencePathTarget.oldPointDeleting ||
            audiencePathTarget.newPointCreation) &&
            GUILayout.Button("Edit Points Finish"))
        {

            audiencePathTarget.newPointCreation = false;
            audiencePathTarget.oldPointDeleting = false;
            audiencePathTarget.EditorLock(false);
        }

        if (!audiencePathTarget.newPointCreation &&
            !audiencePathTarget.oldPointDeleting)
        {

            if (GUILayout.Button("Add Points"))
            {
                audiencePathTarget.newPointCreation = true;
                audiencePathTarget.EditorLock(true);
            }

            if (GUILayout.Button("Delete Points"))
            {
                audiencePathTarget.oldPointDeleting = true;
                audiencePathTarget.EditorLock(true);
            }
            if (!walkPath.disableLineDraw)
            {
                if (GUILayout.Button("HIDE GRAPHICS"))
                {
                    walkPath.disableLineDraw = true;
                    walkPath.HideExistingIcons();
                    return;
                }
            }
            if (walkPath.disableLineDraw)
            {
                if (GUILayout.Button("SHOW GRAPHICS"))
                {
                    walkPath.disableLineDraw = false;
                    walkPath.ShowExistingIcons();
                }
            }
        }

        if (GUILayout.Button("Re-Build Points"))
        {
            Transform parentOfPoints = walkPath.transform.Find("points");

            Transform[] pointsTransform = parentOfPoints.GetComponentsInChildren<Transform>();

            walkPath.pathPoint.Clear();
            walkPath.pathPointTransform.Clear();

            for (int i = 1; i < pointsTransform.Length; i++)
            {
                walkPath.pathPoint.Add(pointsTransform[i].position);
                walkPath.pathPointTransform.Add(pointsTransform[i].gameObject);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}

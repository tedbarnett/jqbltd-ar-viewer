using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NewPath))]
public class NewPathEditor : Editor
{
	public void OnSceneGUI()
	{
		NewPath _NewPath = target as NewPath;

		if(!_NewPath.exit)
		{
			if(Event.current.type == EventType.MouseMove) SceneView.RepaintAll();
 			RaycastHit hit;
			Vector2 mPos = Event.current.mousePosition;
			mPos.y = Screen.height - mPos.y - 40;
			Ray ray = Camera.current.ScreenPointToRay(mPos);

    		if (Physics.Raycast(ray, out hit, 3000)) 
    		{
    			_NewPath.mousePos = hit.point;

    			if((Event.current.type == EventType.MouseDown && Event.current.button == 0))
    			{
    					_NewPath.PointSet(_NewPath.pointLenght, hit.point);
    					_NewPath.pointLenght++;
    			}
        	}
        }
    	if(Event.current.keyCode == (KeyCode.Escape))
    		_NewPath.exit = true;
	}

    public override void OnInspectorGUI()
    {
		NewPath _NewPath = target as NewPath;
    	EditorGUILayout.Space();
    	if(!_NewPath.exit)
    	{
    		EditorGUILayout.HelpBox("Click left mouse button to create next point, esc to finish draw.", MessageType.Info);
    		EditorGUILayout.Space();
    	}

    	_NewPath.pathName = EditorGUILayout.TextField("Path name: ", _NewPath.pathName);
    	EditorGUILayout.Space();

    	if(GUILayout.Button("Finish"))
    	{
    		if(!_NewPath.errors)
    		{
                DestroyImmediate(_NewPath.par);

    			_NewPath.gameObject.name = _NewPath.pathName;

                WalkPath wp = null;

                if (_NewPath.PathType == PathType.PeoplePath)
                {
                    wp = _NewPath.gameObject.AddComponent<PeopleWalkPath>() as WalkPath;
                }
                else
                {
                    wp = _NewPath.gameObject.AddComponent<AudiencePath>() as WalkPath;
                }

                wp.pathPoint = _NewPath.PointsGet();
                wp.pathType = _NewPath.PathType;

                GameObject _myPoints = new GameObject();
                _myPoints.transform.parent = _NewPath.gameObject.transform;
                _myPoints.name = "points";

                for (int i = 0; i < wp.pathPoint.Count; i++)
                {
                    var _point = Instantiate(GameObject.Find("Population System").GetComponent<PopulationSystemManager>().pointPrefab, wp.pathPoint[i], Quaternion.identity) as GameObject;
                    _point.name = "p" + i;
                    _point.transform.parent = _myPoints.transform;
                    wp.pathPointTransform.Add(_point);
                }

                ActiveEditorTracker.sharedTracker.isLocked = false;
                DestroyImmediate(_NewPath.gameObject.GetComponent<NewPath>());
    		}
    	}
    	EditorGUILayout.Space();

    	if(_NewPath.pointLenght < 2)
    	{
    		_NewPath.errors = true;
    		EditorGUILayout.HelpBox("To create a path must be at least 2 points", MessageType.Warning);
    	}

    	if(string.IsNullOrEmpty(_NewPath.pathName))
    	{
    		_NewPath.errors = true;
    		EditorGUILayout.HelpBox("Enter the path name", MessageType.Warning);
    	}

    	if(_NewPath.pointLenght >= 2 && !string.IsNullOrEmpty(_NewPath.pathName))
    		_NewPath.errors = false;

    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PopulationSystemManager))]
public class PopulationSystemManagerEditor : Editor {
	
	
	public override void OnInspectorGUI()
	{
		PopulationSystemManager _PSM = target as PopulationSystemManager;
		if(_PSM.isConcert || _PSM.isStreet)
		{
		    EditorGUILayout.HelpBox("Click left mouse button in point", MessageType.Info);
		}
		else 
		DrawDefaultInspector();
	}


	public void OnSceneGUI()
	{
		PopulationSystemManager _PSM = target as PopulationSystemManager;

		if(Event.current.type == EventType.MouseMove) SceneView.RepaintAll();

		if(_PSM.isConcert || _PSM.isStreet)
		{
		    EditorGUILayout.HelpBox("Click left mouse button in point", MessageType.Info);
 			RaycastHit hit;
			Vector2 mPos = Event.current.mousePosition;
			mPos.y = Screen.height - mPos.y - 40;
			Ray ray = Camera.current.ScreenPointToRay(mPos);

    		if (Physics.Raycast(ray, out hit, 3000)) 
    		{
    			_PSM.mousePos = hit.point;

    			if((Event.current.type == EventType.MouseDown && Event.current.button == 0))
    			{
    				if(_PSM.isConcert)
    				{
    					_PSM.Concert(_PSM.mousePos);
    					_PSM.isConcert = false;
    				}

    				else if(_PSM.isStreet)
    				{
    					_PSM.Street(_PSM.mousePos);
    					_PSM.isStreet = false;
    				}
    			}
        	}
    	}
	}
	

}

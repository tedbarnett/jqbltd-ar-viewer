using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(StandingPeopleStreet))]
public class StandingPeopleStreetEditor : Editor
{

    public override void OnInspectorGUI()
    {
        StandingPeopleStreet _SPC = target as StandingPeopleStreet;
        SceneView.RepaintAll();

      if(_SPC.peopleCount < 1) _SPC.peopleCount = 1;

        if(_SPC.showSurface)
            _SPC.surface.GetComponent<MeshRenderer>().enabled = true;
        else
            _SPC.surface.GetComponent<MeshRenderer>().enabled = false;

        DrawDefaultInspector();

        if(_SPC.SurfaceType.ToString() == "Rectangle"){

            if(_SPC.isCircle)
                _SPC.SpawnRectangleSurface();

            _SPC.planeSize = EditorGUILayout.Vector2Field(new GUIContent("Rectangle size:", "Rectangle size / Размер квадрата"), _SPC.planeSize);

            _SPC.isCircle = false;
        	
    	}

    	else if(_SPC.SurfaceType.ToString() == "Circle"){

            if(!_SPC.isCircle)
                _SPC.SpawnCircleSurface();

            _SPC.circleDiametr = EditorGUILayout.FloatField(new GUIContent("Circle diameter:", "Circle diameter / Диаметр круга"), _SPC.circleDiametr);

            _SPC.isCircle = true;
    	}

        _SPC.showSurface = EditorGUILayout.Toggle(new GUIContent("Show surface:", "Show surface / Показать поверхность"), _SPC.showSurface);
        EditorGUILayout.Space();

        _SPC.peopleCount = EditorGUILayout.IntField(new GUIContent("People count:", "People count / Количество людей"), _SPC.peopleCount);
        EditorGUILayout.Space();

        _SPC.highToSpawn = EditorGUILayout.FloatField(new GUIContent("High to spawn:", "High to spawn of people / Высота проверки спауна людей"), _SPC.highToSpawn);
        EditorGUILayout.Space();

        GUI.backgroundColor = Color.green;

        if (GUILayout.Button("Populate!"))
        {
            _SPC.PopulateButton();
        }

        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Remove people"))
        {
            _SPC.RemoveButton();
        }   

    }
}
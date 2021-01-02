    using UnityEngine;
    using UnityEditor;
    using System.Collections;
     
    // CopyComponents - by Michael L. Croswell for Colorado Game Coders, LLC
    // March 2010
     
    //Modified by Kristian Helle Jespersen
    //June 2011
     
    public class ReplaceGameObjects : ScriptableWizard
    {
        public bool copyValues = true;
        public GameObject NewType;
        public GameObject[] OldObjects;
     
        [MenuItem("Custom/Replace GameObjects")]
        static void CreateWizard()
        {
            var replaceGameObjects = ScriptableWizard.DisplayWizard <ReplaceGameObjects>("Replace GameObjects", "Replace");
            replaceGameObjects.OldObjects = Selection.gameObjects;
        }
     
        void OnWizardCreate()
        {
            //Transform[] Replaces;
            //Replaces = Replace.GetComponentsInChildren<Transform>();
     
            foreach (GameObject go in OldObjects)
            {
                GameObject newObject;
                newObject = (GameObject)EditorUtility.InstantiatePrefab(NewType);
                newObject.transform.parent = go.transform.parent;
                newObject.transform.localPosition = go.transform.localPosition;
                newObject.transform.localRotation = go.transform.localRotation;
                newObject.transform.localScale = go.transform.localScale;
     
                DestroyImmediate(go);
            }
        }
     
    }

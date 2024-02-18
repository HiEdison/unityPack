using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace WoogiWorld.Custom.Utility
{
    public class PathPoint : MonoBehaviour
    {
        [Serializable]
        public class PathPointList
        {
            public PathPoint path;
            public Transform[] items = new Transform[0];
            ~PathPointList()
            {
                path = null;
                items = null;
            }
        }
        public PathPointList pathPointList = new PathPointList();

#if UNITY_EDITOR
        public TagIconManager.LabelIcon tagIcon = TagIconManager.LabelIcon.Blue;
#endif
        void OnDestroy()
        {
            pathPointList = null;
        }

        public Transform[] PathPoints
        {
            get { return pathPointList.items; }
        }

        public Vector3[] GetPoints()
        {
            Transform[] items = PathPoints;
            Vector3[] pathPoints = new Vector3[items.Length];
            for (int i = 0; i < items.Length; ++i)
            {
                if (items[i] == null)
                {
                    continue;
                }
                pathPoints[i] = items[i].transform.position;
            }
            return pathPoints;
        }

        private void OnDrawGizmos()
        {
            DrawGizmos(false);
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(true);
        }

        private void DrawGizmos(bool selected)
        {
            pathPointList.path = this;
            if (PathPoints.Length > 1)
            {
                DrawPathHelper(GetPoints(), selected ? Color.green : Color.red, "gizmos");
            }
        }

        void DrawPathHelper(Vector3[] path, Color color, string method)
        {
            Vector3[] vector3s = PathControlPointGenerator(path);

            //Line Draw:
            Vector3 prevPt = Interp(vector3s, 0);
            Gizmos.color = color;
            int SmoothAmount = path.Length * 20;
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float pm = (float)i / SmoothAmount;
                Vector3 currPt = Interp(vector3s, pm);
                if (method == "gizmos")
                {
                    Gizmos.DrawLine(currPt, prevPt);
                }
                else if (method == "handles")
                {
                    WDebug.LogError("iTween Error: Drawing a path with Handles is temporarily disabled because of compatability issues with Unity 2.6!");
                    //UnityEditor.Handles.DrawLine(currPt, prevPt);
                }
                prevPt = currPt;
            }
        }

        Vector3[] PathControlPointGenerator(Vector3[] path)
        {
            Vector3[] suppliedPath;
            Vector3[] vector3s;

            //create and store path points:
            suppliedPath = path;

            //populate calculate path;
            int offset = 2;
            vector3s = new Vector3[suppliedPath.Length + offset];
            WDebug.Log($"Array.Copy->{107}");
            Array.Copy(suppliedPath, 0, vector3s, 1, suppliedPath.Length);
            WDebug.Log($"Array.Copy->{109}");
            //populate start and end control points:
            //vector3s[0] = vector3s[1] - vector3s[2];
            vector3s[0] = vector3s[1] + (vector3s[1] - vector3s[2]);
            vector3s[vector3s.Length - 1] = vector3s[vector3s.Length - 2] + (vector3s[vector3s.Length - 2] - vector3s[vector3s.Length - 3]);

            //is this a closed, continuous loop? yes? well then so let's make a continuous Catmull-Rom spline!
            if (vector3s[1] == vector3s[vector3s.Length - 2])
            {
                Vector3[] tmpLoopSpline = new Vector3[vector3s.Length];
                WDebug.Log($"Array.Copy->{119}");
                Array.Copy(vector3s, tmpLoopSpline, vector3s.Length);
                WDebug.Log($"Array.Copy->{121}");
                tmpLoopSpline[0] = tmpLoopSpline[tmpLoopSpline.Length - 3];
                tmpLoopSpline[tmpLoopSpline.Length - 1] = tmpLoopSpline[2];
                vector3s = new Vector3[tmpLoopSpline.Length];
                WDebug.Log($"Array.Copy->{125}");
                Array.Copy(tmpLoopSpline, vector3s, tmpLoopSpline.Length);
                WDebug.Log($"Array.Copy->{127}");
            }

            return (vector3s);
        }

        Vector3 Interp(Vector3[] pts, float t)
        {
            int numSections = pts.Length - 3;
            int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
            float u = t * (float)numSections - (float)currPt;

            Vector3 a = pts[currPt];
            Vector3 b = pts[currPt + 1];
            Vector3 c = pts[currPt + 2];
            Vector3 d = pts[currPt + 3];

            return .5f * (
                (-a + 3f * b - 3f * c + d) * (u * u * u)
                + (2f * a - 5f * b + 4f * c - d) * (u * u)
                + (-a + c) * u
                + 2f * b
            );
        }
    }
}

namespace WoogiWorld.Custom.Utility
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PathPoint.PathPointList))]
    public class PathpointListDrawer : PropertyDrawer
    {
        private float lineHeight = 18;
        private float spacing = 4;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            float x = position.x;
            float y = position.y;
            float inspectorWidth = position.width;

            // Draw label


            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var items = property.FindPropertyRelative("items");
            var titles = new string[] { "Transform", "", "", "" };
            var props = new string[] { "transform", "^", "v", "-" };
            var widths = new float[] { .7f, .1f, .1f, .1f };
            float lineHeight = 18;
            bool changedLength = false;
            if (items.arraySize > 0)
            {
                for (int i = -1; i < items.arraySize; ++i)
                {
                    var item = items.GetArrayElementAtIndex(i);

                    float rowX = x;
                    for (int n = 0; n < props.Length; ++n)
                    {
                        float w = widths[n] * inspectorWidth;

                        // Calculate rects
                        Rect rect = new Rect(rowX, y, w, lineHeight);
                        rowX += w;

                        if (i == -1)
                        {
                            EditorGUI.LabelField(rect, titles[n]);
                        }
                        else
                        {
                            if (n == 0)
                            {
                                EditorGUI.ObjectField(rect, item.objectReferenceValue, typeof(Transform), true);
                            }
                            else
                            {
                                if (GUI.Button(rect, props[n]))
                                {
                                    switch (props[n])
                                    {
                                        case "-":
                                            items.DeleteArrayElementAtIndex(i);
                                            items.DeleteArrayElementAtIndex(i);
                                            changedLength = true;
                                            break;
                                        case "v":
                                            if (i > 0)
                                            {
                                                items.MoveArrayElement(i, i + 1);
                                            }
                                            break;
                                        case "^":
                                            if (i < items.arraySize - 1)
                                            {
                                                items.MoveArrayElement(i, i - 1);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    y += lineHeight + spacing;
                    if (changedLength)
                    {
                        break;
                    }
                }
            }
            else
            {
                // add button
                var addButtonRect = new Rect((x + position.width) - widths[widths.Length - 1] * inspectorWidth, y,
                                             widths[widths.Length - 1] * inspectorWidth, lineHeight);
                if (GUI.Button(addButtonRect, "+"))
                {
                    items.InsertArrayElementAtIndex(items.arraySize);
                }

                y += lineHeight + spacing;
            }

            // add all button
            var addAllButtonRect = new Rect(x, y, inspectorWidth, lineHeight);
            if (GUI.Button(addAllButtonRect, "Assign using all child objects"))
            {
                var path = property.FindPropertyRelative("path").objectReferenceValue as PathPoint;
                var children = new Transform[path.transform.childCount];
                int n = 0;
                foreach (Transform child in path.transform)
                {
                    children[n++] = child;
                }
                Array.Sort(children, new TransformNameComparer());
                path.pathPointList.items = new Transform[children.Length];
                for (n = 0; n < children.Length; ++n)
                {
                    path.pathPointList.items[n] = children[n];
                }
            }
            y += lineHeight + spacing;

            // rename all button
            var renameButtonRect = new Rect(x, y, inspectorWidth, lineHeight);
            if (GUI.Button(renameButtonRect, "Auto Rename numerically from this order"))
            {
                var path = property.FindPropertyRelative("path").objectReferenceValue as PathPoint;
                int n = 0;
                foreach (Transform child in path.pathPointList.items)
                {
                    child.name = "P." + (n++).ToString();
                    TagIconManager.SetIcon(child.gameObject, path.tagIcon);
                }
            }
            y += lineHeight + spacing;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty items = property.FindPropertyRelative("items");
            float lineAndSpace = lineHeight + spacing;
            return 40 + (items.arraySize * lineAndSpace) + lineAndSpace;
        }


        // comparer for check distances in ray cast hits
        public class TransformNameComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                return ((Transform)x).GetSiblingIndex().CompareTo(((Transform)y).GetSiblingIndex());
            }
        }
    }
#endif
}

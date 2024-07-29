using Revo.Methods;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SpiderAI))]
public class SpiderAIEditor : Editor
{
    private Dictionary<string, object[]> methodParameters = new Dictionary<string, object[]>();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpiderAI spiderAI = (SpiderAI)target;

        // Get all the public methods of SpiderAI
        MethodInfo[] methods = typeof(SpiderAI).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        foreach (MethodInfo method in methods)
        {
            // Display the method name
            EditorGUILayout.LabelField(method.Name);

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                // Prepare parameters storage if not present
                if (!methodParameters.ContainsKey(method.Name))
                {
                    methodParameters[method.Name] = new object[parameters.Length];
                }

                // Display input fields for each parameter
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType == typeof(float))
                    {
                        if (methodParameters[method.Name][i] == null)
                        {
                            methodParameters[method.Name][i] = 0f;
                        }

                        var rangeAttribute = method.GetCustomAttribute<FloatRangeAttribute>();
                        if (rangeAttribute != null)
                        {
                            methodParameters[method.Name][i] = EditorGUILayout.Slider(parameters[i].Name, (float)methodParameters[method.Name][i], rangeAttribute.Min, rangeAttribute.Max);
                        }
                        else
                        {
                            methodParameters[method.Name][i] = EditorGUILayout.FloatField(parameters[i].Name, (float)methodParameters[method.Name][i]);
                        }
                    }
                    // Add more conditions here for other parameter types
                }


            }

            if (GUILayout.Button("Invoke " + method.Name))
            {
                method.Invoke(spiderAI, methodParameters.ContainsKey(method.Name) ? methodParameters[method.Name] : null);
            }
        }
    }
}

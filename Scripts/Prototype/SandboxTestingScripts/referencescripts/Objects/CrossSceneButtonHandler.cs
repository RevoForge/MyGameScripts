using UnityEngine;

public class CrossSceneButtonHandler : MonoBehaviour
{
    public string objectName;
    private GameObject targetObject;
    public string targetScript;
    public string targetFunction;

    private void OnEnable()
    {
        targetObject = GameObject.Find(objectName);
    }

    public void ButtonClicked()
    {
        System.Type targetType = System.Type.GetType(targetScript);
        Component targetComponent = targetObject.GetComponent(targetType);
        System.Reflection.MethodInfo targetMethod = targetType.GetMethod(targetFunction);
        targetMethod.Invoke(targetComponent, null);
    }
}

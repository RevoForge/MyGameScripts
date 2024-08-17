using UnityEngine;

public class SkillDetection : MonoBehaviour
{
    public GameObject skillDetection_SD;
    public GameObject skillDetection_SU;
    public GameObject skillDetection_LTR;
    public GameObject skillDetection_RTL;
    public GameObject skillDetection_DDL;
    public GameObject skillDetection_DDR;
    public GameObject skillDetection_DUL;
    public GameObject skillDetection_DUR;
    public GameObject skillDetection_TU;
    public GameObject skillDetection_TD;
    public GameObject skillDetection_TR;
    public GameObject skillDetection_TL;
    public GameObject skillDetection_TF;
    public Material red;
    public Material green;
    private GameObject changedLight;


    public void ResetSkillDetection()
    {
        if (changedLight != null)
        {
            changedLight.GetComponent<MeshRenderer>().material = red;
            changedLight = null;
        }
    }
    public void StraightDown()
    {
        changedLight = skillDetection_SD;
        skillDetection_SD.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void StraightUp()
    {
        changedLight = skillDetection_SU;
        skillDetection_SU.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void LeftToRight()
    {
        changedLight = skillDetection_LTR;
        skillDetection_LTR.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void RightToLeft()
    {
        changedLight = skillDetection_RTL;
        skillDetection_RTL.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void DownDiagonalLeft()
    {
        changedLight = skillDetection_DDL;
        skillDetection_DDL.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void DownDiagonalRight()
    {
        changedLight = skillDetection_DDR;
        skillDetection_DDR.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void UpDiagonalLeft()
    {
        changedLight = skillDetection_DUL;
        skillDetection_DUL.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void UpDiagonalRight()
    {
        changedLight = skillDetection_DUR;
        skillDetection_DUR.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void ThrustForward()
    {
        changedLight = skillDetection_TF;
        skillDetection_TF.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    // Unused for now
    // --------------------------
    public void ThrustUp()
    {
        changedLight = skillDetection_TU;
        skillDetection_TU.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void ThrustDown()
    {
        changedLight = skillDetection_TD;
        skillDetection_TD.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void ThrustRight()
    {
        changedLight = skillDetection_TR;
        skillDetection_TR.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    public void ThrustLeft()
    {
        changedLight = skillDetection_TL;
        skillDetection_TL.GetComponent<MeshRenderer>().material = green;
        //Debug.Log("Green Light");
    }
    // ---------------------------


}

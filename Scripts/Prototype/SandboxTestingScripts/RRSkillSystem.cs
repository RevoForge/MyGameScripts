using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Valve.VR;
[Serializable]
public enum SkillType
{
    skill1,
    skill2,
    skill3,
    skill4,
    skill5,
    skill6,
    skill7,
    skill8,
    skill9
}

public class RRSkillSystem : MonoBehaviour
{
    [Header("References")]
    // so we can use the player methods
    private PlayerStats playerStats;
    // perferably use the chest of the player model
    public GameObject playerForward;
    // references to the left and right hand objects
    public GameObject motionObjectR;
    public GameObject motionObjectL;
    // references to the left and right hand UI elements
    public SkillDetection skillDetectionR;
    public SkillDetection skillDetectionL;
    public TextMeshProUGUI skillTimerR;
    public TextMeshProUGUI skillTimerL;
    public TextMeshProUGUI skillExpR;
    public TextMeshProUGUI skillExpL;
    // reference to the user input action for the skill button
    public SteamVR_Action_Boolean userSkillInput;
    // references to the skill prototypes
    public List<Skill> skillList;

    [Header("Settings")]
    public float coolDownTimer = 2f;
    public float minimumMovement = .1f;
    public float minimumSkillMovement = 0.5f;
    public float minimumThrustSkillMovement = 0.5f;

    [Header("Skill Particle prefabs")]
    // Need to Switch later to a single set of prefabs and have an alternate method of differentiating left and right
    // Right Hand
    public GameObject RightSkillDownDiagonalRight;
    public GameObject RightSkillDownDiagonalLeft;
    public GameObject RightSkillUpDiagonalRight;
    public GameObject RightSkillUpDiagonalLeft;
    public GameObject RightSkillRightToLeft;
    public GameObject RightSkillLeftToRight;
    public GameObject RightSkillStraightDown;
    public GameObject RightSkillStraightUp;
    public GameObject RightSkillThrustForward;
    // Left Hand
    public GameObject LeftSkillDownDiagonalRight;
    public GameObject LeftSkillDownDiagonalLeft;
    public GameObject LeftSkillUpDiagonalRight;
    public GameObject LeftSkillUpDiagonalLeft;
    public GameObject LeftSkillRightToLeft;
    public GameObject LeftSkillLeftToRight;
    public GameObject LeftSkillStraightDown;
    public GameObject LeftSkillStraightUp;
    public GameObject LeftSkillThrustForward;
    // Ui references
    public GameObject UiSkill1;
    public GameObject UiSkill2;
    public GameObject UiSkill3;
    public GameObject UiSkill4;
    public GameObject UiSkill5;
    public GameObject UiSkill6;
    public GameObject UiSkill7;
    public GameObject UiSkill8;
    public GameObject UiSkill9;

    // Private Variables
    private SkillType currentSkill;
    private SkillGoFoward currentSkillObject;
    private Quaternion ZeroRotation = Quaternion.identity;
    private Quaternion StraightRotation = Quaternion.Euler(0, 0, 90);
    private Quaternion LeftRotation = Quaternion.Euler(0, 0, -45);
    private Quaternion RightRotation = Quaternion.Euler(0, 0, 45);
    // Right Hand
    private Vector3 initialPositionR;
    private float totalMovementXR = 0.0f;
    private float totalMovementYR = 0.0f;
    private float totalMovementZR = 0.0f;
    private float timerR = 0.0f;
    private float skillResetTimerR = 0.0f;
    private bool skillActiveR = false;
    private bool skilltriggerR = false;
    // Left Hand
    private Vector3 initialPositionL;
    private float totalMovementXL = 0.0f;
    private float totalMovementYL = 0.0f;
    private float totalMovementZL = 0.0f;
    private float timerL = 0.0f;
    private float skillResetTimerL = 0.0f;
    private bool skillActiveL = false;
    private bool skilltriggerL = false;
    //--------------------------------
    // Prototype variables
    public bool usingSkillUI = false;
    public int skillEXP = 10;
    private int skillPointsEarned;
    public int SkillPointCost;
    public float SkillPointCostModifier = 1;
    // for testing purposes
    public int currentSkillPoints = 1;
    public List<TextMeshProUGUI> skilllevelref;


    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        skillResetTimerR = coolDownTimer;
        skillTimerR.text = skillResetTimerR.ToString("F0");
    }
    private void Update()
    {
        if (timerR > coolDownTimer)
        {
            TimerReset(true);
        }
        if (timerL > coolDownTimer)
        {
            TimerReset(false);
        }
        if (!usingSkillUI)
        {
            // Watching for User input from right hand
            if (userSkillInput.GetState(SteamVR_Input_Sources.RightHand) && !skilltriggerR)
            {
                SkillTriggered(true);
            }
            // Watching for User input from left hand
            if (userSkillInput.GetState(SteamVR_Input_Sources.LeftHand) && !skilltriggerL)
            {
                SkillTriggered(false);
            }
        }

        if (skilltriggerR && skillResetTimerR > 0.0f)
        {
            SkillTimerReset(true);
        }
        if (skilltriggerL && skillResetTimerL > 0.0f)
        {
            SkillTimerReset(false);
        }

    }

    // When the player presses the skill button
    private void SkillTriggered(bool isRight)
    {
        if (isRight)
        {
            //Debug.Log("Right Skill Action Down");
            skilltriggerR = true;
            skillActiveR = true;
            initialPositionR = motionObjectR.transform.position;
        }
        else
        {
            //Debug.Log("Left Skill Action Down");
            skilltriggerL = true;
            skillActiveL = true;
            initialPositionL = motionObjectL.transform.position;
        }
    }
    // When the Cool Down Timer runs out
    private void TimerReset(bool isRight)
    {
        if (isRight)
        {
            timerR = 0.0f;
            skillActiveR = false;
            skilltriggerR = false;
            skillResetTimerR = coolDownTimer;
            skillTimerR.text = skillResetTimerR.ToString("F0");
            skillExpR.text = "";
            // Debug.Log("Skill Timer reset");
            skillDetectionR.ResetSkillDetection();
        }
        else
        {
            timerL = 0.0f;
            skillActiveL = false;
            skilltriggerL = false;
            skillResetTimerL = coolDownTimer;
            skillTimerL.text = skillResetTimerL.ToString("F0");
            skillExpL.text = "";
            // Debug.Log("Skill Timer reset");
            skillDetectionL.ResetSkillDetection();
        }
    }
    // Tracks the Cool Down Timer in reverse and displays it on the screen
    private void SkillTimerReset(bool isRight)
    {
        if (isRight)
        {
            skillResetTimerR -= Time.deltaTime;
            skillTimerR.text = skillResetTimerR.ToString("F1");
        }
        else
        {
            skillResetTimerL -= Time.deltaTime;
            skillTimerL.text = skillResetTimerL.ToString("F1");
        }
    }
    // Object Movement checking logic in Fixed Update to avoid issues with FPS
    void FixedUpdate()
    {
        if (skilltriggerR)
        {
            timerR += Time.fixedDeltaTime;
        }
        if (skilltriggerL)
        {
            timerL += Time.fixedDeltaTime;
        }
        if (skillActiveR)
        {
            EvaluateMovement(true);
        }
        if (skillActiveL)
        {
            EvaluateMovement(false);
        }
    }
    // Evaluate the movement based on the total accumulated movement and wich hand is being used
    private void EvaluateMovement(bool isRight)
    {
        Vector3 currentPosition = CalcPositions(isRight);
        float totalMovementX = isRight ? totalMovementXR : totalMovementXL;
        float totalMovementY = isRight ? totalMovementYR : totalMovementYL;
        float totalMovementZ = isRight ? totalMovementZR : totalMovementZL;
        // Check for general movements based on the total accumulated movement use of Abs to avoid issues with negative movement

        if (Math.Abs(totalMovementX) > minimumMovement && Math.Abs(totalMovementY) > minimumMovement)
        {
            if (totalMovementY < -minimumMovement)
            {
                DiaginalDownSkillsCheck(isRight);
            }
            else
            {
                DiaginalUpSkillsCheck(isRight);
            }
        }
        else if (Math.Abs(totalMovementX) > minimumMovement)
        {
            HorizontalSkillsCheck(isRight);
        }
        else if (Math.Abs(totalMovementY) > minimumMovement)
        {
            VerticalSkillsCheck(isRight);
        }
        else if (totalMovementZ > minimumMovement)
        {
            ThrustForwardCheck(isRight);
        }
        // Update initial position for the next frame

        if (isRight)
        {
            initialPositionR = currentPosition;
        }
        else
        {
            initialPositionL = currentPosition;
        }
    }
    // Calculates the total accumulated movement of the motionObject based on the Hand being used
    private Vector3 CalcPositions(bool isRight)
    {
        // Get the Camera's vectors to base skill directions off from it
        Vector3 cameraForward = playerForward.transform.forward;
        Vector3 cameraRight = playerForward.transform.right;
        Vector3 cameraUp = playerForward.transform.up;

        Vector3 currentPosition = isRight ? motionObjectR.transform.position : motionObjectL.transform.position;
        // Calculate the movement since the last frame
        Vector3 frameMovement = currentPosition - (isRight ? initialPositionR : initialPositionL);
        // Get the vectors to match against
        float movementX = Vector3.Dot(frameMovement, cameraRight);
        float movementY = Vector3.Dot(frameMovement, cameraUp);
        float movementZ = Vector3.Dot(frameMovement, cameraForward);

        // Accumulate the total movement
        if (isRight)
        {
            totalMovementXR += movementX;
            totalMovementYR += movementY;
            totalMovementZR += movementZ;
        }
        else
        {
            totalMovementXL += movementX;
            totalMovementYL += movementY;
            totalMovementZL += movementZ;
        }
        return currentPosition;
    }
    /// <summary>
    /// Checks for valid diagonal down movements based on the total movement in the X and Y directions.
    /// </summary>
    /// <param name="isRight">Boolean indicating if the movement is from the right hand.</param>
    private void DiaginalDownSkillsCheck(bool isRight)
    {
        // Determine the total movement in X and Y directions based on the hand
        float totalMovementX = isRight ? totalMovementXR : totalMovementXL;
        float totalMovementY = isRight ? totalMovementYR : totalMovementYL;
        //Debug.Log("Valid diagonal movement detected");
        if (totalMovementX < -minimumSkillMovement && totalMovementY < -minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.DownDiagonalRight();
                InstantiateSkillAndResetTrigger(isRight, RightSkillDownDiagonalRight, motionObjectR.transform.position, RightRotation);
            }
            else
            {
                skillDetectionL.DownDiagonalRight();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillDownDiagonalRight, motionObjectL.transform.position, RightRotation);
            }
            //Debug.Log("Valid diagonal downwards movement to the right detected");
        }
        if (totalMovementX > minimumSkillMovement && totalMovementY < -minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.DownDiagonalLeft();
                InstantiateSkillAndResetTrigger(isRight, RightSkillDownDiagonalLeft, motionObjectR.transform.position, LeftRotation);

            }
            else
            {
                skillDetectionL.DownDiagonalLeft();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillDownDiagonalLeft, motionObjectL.transform.position, LeftRotation);
            }
            //Debug.Log("Valid diagonal downwards movement to the left detected");
        }
    }
    /// <summary>
    /// Checks for valid diagonal up movements based on the total movement in the X and Y directions.
    /// </summary>
    /// <param name="isRight">Boolean indicating if the movement is from the right hand.</param>
    private void DiaginalUpSkillsCheck(bool isRight)
    {
        // Determine the total movement in X and Y directions based on the hand
        float totalMovementX = isRight ? totalMovementXR : totalMovementXL;
        float totalMovementY = isRight ? totalMovementYR : totalMovementYL;
        //Debug.Log("Valid diagonal movement detected");
        if (totalMovementX < -minimumSkillMovement && totalMovementY > minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.UpDiagonalRight();
                InstantiateSkillAndResetTrigger(isRight, RightSkillUpDiagonalRight, motionObjectR.transform.position, RightRotation);
            }
            else
            {
                skillDetectionL.UpDiagonalRight();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillUpDiagonalRight, motionObjectL.transform.position, RightRotation);
            }
            //Debug.Log("Valid diagonal upward movement to the right detected");
        }
        if (totalMovementX > minimumSkillMovement && totalMovementY > minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.UpDiagonalLeft();
                InstantiateSkillAndResetTrigger(isRight, RightSkillUpDiagonalLeft, motionObjectR.transform.position, LeftRotation);

            }
            else
            {
                skillDetectionL.UpDiagonalLeft();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillUpDiagonalLeft, motionObjectL.transform.position, LeftRotation);
            }
            //Debug.Log("Valid diagonal upward movement to the left detected");
        }
    }
    // Checks for specific Horizontal movements based on the total accumulated movement and wich hand is being used after the minimum movement is reached
    private void HorizontalSkillsCheck(bool isRight)
    {
        float totalMovementX = isRight ? totalMovementXR : totalMovementXL;
        //Debug.Log("Valid horizontal movement detected");

        if (totalMovementX > minimumSkillMovement)
        {
            //Debug.Log("Valid horizontal movement from right to left detected");
            if (isRight)
            {
                skillDetectionR.RightToLeft();
                InstantiateSkillAndResetTrigger(isRight, RightSkillRightToLeft, motionObjectR.transform.position, ZeroRotation);
            }
            else
            {
                skillDetectionL.RightToLeft();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillRightToLeft, motionObjectL.transform.position, ZeroRotation);
            }
        }
        if (totalMovementX < -minimumSkillMovement)
        {
            //Debug.Log("Valid horizontal movement from left to right detected");
            if (isRight)
            {
                skillDetectionR.LeftToRight();
                InstantiateSkillAndResetTrigger(isRight, RightSkillLeftToRight, motionObjectR.transform.position, ZeroRotation);
            }
            else
            {
                skillDetectionL.LeftToRight();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillLeftToRight, motionObjectL.transform.position, ZeroRotation);
            }
        }
    }
    // Checks for specific Vertical movements based on the total accumulated movement and wich hand is being used after the minimum movement is reached
    private void VerticalSkillsCheck(bool isRight)
    {
        float totalMovementY = isRight ? totalMovementYR : totalMovementYL;
        //Debug.Log("Valid vertical movement detected");

        // Check for Straight downwards movement
        if (totalMovementY < -minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.StraightDown();
                InstantiateSkillAndResetTrigger(isRight, RightSkillStraightDown, motionObjectR.transform.position, StraightRotation);
            }
            else
            {
                skillDetectionL.StraightDown();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillStraightDown, motionObjectL.transform.position, StraightRotation);
            }
            //Debug.Log("Valid straight downwards movement detected");
        }
        // Check for upward movement
        if (totalMovementY > minimumSkillMovement)
        {
            if (isRight)
            {
                skillDetectionR.StraightUp();
                InstantiateSkillAndResetTrigger(isRight, RightSkillStraightUp, motionObjectR.transform.position, StraightRotation);
            }
            else
            {
                skillDetectionL.StraightUp();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillStraightUp, motionObjectL.transform.position, StraightRotation);
            }
            //Debug.Log("Valid upward movement detected");
        }
    }
    // Checks for specific Thrust movements based on the total accumulated movement and wich hand is being used after the minimum movement is reached
    private void ThrustForwardCheck(bool isRight)
    {
        float totalMovement = isRight ? totalMovementZR : totalMovementZL;
        //Debug.Log("Valid Thrust movement detected");
        if (totalMovement > minimumThrustSkillMovement)
        {
            //Debug.Log("Valid forward movement detected");
            if (isRight)
            {
                skillDetectionR.ThrustForward();
                InstantiateSkillAndResetTrigger(isRight, RightSkillThrustForward, motionObjectR.transform.position, ZeroRotation);
            }
            else
            {
                skillDetectionL.ThrustForward();
                InstantiateSkillAndResetTrigger(isRight, LeftSkillThrustForward, motionObjectL.transform.position, ZeroRotation);
            }
        }
    }
    private void InstantiateSkillAndResetTrigger(bool isRight,GameObject skill,Vector3 skillPosition,Quaternion skillRotation)
    {
        if (playerStats != null)
        {
            currentSkillObject = Instantiate(skill, skillPosition, skillRotation).GetComponent<SkillGoFoward>();
            // Check if the player has enough resource to use the skill
            if (playerStats.SkillWasUsed(currentSkillObject.SkillScriptable))
            {
                currentSkillObject.isRight = isRight;
                currentSkillObject.rrSkillSystem = this;
                currentSkill = currentSkillObject.SkillScriptable.SkillName;
                ResetForSkillTrigger(isRight);
            }
            // Fizzle out the skill if the player does not have enough resource
            else
            {
                Destroy(currentSkillObject);
                // maybe add a sound here or a particle fizzle or something
                ResetForSkillTrigger(isRight);
            }
        }
        // contains prototype of skill level system for testing without player stats
        // TODO: remove this
        else
        {
            currentSkillObject = Instantiate(skill, skillPosition, skillRotation).GetComponent<SkillGoFoward>();
            currentSkillObject.isRight = isRight;
            currentSkillObject.rrSkillSystem = this;
            currentSkill = currentSkillObject.SkillScriptable.SkillName;
            ResetForSkillTrigger(isRight);
        }

    }
    // Resets the total accumulated movement and the state after the skill has been triggered based on the hand
    private void ResetForSkillTrigger(bool isRight)
    {
        if (isRight)
        {
            totalMovementXR = 0.0f;
            totalMovementYR = 0.0f;
            totalMovementZR = 0.0f;
            skillActiveR = false;
        }
        else
        {
            totalMovementXL = 0.0f;
            totalMovementYL = 0.0f;
            totalMovementZL = 0.0f;
            skillActiveL = false;
        }

    }
    // Call when Skill Menu is Open
    public void CheckSkillExpForNextLevel()
    {
        SkillPointCost = (int)(skillPointsEarned * SkillPointCostModifier);
        while (skillEXP > SkillPointCost)
        {
            skillEXP -= SkillPointCost;
            skillPointsEarned += 1;
            SkillPointCost = (int)(skillPointsEarned * SkillPointCostModifier);
            if (playerStats != null)
            {
                playerStats.AvalibleSkillPoints += 1;
            }
            // for testing purposes
            currentSkillPoints += 1;
        }

    }
    /// <summary>
    /// Called From Skills when they hit a mob if the hit was a kill add extra XP
    /// </summary>
    /// <param name="isKill">If the hit was a killing blow</param>
    public void AddSkillXP(bool isKill, bool isRight)
    {
        // Add skill XP when a skill hits a mob
        if (isKill)
        {
            //give bonus XP
            skillEXP += 10;
            // UI popup of skill exp
            if (isRight)
            {
                skillExpR.text = "Skill Exp: " + 10 + " XP";
                //Debug.Log(skillExp[SkillTypeRef].ToString());
            }
            else
            {
                skillExpL.text = "Skill Exp: " + 10 + " XP";
                //Debug.Log(skillExp[SkillTypeRef].ToString());
            }
        }
        else
        {
            //give XP
            skillEXP += 1;
            // UI popup of skill exp
            if (isRight)
            {
                skillExpR.text = "Skill Exp: " + 1 + " XP";
                //Debug.Log(skillExp[SkillTypeRef].ToString());
            }
            else
            {
                skillExpL.text = "Skill Exp: " + 1 + " XP";
                //Debug.Log(skillExp[SkillTypeRef].ToString());
            }
        }
    }
    // Prototype of Adding levels to Skills
    public void AddSkillPoints(string skillNumber)
    {
        if (currentSkillPoints > 0)
        {
            currentSkillPoints -= 1;
            switch (skillNumber)
            {
                case "1":
                    AddSkillPointsToSkill(SkillType.skill1,0);
                    break;
                case "2":
                    AddSkillPointsToSkill(SkillType.skill2,1);
                    break;
                case "3":
                    AddSkillPointsToSkill(SkillType.skill3,2);
                    break;
                case "4":
                    AddSkillPointsToSkill(SkillType.skill4,3);
                    break;
                case "5":
                    AddSkillPointsToSkill(SkillType.skill5,4);
                    break;
                case "6":
                    AddSkillPointsToSkill(SkillType.skill6,5);
                    break;
                case "7":
                    AddSkillPointsToSkill(SkillType.skill7,6);
                    break;
                case "8":
                    AddSkillPointsToSkill(SkillType.skill8,7);
                    break;
                case "9":
                    AddSkillPointsToSkill(SkillType.skill9,8);
                    break;
                default:
                    break;
            }
        }

    }
    private void AddSkillPointsToSkill(SkillType skillType,int skillRef)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].SkillName == skillType)
            {
                skillList[i].Level += 1;
                skilllevelref[skillRef].text = skillType.ToString() + " Level:" + skillList[i].Level;
                break;
            }
        }
    }

    // UI Skill Changed Prototypes TMP_Dropdown receivers
    public void RightStraightDownUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillStraightDown);
    }
    public void RightStraightUpUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillStraightUp);
    }
    public void RightLeftToRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillLeftToRight);
    }
    public void RightRightToLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillRightToLeft);
    }
    public void RightDownDiagonalRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillDownDiagonalRight);
    }
    public void RightDownDiagonalLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillDownDiagonalLeft);
    }
    public void RightUpDiagonalRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillUpDiagonalRight);
    }
    public void RightUpDiagonalLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillUpDiagonalLeft);
    }
    public void RightThrustUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref RightSkillThrustForward);
    }
    public void LeftStraightDownUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillStraightDown);
    }
    public void LeftStraightUpUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillStraightUp);
    }
    public void LeftLeftToRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillLeftToRight);
    }
    public void LeftRightToLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillRightToLeft);
    }
    public void LeftDownDiagonalRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillDownDiagonalRight);
    }
    public void LeftDownDiagonalLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillDownDiagonalLeft);
    }
    public void LeftUpDiagonalRightUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillUpDiagonalRight);
    }
    public void LeftUpDiagonalLeftUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillUpDiagonalLeft);
    }
    public void LeftThrustUISkillChanged(TMP_Dropdown dropdown)
    {
        UpdateSkill(dropdown, ref LeftSkillThrustForward);
    }
    // redundency reduction
    private void UpdateSkill(TMP_Dropdown dropdown,ref GameObject targetSkill)
    {
        targetSkill = dropdown.value switch
        {
            0 => UiSkill1,
            1 => UiSkill2,
            2 => UiSkill3,
            3 => UiSkill4,
            4 => UiSkill5,
            5 => UiSkill6,
            6 => UiSkill7,
            7 => UiSkill8,
            8 => UiSkill9,
            _ => targetSkill
        };
    }
}
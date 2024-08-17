using UnityEngine;
using UnityEngine.UI;

public class YourScript : MonoBehaviour
{
    public GameObject target;
    private Animator animControl;

    public bool isStatue = false;
    public bool isWalking = false;

    public GameObject statuePanel;
    public GameObject groundPanel;

    public GameObject walkingText;

    private void Start()
    {
        animControl = target.GetComponent<Animator>();
    }

    public void ToggleWalking()
    {
        if (isWalking)
        {
            if (isStatue)
            {
                isStatue = false;
                animControl.SetTrigger("makeAlive");
            }
            isWalking = false;
            walkingText.GetComponent<Text>().text = "Start Walking";
            animControl.SetBool("isWalking", false);
            animControl.SetTrigger("stopWalking");
        }
        else if (!isWalking)
        {
            if (isStatue)
            {
                isStatue = false;
                animControl.SetTrigger("makeAlive");
            }
            isWalking = true;
            walkingText.GetComponent<Text>().text = "Stop Walking";
            animControl.SetBool("isWalking", true);
            animControl.SetTrigger("startWalking");
        }
    }

    public void Death()
    {
        animControl.SetTrigger("die");
    }

    public void IdleBreak()
    {
        animControl.SetTrigger("idleBreak");
    }

    public void GotHit()
    {
        animControl.SetTrigger("gotHit");
    }

    public void Attack1()
    {
        animControl.SetTrigger("attack1");
    }

    public void Attack2()
    {
        animControl.SetTrigger("attack2");
    }

    public void Attack3()
    {
        animControl.SetTrigger("attack3");
    }

    public void Statue1()
    {
        animControl.SetTrigger("statue1");
    }

    public void Statue2()
    {
        animControl.SetTrigger("statue2");
    }

    public void Statue3()
    {
        animControl.SetTrigger("statue3");
    }

    public void Attack4()
    {
        animControl.SetTrigger("attack4");
    }

    public void Block()
    {
        animControl.SetTrigger("block");
    }

    public void StopAttack4()
    {
        animControl.SetTrigger("stopAttack4");
    }

    public void StopBlocking()
    {
        animControl.SetTrigger("stopBlock");
    }

    public void Squash()
    {
        animControl.SetTrigger("squash");
    }

    public void StartBackward()
    {
        animControl.SetTrigger("startWalkBack");
    }

    public void StopBackward()
    {
        animControl.SetTrigger("stopWalkBack");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private Animator anim;
    int strikeNum = -1;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        anim.speed = 1;
    }

    public void IncreaseAnimSpeed()
    {
        anim.speed = anim.speed + 0.01f;
    }

    public void Run()
    {
        Debug.Log("run hit");
        if (!GameManager.Instance.isBonus)
            anim.SetTrigger("run");
        else
            Idle();
    }
    public void Idle()
    {
        anim.SetTrigger("idle");
    }

    public void DecreaseRunSpeed()
    {
        anim.speed = 0.6f;
    }

    public void Die()
    {
        anim.SetTrigger("die");
    }

    public void QuickSlash()
    {
        anim.speed = 1f;
        anim.SetTrigger("quickSlash");
    }
    public void WarpAnim()
    {
        anim.speed = 1f;
        //anim.SetTrigger("warp");
        anim.Play("warp");
    }

    public void Falling()
    {
        if (!GameManager.Instance.isBonus)
            anim.SetTrigger("falling");
        else
            Idle();
    }
    public void RegularJump()
    {
        anim.SetTrigger("jump");
    }
    public void WallRun(bool isLeft)
    {
        if (isLeft)
        {
            anim.SetTrigger("leftWallRun");
        }
        else
        {
            anim.SetTrigger("rightWallRun");
        }
    }
    public void UpWallRun()
    {
        anim.SetTrigger("upwall");
    }

    public void Vault()
    {
        anim.SetTrigger("vault");
    }

    public void Land()
    {
        anim.SetTrigger("land");
    }

    public void Slide()
    {
        anim.SetTrigger("slide");
    }

    public void Dash()
    {
        anim.SetTrigger("dash");
    }

    public void SideJump()
    {
        anim.SetTrigger("sidejump");
    }

    public void StrikeToHalf()
    {
        /*
        strikeNum++;
        if (strikeNum > 3)
            strikeNum = 0;
        anim.SetInteger("strike num", strikeNum);
        */
        anim.SetTrigger("strike");
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Water")) {
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }

    }

    public void ResetAnims()
    {
        anim.ResetTrigger("run");
        anim.ResetTrigger("falling");
        anim.ResetTrigger("slide");
        anim.ResetTrigger("vault");
        anim.ResetTrigger("land");
        anim.ResetTrigger("rightWallRun");
        anim.ResetTrigger("leftWallRun");
        anim.ResetTrigger("upwall");
        anim.ResetTrigger("strike");
    }

    public void Turn(float turnAmmount)
    {
        anim.SetFloat("Turn", turnAmmount);
    }
    public bool IsAnimationPlaying(string animName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
        {
            return true;
        }
        return false;
    }

    public bool IsAttackAnimPlaying()
    {
        if (IsAnimationPlaying("warp") || IsAnimationPlaying("dash") || IsAnimationPlaying("After slash") || IsAnimationPlaying("After slash 2") || IsAnimationPlaying("After slash 3") || IsAnimationPlaying("After slash 4") || IsAnimationPlaying("After slash 5"))
            return true;

        return false;
    }

    public bool IsTriggerActive(string stirggerName)
    {
        if (anim.GetBool(stirggerName) == false)
        {
            return true;
        }
        return false;
    }
    public void StopAll()
    {
        //anim.StopPlayback();
    }
}

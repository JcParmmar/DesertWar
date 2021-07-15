using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    //Animatior for Perform Animations
    Animator anim;

    private void Awake()
    {
        //Set Animatior
        anim = GetComponent<Animator>();
    }
    //Change Animation Modes
    //a = 0 is Idle
    //a = 1 is Movement
    public void OnChangeAnimationMode(int a)
    {
        anim.SetInteger("Animations", a);
    }

    //Set Which Animations will Playe 
    //OnMoveAnimations(Forward/backward , Left/Right , Is Running Or Not);
    public void OnMoveAnimations(float hor,float ver,bool r)
    {
        anim.SetFloat("Hor",hor);
        anim.SetFloat("Ver",ver);

        if (r)
        {
            anim.SetFloat("Running", 1);
        }
        else
        {
            anim.SetFloat("Running", 0);
        }
    }

}

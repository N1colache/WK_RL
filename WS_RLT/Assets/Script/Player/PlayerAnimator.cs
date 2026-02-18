using UnityEngine;
using UnityEngine.Animations;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField]private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.LogError("Animator non trouvé !");
        
    }

    void Update()
    {
        Debug.Log(_animator.GetFloat("Speed"));
    }

    
    
    public void SetWalkingSpeed(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void IsJumping(bool isJumping)
    {
        _animator.SetBool("IsJumping", isJumping);
    }

    public void SetJumpingSpeed(float speedJump)
    {
        _animator.SetFloat("SpeedJump", speedJump);
    }

    
    public void AttackLeft()
    {
        _animator.SetBool("IsAttacking", true);
        
    }
    public void AttackRight()
    {
        _animator.SetBool("IsAttacking", true);
        
    }

    public void StopAttacking()
    {
        _animator.SetBool("IsAttacking", false);
    }
}

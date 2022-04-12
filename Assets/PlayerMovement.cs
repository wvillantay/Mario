using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    //public FixedJoystick variableJoystick;
   
    public static PlayerMovement instance;
    public GameObject Attackpos;
    public GameObject Ball;

    [SerializeField] float MovementSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] Vector3 range;
    [SerializeField] Transform GroundPos;
    [SerializeField] LayerMask GroundLayer;
    float MoveInput;
    Rigidbody2D Mybody;
    public bool IsFacingRight;
    Animator anim;
    [HideInInspector]
    public int Keycount;


    bool isGrounded;

   public CapsuleCollider2D mainCollider;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Mybody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        mainCollider = GetComponent<CapsuleCollider2D>();
        IsFacingRight = true;
        //Jumpheight = 0.4f;
    }
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    anim.SetBool("attack", true);
            
        //}
        //else
        //{
        //    anim.SetBool("attack", false);

        //}

    }
    public void Fire()
    {

        StartCoroutine(Fire1());
    }
    IEnumerator Fire1()
    {
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(.15f);
        anim.SetBool("attack", false);


    }
    public void SpawnBall()
    {
        Instantiate(Ball, Attackpos.transform.position, Quaternion.identity);
    }
    private void FixedUpdate()
    {

        Movement();
        CheckCollisionForJump();
    }
    void Movement()
    {
        MoveInput = Input.GetAxisRaw("Horizontal") * MovementSpeed /** variableJoystick.Horizontal */;

        //  Debug.Log(MoveInput);

        
        anim.SetFloat("Run", Mathf.Abs(MoveInput));
        Mybody.velocity = new Vector2(MoveInput, Mybody.velocity.y);
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    if(Mybody.velocity.y>0)
        //    {
        //        Mybody.velocity = new Vector2(Mybody.velocity.x, Mybody.velocity.y + Jumpheight);
        //    }
        //}
        if (!IsFacingRight && MoveInput > 0 || IsFacingRight && MoveInput < 0)
        {
            Flip();
        }
    }
    void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 LocalScale = transform.localScale;
        LocalScale.x *= -1;
        transform.localScale = LocalScale;
    }
    public void CheckCollisionForJump()
    {
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (isGrounded)
        {

            Mybody.velocity = new Vector2(Mybody.velocity.x, JumpForce);
            StopCoroutine(SetJumpAnim());
            StartCoroutine(SetJumpAnim());
        }
    }

    IEnumerator SetJumpAnim()
    {
        anim.SetBool("jump", true);
        yield return new WaitForSeconds(.9f);
        while (!isGrounded)
        {
            yield return null;
        }
        if (isGrounded)
        {
            anim.SetBool("jump", false);

        }


    }


}
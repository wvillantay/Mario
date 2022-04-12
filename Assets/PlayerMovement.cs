using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public TMP_Text score;
   
    public static PlayerMovement instance;
    public GameObject Attackpos;
    public LayerMask enemylayer;
    int Score;
    public float MovementSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] Vector3 range;
 
    [SerializeField] LayerMask GroundLayer;
    float MoveInput;
    Rigidbody Mybody;
    public bool IsFacingRight;
   // Animator anim;
    [HideInInspector]
    public int Keycount;
    public float dist;
    float force;


    bool isGrounded;

   public BoxCollider mainCollider;
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
        Mybody = GetComponent<Rigidbody>();
       // anim = GetComponent<Animator>();
    }
    private void Start()
    {
        mainCollider = GetComponent<BoxCollider>();
        IsFacingRight = true;
        
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {


            Fire();
        }


    }
    public void Fire()
    {

        if (transform.localScale.x > 0)
        {
            force = -1;
        }
        else if (transform.localScale.x < 0)
        {
            force = 1;
        }


        //RaycastHit2D hit3 = Physics2D.Raycast(Attackpos.transform.position, Vector2.left * force*dist);
        //Debug.DrawRay(Attackpos.transform.position, Vector2.left * force*dist, Color.red);

        //if (hit3.collider != null)
        //{

        //    if (hit3.collider.gameObject.CompareTag("enemy"))
        //    {
        //        Debug.Log("Fire");
        //        Debug.DrawRay(Attackpos.transform.position, Vector2.left * force, Color.yellow);
        //        Destroy(hit3.collider.gameObject);
        //    }
        //}
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left)*force, out hit, 3, enemylayer))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left)*force * hit.distance, Color.yellow);
            Destroy(hit.transform.gameObject);
            Score += 100;
            score.text = "score : " + Score.ToString();
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left)*force * 2, Color.white);
            Debug.Log("Did not Hit");
        }
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

        
        //anim.SetFloat("Run", Mathf.Abs(MoveInput));
        Mybody.velocity = new Vector2(MoveInput, Mybody.velocity.y);
       
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

        if (isGrounded && Input.GetKey(KeyCode.Space))
        {

            Mybody.velocity = new Vector2(Mybody.velocity.x, JumpForce);
            StopCoroutine(SetJumpAnim());
            StartCoroutine(SetJumpAnim());
        }
    }

    IEnumerator SetJumpAnim()
    {
      //  anim.SetBool("jump", true);
        yield return new WaitForSeconds(.9f);
        while (!isGrounded)
        {
            yield return null;
        }
        if (isGrounded)
        {
           // anim.SetBool("jump", false);

        }


    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Coin") || collision.gameObject.CompareTag("enemy") 
            || collision.gameObject.CompareTag("Ground") )
            {
            Destroy(collision.gameObject);
            Score += 100;
            score.text = "score : " + Score;
        }
        if (collision.gameObject.CompareTag("finish"))
        {
            Debug.Log("you win");
            Score += 100;
            score.text = "score : " + Score;
            MovementSpeed = 0f;
        }
    }

}
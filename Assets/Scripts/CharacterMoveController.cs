using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    public float moveAccel, maxSpeed, jumpAccel, scoringRatio, fallPositionY;
    private Rigidbody2D rb;
    public float groundRaycastDistance;
    public LayerMask groundLayerMask;
    public GameObject gameoverScreen;
    public ScoreController score;
    private bool isJumping, isOnGround;
    private Animator anim;
    public CameraMoveController gameCamera;
    private CharacterSoundController sound;
    private float lastPositionX;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sound = GetComponent<CharacterSoundController>();
        lastPositionX = transform.position.x;
    }
    void Update()
    {
        //Kalau tekan mouse kiri atau tap layar di android
        if (Input.GetMouseButtonDown(0))
        {
            if (isOnGround)
            {
                isJumping = true;
                sound.PlayJump();
            }
        }
        anim.SetBool("isOnGround", isOnGround);

        int distancePassed = Mathf.FloorToInt(transform.position.x - lastPositionX);
        int scoreIncrement = Mathf.FloorToInt(distancePassed / scoringRatio);
        if (scoreIncrement > 0)
        {
            score.IncreaseCurrentScore(scoreIncrement);
            lastPositionX += distancePassed;
        }
        if(transform.position.y < fallPositionY)
        {
            GameOver();
        }
    }
    void FixedUpdate()
    {
        Vector2 velocity = rb.velocity;
        velocity.x = Mathf.Clamp(velocity.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);
        rb.velocity = velocity;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastDistance, groundLayerMask);
        if (hit)
        {
            if (!isOnGround && rb.velocity.y <= 0)
            {
                isOnGround = true;
            }
        }
        else
        {
            isOnGround = false;
        }
        // hitung velocity vector
        Vector2 velocityVector = rb.velocity;
        if (isJumping)
        {
            velocityVector.y += jumpAccel;
            isJumping = false;
        }
        velocityVector.x = Mathf.Clamp(velocityVector.x + moveAccel * Time.deltaTime, 0.0f, maxSpeed);
        rb.velocity = velocityVector;
    }
    void GameOver()
    {
        score.FinishScoring();
        gameCamera.enabled = false;

        gameoverScreen.SetActive(true);
        this.enabled = false;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, transform.position + (Vector3.down * groundRaycastDistance), Color.white);
    }
}

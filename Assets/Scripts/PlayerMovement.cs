using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    
    public Vector2 Velocity;
    public bool isDead = false;
    public float groundDistance;
    private Vector2 touchStartPos;
    private Vector2 touchEndPos;
    public float swipeThreshold;

    //X Components
    public float Acceleration;
    public float maxAccel;
    public float maxVelocity;
    public float score = 0;

    //Y Components
    public float jumpVelocity = 20;
    public float groundHeight = 2f;
    public float newGroundHeight;

    public bool isGrounded = false;
    public float gravity;

    public float jumpGroundThreshold = 0.3f; // Leniency for player

    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;

    //Parkour 
    public float vaultDuration = 1f;
    public float dynamicVaultDuration;

    public float longDuration = 0.8f;
    public float dynamicLongDuration;

    public float minDuration = 0.1f;
    public float fastJumpVelocity = 30f;

    //Pursuer
    public Pursuer pursuer;
    public float targetX;
    public float moveSpeed;
    public float interFactor;
    public float newX;
    public float minX = -3f;
    public float durationMultiplier;

    //Animation
    public bool isVaulting = false;
    public bool isShort = false;
    public bool isLong = false;
    public bool isHit = false;
    public bool isSliding = false;

    //Audio
    public AudioManager audioManager;

    private enum SwipeDirection
    {
        None,
        Up,
        Down,
        Right,
        Left
    }

    private SwipeDirection currentSwipe = SwipeDirection.None;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        audioManager = GetComponent<AudioManager>();
    }

    private SwipeDirection GetSwipeDirection(Vector2 startPos, Vector2 endPos)
    {
        Vector2 swipeVector = endPos - startPos;

        // Check if the swipe is mostly horizontal or vertical
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            // Horizontal swipe
            if (swipeVector.x > 0)
            {
                return SwipeDirection.Right;
            }
            else
            {
                return SwipeDirection.Left;
            }
        }
        else
        {
            // Vertical swipe
            if (swipeVector.y > 0)
            {
                return SwipeDirection.Up;
            }
            else
            {
                return SwipeDirection.Down;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = transform.position;
        groundDistance = Mathf.Abs(pos.y - groundHeight);
        float playerSpeed = Mathf.Abs(Velocity.x);
        dynamicLongDuration = Mathf.Lerp(longDuration, minDuration, playerSpeed / maxVelocity);
        dynamicVaultDuration = Mathf.Lerp(vaultDuration, minDuration, playerSpeed / maxVelocity);

        //Mobile controls
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStartPos = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchEndPos = Input.GetTouch(0).position;

            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    touchStartPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended && !isHit && isGrounded)
                {
                    touchEndPos = touch.position;

                    // Get the swipe direction
                    SwipeDirection direction = GetSwipeDirection(touchStartPos, touchEndPos);

                    // Switch case to handle different swipe directions
                    switch (direction)
                    {
                        case SwipeDirection.Up:
                            // Perform action for swipe up (jumping)
                            isGrounded = false;
                            Velocity.y = jumpVelocity;
                            audioManager.PlayJumpSound();
                            break;

                        case SwipeDirection.Down:
                            // Perform action for swipe down (sliding)
                            StartCoroutine(sliding());
                            break;

                        case SwipeDirection.Right:
                            // Perform action for swipe right (vaulting)
                            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, new Vector2(1f, 1f), 0f);
                            foreach (Collider2D collider in colliders)
                            {
                                if (collider.gameObject.layer == LayerMask.NameToLayer("Box"))
                                {
                                    StartCoroutine(HandleVault());
                                    audioManager.PlayJumpSound();
                                }

                                if (collider.gameObject.layer == LayerMask.NameToLayer("Long"))
                                {
                                    StartCoroutine(HandleLongVault());
                                    audioManager.PlayJumpSound();
                                }
                            }
                            break;

                        case SwipeDirection.None:
                        default:
                            // No swipe action
                            break;
                    }
                }
            }
        }


        interFactor = Time.deltaTime * moveSpeed;
        if (newX <= minX)
        {
            newX = minX;
        }

        if (Mathf.Abs(pursuer.transform.position.x - newX) > 0.01f)
        {
            // Interpolate pursuer's position towards the new x
            pursuer.transform.position = Vector3.Lerp(pursuer.transform.position, new Vector3(newX, pursuer.transform.position.y, pursuer.transform.position.z), interFactor);
        }
    }

    private void FixedUpdate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 pos = transform.position;

        if (isDead)
        {
            return;
        }

        if (pos.y < -20)
        {
            isDead = true;
        }

        if (!isGrounded)
        {
            pos.y += Velocity.y * Time.fixedDeltaTime;
            Velocity.y += -gravity * Time.fixedDeltaTime;

            Vector2 rayOrigin = new Vector2(pos.x + 0.4f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = Velocity.y * Time.fixedDeltaTime;

            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, groundLayerMask);
            if (hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y >= ground.groundHeight && !isVaulting)
                    {
                        groundHeight = ground.groundHeight;
                        pos.y = groundHeight;
                        Velocity.y = 0;
                        isGrounded = true;
                        audioManager.PlayLandingSound();
                    }
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

            //Wall Hit
            Vector2 wallOrigin = new Vector2(pos.x, pos.y);
            RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, Vector2.right, Velocity.x * Time.fixedDeltaTime, groundLayerMask);
            
            if (wallHit.collider != null)
            {
                Ground ground = wallHit.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y < ground.groundHeight)
                    {
                        Debug.Log($"wut");

                        Velocity.x = 0;
                    }
                }
            }
        }

        if (isGrounded)
        {
            float velocityRatio = Velocity.x / maxVelocity;
            Acceleration = maxAccel * (1 - velocityRatio);

            Velocity.x += Acceleration * Time.fixedDeltaTime;
            
            if (Velocity.x >= maxVelocity)
            {
                Velocity.x = maxVelocity;
            }

            if (!isVaulting)
            {
                Vector2 rayOrigin = new Vector2(pos.x - 0.4f, pos.y);
                Vector2 rayDirection = Vector2.up;
                float rayDistance = Velocity.y * Time.fixedDeltaTime;

                RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
                if (hit2D.collider == null)
                {
                    isGrounded = false;
                }
            }
        }

        //Obstacle collision
        if (!isVaulting)
        {
            Vector2 obsOrigin = new Vector2(pos.x, pos.y + 0.5f);
            if (isSliding)
            {
                obsOrigin.y -= 0.2f; 
            }

            RaycastHit2D obsHitX = Physics2D.Raycast(obsOrigin, Vector2.right, Velocity.x * Time.fixedDeltaTime, obstacleLayerMask);
            if (obsHitX.collider != null)
            {
                Obstacle obstacle = obsHitX.collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    hitObstacle(obstacle);
                }
            }
            Debug.DrawRay(obsOrigin, Vector2.right * Velocity.x * Time.fixedDeltaTime, Color.white);
            RaycastHit2D obsHitY = Physics2D.Raycast(obsOrigin, Vector2.up, Velocity.y * Time.fixedDeltaTime, obstacleLayerMask);
            if (obsHitY.collider != null)
            {
                Obstacle obstacle = obsHitY.collider.GetComponent<Obstacle>();
                if (obstacle != null)
                {
                    hitObstacle(obstacle);
                }
            }
        }

        //Pursuer catches you
        float playerLeft = spriteRenderer.bounds.min.x;
        if (pursuer.pursuerRight >= playerLeft)
        {
            isDead = true;
            Time.timeScale = 0f;
        }

        score += (Velocity.x * Time.fixedDeltaTime) * 100;

        transform.position = pos;
    }

    void hitObstacle(Obstacle obstacle)
    {
        StartCoroutine(FallAndDestroy(obstacle));
        Velocity.x *= 0.85f;
        newX = pursuer.transform.position.x + targetX + 0.8f;
        isHit = true;
        StartCoroutine(ResetHitStatus());
    }

    IEnumerator FallAndDestroy(Obstacle obstacle)
    {
        Destroy(obstacle.GetComponent<BoxCollider2D>());
        float fallDuration = 0.4f;  // Adjust the duration as needed
        float elapsedTime = 0f;

        Vector3 initialPosition = obstacle.transform.position;
        Vector3 targetPosition = initialPosition - new Vector3(0f, 2f, 0f);  // Adjust the fall distance as needed

        while (elapsedTime < fallDuration)
        {
            obstacle.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(obstacle.gameObject);
    }

    IEnumerator ResetHitStatus()
    {
        yield return new WaitForSeconds(0.3f);
        isHit = false;
    }

    private IEnumerator boxVault()
    {
        float startTime = Time.time;
        float endTime = startTime + dynamicVaultDuration;
        isShort = true;

        float originalGravity = gravity; // Store the original gravity value
        float originalJumpVelocity = jumpVelocity; // Store the original jump velocity value

        // Gradually increase jump velocity and reduce gravity
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / dynamicVaultDuration;

            // Adjust the jump velocity and gravity based on the interpolation factor t
            jumpVelocity = Mathf.Lerp(originalJumpVelocity, 0, t);
            gravity = Mathf.Lerp(originalGravity, 0f, t);

            // Simulate the vaulting effect by moving the player upwards
            float floatHeight = Mathf.Lerp(groundHeight, groundHeight + 1f, t * 4f);
            transform.position = new Vector3(transform.position.x, floatHeight, transform.position.z);

            yield return null;
        }

        // Reset jump velocity and gravity to their original values
        jumpVelocity = originalJumpVelocity;
        gravity = originalGravity;

        // Ensure the player is back on the ground level
        isShort = false;
        newX = pursuer.transform.position.x - targetX;
    }

    private IEnumerator HandleVault()
    {
        isVaulting = true;
        yield return StartCoroutine(boxVault());
        isVaulting = false;
    }

    private IEnumerator vault()
    {
        float startTime = Time.time;
        float endTime = startTime + dynamicLongDuration;
        isLong = true;

        float originalGravity = gravity;
        float originalJumpVelocity = jumpVelocity;

        // Gradually increase jump velocity and reduce gravity
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / dynamicLongDuration;

            // Adjust the jump velocity and gravity based on the interpolation factor t
            jumpVelocity = Mathf.Lerp(originalJumpVelocity, 0, t);
            gravity = Mathf.Lerp(originalGravity, 0f, t);

            // Simulate the vaulting effect by moving the player upwards
            float floatHeight = Mathf.Lerp(groundHeight, groundHeight + 1f, t * 4);
            transform.position = new Vector3(transform.position.x, floatHeight, transform.position.z);

            yield return null;
        }
        jumpVelocity = originalJumpVelocity;
        gravity = originalGravity;

        // Ensure the player is back on the ground level
        isLong = false;
        newX = pursuer.transform.position.x - targetX;
    }

    private IEnumerator HandleLongVault()
    {
        isVaulting = true;
        yield return StartCoroutine(vault());
        isVaulting = false;
    }

    IEnumerator sliding()
    {
        isSliding = true;
        yield return new WaitForSeconds(0.45f);
        newX = pursuer.transform.position.x - targetX;
        isSliding = false;
    }

    void Jump()
    {
        Vector2 pos = transform.position;

        if (isGrounded || groundDistance <= jumpGroundThreshold)
        {
            isGrounded = false;
            Velocity.y = jumpVelocity;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(pos, new Vector2(1f, 1f), 0f);
            audioManager.PlayJumpSound();

            foreach (Collider2D collider in colliders)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Box"))
                {
                    StartCoroutine(HandleVault());
                }

                if (collider.gameObject.layer == LayerMask.NameToLayer("Long"))
                {
                    StartCoroutine(HandleLongVault());
                }
            }
        }
    }
}

/*calculate swipe{
    //code to calculate swipe

    if(swipe right){
        right
    }
}

if (right){
    startcoroutine
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Animations;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public bool run;
    public bool walk;
    public float moveSpeed = 5f;
    public Rigidbody2D playerRb;
    public SpriteRenderer PlayerSr;
    public Animator PlayerAnim;
    public GameObject sword0001;
    // optional: set in inspector (0 = auto-detect clip length)
    public float hitAnimationDuration = 0f;
    public float damageAnimationDuration = 0f; // new: optional override for damage clip length

    Vector2 movement;
    bool isAttacking = false;
    bool isDamaged = false; // new: prevent retriggering Damage while playing

    // cache animation clip lengths for quick lookup
    Dictionary<string, float> clipLengths = new Dictionary<string, float>();

    void Awake()
    {
        if (PlayerAnim != null && PlayerAnim.runtimeAnimatorController != null)
        {
            foreach (var clip in PlayerAnim.runtimeAnimatorController.animationClips)
            {
                clipLengths[clip.name] = clip.length;
            }
        }
    }

    void Update()
    {
        //input and control
        if (movement.x == -1f)
        {
            PlayerSr.flipX = true;

        }
        else if (movement.x == 1f)
        {

            PlayerSr.flipX = false;


        }
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");



        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift) && movement.sqrMagnitude > 0)
        {

            moveSpeed = 15f;

            PlayerAnim.SetFloat("Speed", moveSpeed);
        }
        else if (movement.sqrMagnitude > 0)
        {

            PlayerAnim.SetFloat("Speed", moveSpeed);
        }
        else
        {
            PlayerAnim.SetFloat("Speed", 0f);

        }

        // start hit only if not already playing
        if (Input.GetButtonDown("Fire1") && !isAttacking)
        {
            sword0001.gameObject.SetActive(false);
            StartCoroutine(PlayHitAndReturn());

        }

        // start damage only if not already playing
        if (Input.GetKeyDown("o") && !isDamaged)
        {
            StartCoroutine(PlayDamageAndReturn());
        }


    }

    IEnumerator PlayHitAndReturn()
    {
        isAttacking = true;
        PlayerAnim.SetTrigger("Hit");

        // simplified: use cached length or inspector override or fallback
        float waitTime = hitAnimationDuration > 0f
            ? hitAnimationDuration
            : (clipLengths.TryGetValue("Hit", out var t) ? t : 0.5f);

        yield return new WaitForSeconds(waitTime);

        // return to movement/idle immediately
        PlayerAnim.ResetTrigger("Hit");
        sword0001.gameObject.SetActive(true);
        string idleState = "Player1_Idle";
        string walkState = "Player1_Walking";

        if (movement.sqrMagnitude > 0f)
            PlayerAnim.CrossFade(walkState, 0.05f);
        else
            PlayerAnim.CrossFade(idleState, 0.05f);

        PlayerAnim.SetFloat("Speed", movement.sqrMagnitude > 0 ? moveSpeed : 0f);
        isAttacking = false;
    }

    IEnumerator PlayDamageAndReturn()
    {
        isDamaged = true;
        PlayerAnim.SetTrigger("Damage");

        float waitTime = damageAnimationDuration > 0f
            ? damageAnimationDuration
            : (clipLengths.TryGetValue("Damage", out var t) ? t : 0.5f);

        yield return new WaitForSeconds(waitTime);

        PlayerAnim.ResetTrigger("Damage");

        string idleState = "Player1_Idle";
        string walkState = "Player1_Walking";

        if (movement.sqrMagnitude > 0f)
            PlayerAnim.CrossFade(walkState, 0.05f);
        else
            PlayerAnim.CrossFade(idleState, 0.05f);

        PlayerAnim.SetFloat("Speed", movement.sqrMagnitude > 0 ? moveSpeed : 0f);
        isDamaged = false;
    }

    void FixedUpdate()
    {
        //movement

        playerRb.MovePosition(playerRb.position + movement * moveSpeed * Time.fixedDeltaTime);

    }
}
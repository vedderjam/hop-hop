using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float upForce = 200f;

    private bool isDead = true;
    private Rigidbody2D rb2d;
    private Animator anim;
    private Vector3 initialPosition;

    #region Unity Callbacks
    
    private void Awake()
    {
        EventBroker.StartPlaying += StartPlaying;
        EventBroker.StartIdling += ResetBird;
        UpdateManager.UpdateEvent += UpdateEvent;
    }
    void Start()
    {
        initialPosition = new Vector3(-1.5f, 0f, 0f);
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb2d.bodyType = RigidbodyType2D.Kinematic;
    }

    private void UpdateEvent()
    {
        if (!isDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Flap();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb2d.velocity = Vector2.zero;
        if(!isDead)
            EventBroker.CallGameOver();
        isDead = true;
        anim.SetBool("Die", true);
    }

    private void OnDestroy()
    {
        EventBroker.StartPlaying -= StartPlaying;
        EventBroker.StartIdling -= ResetBird;
        UpdateManager.UpdateEvent -= UpdateEvent;
    }

    private void OnDrawGizmos()
    {
        if(rb2d != null)
            Gizmos.DrawLine(transform.position, rb2d.velocity);
    }
    
    #endregion

    #region Events
    
    private void StartPlaying()
    {
        isDead = false;
        ResetBird();
        rb2d.bodyType = RigidbodyType2D.Dynamic;
        Flap();
    }
    
    #endregion

    #region Methods
    
    private void Flap()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(0f, upForce));
        anim.SetTrigger("Flap");
    }

    private void ResetBird()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.Sleep();
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.WakeUp();
        anim.SetBool("Die", false);
    }

    #endregion
}

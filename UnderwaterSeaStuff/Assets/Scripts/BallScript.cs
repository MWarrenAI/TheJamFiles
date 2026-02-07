using UnityEngine;

public class BallScript : MonoBehaviour
{
    public GameObject e_2;
    private bool playerInRange;

    void Start()
    {
        if (e_2 != null) e_2.SetActive(false);
    }

    void Update()
    {
        // 1. CONSTANT CHECK FOR PROMPT VISIBILITY
        // If you are in range and the ball is lost, show E2. 
        // Otherwise (or if you already picked it up), hide it.
        if (e_2 != null)
        {
            bool shouldShow = playerInRange && GameState.lostBall;
            if (e_2.activeSelf != shouldShow)
            {
                e_2.SetActive(shouldShow);
            }
        }

        // 2. PICKUP LOGIC
        if (playerInRange && GameState.lostBall && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void PickUp()
    {
        GameState.hasBall = true;
        if (e_2 != null) e_2.SetActive(false);
        gameObject.SetActive(false);
        Debug.Log("Ball Picked Up!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
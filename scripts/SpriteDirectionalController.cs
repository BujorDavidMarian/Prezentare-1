using UnityEngine;

public class SpriteDirectionalController : MonoBehaviour
{
    [SerializeField] float backAngle = 65f;
    [SerializeField] float sideAngle = 155f;
    [SerializeField] Transform mainTransform;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator.speed = 0.3f; 
    }


    private void LateUpdate()
    {
        Vector3 camForwardVector = new Vector3(Camera.main.transform.forward.x, 0f, Camera.main.transform.forward.z).normalized;
        Vector3 toCameraVector = (Camera.main.transform.position - mainTransform.position);
        toCameraVector.y = 0f; 
        toCameraVector.Normalize();

        Debug.DrawRay(mainTransform.position, toCameraVector * 5f, Color.magenta);

        float signedAngle = Vector3.SignedAngle(mainTransform.forward, toCameraVector, Vector3.up);

        Vector2 animationDirection = new Vector2(0f, -1f); 

        float angle = Mathf.Abs(signedAngle);

        if (angle < backAngle)
        {
            animationDirection = new Vector2(0f, -1f); 
        }
        else if (angle < sideAngle)
        {
            animationDirection = new Vector2(1f, 0f); 

            spriteRenderer.flipX = signedAngle < 0;
        }
        else
        {
            animationDirection = new Vector2(0f, 1f); 
        }

        animator.SetFloat("moveX", animationDirection.x);
        animator.SetFloat("moveY", animationDirection.y);
    }
}
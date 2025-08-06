using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator m_Animator;
    [SerializeField] SpriteRenderer m_spriteRenderer;

    public float speed = 5f;
    private Vector3 target;

    void Start()
    {
        target = transform.position;
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = transform.position.z;
            target = mouseWorldPos;

            if (target.x > transform.position.x)
                m_spriteRenderer.flipX = false;
            else if (target.x < transform.position.x)
                m_spriteRenderer.flipX = true;
        }

        if (target != transform.position)
        {
            m_Animator.SetFloat("speed", 1);
        }
        else
        {
            m_Animator.SetFloat("speed", 0);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}

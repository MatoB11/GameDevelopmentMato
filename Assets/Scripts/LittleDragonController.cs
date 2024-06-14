using UnityEngine;

public class LittleDragonController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Spustite animáciu priamo, ak je len jedna
        animator.Play("FlyingAnimation");
    }

    // Ak chcete prepínať medzi animáciami, použite parametre
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Fly");
        }
    }
}

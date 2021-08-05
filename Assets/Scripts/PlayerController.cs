using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Variables"), SerializeField]
    private float speed = 2;

    [Header("Components"), SerializeField]
    private Rigidbody2D rb;

    [Header("Objects From Scene"), SerializeField]
    private GameController gameController;

    private void Start()
    {
        StartCoroutine(Move());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Finish"))
        {
            StartCoroutine(PlayerFinished());
        }
    }

    private IEnumerator PlayerFinished()
    {
        yield return new WaitForSeconds(0.2f);

        BounceEffectAnimation();

        yield return new WaitForSeconds(1);

        gameController.GenerateNewMaze();
    }

    private IEnumerator Move()
    {
        while (true)
        {
            float X = Input.GetAxis("Horizontal");
            float Y = Input.GetAxis("Vertical");

            rb.velocity = new Vector2(X * speed, Y * speed);

            yield return null;
        }
    }

    private void BounceEffectAnimation()
    {
        transform.DOMoveY(transform.position.y + 0.25f, 0.125f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOMoveY(transform.position.y - 0.25f, 0.125f).SetEase(Ease.InQuad);
        });
    }
}
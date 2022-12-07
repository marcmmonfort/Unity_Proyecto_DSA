using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target; // Almacena la posición del "Player" para saber hacia donde debe tirar el "Enemy".
    private bool skipMove;

    // Audio effects ...
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    protected override void Start()
    {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir); // Llamamos a la función AttemptMove de "MovingObject" (!).

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) // Si tenemos al "Player" y al "Enemy" en la misma columna, el enemigo se mueve verticalmente ...
        { 
            yDir = target.position.y > transform.position.y ? 1 : -1; // "1" es que nos movemos UP, mientras que "-1" es que nos movemos DOWN.

            // Antes de probar el movimiento, miramos si sería posible ...
            RaycastHit2D hit;
            bool movPossible = base.isMovementPossible(xDir, yDir); // "true" si se podría mover, "false" si no.

            if (movPossible)
            {
                AttemptMove<Player>(xDir, yDir);
            }
            else
            {
                xDir = target.position.x > transform.position.x ? 1 : -1;
                yDir = 0;
                AttemptMove<Player>(xDir, yDir);
            }
        }
        else // Si tenemos al "Player" y al "Enemy" en diferentes columnas, el enemigo se mueve horizontalmente ...
        {
            xDir = target.position.x > transform.position.x ? 1 : -1; // "1" es que nos movemos RIGHT, mientras que "-1" es que nos movemos LEFT.

            // Antes de probar el movimiento, miramos si sería posible ...
            RaycastHit2D hit;
            bool movPossible = base.isMovementPossible(xDir, yDir); // "true" si se podría mover, "false" si no.

            if (movPossible)
            {
                AttemptMove<Player>(xDir, yDir);
            }
            else
            {
                yDir = target.position.y > transform.position.y ? 1 : -1;
                xDir = 0;
                AttemptMove<Player>(xDir, yDir);
            }
        }
    }

    protected override void OnCantMove <T> (T component) // Para el caso en el que el "Enemy" se quiera mover al sitio donde está el "Player".
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        hitPlayer.LoseFood(playerDamage);

        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}

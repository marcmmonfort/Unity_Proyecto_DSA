using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f; // Tiempo que nuestro objeto va a tardar en moverse.
    public LayerMask blockingLayer; // Layer en la que comprobaremos las colisiones.

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime; // Más eficiente computacionalmente.
    }

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit) // Función para movernos.
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) // Si no chocamos con nada ...
        {
            StartCoroutine(SmoothMovement(end));
            return true; // Para indicar que hemos sido capaces de movernos porque no ha habido ninguna colisión.
        }
        return false; // Para indicar que el movimiento no se ha podido hacer porque hemos chocado con algo.
    }

    protected IEnumerator SmoothMovement (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component // T is the type of rival we are colliding to (Enemy or Player).
    {
        RaycastHit2D hit;

        bool canMove = Move(xDir, yDir, out hit);

        // Si no nos golpeamos con nadie ...
        if (hit.transform == null) 
        {
            return;
        }

        // Nos golpeamos con el rival ...
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    // Función para mejorar la IA de los enemigos ...

    protected bool isMovementPossible(int xDir, int yDir) // Función para ver si nos podríamos mover.
    {
        RaycastHit2D hit;

        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        /*
        hit = Physics2D.Linecast(transform.position, treeSight.position, 1 << LayerMask.NameToLayer("Tree"))) {

            if (hit.collider != null)
                Destroy(hit.collider.gameObject);
        */

        if (hit.transform == null) // Si no chocamos con nada ...
        {
            return true; // Para indicar que SÍ nos podemos mover.
        }
        else if (hit.collider.gameObject.name == "Player") // Si nos chocamos con un Enemigo / Player ...
        {
            return true;
        }
        return false; // En caso de chocar con una pared / obstáculo, aquí es cuando habrá que buscar alternativas.
    }
}

using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class MovementBehaviour : MonoBehaviour
{
    public abstract void Move(ShapeScript shapeScript);
    public abstract void SetStartDirection(ShapeScript shapeScript);
}



public class LinearMovement : MovementBehaviour
{
    public override void Move(ShapeScript shapeScript)
    {
        // Basic linear movement logic
        //shapeScript.transform.position = Vector3.MoveTowards(
        //    shapeScript.transform.position,
        //    shapeScript.TargetPos,
        //    shapeScript.MoveSpeed * Time.deltaTime
        //);

        if (Input.GetKeyDown(shapeScript.Left) && transform.position.x > shapeScript.Generals.leftBound)
        {

            shapeScript.Movement += Vector3.left * shapeScript.PlayerSpeed; shapeScript.PlaySound();
            shapeScript.animator.SetTrigger("Move");

        }
        if (Input.GetKeyDown(shapeScript.Right) && transform.position.x < shapeScript.Generals.rightBound)
        {
            shapeScript.Movement += Vector3.right * shapeScript.PlayerSpeed; shapeScript.PlaySound();
            shapeScript.animator.SetTrigger("Move");


        }
        if (Input.GetKeyDown(shapeScript.Down) && transform.position.y > shapeScript.Generals.LowBound)
        {
            shapeScript.Movement += Vector3.down * shapeScript.PlayerSpeed; shapeScript.PlaySound();
            shapeScript.animator.SetTrigger("Move");


        }
        if (Input.GetKeyDown(shapeScript.Up) && transform.position.y < shapeScript.Generals.UpBound)
        {
            shapeScript.Movement += Vector3.up * shapeScript.PlayerSpeed; shapeScript.PlaySound();
            shapeScript.animator.SetTrigger("Move");


        }

        if (shapeScript.Movement != Vector3.zero && !shapeScript.isMoving)
        {
            shapeScript.TargetPos = transform.position + shapeScript.Movement; // Set the new target position
            shapeScript.isMoving = true;
        }

        //if (transform.position != targetPos)
        if (shapeScript.isMoving)
        {
            Vector3 originalPos = transform.position;
            transform.position = Vector3.MoveTowards(originalPos, shapeScript.TargetPos, shapeScript.MoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, shapeScript.TargetPos) < 0.1f)
            {
                transform.position = shapeScript.TargetPos;
                shapeScript.isMoving = false;
                shapeScript.animator.SetBool("Moving", false);
            }
        }
    }
    public override void SetStartDirection(ShapeScript shapeScript) { }

}
public class RollingMovement : MovementBehaviour
{

    public override void Move(ShapeScript shapeScript)
    {
        //// Basic linear movement logic
        //shapeScript.transform.position = Vector3.MoveTowards(
        //    shapeScript.transform.position,
        //    shapeScript.TargetPos,
        //    shapeScript.MoveSpeed * Time.deltaTime
        //);
        Rigidbody2D rb = shapeScript.GetComponent<Rigidbody2D>();
        Vector2 DesiredDirection = Vector2.zero;

        if (Input.GetKey(shapeScript.Left))
        {
            DesiredDirection += Vector2.left;
        }
        if (Input.GetKey(shapeScript.Right))
        {
            DesiredDirection += Vector2.right;
        }
        if (Input.GetKey(shapeScript.Up))
        {
            DesiredDirection += Vector2.up;
        }
        if (Input.GetKey(shapeScript.Down))
        {
            DesiredDirection += Vector2.down;
        }

        rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -10, 10), Mathf.Clamp(rb.linearVelocity.y, -10, 10));
        Vector2 totalForce = Vector2.zero;
        float attractiondistance = 3;

        for (int i = 0; i < shapeScript.Generals.ComponentsInPlay.Count; i++)
        {
            GameObject target = shapeScript.Generals.ComponentsInPlay[i];

            if (target != null)
            {
                if (target.GetComponent<HolesScript>() != null)
                {
                    if (target.GetComponent<HolesScript>().ShapeType == shapeScript.ShapeType)
                    {
                        float DistanceToTarget = Vector2.Distance(target.transform.position, shapeScript.transform.position);

                        if (DistanceToTarget < attractiondistance)
                        {
                            Vector2 DirectionTo = (target.transform.position - shapeScript.transform.position).normalized;
                            float forceMagnitude = (50 * (1 - (DistanceToTarget / attractiondistance)));

                            totalForce += DirectionTo * forceMagnitude;
                        }
                    }
                }
            }
        }
        if (totalForce != Vector2.zero)
        {
            rb.AddForce(totalForce * Time.deltaTime * 300);
        }

        rb.AddForce(DesiredDirection * Time.deltaTime * shapeScript.PlayerSpeed);
    }
    public override void SetStartDirection(ShapeScript shapeScript) { }

}


public class SnakeMovement : MovementBehaviour
{
    public Vector2 DesiredDirection;
    Vector2 CurrentDirection;
    bool[] Directions;
    bool isMoving;
    Vector2 lastPlayedTile = Vector2.positiveInfinity;

    private void Start()
    {
        CurrentDirection = Vector2.up;
    }

    public override void Move(ShapeScript shapeScript)
    {
        //// Basic linear movement logic
        //shapeScript.transform.position = Vector3.MoveTowards(
        //    shapeScript.transform.position,
        //    shapeScript.TargetPos,
        //    shapeScript.MoveSpeed * Time.deltaTime
        //);

        

        if (Input.GetKeyDown(shapeScript.Left) && CurrentDirection != Vector2.right && shapeScript.transform.position.x > shapeScript.Generals.leftBound)
        {
            SnapToGrid(shapeScript);
            DesiredDirection.x = shapeScript.Generals.leftBound;
            DesiredDirection.y = shapeScript.transform.position.y;
            
            CurrentDirection = Vector2.left;
            isMoving = true;

        }
        if (Input.GetKeyDown(shapeScript.Right) && CurrentDirection != Vector2.left && shapeScript.transform.position.x < shapeScript.Generals.rightBound)
        {
            SnapToGrid(shapeScript);
            DesiredDirection.x = shapeScript.Generals.rightBound;
            DesiredDirection.y = shapeScript.transform.position.y;
            CurrentDirection = Vector2.right;
            isMoving = true;

        }
        if (Input.GetKeyDown(shapeScript.Up) && CurrentDirection != Vector2.down && shapeScript.transform.position.y < shapeScript.Generals.UpBound)
        {
            SnapToGrid(shapeScript);

            DesiredDirection.x = shapeScript.transform.position.x;
            DesiredDirection.y = shapeScript.Generals.UpBound;
            CurrentDirection = Vector2.up;
            isMoving = true;

        }
        if (Input.GetKeyDown(shapeScript.Down) && CurrentDirection != Vector2.up && shapeScript.transform.position.y > shapeScript.Generals.LowBound)
        {
            SnapToGrid(shapeScript);

            DesiredDirection.x = shapeScript.transform.position.x;
            DesiredDirection.y = shapeScript.Generals.LowBound;
            CurrentDirection = Vector2.down;
            isMoving = true;
        }

        if (Vector2.Distance(DesiredDirection,MakeVector2(shapeScript.transform.position)) < 0.1f) isMoving = false;
        shapeScript.transform.position = Vector3.MoveTowards(shapeScript.transform.position, DesiredDirection, 15 * Time.deltaTime);
        Vector2 directionToTarget = DesiredDirection - (Vector2)shapeScript.transform.position; // Difference in 2D
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg; // Convert to degrees

        // Smoothly rotate towards the target direction using Quaternion.Lerp (for smooth rotation)
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
        shapeScript.transform.rotation = Quaternion.Lerp(shapeScript.transform.rotation, targetRotation, Time.deltaTime * 20f);

        if (isMoving)
        {
            Vector2 currentTile = new Vector2(
          Mathf.Round(shapeScript.transform.position.x),
          Mathf.Round(shapeScript.transform.position.y)
      );
            float epsilon = 0.05f;
            // Check if we're exactly on a new tile and haven't played the sound for this tile yet
            if (Mathf.Abs(shapeScript.transform.position.x % 1) < epsilon &&
        Mathf.Abs(shapeScript.transform.position.y % 1) < epsilon &&
                currentTile != lastPlayedTile)
            {
                //shapeScript.PlaySound();
                lastPlayedTile = currentTile; // Update last played tile
            }
        }
    }

    void SnapToGrid(ShapeScript shapeScript)
    {
        Vector2 position = shapeScript.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        shapeScript.transform.position = position;
            
    }

    public override void SetStartDirection(ShapeScript shapeScript)
    {
        DesiredDirection.x = shapeScript.transform.position.x;
        DesiredDirection.y = 7;
    }
    Vector2 MakeVector2(Vector3 vector)
    {
        Vector2 newVector;
        newVector.x = vector.x;
        newVector.y = vector.y;
        return newVector;
    }
}
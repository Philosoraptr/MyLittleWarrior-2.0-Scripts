using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {
    public GameObject gameController;
    public LayerMask collisionMask;

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    public bool enemyHitBool;
    public bool exitHitBool;

    void Start() {
        boxCollider = GetComponent<BoxCollider2D> ();
        CalculateRaySpacing ();
    }

    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins ();
        collisions.Reset ();

        if (velocity.x != 0) {
            HorizontalCollisions (ref velocity);
        }
        if (velocity.y != 0) {
            VerticalCollisions (ref velocity);
        }

        transform.Translate (velocity);
    }

    void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = Mathf.Sign (velocity.x);
        float rayLength = Mathf.Abs (velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i ++) {
            Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

            if (hit) {
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;
                Debug.Log(hit.collider.tag);
                switch(hit.collider.tag){
                    case "Exit":
                        hit.collider.gameObject.GetComponent<Warp>().WarpToTarget(gameObject);
                    // This is to avoid multiple calls
                        if(!exitHitBool){
                            Debug.Log("exitHit");
                            exitHitBool = true;
                            StartCoroutine(ResetHitBools(2f)); //reset the exitHit boolean, so it can be used again
                            gameController.GetComponent<EventCollisions>().ExitCollision();
                        }
                        break;
                    case "Enemy":
                        collisions.enemy = true;
                    // This is to avoid multiple calls
                        if(!enemyHitBool){
                            enemyHitBool = true;
                            StartCoroutine(ResetHitBools(2f)); //reset the enemyHit boolean, so it can be used again
                            gameController.GetComponent<EventCollisions>().EnemyCollision(hit.collider.gameObject, gameObject);
                        }
                        break;
                    case "Chest":
                        gameController.GetComponent<EventCollisions>().ChestCollision(hit.collider.gameObject, gameObject);
                        //Player should do a celebration animation here
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign (velocity.y);
        float rayLength = Mathf.Abs (velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i ++) {
            Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

            if (hit) {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    void UpdateRaycastOrigins() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand (skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing() {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand (skinWidth * -2);

        horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    void ChangeLevel(int index){
        SceneManager.LoadScene(index);
    }

    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;
        public bool enemy;

        public void Reset() {
            above = below = false;
            left = right = false;
            enemy = false;
        }
    }

    public IEnumerator ResetHitBools(float seconds) {
        yield return new WaitForSeconds(seconds);
        enemyHitBool = false;
        exitHitBool = false;
    }
}
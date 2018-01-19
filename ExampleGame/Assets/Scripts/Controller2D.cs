using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController {

  float maxClimbAngle = 80f;
  float maxDescendAngle = 75f;

  public CollisionInfo collisions;

  public Vector2 playerInput;

  public override void Start() {
    base.Start();
  }

  public void Move(Vector2 displacement, bool standingOnPlatform) {
    Move(displacement, Vector2.zero, standingOnPlatform);
  }

  public void Move(Vector2 displacement, Vector2 input, bool standingOnPlatform = false) {
    UpdateRaycastOrigins();
    collisions.Reset();
    collisions.displacementOld = displacement;
    playerInput = input;

    if (displacement.y < 0) {
      DescendSlope(ref displacement);
    }
    if (displacement.x != 0) {
      HorizontalCollisions(ref displacement);
    }
    if (displacement.y != 0) {
      VerticalCollisions(ref displacement);
    }

    transform.Translate(displacement);

    if (standingOnPlatform) {
      collisions.below = true;
    }
  }

  private void HorizontalCollisions(ref Vector2 displacement) {
    float directionX = Mathf.Sign(displacement.x);
    float rayLength = Mathf.Abs(displacement.x) + skinWidth;
    for (int i = 0; i < horizontalRayCount; i++) {
      Vector2 rayOrigin = (directionX == -1) ? rayOrigin = raycastOrigins.bottomLeft : rayOrigin = raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
      Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.blue);

      if (hit) {
        if (hit.distance == 0) {
          continue;
        }

        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (i == 0 && slopeAngle <= maxClimbAngle) {
          if (collisions.descendingSlope) {
            collisions.descendingSlope = false;
            displacement = collisions.displacementOld;
          }
          float distanceToSlopeStart = 0;
          if (slopeAngle != collisions.slopeAngleOld) {
            distanceToSlopeStart = hit.distance - skinWidth;
            displacement.x -= distanceToSlopeStart * directionX;
          }
          ClimbSlope(ref displacement, slopeAngle);
          displacement.x += distanceToSlopeStart * directionX;
        }

        if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
          displacement.x = (hit.distance - skinWidth) * directionX;
          rayLength = hit.distance;

          if (collisions.climbingSlope) {
            displacement.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(displacement.x);
          }

          collisions.left = directionX == -1;
          collisions.right = directionX == 1;
        }
      }
    }
  }

  private void VerticalCollisions(ref Vector2 displacement) {
    float directionY = Mathf.Sign(displacement.y);
    float rayLength = Mathf.Abs(displacement.y) + skinWidth;
    for (int i = 0; i < verticalRayCount; i++) {
      Vector2 rayOrigin = (directionY == -1) ? rayOrigin = raycastOrigins.bottomLeft : rayOrigin = raycastOrigins.topLeft;
      rayOrigin += Vector2.right * (verticalRaySpacing * i + displacement.x);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);
      Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.blue);

      if (hit) {
        if (hit.collider.tag == "Platform") {
          if (directionY == 1 || hit.distance == 0) {
            continue;
          }
          if (collisions.fallingThroughPlatform) {
            continue;
          }
          if (playerInput.y == -1) {
            collisions.fallingThroughPlatform = true;
            Invoke("ResetFallingThroughPlatform", 0.5f);
            continue;
          }
        }

        displacement.y = (hit.distance - skinWidth) * directionY;
        rayLength = hit.distance;

        if (collisions.climbingSlope) {
          displacement.x = displacement.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(displacement.x);
        }

        collisions.below = directionY == -1;
        collisions.above = directionY == 1;
      }
    }

    if (collisions.climbingSlope) {
      float directionX = Mathf.Sign(displacement.x);
      rayLength = Mathf.Abs(displacement.x) + skinWidth;
      Vector2 rayOrigin = ((directionX == -1) ? rayOrigin = raycastOrigins.bottomLeft : rayOrigin = raycastOrigins.bottomRight) + Vector2.up * displacement.y;
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
      
      if (hit) {
        float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
        if (slopeAngle != collisions.slopeAngle) {
          displacement.x = (hit.distance - skinWidth) * directionX;
          collisions.slopeAngle = slopeAngle;
        }
      }
    }
  }

  private void ClimbSlope(ref Vector2 displacement, float slopeAngle) {
    float moveDistance = Mathf.Abs(displacement.x);
    float climbdisplacementY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

    if (displacement.y <= climbdisplacementY) {
      displacement.y = climbdisplacementY;
      displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(displacement.x);
      collisions.below = true;
      collisions.climbingSlope = true;
      collisions.slopeAngle = slopeAngle;
    }
  }

  private void DescendSlope(ref Vector2 displacement) {
    float directionX = Mathf.Sign(displacement.x);
    Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

    if (hit) {
      float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
      if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
        if (Mathf.Sign(hit.normal.x) == directionX) {
          if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(displacement.x)) {
            float moveDistance = Mathf.Abs(displacement.x);
            float descenddisplacementY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
            displacement.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * directionX;
            displacement.y -= descenddisplacementY;

            collisions.slopeAngle = slopeAngle;
            collisions.descendingSlope = true;
            collisions.below = true;
          }
        }
      }
    }
  }

  void ResetFallingThroughPlatform() {
    collisions.fallingThroughPlatform = false;
  }

  public struct CollisionInfo {
    public bool above, below;
    public bool left, right;

    public bool climbingSlope, descendingSlope;
    public float slopeAngle, slopeAngleOld;
    public Vector2 displacementOld;

    public bool fallingThroughPlatform;

    public void Reset() {
      above = below = false;
      left = right = false;
      climbingSlope = descendingSlope = false;
      slopeAngleOld = slopeAngle;
      slopeAngle = 0;
    }
  }
}

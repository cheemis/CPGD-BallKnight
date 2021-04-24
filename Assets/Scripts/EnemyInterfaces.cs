using UnityEngine;
public interface Enemy
{
    void attack();
    void follow(Vector3 player_position);
    void wander(Vector3 current_dest);

}

using UnityEngine;
public interface Enemy
{
    void attack(Vector3 player_position);
    void follow(Vector3 player_position);
    void wander(Vector3 current_dest);
    void switchPatrolMode(short old_mode, short new_mode);

}

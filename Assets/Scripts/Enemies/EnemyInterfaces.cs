using UnityEngine;
public interface Enemy
{
    void Attack(Vector3 player_position);
    void Follow(Vector3 player_position);
    void Wander(Vector3 current_dest);
    void SwitchPatrolMode(short old_mode, short new_mode);

    float OnCollision(Vector3 other_position, Vector3 other_velocity);

}

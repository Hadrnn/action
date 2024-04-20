using UnityEngine;
using System;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Collections.Generic;



public class BotTankMovement : BotMovement
{
    public int target_radius;
    public int Astar_deep;
    protected override void Decision()
    {
        Transform Enemy = FindClosestEnemy(teamNumber, transform, collector);
        if (Enemy.Equals(transform)) {
            return;
        }   

        Vector3 EnemyPosition = Enemy.position;
        Vector3 MyPosition = transform.position;
        Vector3 DeltaPosition = MyPosition - EnemyPosition;
        float Length = DeltaPosition.magnitude;
        if (!Gun.onReload)
        {
            GameState Start = new GameState(0, discret, target_radius);
            
            Start.position = transform.position;
            Start.forward = transform.forward;
            Start.forward_multiplyer = forvard_multiplyer;
            Start.TargetPosition = EnemyPosition;
            Vector3 dist = Start.TargetPosition - Start.position;
            Start.distance_to_finish = dist.magnitude;
            Start.distance_to_start = 0;
            Start.ourRigidbody = GetComponent<Rigidbody>();
            Start.hitbox = GetComponent<BoxCollider>();
            Vector3 decision = AStar(Start, Astar_deep);
            if (decision.x == 0 & decision.z == 0)
            {
                List<Vector2> numbers = new List<Vector2> { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };
                System.Random rd = new System.Random();
                int randomIndex = rd.Next(0, 8);
                Vector2 randomNumber = numbers[randomIndex];
                decision.x = randomNumber.x;
                decision.z = randomNumber.y;
            }
            m_HorizontalInputValue = decision.x;
            m_VerticalInputValue = decision.z;
        }
        else
        {
            float current_distanse_to_my_cover;
            float distanse_from_enemy_to_cover;
            float min_summ = 10000;
            int cover_index = -1;

            for (int i = 0; i < collector.mapObjects.Count; i++)
            {

                current_distanse_to_my_cover = (collector.mapObjects[i].transform.position - MyPosition).magnitude;
                distanse_from_enemy_to_cover = (collector.mapObjects[i].transform.position - EnemyPosition).magnitude;
                if (current_distanse_to_my_cover < min_summ & distanse_from_enemy_to_cover < 45)
                {
                    min_summ = current_distanse_to_my_cover;
                    cover_index = i;
                }
            }
            if (cover_index < 0)
            {
                return;
            }
            Vector3 cover_position = collector.mapObjects[cover_index].transform.position;

            Vector3 connect_line = cover_position - EnemyPosition;

            Vector3 additional_element = new Vector3(20, 0, 20);

            connect_line = connect_line.normalized;

            float mult_x = connect_line.x * additional_element.x;
            float mult_z = connect_line.z * additional_element.z;
            Vector3 mult = new Vector3(mult_x, 0, mult_z);

            Vector3 TargetPosition = cover_position + mult;

            GameState Start = new GameState(0, discret, target_radius);
            Start.position = transform.position;
            Start.forward = transform.forward;
            Start.forward_multiplyer = forvard_multiplyer;
            Start.TargetPosition = TargetPosition;
            Vector3 dist = Start.TargetPosition - Start.position;
            Start.distance_to_finish = dist.magnitude;
            Start.distance_to_start = 0;
            Start.ourRigidbody = GetComponent<Rigidbody>();
            Start.hitbox = GetComponent<BoxCollider>();
            Vector3 decision = AStar(Start, Astar_deep);
            if (decision.x == 0 & decision.z == 0)
            {
                List<Vector2> numbers = new List<Vector2> { new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1) };
                System.Random rd = new System.Random();
                int randomIndex = rd.Next(0, 8);
                Vector2 randomNumber = numbers[randomIndex];
                decision.x = randomNumber.x;
                decision.z = randomNumber.y;
            }
            m_HorizontalInputValue = decision.x;
            m_VerticalInputValue = decision.z;
            
        }
    }
    protected override void Move()
    {
        if (System.Math.Abs(m_HorizontalInputValue) < 0.05f && System.Math.Abs(m_VerticalInputValue) < 0.05f)
        {
            return;
        }
        double control_angle = (System.Math.Acos((0 * m_VerticalInputValue + 1 * m_HorizontalInputValue) /
            ((System.Math.Sqrt(m_HorizontalInputValue * m_HorizontalInputValue + m_VerticalInputValue * m_VerticalInputValue))))) * 57.3;
        double forward_angle = (System.Math.Acos((transform.forward.x * forvard_multiplyer * 1 + transform.forward.z * 0 * forvard_multiplyer) /
            ((System.Math.Sqrt(transform.forward.x * transform.forward.x + transform.forward.z * transform.forward.z))))) * 57.3;
        double delta_angle = 0;
        if (m_VerticalInputValue < 0)
        {
            control_angle = -control_angle;
        }
        if (transform.forward.z * forvard_multiplyer < 0)
        {
            forward_angle = -forward_angle;
        }
        delta_angle = control_angle - forward_angle;
        if (delta_angle < -180)
        {
            delta_angle = 360 + delta_angle;
        }
        else if (delta_angle > 180)
        {
            delta_angle = -(360 - delta_angle);
        }
        if (System.Math.Abs(delta_angle) < 3)
        {
            delta_angle = 0;
        }
        if (delta_angle > 95)
        {
            forvard_multiplyer = -1 * forvard_multiplyer;
            delta_angle = (180 - delta_angle) * forvard_multiplyer;
        }
        else if (delta_angle < -95)
        {
            forvard_multiplyer = -1 * forvard_multiplyer;
            delta_angle = (180 + delta_angle) * forvard_multiplyer;
        }
        float turn;
        if (System.Math.Abs(delta_angle) < 3)
        {
            turn = 0;
        }
        else
        {
            turn = -(float)(System.Math.Sign(delta_angle) * Time.deltaTime * m_TurnSpeed);
        }
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        Vector3 movement = new Vector3(0f, 0f, 0f);
        if ((m_VerticalInputValue != 0) || (m_HorizontalInputValue != 0))
        {
            movement = transform.forward * m_Speed * Time.deltaTime * forvard_multiplyer;
        }

        Collider[] collisionArray = Physics.OverlapBox(m_Rigidbody.position + movement, m_Collider.size / 2, m_Rigidbody.rotation, ~0, QueryTriggerInteraction.Ignore);

        if (collisionArray.Length == 1)
        {
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
        }
        else
        {
            counter = discret;
        }
    }
}

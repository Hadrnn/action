using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Collections.Generic;


public class BotHeavyTankMovement : BotMovement
{
    public int target_radius;
    protected override void Decision()
    {
        Transform Enemy = FindClosestEnemy(teamNumber, transform, collector);
        if (Enemy.Equals(transform))
        {
            return;
        }

        Vector3 EnemyPosition = Enemy.position;

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
        Vector3 decision = AStar(Start);
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

        //m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
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

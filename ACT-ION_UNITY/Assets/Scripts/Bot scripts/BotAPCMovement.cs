using UnityEngine;
using System;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using TMPro;


public class BotAPCMovement : BotMovement
{
    public int target_radius;
    public int Astar_deep;
    public bool was_change_radius_on_last_3_iterration = false;
    public int radius_of_search;
    public int iterration_counter = 0;
    public int max_value_iterration_counter = 3;
    protected override void Decision()
    {
        Vector3 MyPosition = transform.position;

        if ((GameSingleton.GetInstance().currentGameMode != GameSingleton.GameMode.Domination) & (GameSingleton.GetInstance().currentGameMode != GameSingleton.GameMode.CaptureTheFlag))
        {
            Transform Enemy = FindClosestEnemy(teamNumber, transform, collector);
            if (Enemy.Equals(transform))
            {
                return;
            }
            Vector3 EnemyPosition = Enemy.position;

            Vector3 TargetPosition;

            TargetPosition = EnemyPosition;


            if (!Gun.onReload)
            {
                float length = (TargetPosition - MyPosition).magnitude;

                if (!was_change_radius_on_last_3_iterration)
                {
                    if (length > 10 && length < 90)
                    {
                        radius_of_search = (int)(length * 0.75f);
                    }
                    else if (length > 90)
                    {
                        radius_of_search = (int)(length - 20f);
                    }
                    else
                    {
                        radius_of_search = 4;
                    }
                    iterration_counter = 0;
                }
                else
                {
                    if (iterration_counter == max_value_iterration_counter)
                    {
                        was_change_radius_on_last_3_iterration = false;
                    }
                    else
                    {
                        iterration_counter++;
                    }
                }

                GameState Start = new GameState(GetComponent<Rigidbody>(), Enemy.GetComponent<Rigidbody>(), GetComponent<BoxCollider>(), transform.position,
                    transform.forward, forvard_multiplyer, m_TurnSpeed, m_Speed, radius_of_search, TargetPosition, new Vector3(0, 0, 0),
                    (TargetPosition - transform.position).magnitude, 0, 0, discret);

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
                    distanse_from_enemy_to_cover = (collector.mapObjects[i].transform.position - TargetPosition).magnitude;
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

                Vector3 connect_line = cover_position - TargetPosition;

                Vector3 additional_element = new Vector3(20, 0, 20);

                connect_line = connect_line.normalized;

                float mult_x = connect_line.x * additional_element.x;
                float mult_z = connect_line.z * additional_element.z;
                Vector3 mult = new Vector3(mult_x, 0, mult_z);

                Vector3 RecalculateTargetPosition = cover_position + mult;

                float length = (RecalculateTargetPosition - MyPosition).magnitude;


                if (!was_change_radius_on_last_3_iterration)
                {
                    if (length > 10 && length < 90)
                    {
                        radius_of_search = (int)(length * 0.75f);
                    }
                    else if (length > 90)
                    {
                        radius_of_search = (int)(length - 20f);
                    }
                    else
                    {
                        radius_of_search = 4;
                    }
                    iterration_counter = 0;
                }
                else
                {
                    if (iterration_counter == max_value_iterration_counter)
                    {
                        was_change_radius_on_last_3_iterration = false;
                    }
                    else
                    {
                        iterration_counter++;
                    }
                }

                GameState Start = new GameState(GetComponent<Rigidbody>(), Enemy.GetComponent<Rigidbody>(), GetComponent<BoxCollider>(), transform.position,
                    transform.forward, forvard_multiplyer, m_TurnSpeed, m_Speed, radius_of_search, RecalculateTargetPosition, new Vector3(0, 0, 0),
                    (RecalculateTargetPosition - transform.position).magnitude, 0, 0, discret);
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
        else if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.Domination)
        {
            Transform Base = collector.objectives[0].transform;

            Vector3 BasePosition = Base.position;

            float length = (transform.position - BasePosition).magnitude;

            if (!was_change_radius_on_last_3_iterration)
            {
                if (length > 10 && length < 90)
                {
                    radius_of_search = (int)(length * 0.75f);
                }
                else if (length > 90)
                {
                    radius_of_search = (int)(length - 20f);
                }
                else
                {
                    radius_of_search = 4;
                }
                iterration_counter = 0;
            }
            else
            {
                if (iterration_counter == max_value_iterration_counter)
                {
                    was_change_radius_on_last_3_iterration = false;
                }
                else
                {
                    iterration_counter++;
                }
            }


            GameState Start = new GameState(GetComponent<Rigidbody>(), GetComponent<Rigidbody>(), GetComponent<BoxCollider>(), transform.position,
                transform.forward, forvard_multiplyer, m_TurnSpeed, m_Speed, radius_of_search, BasePosition, new Vector3(0, 0, 0),
                (BasePosition - transform.position).magnitude, 0, 0, discret);

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
        else if (GameSingleton.GetInstance().currentGameMode == GameSingleton.GameMode.CaptureTheFlag)
        {
            Transform EnemyFlag = null;
            Transform MyFlag = null;
            Transform EnemyBase = null;
            Transform MyBase = null;

            for (int i = 0; i < collector.objectives.Count; i++)
            {
                if (collector.objectives[i].GetComponent<FlagBase>() != null)
                {
                    if (collector.objectives[i].GetComponent<FlagBase>().teamNumber == teamNumber)
                    {
                        MyBase = collector.objectives[i].GetComponent<FlagBase>().transform;
                    }
                    else
                    {
                        EnemyBase = collector.objectives[i].GetComponent<FlagBase>().transform;
                    }
                }
                if (collector.objectives[i].GetComponent<FlagCapture>() != null)
                {
                    if (collector.objectives[i].GetComponent<FlagCapture>().teamNumber == teamNumber)
                    {
                        MyFlag = collector.objectives[i].GetComponent<FlagCapture>().transform;
                    }
                    else
                    {
                        EnemyFlag = collector.objectives[i].GetComponent<FlagCapture>().transform;
                    }
                }
            }

            Vector3 TargetPosition = Vector3.zero;

            float dist_from_me_to_en_flag = (EnemyFlag.position - transform.position).magnitude;
            float dist_from_me_to_my_flag = (MyFlag.position - transform.position).magnitude;
            float dist_from_me_to_my_base = (MyBase.position - transform.position).magnitude;
            float dist_from_my_flag_to_my_base = (MyBase.position - MyFlag.position).magnitude;
            float dist_from_en_flag_to_en_base = (EnemyFlag.position - EnemyBase.position).magnitude;

            if ((dist_from_en_flag_to_en_base <= 3) & (dist_from_my_flag_to_my_base <= 3))
            {
                TargetPosition = EnemyFlag.position;
            }
            else if ((dist_from_me_to_en_flag <= 3) & (dist_from_my_flag_to_my_base <= 3))
            {
                TargetPosition = MyBase.position;
            }
            else if ((dist_from_me_to_en_flag <= 3) & (dist_from_my_flag_to_my_base > 3))
            {
                if (dist_from_me_to_my_base > dist_from_me_to_my_flag)
                {
                    TargetPosition = MyFlag.position;
                }
                else
                {
                    TargetPosition = MyBase.position;
                }
            }
            else if ((dist_from_me_to_en_flag > 3) & (dist_from_my_flag_to_my_base > 3))
            {
                if (dist_from_me_to_en_flag > dist_from_me_to_my_flag)
                {
                    TargetPosition = EnemyFlag.position;
                }
                else
                {
                    TargetPosition = MyFlag.position;
                }
            }
            else
            {
                m_HorizontalInputValue = 0;
                m_HorizontalInputValue = 0;
                Debug.LogWarning("Непродуманный сценарий");
                return;
            }

            float length = (TargetPosition - MyPosition).magnitude;

            if (!was_change_radius_on_last_3_iterration)
            {
                if (length > 10 && length < 90)
                {
                    radius_of_search = (int)(length * 0.75f);
                }
                else if (length > 90)
                {
                    radius_of_search = (int)(length - 20f);
                }
                else
                {
                    radius_of_search = 4;
                }
                iterration_counter = 0;
            }
            else
            {
                if (iterration_counter == max_value_iterration_counter)
                {
                    was_change_radius_on_last_3_iterration = false;
                }
                else
                {
                    iterration_counter++;
                }
            }

            GameState Start = new GameState(GetComponent<Rigidbody>(), GetComponent<Rigidbody>(), GetComponent<BoxCollider>(), transform.position,
                   transform.forward, forvard_multiplyer, m_TurnSpeed, m_Speed, radius_of_search, TargetPosition, new Vector3(0, 0, 0),
                   (TargetPosition - transform.position).magnitude, 0, 0, discret);

            Vector3 decision = AStar(Start, Astar_deep);
            if (decision.x == 0 && decision.z == 0)
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
}

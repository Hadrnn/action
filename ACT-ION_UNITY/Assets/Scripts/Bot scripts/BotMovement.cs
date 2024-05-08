﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BotMovement : TankMovement
{

    public int discret = 25;
    public int counter = 0;
    
    public double AgressiveDistance;

    protected BotShooting Gun;

    static string GameStateToString(GameState current)
    {
        string result = $"{(int)(current.position.x * 10)} {(int)(current.position.y * 10)} {(int)(current.position.z * 10)} {(int)(current.forward.x * 10)} {(int)(current.forward.y * 10)} {(int)(current.forward.z * 10)}";
        return result;
    }
    protected static Vector3 AStar(GameState start, int Astar_deep)
    {
        bool broken_out = false;
        PriorityQueue<GameState> que = new PriorityQueue<GameState>();
        Dictionary<string, GameState> visited = new Dictionary<string, GameState>();
        que.Push(start);
        visited[GameStateToString(start)] = start;
        GameState current = start;
        int i = 0;
        if (!start.IsFinish())
        {
            while (!que.empty)
            {
                if (i > Astar_deep)
                {
                    Debug.LogWarning("More than Astar Deep");
                    broken_out = true;
                    break;
                }
                i++;
                current = que.top;
                que.Pop();

                if (current.IsFinish())
                {
                    //Debug.Log(i);
                    break;
                }
                if (current.CanMoveUp())
                {
                    GameState newState = current.MoveUp();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveDown())
                {
                    GameState newState = current.MoveDown();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveRight())
                {
                    GameState newState = current.MoveRight();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveLeft())
                {
                    GameState newState = current.MoveLeft();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveRightUp())
                {
                    GameState newState = current.MoveRightUp();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveLeftUp())
                {
                    GameState newState = current.MoveLeftUp();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveRightDown())
                {
                    GameState newState = current.MoveRightDown();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (current.CanMoveLeftDown())
                {
                    GameState newState = current.MoveLeftDown();
                    if (!visited.ContainsKey(GameStateToString(newState)))
                    {
                        visited[GameStateToString(newState)] = newState;
                        que.Push(newState);
                    }
                }
                if (que.empty)
                {
/*                    Debug.LogWarning("Que is empty");*/
                    broken_out = true;
                    break;
                }
            }
            if (current.prew_state != null)
            {
                while (current.prew_state.prew_state != null)
                {
                    current = current.prew_state;
                }
            }
        }
        else
        {
            /*            que.Pop();
                        if (start.CanMoveUp())
                        {
                            GameState newState = start.MoveUp();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveDown())
                        {
                            GameState newState = start.MoveDown();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveRight())
                        {
                            GameState newState = start.MoveRight();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveLeft())
                        {
                            GameState newState = start.MoveLeft();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (current.CanMoveRightUp())
                        {
                            GameState newState = start.MoveRightUp();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveLeftUp())
                        {
                            GameState newState = start.MoveLeftUp();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveRightDown())
                        {
                            GameState newState = start.MoveRightDown();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (start.CanMoveLeftDown())
                        {
                            GameState newState = start.MoveLeftDown();
                            if (!visited.ContainsKey(GameStateToString(newState)))
                            {
                                visited[GameStateToString(newState)] = newState;
                                que.Push(newState);
                            }
                        }
                        if (!que.empty)
                        {
                            current = que.last;
                        }
                        else
                        {
                            current = start;
                        }*/
            current = start;
        }
        que = new PriorityQueue<GameState>();
        que.Push(start);
        if (broken_out)
        {
            if (start.CanMoveUp())
            {
                GameState newState = start.MoveUp();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveDown())
            {
                GameState newState = start.MoveDown();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveRight())
            {
                GameState newState = start.MoveRight();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveLeft())
            {
                GameState newState = start.MoveLeft();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (current.CanMoveRightUp())
            {
                GameState newState = start.MoveRightUp();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveLeftUp())
            {
                GameState newState = start.MoveLeftUp();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveRightDown())
            {
                GameState newState = start.MoveRightDown();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            if (start.CanMoveLeftDown())
            {
                GameState newState = start.MoveLeftDown();
                if (!visited.ContainsKey(GameStateToString(newState)))
                {
                    visited[GameStateToString(newState)] = newState;
                    que.Push(newState);
                }
            }
            current = que.top;
        }
        Vector3 result = current.move;
        return result;
    }
    public static class Utilities
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
    public enum Comparator { less = -1, equal = 0, greater = 1 }
    public class PriorityQueue<T> where T : IComparable
    {
        private int _size;
        private int capacity;
        private T[] elements;
        private Comparator comparator;

        public int size { get => _size; }
        public bool empty { get => (size == 0); }
        public T top { get => elements[0]; }
        public T last { get => elements[_size - 1]; }

        public PriorityQueue(Comparator comparator = Comparator.less, int capacity = 1)
        {
            _size = 0;
            this.capacity = capacity;
            this.comparator = comparator;
            elements = new T[capacity];
        }

        private void ShiftDown()
        {
            int cur = 0;
            int child = 1;
            while (child < size)
            {
                if (child + 1 < size && elements[child + 1].CompareTo(elements[child]) == (int)comparator)
                    child++;

                if (elements[child].CompareTo(elements[cur]) == (int)comparator)
                {
                    Utilities.Swap<T>(ref elements[child], ref elements[cur]);
                    cur = child;
                    child = 2 * cur + 1;
                }
                else break;
            }
        }

        private void ShiftUp()
        {
            int cur = size - 1;
            int parent = (cur - 1) / 2;
            while (cur > 0)
            {
                if (elements[cur].CompareTo(elements[parent]) == (int)comparator)
                {
                    Utilities.Swap<T>(ref elements[cur], ref elements[parent]);
                    cur = parent;
                    parent = (cur - 1) / 2;
                }
                else break;
            }
        }

        private void ExpandCapacity()
        {
            capacity = Mathf.CeilToInt(capacity * 1.5f);
            T[] temp = new T[capacity];
            for (int i = 0; i < size; i++)
                temp[i] = elements[i];
            elements = temp;
        }

        public void Push(T value)
        {
            if (size == capacity)
                ExpandCapacity();

            elements[_size++] = value;
            ShiftUp();
        }

        public void Pop()
        {
            if (size == 0)
                return;

            Utilities.Swap<T>(ref elements[0], ref elements[size - 1]);
            _size--;
            ShiftDown();
        }
    }
    public class GameState : IComparable
    {
        public Rigidbody ourRigidbody;
        public BoxCollider hitbox;

        public Vector3 position = Vector3.zero;
        public Vector3 forward = Vector3.zero;
        public int forward_multiplyer;

        public float m_TurnSpeed = 300f;
        public float m_Speed = 12;
        public int target_radius;
        public Vector3 TargetPosition = Vector3.zero;
        public GameState prew_state = null;
        public Vector3 move = Vector3.zero;
        public float distance_to_finish;
        public float distance_to_start;
        public int iterration_number;
        public int discret;
        public GameState(int iterration_number_in, int discret_in, int target_radius_in)
        {
            iterration_number = iterration_number_in;
            discret = discret_in;
            target_radius = target_radius_in;
        }

        public List<Vector3> shells_positions_recalculate(List<Vector3> shells_positions, List<Vector3> shells_forwards, List<float> shells_speeds)
        {
            for (int i = 0; i < shells_positions.Count; ++i)
            {
                shells_positions[i] = shells_positions[i] + shells_forwards[i]*shells_speeds[i]*Time.deltaTime;
            }
            return shells_positions;
        }

        public bool CanMove(double control_angle, Vector3 control)
        {
            List<Vector3> shells_positions = new List<Vector3>();
            List<Vector3> shells_forwards = new List<Vector3>();
            List<float> shells_speeds = new List<float>();
            Vector3 axis = new Vector3(0, 1, 0);
            for (int i = 0; i < collector.shells.Count; i++)
            {
                if (Mathf.Abs(Vector3.SignedAngle((this.position -
                    (collector.shells[i].transform.position +
                    collector.shells[i].transform.forward * iterration_number * discret * Time.deltaTime * collector.shells[i].transform.GetComponent<Rigidbody>().velocity.magnitude)),
                    collector.shells[i].transform.forward, axis)) < 90)
                {
                    shells_positions.Add(collector.shells[i].transform.position);
                    shells_forwards.Add(collector.shells[i].transform.forward);
                    shells_speeds.Add(collector.shells[i].transform.GetComponent<Rigidbody>().velocity.magnitude);
                }
            }
            


            GameState next = new GameState(iterration_number + 1, discret, target_radius);
            next.position = this.position;
            next.forward = this.forward;
            next.forward_multiplyer = this.forward_multiplyer;
            next.m_Speed = this.m_Speed;
            next.m_TurnSpeed = this.m_TurnSpeed;
            next.TargetPosition = this.TargetPosition;
            next.hitbox = this.hitbox;
            next.ourRigidbody = this.ourRigidbody;

            for (int i = 0; i < discret; i++)
            {
                double forward_angle = (System.Math.Acos((next.forward.x * next.forward_multiplyer * 1 + next.forward.z * 0 * next.forward_multiplyer) /
               ((System.Math.Sqrt(next.forward.x * next.forward.x + next.forward.z * next.forward.z))))) * 57.3;
                if (next.forward.z * next.forward_multiplyer < 0)
                {
                    forward_angle = -forward_angle;
                }
                double delta_angle = control_angle - forward_angle;
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
                    next.forward_multiplyer = -1 * next.forward_multiplyer;
                    delta_angle = (180 - delta_angle) * next.forward_multiplyer;
                }
                else if (delta_angle < -95)
                {
                    next.forward_multiplyer = -1 * next.forward_multiplyer;
                    delta_angle = (180 + delta_angle) * next.forward_multiplyer;
                }

                float turn;
                if (System.Math.Abs(delta_angle) > 3)
                {
                    turn = -(float)(System.Math.Sign(delta_angle) * Time.deltaTime * next.m_TurnSpeed); ;
                }
                else
                {
                    turn = 0;
                }

                Quaternion rotationQuaternion = Quaternion.AngleAxis(turn, Vector3.up);
                next.forward = rotationQuaternion * next.forward;

                Vector3 movement = new Vector3(0f, 0f, 0f);
                movement = next.forward * next.m_Speed * Time.deltaTime * next.forward_multiplyer;
                next.position += movement;


                Vector3 new_forward_angle = new Vector3(0, (float)(forward_angle + turn), 0);
                Vector3 new_position = next.position;
                Quaternion newRotation = Quaternion.Euler(new_forward_angle);
                Vector3 colliderSize = hitbox.size;

                Collider[] collisionArray = Physics.OverlapBox(new_position, colliderSize / 2, newRotation, ~0, QueryTriggerInteraction.Ignore);

                if (collisionArray.Length == 1 && collisionArray[0].GetComponent<Rigidbody>() == ourRigidbody)
                {
                    continue;
                }
                else if (collisionArray.Length != 0)
                {
                    return false;
                }

                shells_positions = shells_positions_recalculate(shells_positions, shells_forwards, shells_speeds);

                for (int j = 0; j < shells_positions.Count; j++)
                {
                    if ((shells_positions[j] - new_position).magnitude < (colliderSize.magnitude*1.15))
                    {
                        return false;
                    }
                }
                
            }
            return true;
        }
        public bool CanMoveUp()
        {
            Vector3 control = new Vector3(0, 0, 1);
            return CanMove(90, control);
        }
        public bool CanMoveDown()
        {
            Vector3 control = new Vector3(0, 0, -1);
            return CanMove(-90, control);
        }
        public bool CanMoveRight()
        {
            Vector3 control = new Vector3(1, 0, 0);
            return CanMove(0, control);
        }
        public bool CanMoveLeft()
        {
            Vector3 control = new Vector3(-1, 0, 0);
            return CanMove(180, control);
        }
        public bool CanMoveRightUp()
        {
            Vector3 control = new Vector3(1, 0, 1);
            return CanMove(45, control);
        }
        public bool CanMoveLeftUp()
        {
            Vector3 control = new Vector3(-1, 0, 1);
            return CanMove(135, control);
        }
        public bool CanMoveRightDown()
        {
            Vector3 control = new Vector3(1, 0, -1);
            return CanMove(-45, control);
        }
        public bool CanMoveLeftDown()
        {
            Vector3 control = new Vector3(-1, 0, -1);
            return CanMove(-135, control);
        }

        public GameState MakeMove(double control_angle, Vector3 control)
        {
            GameState next = new GameState(iterration_number + 1, discret, target_radius);
            next.position = this.position;
            next.forward = this.forward;
            next.forward_multiplyer = this.forward_multiplyer;
            next.m_Speed = this.m_Speed;
            next.m_TurnSpeed = this.m_TurnSpeed;
            next.TargetPosition = this.TargetPosition;
            next.ourRigidbody = ourRigidbody;
            next.hitbox = this.hitbox;

            for (int i = 0; i < discret; i++)
            {
                double forward_angle = (System.Math.Acos((next.forward.x * next.forward_multiplyer * 1) /
               ((System.Math.Sqrt(next.forward.x * next.forward.x + next.forward.z * next.forward.z))))) * 57.3;
                if (next.forward.z * next.forward_multiplyer < 0)
                {
                    forward_angle = -forward_angle;
                }
                double delta_angle = control_angle - forward_angle;
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
                    next.forward_multiplyer = -1 * next.forward_multiplyer;
                    delta_angle = (180 - delta_angle) * next.forward_multiplyer;
                }
                else if (delta_angle < -95)
                {
                    next.forward_multiplyer = -1 * next.forward_multiplyer;
                    delta_angle = (180 + delta_angle) * next.forward_multiplyer;
                }
                float turn;
                if (System.Math.Abs(delta_angle) > 3)
                {
                    turn = -(float)(System.Math.Sign(delta_angle) * Time.deltaTime * next.m_TurnSpeed); ;
                }
                else
                {
                    turn = 0;
                }
                Quaternion rotationQuaternion = Quaternion.AngleAxis(turn, Vector3.up);
                next.forward = rotationQuaternion * next.forward;
                Vector3 movement = next.forward * next.m_Speed * Time.deltaTime * next.forward_multiplyer;
                next.position = next.position + movement;
                if (next.IsFinish())
                {
                    break;
                }
            }
            next.prew_state = this;
            next.move = control;
            Vector3 dist = next.TargetPosition - next.position;
            next.distance_to_finish = dist.magnitude;
            Vector3 remove = next.position - this.position;
            next.distance_to_start = this.distance_to_start + remove.magnitude;
            return next;
        }
        public GameState MoveUp()
        {
            Vector3 control = new Vector3(0, 0, 1);
            GameState next = MakeMove(90, control);
            return next;
        }
        public GameState MoveDown()
        {
            Vector3 control = new Vector3(0, 0, -1);
            GameState next = MakeMove(-90, control);
            return next;
        }
        public GameState MoveRight()
        {
            Vector3 control = new Vector3(1, 0, 0);
            GameState next = MakeMove(0, control);
            return next;
        }
        public GameState MoveLeft()
        {
            Vector3 control = new Vector3(-1, 0, 0);
            GameState next = MakeMove(-180, control);
            return next;
        }
        public GameState MoveRightUp()
        {
            Vector3 control = new Vector3(1, 0, 1);
            GameState next = MakeMove(45, control);
            return next;
        }
        public GameState MoveLeftUp()
        {
            Vector3 control = new Vector3(-1, 0, 1);
            GameState next = MakeMove(135, control);
            return next;
        }
        public GameState MoveRightDown()
        {
            Vector3 control = new Vector3(1, 0, -1);
            GameState next = MakeMove(-45, control);
            return next;
        }
        public GameState MoveLeftDown()
        {
            Vector3 control = new Vector3(-1, 0, -1);
            GameState next = MakeMove(-135, control);
            return next;
        }
        public bool IsFinish()
        {
            Vector3 Length = position - TargetPosition;
            return (Length.magnitude < target_radius);
        }
        public int CompareTo(object obj)
        {
            GameState other = obj as GameState;
            return (distance_to_finish + distance_to_start).CompareTo((other.distance_to_finish + other.distance_to_start));
        }
    }



    private void OnEnable()
    {
        // When the tank is turned on, make sure it's not kinematic.
        m_Rigidbody.isKinematic = false;

        // Also reset the input values.
        m_VerticalInputValue = 0f;
        m_HorizontalInputValue = 0f;
    }

    private void OnDisable()
    {
        m_Rigidbody.isKinematic = true;
    }

    private void Update()
    {
        EngineAudio();
    }

    private void Start()
    {
        //sw = new StreamWriter("C:\\Users\\��������\\Desktop\\Log.txt");
        // Add tank object to InfoCollector
        if (!collector) collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        //else Debug.Log("Collector already set");

        GetComponent<TankShooting>().tankHolder = collector.AddTank(gameObject);

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;

        Gun = gameObject.GetComponentInChildren<BotShooting>();
    }

    private void FixedUpdate()
    {
        if (counter < discret)
        {
            counter++;
        }
        else
        {
            Decision();
            counter = 0;
        }
        Move();
    }

    protected abstract void Move();
    protected abstract void Decision();

}

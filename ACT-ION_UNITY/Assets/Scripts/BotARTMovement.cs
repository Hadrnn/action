using UnityEngine;
using System;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;




public class BotARTMovement : MonoBehaviour
{
    public const int discret = 25;
    public int teamNumber = 1;
    public int counter = 0;
    public float m_Speed = 12f;                 // How fast the tank moves forward and back.
    public float m_TurnSpeed = 300f;            // How fast the tank turns in degrees per second.
    public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
    public AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
    public AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
    public float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.
    public double AgressiveDistance;

    private int Astar_deep = 300;
    private int forvard_multiplyer = 1;
    private Rigidbody m_Rigidbody;              // Reference used to move the tank.
    private float m_VerticalInputValue = 0;         // The current value of the movement input.
    private float m_HorizontalInputValue = 0;             // The current value of the turn input.
    private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
    private BoxCollider m_Collider;

    string GameStateToString(GameState current)
    {
        string result = $"{(int)(current.position.x * 10)} {(int)(current.position.y * 10)} {(int)(current.position.z * 10)} {(int)(current.forward.x * 10)} {(int)(current.forward.y * 10)} {(int)(current.forward.z * 10)}";
        return result;
    }
    Vector3 AStar(GameState start)
    {
        PriorityQueue<GameState> que = new PriorityQueue<GameState>();
        Dictionary<string, GameState> visited = new Dictionary<string, GameState>();
        que.Push(start);
        visited[GameStateToString(start)] = start;
        GameState current = start;
        int i = 0;
        while (!que.empty)
        {
            if (i > Astar_deep)
            {
                Debug.Log("Превышено количество итерраций");
                Vector3 broken = new Vector3(0, 0, 0);
                return broken;
            }
            i++;
            current = que.top;
            que.Pop();

            if (current.IsFinish())
            {
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
                Debug.Log("Очередь опустошена");
                Vector3 broken = new Vector3(0, 0, 0);
                return broken;
            }
        }
        if (current.prew_state != null)
        {
            while (current.prew_state.prew_state != null)
            {
                current = current.prew_state;
            }

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
        static public Rigidbody ourRigidbody;
        static public BoxCollider hitbox;

        public Vector3 position = Vector3.zero;
        public Vector3 forward = Vector3.zero;
        public int forward_multiplyer;

        public float m_TurnSpeed = 300f;
        public float m_Speed = 12;
        public int target_radius = 10;
        public Vector3 TargetPosition = Vector3.zero;
        public GameState prew_state = null;
        public Vector3 move = Vector3.zero;
        public float distance_to_finish;
        public float distance_to_start;

        public bool CanMove(double control_angle, Vector3 control)
        {
            GameState next = new GameState();
            next.position = this.position;
            next.forward = this.forward;
            next.forward_multiplyer = this.forward_multiplyer;
            next.m_Speed = this.m_Speed;
            next.m_TurnSpeed = this.m_TurnSpeed;
            next.TargetPosition = this.TargetPosition;

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
                    turn = -(float)(System.Math.Sign(delta_angle) * 0.02f * next.m_TurnSpeed * 0.8); ;
                }
                else
                {
                    turn = 0;
                }

                Quaternion rotationQuaternion = Quaternion.AngleAxis(turn, Vector3.up);
                next.forward = rotationQuaternion * next.forward;

                Vector3 movement = new Vector3(0f, 0f, 0f);
                movement = next.forward * next.m_Speed * 0.02f * 1.3f * next.forward_multiplyer;
                next.position += movement;


                Vector3 new_forward_angle = new Vector3(0, (float)(forward_angle + turn), 0);
                Vector3 new_position = next.position;
                Quaternion newRotation = Quaternion.Euler(new_forward_angle);
                Vector3 colliderSize = GameState.hitbox.size;

                Collider[] collisionArray = Physics.OverlapBox(new_position, colliderSize / 2, newRotation, ~0, QueryTriggerInteraction.Ignore);

                if (collisionArray.Length == 1 && collisionArray[0].GetComponent<Rigidbody>() == ourRigidbody)
                {
                    continue;
                }
                else if (collisionArray.Length != 0)
                {
                    return false;
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
            GameState next = new GameState();
            next.position = this.position;
            next.forward = this.forward;
            next.forward_multiplyer = this.forward_multiplyer;
            next.m_Speed = this.m_Speed;
            next.m_TurnSpeed = this.m_TurnSpeed;
            next.TargetPosition = this.TargetPosition;
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
                    turn = -(float)(System.Math.Sign(delta_angle) * 0.02f * next.m_TurnSpeed * 0.8); ;
                }
                else
                {
                    turn = 0;
                }
                Quaternion rotationQuaternion = Quaternion.AngleAxis(turn, Vector3.up);
                next.forward = rotationQuaternion * next.forward;
                Vector3 movement = next.forward * next.m_Speed * 0.02f * 1.3f * next.forward_multiplyer;
                next.position = next.position + movement;
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

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider = GetComponent<BoxCollider>();
        GameState.ourRigidbody = m_Rigidbody;
        GameState.hitbox = m_Collider;
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
        // When the tank is turned off, set it to kinematic so it stops moving.
        m_Rigidbody.isKinematic = true;
    }
    private void Start()
    {
        //sw = new StreamWriter("C:\\Users\\Мегамозг\\Desktop\\Log.txt");

        // Add tank object to InfoCollector
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();
        collector.teams[teamNumber].tanks.Add(gameObject);

        // Store the original pitch of the audio source.
        m_OriginalPitch = m_MovementAudio.pitch;

/*        Gun = gameObject.GetComponentInChildren<BotAPCShooting>();*/
    }
    private void Update()
    {
        //Debug.LogWarning(m_MovementInputValue);
        EngineAudio();
    }
    private void EngineAudio()
    {
        // If there is no input (the tank is stationary)...
        if (Mathf.Abs(m_VerticalInputValue) < 0.1f && Mathf.Abs(m_HorizontalInputValue) < 0.1f)
        {
            // ... and if the audio source is currently playing the driving clip...
            if (m_MovementAudio.clip == m_EngineDriving)
            {
                // ... change the clip to idling and play it.
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else
        {
            // Otherwise if the tank is moving and if the idling clip is currently playing...
            if (m_MovementAudio.clip == m_EngineIdling)
            {
                // ... change the clip to driving and play.
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
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
    private void Decision()
    {
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        Vector3 MyPosition = transform.position;

        Vector3 EnemyPosition = collector.teams[0].tanks[0].transform.position;

        float current_distanse_to_my_cover = (collector.mapObjects[0].transform.position - MyPosition).magnitude;
        float current_distanse_to_enemy_cover = (collector.mapObjects[0].transform.position - EnemyPosition).magnitude;
        float min_summ = current_distanse_to_my_cover + current_distanse_to_enemy_cover;
        int cover_index = 0;

        for (int i = 1; i < collector.mapObjects.Count; i++)
        {
            current_distanse_to_my_cover = (collector.mapObjects[i].transform.position - MyPosition).magnitude; 
            current_distanse_to_enemy_cover = (collector.mapObjects[i].transform.position - EnemyPosition).magnitude;
            if (current_distanse_to_my_cover + current_distanse_to_enemy_cover < min_summ)
            {
                min_summ = current_distanse_to_my_cover + current_distanse_to_enemy_cover;
                cover_index = i;
            }
        }
        Vector3 cover_position = collector.mapObjects[cover_index].transform.position;

        Vector3 connect_line = cover_position - EnemyPosition;

        Vector3 additional_element = new Vector3(20, 0, 20);

        connect_line = connect_line.normalized;

        float mult_x = connect_line.x * additional_element.x;
        float mult_z = connect_line.z * additional_element.z;
        Vector3 mult = new Vector3(mult_x, 0, mult_z);

        Vector3 TargetPosition = cover_position + mult;

        GameState Start = new GameState();
        Start.position = transform.position;
        Start.forward = transform.forward;
        Start.forward_multiplyer = forvard_multiplyer;
        Start.TargetPosition = TargetPosition;
        Vector3 dist = Start.TargetPosition - Start.position;
        Start.distance_to_finish = dist.magnitude;
        Start.distance_to_start = 0;
        Vector3 decision = AStar(Start);
        m_HorizontalInputValue = decision.x;
        m_VerticalInputValue = decision.z;
}
    private void Move()
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
            turn = -(float)(System.Math.Sign(delta_angle) * 0.02f * m_TurnSpeed * 0.8);
        }
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
        Vector3 movement = new Vector3(0f, 0f, 0f);
        if ((m_VerticalInputValue != 0) || (m_HorizontalInputValue != 0))
        {
            movement = transform.forward * m_Speed * 0.02f * 1.3f * forvard_multiplyer;
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

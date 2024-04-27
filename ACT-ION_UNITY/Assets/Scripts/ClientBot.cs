using UnityEngine;
using System.Net.Sockets;
using System.Text;


public class ClientBot : MonoBehaviour
{
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create socket
    public string message = "Fisting is ...?"; // Create message to server
    public string d_answer; // Create answer from server
    public string SERVER_IP = "127.0.0.1"; // Server ip
    public int PORT = 9999; // Server port



    // Start is called before the first frame update
    void Start()
    {
        while (true)
        {
            try 
            {
            //Прописать вывод ошибки и повторное подключение
                socket.Connect(SERVER_IP, PORT);
            }
            catch {
                continue;
            }
            break;
        }
    }

    void Update() {
        Start_working();
    }

    void Start_working()
    {
        InfoCollector collector = GameObject.Find("InfoCollector").GetComponent<InfoCollector>();

        // Вызов функции для сообщения для сервера: message = GetInfo()

        Vector3 AL_bot_pos;
        Vector3 NN_bot_pos;

        ////////////
        /// Bots positions 
        NeuralTankMovement possiblyNeural = collector.teams[1].tanks[0].tank.GetComponent<NeuralTankMovement>();
        int AL_bot = -1;
        if (possiblyNeural)
        {
            Debug.Log("Im right about possibly neural tanks");
            AL_bot_pos = collector.teams[0].tanks[0].tank.transform.position;
            NN_bot_pos = collector.teams[1].tanks[0].tank.transform.position;
            AL_bot = 0;
        }
        else
        {
            Debug.Log("Im wrong about possibly neural tanks");
            AL_bot = 1;
            AL_bot_pos = collector.teams[1].tanks[0].tank.transform.position;
            NN_bot_pos = collector.teams[0].tanks[0].tank.transform.position;
            possiblyNeural = collector.teams[0].tanks[0].tank.GetComponent<NeuralTankMovement>();
        }
        /////////////////
        

        /////////////////////
        /// CAN SHOOT 
        Transform fire_transform = possiblyNeural.GetComponentInChildren<NeuralTurretTurning>().transform.Find("TurretModel").Find("FireTransform");

        if (!fire_transform){
            Debug.Log("Couldnt find fire transform of neural Bot");
        }

        Vector3 direction = -(fire_transform.position - AL_bot_pos).normalized;
        float distance = Vector3.Distance(fire_transform.position, AL_bot_pos);
        Collider ALBotCollider = collector.teams[AL_bot].tanks[0].tank.GetComponent<Collider>();
        RaycastHit hit;
        int can_shoot = 0;
        if (Physics.Raycast(fire_transform.position, direction, out hit, distance*2))
        {
            if (hit.collider == ALBotCollider)
            {
                can_shoot = 1;
            }
        }
        ////////////////////




        ////////////////////////////////////////////
        // CLOSEST BULLET POSITION 
        Vector3 ClosestBulletPos;
        if (collector.shells.Count == 0) ClosestBulletPos = new Vector3(1000, 1000, 1000);
        else ClosestBulletPos = new Vector3(0, 0, 0);
        float minDistance = 1000;

        for (int i = 0; i < collector.shells.Count; ++i)
        {
            float currentDistance = Vector3.Distance(NN_bot_pos, collector.shells[i].transform.position);
            if (currentDistance < minDistance)
            {
                ClosestBulletPos = collector.shells[i].transform.position;
                minDistance = currentDistance;
            }
        }
        ////////////////////////////////////////////
        ///



        ////////////////////////////
        /// UP DOWN LEFT RIGHT CLOSEST HITBOX
        float [] collisionArray= new float[4];  
        distance = 1000;
        direction = new Vector3(1, 0, 0);
        if (Physics.Raycast(AL_bot_pos, direction, out hit, distance))
        {
            
            collisionArray[0] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(0, 0, 1);
        if (Physics.Raycast(AL_bot_pos, direction, out hit, distance))
        {
            collisionArray[1] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(-1, 0, 0);
        if (Physics.Raycast(AL_bot_pos, direction, out hit, distance))
        {
            collisionArray[2] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(0, 0, -1);
        if (Physics.Raycast(AL_bot_pos, direction, out hit, distance))
        {
            collisionArray[3] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        ///////////////////////



        message =
            NN_bot_pos.x.ToString() + " " + NN_bot_pos.z.ToString() + " " + AL_bot_pos.x.ToString() + " " + AL_bot_pos.z.ToString() + " " + can_shoot.ToString() + " " + collector.gameResult + " ";

        message += ClosestBulletPos.x.ToString() + " " + ClosestBulletPos.z.ToString() + " ";

        for (int i = 0; i< 4; ++i)
        {
            message += collisionArray[i].ToString() + " ";
        }

        Send_message();
/*        if (collector.gameResult != "Playing")
        {
            Close_connection();
            Destroy(gameObject);
        }*/
        Get_message();
        Debug.Log($"Get message  {d_answer}");
        // Вызов функции для изменения позиции: ChangePosition(d_answer)
        collector.botMovement = d_answer;

    }

    // Send message to python server
    void Send_message()
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        socket.Send(messageBytes);
        //Debug.Log($"Send message {message}");
    }

    // Get message from python server
    void Get_message()
    {
        var answer = new byte[2048];
        socket.Receive(answer);
        d_answer = Encoding.UTF8.GetString(answer);
    }

    void Close_connection()
    {
        socket.Close();
    }
}

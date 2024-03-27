using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;
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

        Vector3 AL_bot_pos = collector.teams[1].tanks[0].transform.position;
        Vector3 NN_bot_pos = collector.teams[0].tanks[0].transform.position;
        //Debug.Log(AL_bot_pos);

        Vector3 direction = (NN_bot_pos - AL_bot_pos).normalized;
        float distance = Vector3.Distance(AL_bot_pos, NN_bot_pos);
        Collider PlayerCollider = collector.teams[1].tanks[0].GetComponent<Collider>();
        RaycastHit hit;
        int can_shoot = 0;
        if (Physics.Raycast(AL_bot_pos, direction, out hit, distance))
        {
            if (hit.collider == PlayerCollider)
            {
                can_shoot = 1;
            }
        }
        message = 
            NN_bot_pos.x.ToString() + " " + NN_bot_pos.z.ToString() + " " + AL_bot_pos.x.ToString() + " " + AL_bot_pos.z.ToString() + " " + can_shoot.ToString() + " " + collector.gameResult;

        Send_message();
        if (collector.gameResult != "Playing")
        {
            Close_connection();
            Destroy(gameObject);
        }
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

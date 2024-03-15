using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
public class ClientBot : MonoBehaviour
{
    public Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // Create socket
    public string message = "Fisting is ...?"; // Create message to server
    public string d_answer; // Create answer from server
    public string SERVER_IP = "127.0.0.1"; // Server ip
    public int PORT = 9999; // Server port
    int t = 0; // Some variable fro testing



    // Start is called before the first frame update
    void Start()
    {
        //Прописать вывод ошибки и повторное подключение
        socket.Connect(SERVER_IP, PORT);
    }

    void Update() {
        Start_working();
    }

    void Start_working()
    {
        t++;
        Get_message();
        Debug.Log($"Get message  {d_answer}");
        // Вызов функции для изменения позиции: ChangePosition(d_answer)
        // Вызов функции для сообщения для сервера: message = GetInfo()
        Send_message();
        if (t == 10000)
        {
        message = "END";
        Send_message();
        Close_connection();
        Destroy(gameObject);
        }
    }

    // Send message to python server
    void Send_message()
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        socket.Send(messageBytes);
        Debug.Log($"Send message {message}");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class ClientBot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 9998);
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect("127.0.0.1", 9999);
        var message = "Fisting is ...?";
        var messageBytes = Encoding.UTF8.GetBytes(message);
        socket.Send(messageBytes);
        Console.WriteLine($"Send message {message}");
        var answer = new byte[2048];
        socket.Receive(answer);
        var d_answer = Encoding.UTF8.GetString(answer);
        Console.WriteLine($"Get message  {d_answer}");
        socket.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

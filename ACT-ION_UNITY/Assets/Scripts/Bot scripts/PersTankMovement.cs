using UnityEngine;
using System;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using System.Data;
using System.Runtime.InteropServices;
using Unity.Burst.CompilerServices;

class NN
{
    // Зададим несколько констант
    const int LOW = -5;
    const int UP = 5;

    // Размеры сети
    public int input_size;
    public int hidden_size_1;
    public int hidden_size_2;
    public int hidden_size_3;
    public int output_size;

    // Сама сеть
    public double[,] fc_1;
    public double[,] fc_2;
    public double[,] fc_3;
    public double[,] fc_4;


    public NN(int in_size, int h_s_1, int h_s_2, int h_s_3, int out_size)
    {
        input_size = in_size;
        hidden_size_1 = h_s_1;
        hidden_size_2 = h_s_2;
        hidden_size_3 = h_s_3;
        output_size = out_size;

        fc_1 = new double[input_size, hidden_size_1];
        fc_2 = new double[hidden_size_1, hidden_size_2];
        fc_3 = new double[hidden_size_2, hidden_size_3];
        fc_4 = new double[hidden_size_3, output_size];
    }

    public void init_random()
    {
        layer_init_random(fc_1);
        layer_init_random(fc_2);
        layer_init_random(fc_3);
        layer_init_random(fc_4);
    }

    void layer_init_random(double[,] layer)
    {
        System.Random rnd = new System.Random(12);
        for (int i = 0; i < layer.GetLength(0); i++)
        {
            for (int j = 0; j < layer.GetLength(1); j++)
            {
                layer[i, j] = rnd.NextDouble() * (UP - LOW) + LOW;
            }
        }
    }

    public double[] forward(double[] input)
    {
        double[] x;
        x = relu(matmul(input, fc_1));
        x = relu(matmul(x, fc_2));
        x = relu(matmul(x, fc_3));
        x = matmul(x, fc_4);
        return x;
    }

    double[] relu(double[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0)
            {
                array[i] = 0;
            }
        }
        return array;
    }

    double[] deriv_relu(double[] array)
    {
        double[] result = new double[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0)
            {
                result[i] = 0;
            }
            else
            {
                result[i] = 1;
            }
        }
        return result;
    }

    double[] matmul(double[] a1, double[,] a2)
    {
        if (a1.Length != a2.GetLength(0))
        {
            Console.WriteLine("Error in matmul\nUnacceptable shapes of matrix");
            return a1;
        }

        double[] result = new double[a2.GetLength(1)];
        double tmp_result = 0;
        for (int i = 0; i < a2.GetLength(1); i++)
        {
            for (int j = 0; j < a2.GetLength(0); j++)
            {
                tmp_result += a1[j] * a2[j, i];
            }
            result[i] = tmp_result;
            tmp_result = 0;
        }
        return result;
    }

    double mse(double target, double predict)
    {
        return (predict - target) * (predict - target);
    }

    double deriv_mse(double target, double predict)
    {
        return 2 * (predict - target);
    }
}

class Check
{
    public static void Main()
    {
        NN net = new NN(2, 10, 20, 10, 1);
        net.init_random();

    }
}

public class PersTankMovement : BotMovement
{
    private NN perseptron;

    private void Start()
    {
        perseptron = new NN(8, 20, 40, 20, 2);
        perseptron.init_random();
    }

    protected override void Decision()
    {
        double[] input = new double[8];
        input[0] = transform.position.x;
        input[1] = transform.position.z;
        Transform Enemy = FindClosestEnemy(teamNumber, transform, collector);
        input[2] = Enemy.position.x;
        input[3] = Enemy.position.z;

        float[] collisionArray = new float[4];
        float distance = 1000;
        Vector3 direction = new Vector3(1, 0, 0);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {

            collisionArray[0] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(0, 0, 1);
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            collisionArray[1] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(-1, 0, 0);
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            collisionArray[2] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        direction = new Vector3(0, 0, -1);
        if (Physics.Raycast(transform.position, direction, out hit, distance))
        {
            collisionArray[3] = hit.distance;
        }
        else
        {
            Debug.LogWarning("DID NOT FIND SOMETHING TO COLLIDE WITH");
        }

        input[4] = collisionArray[0];
        input[5] = collisionArray[1];
        input[6] = collisionArray[2];
        input[7] = collisionArray[3];
        double[] result;
        result = perseptron.forward(input);
        m_HorizontalInputValue = (float)(result[0]);
        m_VerticalInputValue = (float)(result[1]);
    
    }
}
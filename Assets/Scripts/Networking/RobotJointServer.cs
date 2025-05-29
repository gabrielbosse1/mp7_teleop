using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

public class RobotJointServer : MonoBehaviour
{
    public int port = 8080;
    private TcpListener listener;
    private Dictionary<string, float> jointAngles = new Dictionary<string, float>();
    private readonly object lockObj = new object();
    private bool isRunning = true;

    void Start()
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        listener.BeginAcceptTcpClient(OnClientConnected, null);
        InvokeRepeating(nameof(UpdateJointAngles), 0f, 0.1f); // Update joint angles every 0.1 seconds
        Debug.Log($"TCP server started on port {port}");
    }

    void OnApplicationQuit()
    {
        isRunning = false;
        listener.Stop();
    }

    /// <summary>
    /// Updates the dictionary of joint angles by traversing child joints.
    /// </summary>
    void UpdateJointAngles()
    {
        Dictionary<string, float> updated = new Dictionary<string, float>();
        int index = 1;
        Transform current = transform;
        while (true)
        {
            var joint = current.GetComponent<RobotJoint>();
            if (joint != null)
            {
                updated[index.ToString()] = joint.GetAdjustedAngle();
                index++;
                if (current.childCount > 0)
                    current = current.GetChild(0);
                else
                    break;
            }
            else
            {
                break;
            }
        }
        lock (lockObj)
        {
            jointAngles = updated;
        }
    }

    /// <summary>
    /// Callback when a new client connects.
    /// </summary>
    private void OnClientConnected(IAsyncResult ar)
    {
        if (!isRunning)
            return;

        TcpClient client = null;
        try
        {
            client = listener.EndAcceptTcpClient(ar);
            client.NoDelay = true; // Disable Nagle's algorithm for low latency
            Debug.Log("Client connected: " + client.Client.RemoteEndPoint);

            // Start accepting next client immediately
            listener.BeginAcceptTcpClient(OnClientConnected, null);

            // Handle client in a thread pool thread to avoid blocking main thread
            ThreadPool.QueueUserWorkItem(HandleClient, client);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Error accepting client: " + ex.Message);
            client?.Close();
        }
    }

    /// <summary>
    /// Handles communication with a connected client.
    /// Sends the count of floats followed by all joint angles as floats.
    /// </summary>
    private void HandleClient(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;
        using (client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                float[] angles;
                lock (lockObj)
                {
                    angles = new float[jointAngles.Count];
                    int i = 0;
                    foreach (var kv in jointAngles)
                        angles[i++] = kv.Value;
                }

                // Send the number of floats (int32)
                byte[] countBytes = BitConverter.GetBytes(angles.Length);
                stream.Write(countBytes, 0, countBytes.Length);
                stream.Flush();

                // Send all floats sequentially (4 bytes each)
                byte[] buffer = new byte[angles.Length * 4];
                for (int i = 0; i < angles.Length; i++)
                {
                    BitConverter.GetBytes(angles[i]).CopyTo(buffer, i * 4);
                }
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();

                Debug.Log($"Sent {angles.Length} floats to client {client.Client.RemoteEndPoint}");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error handling client: " + ex.Message);
            }
        }
    }
}

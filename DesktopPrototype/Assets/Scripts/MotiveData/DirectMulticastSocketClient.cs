using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace OptitrackManagement
{

    public class DirectStateObject
    {
        public Socket workSocket;
        public const int BufferSize = 65507;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    public static class DirectMulticastSocketClient
    {

        private static Socket client;
        private static bool _isInitRecieveStatus = false;
        private static bool _isIsActiveThread = false;
        private static bool _isDataReceivedCorrectly = false;
        private static StreemData _streemData = null;
        private static String _strFrameLog = String.Empty;

        //bool returnValue = false;
        private static int _dataPort = 1511;
        //private static int _commandPort = 1510;
        private static string _multicastIPAddress = "239.255.42.99";
        //private static string hostIP = "192.168.20.50";
        //private static string localIP = "192.168.20.52";
        private const int SOCKET_BUFSIZE = 0x100000;

        private static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                _streemData = new StreemData();

				client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                //client.ExclusiveAddressUse = false;
                //IPAddress localip1 = IPAddress.Parse(localIP);
				IPEndPoint ipep = new IPEndPoint(IPAddress.Any, _dataPort);
                client.Bind(ipep);

                IPAddress ip = IPAddress.Parse(_multicastIPAddress);
                client.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(ip, IPAddress.Any));

                /*
                //initialize command socket
                mCommandListner = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                mCommandListner.Bind(new IPEndPoint(IPAddress.Any, 0));

                //set socket to boradcast mode
                mCommandListner.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                mCommandListner.Blocking = false;
                mCommandListner.ReceiveBufferSize = SOCKET_BUFSIZE;

                PACKET_FORMAT packet = new PACKET_FORMAT();
                packet.header.iMessage = NAT_REQUEST_MODELDEF;
                packet.header.nDataBytes = 0;
                byte[] msg = new byte[4];
                IntPtr ptr = Marshal.AllocHGlobal(4);
                Marshal.StructureToPtr(packet, ptr, true);
                Marshal.Copy(ptr, msg, 0, 4);
                mCommandListner.SendTo(msg, mRemoteIpEndPoint);
                Marshal.FreeHGlobal(ptr);

                Receive(mCommandListner);
                 */
                _isInitRecieveStatus = Receive(client);
                _isIsActiveThread = _isInitRecieveStatus;

            }
            catch (Exception e)
            {
                Debug.LogError("[UDP] DirectMulticastSocketClient: " + e.ToString());
            }
        }
              
        private static bool Receive(Socket client)
        {
            try
            {
                // Create the state object.
                DirectStateObject state = new DirectStateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, DirectStateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
                return false;
            }
            return true;
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                DirectStateObject state = (DirectStateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0 && _isIsActiveThread)
                {
                    ReadPacket(state.buffer);
                    client.BeginReceive(state.buffer, 0, DirectStateObject.BufferSize, 0,new AsyncCallback(ReceiveCallback), state);                    
                }
                else
                {
                    Debug.LogWarning("[UDP] - End ReceiveCallback");

                    if (_isIsActiveThread == false)
                    {
                        Debug.LogWarning("[UDP] - Closing port");
                        _isInitRecieveStatus = false;
                        //client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }

                    // Signal that all bytes have been received.
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        private static void ReadPacket(Byte[] b)
        {
            int offset = 0;
            int nBytes = 0;
            int[] iData = new int[100];
            float[] fData = new float[500];
            char[] cData = new char[500];

            Buffer.BlockCopy(b, offset, iData, 0, 2); offset += 2;
            int messageID = iData[0];

            Buffer.BlockCopy(b, offset, iData, 0, 2); offset += 2;
            nBytes = iData[0];

            Debug.Log("[UDPClient] Processing Received Packet (Message ID : " + messageID + ")");
            if (messageID == 5)      // Data descriptions
            {
                Debug.Log("DirectParseClient: Data descriptions");

            }
            else if (messageID == 7)   // Frame of Mocap Data
            {
                _strFrameLog = String.Format("DirectParseClient: [UDPClient] Read FrameOfMocapData: {0}\n", nBytes);
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                _strFrameLog += String.Format("Frame # : {0}\n", iData[0]);

                //number of data sets (markersets, rigidbodies, etc)
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                int nMarkerSets = iData[0];
                _strFrameLog += String.Format("MarkerSets # : {0}\n", iData[0]);
                
                for (int i = 0; i < nMarkerSets; i++)
                {
                    String strName = "";
                    int nChars = 0;
                    while (b[offset + nChars] != '\0')
                    {
                        nChars++;
                    }
                    strName = System.Text.Encoding.ASCII.GetString(b, offset, nChars);
                    
                    offset += nChars + 1;

                    Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                    _strFrameLog += String.Format("{0}:" + strName + ": marker count : {1}\n", i, iData[0]);
                    
                    nBytes = iData[0] * 3 * 4;
                    Buffer.BlockCopy(b, offset, fData, 0, nBytes); offset += nBytes;

                    //do not need   
                }

                // Other Markers - All 3D points that were triangulated but not labeled for the given frame.
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                int nOtherMarkers = iData[0];
                //Debug.Log("other markers:" + nOtherMarkers);
                _strFrameLog += String.Format("Other Markers : {0}\n", iData[0]);

                if (nOtherMarkers >= 4)
                {
                    _isDataReceivedCorrectly = true;
                    for (int i = 0; i < nOtherMarkers; i++)
                    {
                        nBytes = 3 * 4;
                        Buffer.BlockCopy(b, offset, fData, 0, nBytes); offset += nBytes;

                        Marker mk = new Marker();
                        mk.pos.x = -fData[0];
                        mk.pos.y = fData[1];
                        mk.pos.z = fData[2];
                        _streemData._markers[i] = mk;
                    }
                }
                else if (nOtherMarkers <= 3)
                {
                    _isDataReceivedCorrectly = false;
                    for (int i = 0; i < nOtherMarkers; i++)
                    {
                        nBytes = 3 * 4;
                        Buffer.BlockCopy(b, offset, fData, 0, nBytes); offset += nBytes;
                    }
                }
                
                // Rigid Bodies
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                _streemData._nRigidBodies = iData[0];

                _strFrameLog += String.Format("Rigid Bodies : {0}\n", iData[0]);

                // Check if we received wrist and glasses rigidbodies
                if (_streemData._nRigidBodies != 2)
                    _isDataReceivedCorrectly = false;
                else
                    _isDataReceivedCorrectly = true;

                for (int i = 0; i < _streemData._nRigidBodies; i++)
                {
                   // _streemData._rigidBody[i].fingerMarker = mk3;
                    ReadRigidBody(b, ref offset, _streemData._rigidBody[i]);
                    //_streemData._rigidBody[0];
                }
            }
            else if (messageID == 100)
            {

            }
        }

        // Unpack RigidBody data
        private static void ReadRigidBody(Byte[] b, ref int offset, RigidBody rb)
        {
            try
            {
                int[] iData = new int[100];
                float[] fData = new float[100];

                // RB ID
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;

                int iBoneID = iData[0];       // lo 16 bits = ID of bone
                rb.ID = iData[0]; // already have it from data descriptions
                // Debug.Log(iBoneID);

                // RB pos
                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.pos.x = -fData[0];

                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.pos.y = fData[0];

                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.pos.z = fData[0];

                // RB ori
                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.ori.x = fData[0];

                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.ori.y = fData[0];

                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.ori.z = -fData[0];

                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;
                rb.ori.w = -fData[0];

                int nRigidMarkers = 0;
                Buffer.BlockCopy(b, offset, iData, 0, 4); offset += 4;
                nRigidMarkers = iData[0];
                //Debug.Log(nRigidMarkers);
                // Debug.Log(rb.pos); 
                Buffer.BlockCopy(b, offset, fData, 0, 4 * 3 * nRigidMarkers); offset += 4 * 3 * nRigidMarkers;

                /*
                if (nRigidMarkers == 5)
                {
                    rb.wristMarker = new Marker[3];
               
                    Marker mk0 = new Marker();
                    rb.wristMarker[0] = mk0;
                    rb.wristMarker[0].pos.x = -fData[0];
                    rb.wristMarker[0].pos.y = fData[1];
                    rb.wristMarker[0].pos.z = fData[2];
                    Debug.Log(rb.wristMarker[0].pos);

                    Marker mk1 = new Marker();
                    rb.wristMarker[1] = mk1;
                    rb.wristMarker[1].pos.x = -fData[3];
                    rb.wristMarker[1].pos.y = fData[4];
                    rb.wristMarker[1].pos.z = fData[5];
                    Debug.Log(rb.wristMarker[1].pos);

                    Marker mk2 = new Marker();
                    rb.wristMarker[2] = mk2;
                    rb.wristMarker[2].pos.x = -fData[6];
                    rb.wristMarker[2].pos.y = fData[7];
                    rb.wristMarker[2].pos.z = fData[8];
                    Debug.Log(rb.wristMarker[2].pos);

                }
                */

                // RB's marker ids
                Buffer.BlockCopy(b, offset, iData, 0, 4 * nRigidMarkers); offset += 4 * nRigidMarkers;
                //for (int i = 0; i < nRigidMarkers; i++){Debug.Log(iData[i]);}

                // RB's marker sizes
                Buffer.BlockCopy(b, offset, fData, 0, 4 * nRigidMarkers); offset += 4 * nRigidMarkers;

                // RB mean error
                Buffer.BlockCopy(b, offset, fData, 0, 4); offset += 4;


                Buffer.BlockCopy(b, offset, iData, 0, 2); offset += 2;

            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }

        } // next rigid body

        // Use this for initialization
        public static void Start()
        {
            StartClient();
        }
        
        public static void Close()
        {
            _isIsActiveThread = false;
        }

        public static bool IsInit()
        {
            return _isInitRecieveStatus;
        }

        public static StreemData GetStreemData()
        {
            return _streemData;
        }

        public static bool IsDataReceivedCorrectly()
        {
            return _isDataReceivedCorrectly;
        }
    }
}
 
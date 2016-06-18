using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OptitrackManagement
{
    public class StreemData
    {
        public RigidBody[] _rigidBody = new RigidBody[200];
        public Marker[] _markers = new Marker[20];
        public int _nRigidBodies = 0;

        public StreemData()
        {
            //Debug.Log("StreemData: Construct");
            InitializeRigidBody();
            InitializeMarkerSet();
        }

        public bool InitializeRigidBody()
        {
            _nRigidBodies = 0;
            for (int i = 0; i < 200; i++)
            {
                _rigidBody[i] = new RigidBody();
            }
            return true;
        }

        public bool InitializeSkeleton()
        {
            return true;
        }

        public bool InitializeMarkerSet()
        {
            for (int i = 0; i < 20; i++)
            {
                _markers[i] = new Marker();
            }
            return true;
        }
    }
}
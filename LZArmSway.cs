using System;
using UnityEngine;
using VNyanInterface;
using LZLayer;
using System.Collections.Generic;

namespace LZArmSway
{

    public class LZArmSway : MonoBehaviour
    {
        // Set up pose layer according to our class
        IPoseLayer LZSwayLayer= new LZLayer.LZLayer();

        public void Start()
        {
            // Register Pose Layer for VNyan to listen to
            VNyanInterface.VNyanInterface.VNyanAvatar.registerPoseLayer(LZSwayLayer);
        }

        public void Update()
        {
           
        }
    }
}
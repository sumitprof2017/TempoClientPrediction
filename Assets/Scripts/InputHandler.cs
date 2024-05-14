using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class InputHandler : NetworkBehaviour
{
    internal enum Driver
    {
        AI,
        Keyboard
    }
    [SerializeField] private Driver driverType;

    public float steerInput;
    public float driveInput;
    public bool brakeInput;
    public bool turboInput;
    public bool escapeInput;

    
   
    [Range(0, 5)] public float steerForce;
   

    //access player and ai
    //private BikramTempoAI AIData;
    public GameObject AI;
    public GameObject player;

    SafaTempoController safaTempoController;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Mala");
        AI = GameObject.FindGameObjectWithTag("Nagas");
        safaTempoController = GetComponent<SafaTempoController>();
        lastServerSequenceNumber.OnValueChanged += OnDataSent;

    }
    private void OnDataSent(int previous, int current)
    {
        print("change is called");
        safaTempoController.StartTempoOverAllMovement();

    }
    private NetworkVariable<int> lastServerSequenceNumber = new NetworkVariable<int>();
    // Update is called once per frame
    void FixedUpdate()
    {
       
        if (driverType == Driver.AI)
        {
            AIDrive();

        }
        else
        {
         
            if (IsOwner && InputDataAvailable())
            {
              
               var data = CollectInput();
                safaTempoController.LocalMovement(data);
                SendInputToServerServerRpc(data,transform.position);
                /*  localSequenceNumber++;

                  MoveTempo_ServerRpc(driveData, localSequenceNumber);*/

            }
            else
            {

            }

        }
   

        
       
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendInputToServerServerRpc(DriveData data,Vector3 currentLocalPosition)
    {
        safaTempoController.ReceiveInputFromClient(data, currentLocalPosition);
    }
    /*  private void Update()
      {
          if (IsOwner && InputDataAvailable())
          {
              print("seding input");
              UpdateDriveData();
              MoveTempo_ServerRpc(driveData);
          }
      }*/
    private int localSequenceNumber = 0;

    [ServerRpc]
    public void MoveTempo_ServerRpc(DriveData driverdata, int sequenceNumber)
    {
        this.driveData = driverdata;
        if (sequenceNumber > lastServerSequenceNumber.Value)
        {
            print("driver data received" + driveData);
            print("driver data horzintal axia" + driveData.driveInput);
            print("driver data steering axia" + driveData.steerInput);
/*            safaTempoController.StartTempoOverAllMovement();
*/            lastServerSequenceNumber.Value = sequenceNumber;
        }
         
    }
    public DriveData driveData;
    private DriveData CollectInput()
    {
        return new DriveData
        {
            driveInput = Input.GetAxis("Vertical"),
            steerInput = Input.GetAxis("Horizontal"),
            brakeInput = Input.GetButton("Jump"),
            turboInput = Input.GetKey(KeyCode.LeftShift),
            escapeInput = Input.GetKey(KeyCode.Escape)
        };
    }
   

    private bool InputDataAvailable()
    {
        // Check if any input data is available
        return Input.GetAxis("Vertical") != 0 ||
               Input.GetAxis("Horizontal") != 0 ||
               Input.GetButton("Jump") ||
               Input.GetKey(KeyCode.LeftShift) ||
               Input.GetButton("Cancel");
    }
    private void AIDrive()
    {
        //AIData = AI.GetComponent<BikramTempoAI>();
        AISteer();
        driveInput = UnityEngine.Random.Range(0.3f,1.0f);
        //driveInput = 0;
        /*
        if (sensorData.hitDetect && !AIData.stuck)
        {
            brakeInput = true;
        }
        else 
        {
            brakeInput = false;
        }
        */
    }

    private void KeyboardDrive()
    {
        driveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
        brakeInput = (Input.GetAxis("Jump") != 0) ? true : false;
        turboInput = (Input.GetKey(KeyCode.LeftShift) ? true : false);
        escapeInput = (Input.GetAxis("Cancel") != 0) ? true : false;
    }

    private void AISteer()
    {
        /*
        if (!AIData.playerInvisionRadius)
        {
            if (AIData.stuck)
            {
                Vector3 relative = transform.InverseTransformPoint(currentWaypoint.previousWaypoint);
                relative /= relative.magnitude;
                steerInput = (relative.x / relative.magnitude) * steerForce;
            }
            
           
            if (sensorData.hitDetect)
            {
                steerInput = sensorData.avoidMultiplier * steerForce;
                //steerInput = Mathf.Lerp(0.3f, sensorData.avoidMultiplier, 1.0f * Time.deltaTime) * steerForce;
                //steerInput = Mathf.Clamp(sensorData.avoidMultiplier * steerForce,-1.2f,1.2f);
            }
            else
            {
            Vector3 relative = transform.InverseTransformPoint(currentWaypoint.currentWaypoint);
                relative /= relative.magnitude;
                steerInput = (relative.x / relative.magnitude) * steerForce;
            }
        }
        else
        {
            Vector3 relative = transform.InverseTransformPoint(player.transform.position);
            relative /= relative.magnitude;
            steerInput = (relative.x / relative.magnitude) * steerForce;
        }
        */
    }



}
public struct DriveData : INetworkSerializeByMemcpy
{
    public float driveInput;
    public float steerInput;
    public bool brakeInput;
    public bool turboInput;
    public bool escapeInput;

    
}
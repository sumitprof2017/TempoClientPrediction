using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject canvas;
    public void StartGame(bool isServerBool)
    {
        if (isServerBool)
        {
            NetworkManager.Singleton.StartServer();
            canvas.SetActive(false);
        }
        else
        {
            NetworkManager.Singleton.StartClient();
            canvas.SetActive(false);

        }
    }

    public List<GameObject> listOfTransforms = new List<GameObject>();
    public CameraFollow cameraFollow;

    public GameObject parentForAllPositions;
    public override void OnNetworkSpawn()
    {


        if (IsServer)
        {


            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoined;
        foreach(Transform child in parentForAllPositions.transform)
        {
            listOfTransforms.Add(child.gameObject);
        }
        }

    }

    // Update is called once per frame
    private void OnPlayerJoined(ulong clientId)
    {
        Debug.Log("Player joined: " + clientId);

        // Pick a random transform from the list
        int randomNumber = Random.Range(0, listOfTransforms.Count);
        Vector3 chosenTransform = listOfTransforms[randomNumber].transform.position;

        // Find the player's NetworkObject
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            // Assuming the player object is the first one in the list of owned objects
            NetworkObject playerNetworkObject = client.PlayerObject;
            ulong networkId = playerNetworkObject.NetworkObjectId;
            SetPositionForEachClientRpc(clientId, networkId, chosenTransform);
            // Move the player's object to the chosen position
            if (playerNetworkObject != null)
            {
                Debug.LogError(" player object found for client: " + clientId);

                playerNetworkObject.transform.position = chosenTransform;
                playerNetworkObject.transform.rotation = transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 255f, transform.rotation.eulerAngles.z);
                if (IsServer)
                {
                    SendRpcToFollowCameraClientRpc(clientId, networkId);
                }
            }
            else
            {
                Debug.LogError("No player object found for client: " + clientId);
            }
        }
        else
        {
            Debug.LogError("Client not found: " + clientId);
        }

        // Additional code here for when a player joins
    }

    [ClientRpc]
    private void SetPositionForEachClientRpc(ulong clientId, ulong networkId,Vector3 chosenTransform)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out NetworkObject networkObject))
            {
                Debug.Log("Found NetworkObject: " + networkObject.gameObject.name);
                networkObject.gameObject.transform.position = chosenTransform;
                networkObject.gameObject.transform.rotation = transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 255f, transform.rotation.eulerAngles.z);
                // Perform actions on the NetworkObject
            }
            else
            {
                Debug.LogError("No NetworkObject found with ID: " + networkId);
            }
        }
    }



    [ClientRpc]
    private void SendRpcToFollowCameraClientRpc(ulong clientId, ulong networkId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {

            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkId, out NetworkObject networkObject))
            {
                Debug.Log("Found NetworkObject: " + networkObject.gameObject.name);
                cameraFollow.SetCameraToFollowPlayer(networkObject.gameObject);

                // Perform actions on the NetworkObject
            }
            else
            {
                Debug.LogError("No NetworkObject found with ID: " + networkId);
            }
        }
    }
}

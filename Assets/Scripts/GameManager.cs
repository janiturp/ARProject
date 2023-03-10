using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;
    public int playerScore = 0;
    public int scoreToWin = 10;
    public GameObject scoreText;
    public GameObject victoryText;
    public GameObject debugText;

    public ARRaycastManager raycastManager;
    List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    public GameObject trashBinPrefab;

    Camera arCam;
    GameObject trashBin;
    bool trashBinSpawned = false;
    public GameObject bullet;
    GameObject bulletInstance;

    int colorIndex = 0;

    public Color[] colorList = new Color[4];
    private void Awake()
    {
        if (manager == null)
        {
            DontDestroyOnLoad(gameObject);
            manager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        trashBin = null;
        arCam = GameObject.Find("AR Camera").GetComponent<Camera>();

        colorList[0] = Color.cyan;
        colorList[1] = Color.green;
        colorList[2] = Color.blue;
        colorList[3] = Color.red;

    }

    // Update is called once per frame
    void Update()
    {
        scoreText.GetComponent<TMP_Text>().text = playerScore.ToString();

        if(playerScore >= 10)
        {
            victoryText.GetComponent<TMP_Text>().text = "You won!";
        }

        if (Input.touchCount == 0)
            return;

        RaycastHit hit;
        Ray ray = arCam.ScreenPointToRay(Input.GetTouch(0).position);

        if (raycastManager.Raycast(Input.GetTouch(0).position, hitList))
        {
            if(Input.GetTouch(0).phase == TouchPhase.Began && trashBin == null) 
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "TrashBin")
                    {
                        trashBin = hit.collider.gameObject;
                    }
                    else if (!trashBinSpawned)
                    {
                        SpawnTrashbin(hitList[0].pose.position);
                        trashBinSpawned = true;
                    }
                    else
                    {
                        ShootBullet();
                    }
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && trashBin != null)
            {
                trashBin.transform.position = hitList[0].pose.position;
            }

            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                trashBin = null;
            }
        }

        if (Input.GetTouch(0).phase == TouchPhase.Began && Input.touchCount == 2)
        {
            trashBin.GetComponent<Material>().color = colorList[colorIndex];
            colorIndex++;
            debugText.GetComponent<TMP_Text>().text = "Input stationary. colorIndex: " + colorIndex.ToString();

            if (colorIndex >= colorList.Length)
            {
                colorIndex = 0;
            }
        }
    }

    private void SpawnTrashbin(Vector3 position) 
    {
        trashBin = Instantiate(trashBinPrefab, position, Quaternion.identity);
    }

    private void ShootBullet()
    {
        bulletInstance = Instantiate(bullet, arCam.transform.position, Quaternion.identity);
        bulletInstance.GetComponent<Rigidbody>().velocity = arCam.transform.forward * 5;

    }
}

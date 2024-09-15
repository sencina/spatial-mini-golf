using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SpatialSys.UnitySDK;

public class BallController : MonoBehaviour
{   
    public float maxPower;
    public float changeAngleSpeed;
    public float lineLength;
    public Slider powerSlider;
    public TextMeshProUGUI puttCountLabel;
    public float minHoleTime;

    private LineRenderer line;
    private Rigidbody ball;
    private float angle;
    private float powerUpTime;
    private float power;
    private int putts;
    private float holeTime;
    private Vector3 lastPosition;

    void Awake()
    {
        ball = GetComponent<Rigidbody>();
        ball.maxAngularVelocity = 1000;
        line = GetComponent<LineRenderer>();
    }

    void Start()
{
    SpatialBridge.coreGUIService.DisplayToastMessage("Hello World");
}

    void Update()
    {
        if(ball.velocity.magnitude < 0.01f){
            if (Input.GetKey(KeyCode.A)){
            changeAngle(-1);
            }
            if (Input.GetKey(KeyCode.D)){
                changeAngle(1);
            }
            if (Input.GetKeyUp(KeyCode.Space)){
                putt();
            }
            if (Input.GetKey(KeyCode.Space)){
                powerUp();
            }
            updateLinePositions();
        }
        else {
            line.enabled = false;
        }
        
    }

    private void changeAngle(int direction){
        angle += changeAngleSpeed * Time.deltaTime * direction;
    }

    private void updateLinePositions(){
        if(holeTime == 0){
            line.enabled = true;
        }
        line.SetPosition(0,transform.position);
        line.SetPosition(1,transform.position + Quaternion.Euler(0,angle,0) * Vector3.forward * lineLength);
    }

    private void putt()
    {
        lastPosition = transform.position;
        ball.AddForce(Quaternion.Euler(0,angle,0) * Vector3.forward * maxPower * power, ForceMode.Impulse);
        power = 0;
        powerSlider.value = 0;
        powerUpTime = 0;
        putts += 1;
        puttCountLabel.text = putts.ToString();
    }

    private void powerUp(){
        powerUpTime += Time.deltaTime;
        power = Mathf.PingPong(powerUpTime,1);
        powerSlider.value = power;
    }

    private void OnTriggerStay(Collider other){
        if(other.tag == "Hole"){
            CountHoleTime();
        }
    }

    private void CountHoleTime(){
        holeTime += Time.deltaTime;
        if (holeTime >= minHoleTime){
            //termino el juego (metio el balon)
            Debug.Log("la puse y me fui");
            holeTime = 0;
        }
    }

    private void OnTriggerExit(Collider other){
        if(other.tag == "Hole"){
            LeftHole();
        }
    }

    private void LeftHole(){
        holeTime = 0;
    }

    private void OnCollisionEnter(Collision collision){
        if(collision.collider.tag == "Out Of Bounds"){
            transform.position = lastPosition;
            ball.velocity = Vector3.zero;
            ball.angularVelocity = Vector3.zero;
        }
    }

}

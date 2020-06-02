using UnityEngine;

public class CarController : MonoBehaviour {

    [SerializeField]
    private WheelJoint2D frontTire, backTire;

    [SerializeField]
    private float speed;

    private float movement, moveSpeed, fuel = 1, fuelConsumption = 0.1f;
    public float Fuel { get => fuel; set { fuel = value; } }

    public Vector3 StartPos { get; set; }

    private void Update() {
        //movement = Input.GetAxis("Horizontal");
        if(GameManager.Instance.GasBtnPressed) {
            movement += 0.009f;
            if(movement > 1f)
                movement = 1f;
        }
        else if(GameManager.Instance.BrakeBtnPressed) {
            movement -= 0.009f;
            if(movement < -1f)
                movement = -1f;
        }
        else if(!GameManager.Instance.GasBtnPressed && !GameManager.Instance.BrakeBtnPressed) {
            movement = 0;
        }
        moveSpeed = movement * speed;

        GameManager.Instance.FuelConsume();  //계속해서 연료가 소모됨
    }

    private void FixedUpdate() {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, StartPos.x, transform.position.x), transform.position.y);

        if(moveSpeed.Equals(0) || fuel <= 0) {   //버튼을 누르지 않거나 연료가 없을 경우 차를 멈춤
            frontTire.useMotor = false;
            backTire.useMotor = false;
        }
        else {
            frontTire.useMotor = true;
            backTire.useMotor = true;
            JointMotor2D motor = new JointMotor2D();
            motor.motorSpeed = moveSpeed;
            motor.maxMotorTorque = 10000;
            frontTire.motor = motor;
            backTire.motor = motor;
        }

        fuel -= fuelConsumption * Mathf.Abs(movement) * Time.fixedDeltaTime;
    }
}
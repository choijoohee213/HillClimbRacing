using UnityEngine;

public class CarController : MonoBehaviour {

    [SerializeField]
    private WheelJoint2D frontTire, backTire;

    [SerializeField]
    private float speed;

    private float movement, moveSpeed, fuel = 1, fuelConsumption = 0.1f;
    public float Fuel { get => fuel; set { fuel = value; } }

    public bool moveStop = false;

    public Vector3 StartPos { get; set; }

    private void Update() {
        //PC : movement = Input.GetAxis("Horizontal");
        //엔진 버튼 누를 시
        if(GameManager.Instance.GasBtnPressed) {
            movement += 0.009f;
            if(movement > 1f)
                movement = 1f;
        }

        //브레이크 버튼 누를 시
        else if(GameManager.Instance.BrakeBtnPressed) {
            movement -= 0.009f;
            if(movement < -1f)
                movement = -1f;
        }

        //아무 버튼도 누르지 않을 시
        else if(!GameManager.Instance.GasBtnPressed && !GameManager.Instance.BrakeBtnPressed) {
            movement = 0;
        }
        moveSpeed = movement * speed;

        GameManager.Instance.FuelConsume();  //연료 소모에 따라 여러가지를 갱신
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

        //게임 오버 시에 차량 속도 0으로
        if(GameManager.Instance.isDie && moveStop) {
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }

        //움직이는 만큼 계속해서 연료 소비
        fuel -= fuelConsumption * Mathf.Abs(movement) * Time.fixedDeltaTime;
    }
}
//====================================================================================
// I2C
#include <Wire.h>

#define SLAVE_ADDRESS 0x12
#define SERIAL_BAUD 57600 
//====================================================================================
// Interrupt and Rotary encoders
// Arduino Leonardo has 4 interrupt Pin, D3, D2, D0, D1
// D0, D1, D2, and D3 for two rotary encoders
int encoderLeftPin1 = 3; // interrupt 0
int encoderLeftPin2 = 2; // interrupt 1
int encoderRightPin1 = 0;// interrupt 2
int encoderRightPin2 = 1;// interrupt 3

// rotary encoders
volatile int leftLastEncoded = 0;
volatile int rightLastEncoded = 0;

volatile long encoderLeftValue = 0;
volatile long encoderRightValue = 0;

//====================================================================================
// Monster Motor Driver VNH2SP30
#define BRAKE 0
#define CW    1
#define CCW   2
#define CS_THRESHOLD 15   // Definition of safety current (Check: "1.3 Monster Shield Example").

//MOTOR Left
#define MOTOR_AL_PIN 7 // D7: CW for motor left
#define MOTOR_BL_PIN 8 // D8: CCW for motor left

//MOTOR Right
#define MOTOR_AR_PIN 4 // D4: CW for motor right
#define MOTOR_BR_PIN 9 // D9: CCW for motor right

// PWM D5, D6
#define PWM_MOTOR_L 5  
#define PWM_MOTOR_R 6  

// current sensor A2, A3
#define CURRENT_SEN_L A2
#define CURRENT_SEN_R A3

// Enable pin for two motors
#define EN_PIN_L A0
#define EN_PIN_R A1

#define MOTOR_L 0
#define MOTOR_R 1

//====================================================================================
// PID motor control
#include <PID_v1.h>
double kp = 5, ki = 1, kd = 0.01;
// input: current position (value of rotary encoder)
// output: result (where to go)
// setPoint: target position (position cmd from Feather)
double input = 0, output = 0, setPoint = 0;

PID leftPID(&input, &output, &setPoint, kp, ki, kd, DIRECT); // DIRECT was defined in PID_v1.h
PID rightPID(&input, &output, &setPoint, kp, ki, kd, DIRECT);

// some variables could be removed, but we have to check with driver shield first
short usSpeed = 150;  //default motor speed
// variables for handling command from Feather
unsigned short usMotor_Status = BRAKE;
String inputString = "";
bool stringComplete = false;
String cmdAngle = "";
String cmdSpeed = "";
long goToAngle = 0;
short goWithSpeed = 0;

//====================================================================================
//Command Table for Pins
//Motor 0:
//STOP : D7 0, D8 0 & D7 1, D7 1
//CCW : D7 0, D8 1 
//CW : D7 1, D8 0
//
//Motor 1:
//STOP : D4 0, D9 0 & D4 1, D9 1
//CCW : D4 0, D9 1 
//CW : D4 1, D9 0

void setup() {
  
  // I2C setup
  Wire.begin(SLAVE_ADDRESS);    // join I2C bus as a slave with address 1
  Wire.onReceive(receiveEvent); // register event

  Serial.begin(SERIAL_BAUD);

  // rotary encoder setup
  pinMode(encoderLeftPin1, INPUT); 
  pinMode(encoderLeftPin2, INPUT);
  pinMode(encoderRightPin1, INPUT); 
  pinMode(encoderRightPin2, INPUT);

  digitalWrite(encoderLeftPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderLeftPin2, HIGH); //turn pullup resistor on
  digitalWrite(encoderRightPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderRightPin2, HIGH); //turn pullup resistor on

  //call updateEncoder() when any high/low changed seen
  //on interrupt 0 (pin 2), or interrupt 1 (pin 3)
  //on interrupt 2 (pin 0), or interrupt 3 (pin 1) 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin1), updateLeftEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin2), updateLeftEncoder, CHANGE);
  attachInterrupt(digitalPinToInterrupt(encoderRightPin1), updateRightEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderRightPin2), updateRightEncoder, CHANGE);

  // DC motor setup
  pinMode(MOTOR_AL_PIN, OUTPUT);
  pinMode(MOTOR_BL_PIN, OUTPUT);

  pinMode(MOTOR_AR_PIN, OUTPUT);
  pinMode(MOTOR_BR_PIN, OUTPUT);

  pinMode(PWM_MOTOR_L, OUTPUT);
  pinMode(PWM_MOTOR_R, OUTPUT);

  pinMode(CURRENT_SEN_L, OUTPUT);
  pinMode(CURRENT_SEN_R, OUTPUT);  

  pinMode(EN_PIN_L, OUTPUT);
  pinMode(EN_PIN_R, OUTPUT);

  // PID control setup
  leftPID.SetMode(AUTOMATIC);
  leftPID.SetOutputLimits(-255, 255);
  rightPID.SetMode(AUTOMATIC);
  rightPID.SetOutputLimits(-255, 255);
  
}

void loop() {

  // for debug
  //Serial.print(encoderLeftValue); Serial.print(" ");
  //Serial.println(encoderRightValue);

}

void receiveEvent(int count) {
  Serial.println("Receive Data:");
  while(Wire.available()) {
//    Serial.print((char) Wire.read());
	  digitalWrite(EN_PIN_L, HIGH);
	  digitalWrite(EN_PIN_R, HIGH);
    char inChar = (char) Wire.read();
    inputString += inChar;
    if (inChar == '\n') {
      stringComplete = true;
    }
  }
  // start spliting cmd
  if (stringComplete) {
    // which motor
    if (inputString.startsWith("L")) {
      inputString = inputString.substring(2);
      // split cmd into angle and speed
      for (int i = 0; i < inputString.length(); i++)
      {
        if (inputString.substring(i, i + 1) == " ")
        {	
          cmdAngle = inputString.substring(0, i);
          cmdSpeed = inputString.substring(i + 1);
  	      break;
        }
      }
  	  // send target position to setPoint
  	  goToAngle = cmdAngle.toInt();
  	  setPoint = (double) goToAngle;
      // send current position of left motor to input
      input = (double) encoderLeftValue;
      leftPID.Compute();
      goWithSpeed = (short)cmdSpeed.toInt();

      // need to test here!!!!!!!!! it seems goWithSpeed didn't use
      if (output > 0) {
	      motorGo(MOTOR_L, CW, output);
	    }
	    else {
        motorGo(MOTOR_L, CCW, abs(output));
	    }
	  }
	  else {
      inputString = inputString.substring(2);
      // split cmd into angle and speed
      for (int i = 0; i < inputString.length(); i++)
      {
        if (inputString.substring(i, i + 1) == " ")
        {	
          cmdAngle = inputString.substring(0, i);
          cmdSpeed = inputString.substring(i + 1);
    	    break;
        }
      }
  	  // send target position to setPoint
  	  goToAngle = cmdAngle.toInt();
  	  setPoint = (double) goToAngle;
  	  // send current position of right motor to input
      input = (double) encoderRightValue;
      rightPID.Compute();
      goWithSpeed = (short)cmdSpeed.toInt();

      // need to test here!!!!!!!!! it seems goWithSpeed didn't use
      if (output > 0) {
  	    motorGo(MOTOR_R, CW, output);
      }
  	  else {
  	    motorGo(MOTOR_R, CCW, abs(output));
  	  }
  
    }
  inputString = "";
  stringComplete = false;

  }
  Serial.println("\n");
}

void updateLeftEncoder() {
  int MSB = digitalRead(encoderLeftPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderLeftPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (leftLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderLeftValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderLeftValue --;

  leftLastEncoded = encoded; //store this value for next time
}

void updateRightEncoder() {
  int MSB = digitalRead(encoderRightPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderRightPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (rightLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderRightValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderRightValue --;

  rightLastEncoded = encoded; //store this value for next time
}


void motorGo(uint8_t motor, uint8_t direct, uint8_t pwm)         
{
  // Function that controls the variables:
  //   motor (0 or 1)
  //   direction (cw or ccw)
  //   pwm (0 to 255)
  
  if(motor == MOTOR_L)
  {
    if(direct == CW)
    {
      digitalWrite(MOTOR_AL_PIN, LOW); 
      digitalWrite(MOTOR_BL_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(MOTOR_AL_PIN, HIGH);
      digitalWrite(MOTOR_BL_PIN, LOW);      
    }
    else
    {
      digitalWrite(MOTOR_AL_PIN, LOW);
      digitalWrite(MOTOR_BL_PIN, LOW);            
    }
    
    analogWrite(PWM_MOTOR_L, pwm); 
  }
  else if(motor == MOTOR_R)
  {
    if(direct == CW)
    {
      digitalWrite(MOTOR_AR_PIN, LOW);
      digitalWrite(MOTOR_BR_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(MOTOR_AR_PIN, HIGH);
      digitalWrite(MOTOR_BR_PIN, LOW);      
    }
    else
    {
      digitalWrite(MOTOR_AR_PIN, LOW);
      digitalWrite(MOTOR_BR_PIN, LOW);            
    }
    
    analogWrite(PWM_MOTOR_R, pwm);
  }
}


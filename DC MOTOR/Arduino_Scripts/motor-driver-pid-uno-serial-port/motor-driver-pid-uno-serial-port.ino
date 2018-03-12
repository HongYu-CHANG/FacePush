/* This sketch supports NILab Project FacePush, as described below:
 * - UNO receives message directly from PC by serial port.
 * - uses Arduino UNO and Monster Motor Shield VNH2SP30 to control 2 DC motors.
 * - control DC motors by rotary encoders and PID
 * - use interrupt pins D2, D3, D10, and D11 (by PinChangeInterrupt library)
 *   to track the value of rotary encoders.
 * 
 *   23.02.18 wjtseng93
 */
#define SERIAL_BAUD 57600
//====================================================================================
// Monster Motor Driver VNH2SP30
#define BRAKE 0
#define CW    1
#define CCW   2
#define CS_THRESHOLD 15   // Definition of safety current (Check: "1.3 Monster Shield Example").

//MOTOR 1
#define MOTOR_A1_PIN 7
#define MOTOR_B1_PIN 8

//MOTOR 2
#define MOTOR_A2_PIN 4
#define MOTOR_B2_PIN 9

#define PWM_MOTOR_1 5
#define PWM_MOTOR_2 6

#define CURRENT_SEN_1 A2
#define CURRENT_SEN_2 A3

#define EN_PIN_1 A0
#define EN_PIN_2 A1

#define MOTOR_1 0
#define MOTOR_2 1

//default motor speed and motor state
short speedLeft = 255;
short speedRight = 255;
unsigned short usMotor_Status = BRAKE;
int motorParameters[4]; // angle left, speed left, angle right, speed right

//====================================================================================
// Interrupt and Rotary encoder
// Arduino UNO has 2 interrupt pins, D2, D3
// use PinChangeInt to make D10, D11 into interrupt pins
#include <PinChangeInt.h>
int encoderLeftPin1 = 3; // interrupt 0
int encoderLeftPin2 = 2; // interrupt 1
int encoderRightPin1 = 10; // use PinChangeInt make it into interrupt pin
int encoderRightPin2 = 11;

// rotary encoders
volatile long leftLastEncoded = 0;
volatile long rightLastEncoded = 0;

volatile long encoderLeftValue = 0;
volatile long encoderRightValue = 0;

//====================================================================================
// PID motor control
#include <PID_v1.h>
double kp = 0.75, ki = 0.09, kd = 0.01;
// input: current position (value of rotary encoder)
// output: result (where to go)
// setPoint: target position (position cmd from Feather)
double inputLeft = 0, outputLeft = 0, setPointLeft = 0;
double inputRight = 0, outputRight = 0, setPointRight = 0;
// DIRECT was defined in PID_v1.h
PID leftPID(&inputLeft, &outputLeft, &setPointLeft, kp, ki, kd, DIRECT); 
PID rightPID(&inputRight, &outputRight, &setPointRight, kp, ki, kd, DIRECT);

String inputString = "";
bool stringComplete = false;
int timer = 0;
void setup()                         
{
  Serial.begin(SERIAL_BAUD);

  // DC motor setup
  pinMode(MOTOR_A1_PIN, OUTPUT);
  pinMode(MOTOR_B1_PIN, OUTPUT);

  pinMode(MOTOR_A2_PIN, OUTPUT);
  pinMode(MOTOR_B2_PIN, OUTPUT);

  pinMode(PWM_MOTOR_1, OUTPUT);
  pinMode(PWM_MOTOR_2, OUTPUT);

  pinMode(CURRENT_SEN_1, OUTPUT);
  pinMode(CURRENT_SEN_2, OUTPUT);  

  pinMode(EN_PIN_1, OUTPUT);
  pinMode(EN_PIN_2, OUTPUT);

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
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin1), updateLeftEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin2), updateLeftEncoder, CHANGE);
  attachPinChangeInterrupt(encoderRightPin1, updateRightEncoder, CHANGE); 
  attachPinChangeInterrupt(encoderRightPin2, updateRightEncoder, CHANGE);

  // PID control setup
  leftPID.SetMode(AUTOMATIC);
  rightPID.SetMode(AUTOMATIC);
  leftPID.SetOutputLimits(-speedLeft, speedLeft);
  rightPID.SetOutputLimits(-speedRight, speedRight);
}

void loop() 
{
//  Uncomment below to check values of PID  
  Serial.print(inputLeft); Serial.print(" ");
  Serial.print(setPointLeft); Serial.print(" ");
  Serial.print(outputLeft); Serial.print(" ");
  Serial.print(inputRight); Serial.print(" ");
  Serial.print(setPointRight); Serial.print(" ");
  Serial.println(outputRight); Serial.print(" ");

  inputLeft = encoderLeftValue;
  inputRight = encoderRightValue;
  // Update PID control
  int currentTime = millis();

  motorPIDControl(&encoderLeftValue, &setPointLeft, &outputLeft, &leftPID, EN_PIN_1, MOTOR_1);
  motorPIDControl(&encoderRightValue, &setPointRight, &outputRight, &rightPID, EN_PIN_2, MOTOR_2);

  if (inputRight == setPointRight && inputLeft == setPointLeft) {
    timer = 0;
  }
  else {
    timer += millis() - currentTime;
  }
  if (timer > 90) {
    setPointLeft = inputLeft;
    outputLeft = 0;
    setPointRight = inputRight;
    outputRight = 0;
  }


  
//  Uncomment below to send data in serial port
  while(Serial.available())
  {
    digitalWrite(EN_PIN_1, HIGH);
    digitalWrite(EN_PIN_2, HIGH); 
    char c = Serial.read();
    inputString += c;
    if (c == '\n') {
      stringComplete = true;   
    }
  }
  if (stringComplete) {
//    Serial.println(inputString);
    for (int i = 0; i < 3; i++) {
      for (int j = 0; j < inputString.length(); j++) {
        if (inputString.substring(j, j + 1) == " ")
        {
          motorParameters[i] = inputString.substring(0, j).toInt();
          if (i == 2) {
            motorParameters[i + 1] = inputString.substring(j + 1).toInt();
          }
          else {
            inputString = inputString.substring(j + 1);
          }
          break;
	      }
      }
    }
    setPointLeft = (double) motorParameters[0];
    speedLeft = (short) motorParameters[1];
    setPointRight = (double) motorParameters[2];
    speedRight = (short) motorParameters[3];
    leftPID.SetOutputLimits(-speedLeft, speedLeft);
    rightPID.SetOutputLimits(-speedRight, speedRight);
//    for (int i = 0; i < 4; i++) {
//      Serial.println(motorParameters[i]);
//    }
    stringComplete = false;
    inputString = "";
  } 
}



// Function that controls the variables:
// motor(0 or 1), direction (cw or ccw), and pwm (0 to 255)
void motorGo(uint8_t motor, uint8_t direct, uint8_t pwm) 
{
  if(motor == MOTOR_1)
  {
    if(direct == CW)
    {
      digitalWrite(MOTOR_A1_PIN, LOW); 
      digitalWrite(MOTOR_B1_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(MOTOR_A1_PIN, HIGH);
      digitalWrite(MOTOR_B1_PIN, LOW);      
    }
    else
    {
      digitalWrite(MOTOR_A1_PIN, LOW);
      digitalWrite(MOTOR_B1_PIN, LOW);            
    }
    
    analogWrite(PWM_MOTOR_1, pwm); 
  }
  else if(motor == MOTOR_2)
  {
    if(direct == CW)
    {
      // CW input for right motor is actually CCW
      digitalWrite(MOTOR_A2_PIN, HIGH);
      digitalWrite(MOTOR_B2_PIN, LOW);
    }
    else if(direct == CCW)
    {
      // CCW input for right motor is actually CW
      digitalWrite(MOTOR_A2_PIN, LOW);
      digitalWrite(MOTOR_B2_PIN, HIGH);      
    }
    else
    {
      digitalWrite(MOTOR_A2_PIN, LOW);
      digitalWrite(MOTOR_B2_PIN, LOW);            
    }
    
    analogWrite(PWM_MOTOR_2, pwm);
  }
}

void updateLeftEncoder() {
  long MSB = digitalRead(encoderLeftPin1); //MSB = most significant bit
  long LSB = digitalRead(encoderLeftPin2); //LSB = least significant bit

  long encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  long sum  = (leftLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderLeftValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderLeftValue --;

  leftLastEncoded = encoded; //store this value for next time
}

void updateRightEncoder() {
  long MSB = digitalRead(encoderRightPin1); //MSB = most significant bit
  long LSB = digitalRead(encoderRightPin2); //LSB = least significant bit

  long encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  long sum  = (rightLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderRightValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderRightValue --;

  rightLastEncoded = encoded; //store this value for next time
}

void motorPIDControl(volatile long *encoderValue, double *setPoint, double *output, PID *motorPID, uint8_t EN_PIN, uint8_t MOTOR) {
    motorPID->Compute();

    if (*output > 0) {
      motorGo(MOTOR, CW, *output);
    }
    else {
      motorGo(MOTOR, CCW, abs(*output));
    }
    if (*encoderValue == *setPoint) {
      motorGo(MOTOR, BRAKE, 0);
      *output = 0;
    }
}

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

short usSpeed = 150;  //default motor speed
unsigned short usMotor_Status = BRAKE;

//====================================================================================
// Interrupt and Rotary encoders
// Arduino Leonardo has 4 interrupt Pin, D3, D2, D0, D1
// D0, D1, D2, and D3 for two rotary encoders
int encoderLeftPin1 = 3; // interrupt 0
int encoderLeftPin2 = 2; // interrupt 1
//int encoderRightPin1 = 0;// interrupt 2
//int encoderRightPin2 = 1;// interrupt 3

// rotary encoders
volatile int leftLastEncoded = 0;
//volatile int rightLastEncoded = 0;

volatile long encoderLeftValue = 0;
//volatile long encoderRightValue = 0;

//====================================================================================
// PID motor control
#include <PID_v1.h>
double kp = 0.5, ki = 0.1, kd = 0.031;
// input: current position (value of rotary encoder)
// output: result (where to go)
// setPoint: target position (position cmd from Feather)
double input = 0, output = 0, setPoint = 0;

PID leftPID(&input, &output, &setPoint, kp, ki, kd, DIRECT); // DIRECT was defined in PID_v1.h
//PID rightPID(&input, &output, &setPoint, kp, ki, kd, DIRECT);

String inputString = "";
bool stringComplete = false;


void setup()                         
{
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
//  pinMode(encoderRightPin1, INPUT); 
//  pinMode(encoderRightPin2, INPUT);

  digitalWrite(encoderLeftPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderLeftPin2, HIGH); //turn pullup resistor on
//  digitalWrite(encoderRightPin1, HIGH); //turn pullup resistor on
//  digitalWrite(encoderRightPin2, HIGH); //turn pullup resistor on

  //call updateEncoder() when any high/low changed seen
  //on interrupt 0 (pin 2), or interrupt 1 (pin 3)
  //on interrupt 2 (pin 0), or interrupt 3 (pin 1) 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin1), updateLeftEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin2), updateLeftEncoder, CHANGE);
//  attachInterrupt(digitalPinToInterrupt(encoderRightPin1), updateRightEncoder, CHANGE); 
//  attachInterrupt(digitalPinToInterrupt(encoderRightPin2), updateRightEncoder, CHANGE);
  
  // PID control setup
  leftPID.SetMode(AUTOMATIC);
//  rightPID.SetMode(AUTOMATIC);
  leftPID.SetOutputLimits(-255, 255);
//  rightPID.SetOutputLimits(-255, 255);

  Serial.begin(9600);              // Initiates the serial to do the monitoring 
  while(!Serial);
  Serial.println("Begin motor control");
  Serial.println(); //Print function list for user selection
  Serial.println("Enter number for control option:");
  Serial.println("1. STOP");
  Serial.println("2. FORWARD");
  Serial.println("3. REVERSE");
  Serial.println("4. READ CURRENT");
  Serial.println("+. INCREASE SPEED");
  Serial.println("-. DECREASE SPEED");
  Serial.println();

}






void loop() 
{
//  char user_input;
  input = encoderLeftValue;
  leftPID.Compute();
  if (output > 0) {
    motorGo(MOTOR_1, CW, output);
  }
  else {
    motorGo(MOTOR_1, CCW, abs(output));
  }
  Serial.print(input); Serial.print(" ");
  Serial.print(setPoint); Serial.print(" ");
  Serial.println(output);

  if (encoderLeftValue == setPoint) {

    motorGo(MOTOR_1, BRAKE, 0);
  }
  
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
    setPoint = inputString.toInt();
    stringComplete = false;
    inputString = "";
  }
//    user_input = Serial.read(); //Read user input and trigger appropriate function
//     
//    if (user_input =='1')
//    {
//       Stop();
//    }
//    else if(user_input =='2')
//    {
//      Forward();
//    }
//    else if(user_input =='3')
//    {
//      Reverse();
//    }
//    else if(user_input =='+')
//    {
//      IncreaseSpeed();
//    }
//    else if(user_input =='-')
//    {
//      DecreaseSpeed();
//    }
//    else
//    {
//      Serial.println("Invalid option entered.");
//    }
      
}

void Stop()
{
  Serial.println("Stop");
  usMotor_Status = BRAKE;
  motorGo(MOTOR_1, usMotor_Status, 0);
  motorGo(MOTOR_2, usMotor_Status, 0);
}

void Forward()
{
  Serial.println("Forward");
  usMotor_Status = CW;
  motorGo(MOTOR_1, usMotor_Status, usSpeed);
  motorGo(MOTOR_2, usMotor_Status, usSpeed);
}

void Reverse()
{
  Serial.println("Reverse");
  usMotor_Status = CCW;
  motorGo(MOTOR_1, usMotor_Status, usSpeed);
  motorGo(MOTOR_2, usMotor_Status, usSpeed);
}

void IncreaseSpeed()
{
  usSpeed = usSpeed + 10;
  if(usSpeed > 255)
  {
    usSpeed = 255;  
  }
  
  Serial.print("Speed +: ");
  Serial.println(usSpeed);

  motorGo(MOTOR_1, usMotor_Status, usSpeed);
  motorGo(MOTOR_2, usMotor_Status, usSpeed);  
}

void DecreaseSpeed()
{
  usSpeed = usSpeed - 10;
  if(usSpeed < 0)
  {
    usSpeed = 0;  
  }
  
  Serial.print("Speed -: ");
  Serial.println(usSpeed);

  motorGo(MOTOR_1, usMotor_Status, usSpeed);
  motorGo(MOTOR_2, usMotor_Status, usSpeed);  
}

void motorGo(uint8_t motor, uint8_t direct, uint8_t pwm)         //Function that controls the variables: motor(0 ou 1), direction (cw ou ccw) e pwm (entra 0 e 255);
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
      digitalWrite(MOTOR_A2_PIN, LOW);
      digitalWrite(MOTOR_B2_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(MOTOR_A2_PIN, HIGH);
      digitalWrite(MOTOR_B2_PIN, LOW);      
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
  int MSB = digitalRead(encoderLeftPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderLeftPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (leftLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderLeftValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderLeftValue --;

  leftLastEncoded = encoded; //store this value for next time
}

//void updateRightEncoder() {
//  int MSB = digitalRead(encoderRightPin1); //MSB = most significant bit
//  int LSB = digitalRead(encoderRightPin2); //LSB = least significant bit
//
//  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
//  int sum  = (rightLastEncoded << 2) | encoded; //adding it to the previous encoded value
//
//  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderRightValue ++;
//  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderRightValue --;
//
//  rightLastEncoded = encoded; //store this value for next time
//}



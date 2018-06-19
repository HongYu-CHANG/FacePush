
#include <PinChangeInt.h>
#include <PID_v1.h>
#define SERIAL_BAUD 9600
#define DEBUG 0 

//All Ｍotor Parameter
#define BRAKE 0
#define CW    1
#define CCW   2
int AllMotor_Parameters[4]; // angle left, speed left, angle right, speed right
//PID Parameter
double kp = 0.75, ki = 0.09, kd = 0.01;
//Read String Parameter
String ReadString_Input = "";
bool ReadString_Complete = false;
int MotorTimer = 0;

// Left default setting(Motor，Rotary Encoder，PID)
#define LeftMotor_A1_PIN 7
#define LeftMotor_B1_PIN 8
#define LeftMotor_PWM 5
#define LeftMotor_EnablePin A0
#define LeftMotor 1
short LeftMotor_Speed = 255;
int LeftEncoder_Pin1 = 3; // interrupt 0
int LeftEncoder_Pin2 = 2; // interrupt 1
volatile long LeftEncoder_LastValue = 0;
volatile long LeftEncoder_Value = 0;
double LeftPID_Input = 0;// input: current position (value of rotary encoder)
double LeftPID_Output = 0;// output: result (where to go)
double LeftPID_Target = 0;// Target: target position (position cmd from Feather)
PID LeftPID_Contorller(&LeftPID_Input, &LeftPID_Output, &LeftPID_Target, kp, ki, kd, DIRECT); 

//Right default setting(Motor，Rotary Encoder，PID)
#define RightMotor_A2_PIN 4
#define RightMotor_B2_PIN 9
#define RightMotor_PWM 6
#define RightMotor_EnablePin A1
#define RightMotor 2
short RightMotor_Speed = 255;
int RightEncoder_Pin1 = 10;//use PinChangeInt to simulate Interript
int RightEncoder_Pin2 = 11;//use PinChangeInt to simulate Interript
volatile long RightEncoder_LastValue = 0;
volatile long RightEncoder_Value = 0;
double RightPID_Input = 0;// input: current position (value of rotary encoder)
double RightPID_Output = 0;// output: result (where to go)
double RightPID_Target = 0;// Target: target position (position cmd from Feather)
PID RightPID_Contorller(&RightPID_Input, &RightPID_Output, &RightPID_Target, kp, ki, kd, DIRECT);

void setup()                         
{
  Serial.begin(SERIAL_BAUD);
  LeftSetup();
  RightSetup();
}

void loop() 
{
//  Uncomment below to check values of PID
#if DEBUG == 1  
  Serial.print(LeftPID_Input); Serial.print(" ");
  Serial.print(LeftPID_Target); Serial.print(" ");
  Serial.print(LeftPID_Output); Serial.print(" ");
  Serial.print(RightPID_Input); Serial.print(" ");
  Serial.print(RightPID_Target); Serial.print(" ");
  Serial.print(RightPID_Output); Serial.print(" ");
  Serial.print(MotorTimer); Serial.println(" ");
#else
  delay(40);
  //Adafruit_BicolorMatrix matrix = Adafruit_BicolorMatrix();
#endif
  

  LeftPID_Input = LeftEncoder_Value;
  motorPIDControl(&LeftEncoder_Value, &LeftPID_Target, &LeftPID_Output, &LeftPID_Contorller, LeftMotor_EnablePin, LeftMotor);
  RightPID_Input = RightEncoder_Value;
  motorPIDControl(&RightEncoder_Value, &RightPID_Target, &RightPID_Output, &RightPID_Contorller, RightMotor_EnablePin, RightMotor);

  if (RightPID_Input != RightPID_Target && LeftPID_Input != LeftPID_Target) MotorTimer++;
  if (MotorTimer > 45) 
  {
    digitalWrite(LeftMotor_EnablePin, LOW);
    digitalWrite(RightMotor_EnablePin, LOW);
  }
  
  while(Serial.available())//Read Data
  {
    digitalWrite(LeftMotor_EnablePin, HIGH);
    digitalWrite(RightMotor_EnablePin, HIGH); 
    MotorTimer = 0;
    char c = Serial.read();
    ReadString_Input += c;
    if (c == '\n')ReadString_Complete = true;   
  }
  if (ReadString_Complete) 
  {
    for (int i = 0; i < 4; i++) 
    {
      for (int j = 0; j < ReadString_Input.length(); j++) 
      {
        if (ReadString_Input.substring(j, j + 1) == " " || ReadString_Input.substring(j, j + 1) == "\n")
        {
          AllMotor_Parameters[i] = ReadString_Input.substring(0, j).toInt();
          ReadString_Input = ReadString_Input.substring(j + 1);
          break;
	      }
      }
    }
    LeftPID_Target = (double) AllMotor_Parameters[0];
    LeftMotor_Speed = (short) AllMotor_Parameters[1];
    LeftPID_Contorller.SetOutputLimits(-LeftMotor_Speed, LeftMotor_Speed);
    
    RightPID_Target = (double) AllMotor_Parameters[2];
    RightMotor_Speed = (short) AllMotor_Parameters[3];
    RightPID_Contorller.SetOutputLimits(-RightMotor_Speed, RightMotor_Speed);
    ReadString_Complete = false;
    ReadString_Input = "";
  } 
}

void motorGo(uint8_t motor, uint8_t direct, uint8_t pwm) 
{
  if(motor == LeftMotor)
  {
    if(direct == CW)
    {
      digitalWrite(LeftMotor_A1_PIN, LOW); 
      digitalWrite(LeftMotor_B1_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(LeftMotor_A1_PIN, HIGH);
      digitalWrite(LeftMotor_B1_PIN, LOW);      
    }
    else
    {
      digitalWrite(LeftMotor_A1_PIN, LOW);
      digitalWrite(LeftMotor_B1_PIN, LOW);            
    }
    
    analogWrite(LeftMotor_PWM, pwm); 
  }
  else if(motor == RightMotor)
  {
    if(direct == CW)
    {
      // CW input for right motor is actually CCW
      digitalWrite(RightMotor_A2_PIN, HIGH);
      digitalWrite(RightMotor_B2_PIN, LOW);
    }
    else if(direct == CCW)
    {
      // CCW input for right motor is actually CW
      digitalWrite(RightMotor_A2_PIN, LOW);
      digitalWrite(RightMotor_B2_PIN, HIGH);      
    }
    else
    {
      digitalWrite(RightMotor_A2_PIN, LOW);
      digitalWrite(RightMotor_B2_PIN, LOW);            
    }
    
    analogWrite(RightMotor_PWM, pwm);
  }
}

void motorPIDControl(volatile long *encoderValue, double *setPoint, double *output, PID *motorPID, uint8_t EN_PIN, uint8_t Motor) 
{
    motorPID->Compute();

    if (*output > 0) 
    {
      motorGo(Motor, CW, *output);
    }
    else if (*output < 0)
    {
      motorGo(Motor, CCW, abs(*output));
    }
    if (*encoderValue == *setPoint) {
      
      motorGo(Motor, BRAKE, 0);
      *output = 0;
    }
}

void LeftSetup()
{
  // Left setup (Motor，Rotary Encoder，PID)
  pinMode(LeftMotor_A1_PIN, OUTPUT);
  pinMode(LeftMotor_B1_PIN, OUTPUT);
  pinMode(LeftMotor_PWM, OUTPUT);
  pinMode(LeftMotor_EnablePin, OUTPUT);
  pinMode(LeftEncoder_Pin1, INPUT); 
  pinMode(LeftEncoder_Pin2, INPUT);
  digitalWrite(LeftEncoder_Pin1, HIGH); //turn pullup resistor on
  digitalWrite(LeftEncoder_Pin2, HIGH); //turn pullup resistor on
  attachInterrupt(digitalPinToInterrupt(LeftEncoder_Pin1), LeftEncoder_Update, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(LeftEncoder_Pin2), LeftEncoder_Update, CHANGE);
  RightPID_Contorller.SetMode(AUTOMATIC);
  RightPID_Contorller.SetOutputLimits(-RightMotor_Speed, RightMotor_Speed);
}

void LeftEncoder_Update() 
{
  long MSB = digitalRead(LeftEncoder_Pin1); //MSB = most significant bit
  long LSB = digitalRead(LeftEncoder_Pin2); //LSB = least significant bit

  long encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  long sum  = (LeftEncoder_LastValue << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) LeftEncoder_Value ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) LeftEncoder_Value --;

  LeftEncoder_LastValue = encoded; //store this value for next time
}


void RightSetup()
{
  // Right setup (Motor，Rotary Encoder，PID)
  pinMode(RightMotor_A2_PIN, OUTPUT);
  pinMode(RightMotor_B2_PIN, OUTPUT);
  pinMode(RightMotor_PWM, OUTPUT);
  pinMode(RightMotor_EnablePin, OUTPUT);
  pinMode(RightEncoder_Pin1, INPUT); 
  pinMode(RightEncoder_Pin2, INPUT);
  digitalWrite(RightEncoder_Pin1, HIGH); //turn pullup resistor on
  digitalWrite(RightEncoder_Pin2, HIGH); //turn pullup resistor on
  attachPinChangeInterrupt(RightEncoder_Pin1, RightEncoder_Update, CHANGE); 
  attachPinChangeInterrupt(RightEncoder_Pin2, RightEncoder_Update, CHANGE);
  LeftPID_Contorller.SetMode(AUTOMATIC);
  LeftPID_Contorller.SetOutputLimits(-LeftMotor_Speed, LeftMotor_Speed);
}

void RightEncoder_Update() 
{
  long MSB = digitalRead(RightEncoder_Pin1); //MSB = most significant bit
  long LSB = digitalRead(RightEncoder_Pin2); //LSB = least significant bit

  long encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  long sum  = (RightEncoder_LastValue << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) RightEncoder_Value ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) RightEncoder_Value --;

  RightEncoder_LastValue = encoded; //store this value for next time
}

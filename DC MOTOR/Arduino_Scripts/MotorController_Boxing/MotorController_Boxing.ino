
#include <PinChangeInt.h>
#include <PID_v1.h>
#define SERIAL_BAUD 115200
#define DEBUG  0

//All Ｍotor Parameter
int AllMotor_Parameters[4] = {0}; // angle left, speed left, angle right, speed right
//Read String Parameter
String ReadString_Input = "";

// Left default setting(Motor，Rotary Encoder，PID)
#define LeftMotor_A1_PIN 8
#define LeftMotor_B1_PIN 7
#define LeftMotor_PWM 5
#define LeftMotor_EnablePin A0
#define LeftMotor 1
double LeftMotor_Speed = 255;
int LeftEncoder_Pin1 = 3; // interrupt 0
int LeftEncoder_Pin2 = 2; // interrupt 1
volatile long LeftEncoder_LastValue = 0;
volatile long LeftEncoder_Value = 0;
double LeftPID_Input = 0;// input: current position (value of rotary encoder)
double LeftPID_Output = 0;// output: result (where to go)
double LeftPID_Target = 0;// Target: target position (position cmd from Feather)
double Left_kp = 0.8, Left_ki = 0.09, Left_kd = 0.05; //PID Parameter
PID LeftPID_Contorller(&LeftPID_Input, &LeftPID_Output, &LeftPID_Target, Left_kp, Left_ki, Left_kd, DIRECT); 

//Right default setting(Motor，Rotary Encoder，PID)
#define RightMotor_A2_PIN 4
#define RightMotor_B2_PIN 9
#define RightMotor_PWM 6
#define RightMotor_EnablePin A1
#define RightMotor 2
double RightMotor_Speed = 255;
int RightEncoder_Pin1 = 10;//use PinChangeInt to simulate Interript
int RightEncoder_Pin2 = 11;//use PinChangeInt to simulate Interript
volatile long RightEncoder_LastValue = 0;
volatile long RightEncoder_Value = 0;
double RightPID_Input = 0;// input: current position (value of rotary encoder)
double RightPID_Output = 0;// output: result (where to go)
double RightPID_Target = 0;// Target: target position (position cmd from Feather)
double Right_kp = 0.8, Right_ki = 0.09, Right_kd = 0.05; //PID Parameter
PID RightPID_Contorller(&RightPID_Input, &RightPID_Output, &RightPID_Target, Right_kp, Right_ki, Right_kd, DIRECT);
  
void setup()                         
{
  Serial.begin(SERIAL_BAUD);
  LeftInitialSetting();
  RightInitialSetting();
}

void loop() 
{
  //  DEBUG == 1 below to check values of PID
  #if DEBUG == 1  
    Serial.print("L: ");
    Serial.print(LeftPID_Input); Serial.print(" ");
    Serial.print(LeftPID_Target); Serial.print(" ");
    Serial.print(LeftPID_Output); Serial.print(" ");
    Serial.print(digitalRead(LeftMotor_A1_PIN)); Serial.print(" ");
    Serial.print(digitalRead(LeftMotor_B1_PIN)); Serial.print(" ");
    Serial.print(digitalRead(LeftMotor_EnablePin)); Serial.print(" ");
    Serial.print("R: ");
    Serial.print(RightPID_Input); Serial.print(" ");
    Serial.print(RightPID_Target); Serial.print(" ");
    Serial.print(RightPID_Output); Serial.print(" ");
    Serial.print(digitalRead(RightMotor_A2_PIN)); Serial.print(" ");
    Serial.print(digitalRead(RightMotor_B2_PIN)); Serial.print(" ");
    Serial.print("Order: ");
    Serial.print(AllMotor_Parameters[0]); Serial.print(" ");
    Serial.print(AllMotor_Parameters[1]); Serial.println(" ");
  #else DEBUG == 0
//    Serial.print(LeftPID_Output); Serial.print(" ");
//    Serial.print(MotorCounter); Serial.println(" ");
    delay(10);
  #endif

  LeftPID_Input = LeftEncoder_Value;
  PID_Calculation(&LeftPID_Output, &LeftPID_Contorller, LeftMotor);
  RightPID_Input = RightEncoder_Value;
  PID_Calculation(&RightPID_Output, &RightPID_Contorller, RightMotor);

  //Control Motor noise
  if (LeftPID_Output == 255) 
  {
      LeftMotor_Speed = 200;
  }
   if (RightPID_Output == 255) 
  {
      RightMotor_Speed = 200; 
  }
  
  //Read Data and Handle Data
  int i = 0;
  while(Serial.available())
  {
    digitalWrite(LeftMotor_EnablePin, HIGH);
    digitalWrite(RightMotor_EnablePin, HIGH); 
    char c = Serial.read();
    if (c == ' ' || c == '\n')
    {
      AllMotor_Parameters[i] = ReadString_Input.toInt();
      i++;
      ReadString_Input = "";
    }
    else
    {
      ReadString_Input += c;
    }
    LeftPID_Target = AllMotor_Parameters[0];
    RightPID_Target = AllMotor_Parameters[1];
    if(AllMotor_Parameters[0] < -10)
    {
      digitalWrite(LeftMotor_EnablePin, LOW);
    }
    if(AllMotor_Parameters[1] < -10)
    {
      digitalWrite(RightMotor_EnablePin, LOW);
    }

    LeftMotor_Speed = 255;
    RightMotor_Speed = 255;
    Serial.flush();
  }
   LeftPID_Contorller.SetOutputLimits(-LeftMotor_Speed, LeftMotor_Speed);
   RightPID_Contorller.SetOutputLimits(-RightMotor_Speed, RightMotor_Speed);
}

void motorAction(uint8_t motor, uint8_t pwm, int PinA_Value, int PinB_Value) 
{
  if(motor == LeftMotor)
  {
    digitalWrite(LeftMotor_A1_PIN, PinA_Value); 
    digitalWrite(LeftMotor_B1_PIN, PinB_Value);
    analogWrite(LeftMotor_PWM, pwm);
  }
  else if(motor == RightMotor)
  {
    digitalWrite(RightMotor_A2_PIN, PinA_Value);
    digitalWrite(RightMotor_B2_PIN, PinB_Value);
    analogWrite(RightMotor_PWM, pwm);
  }
}

void PID_Calculation(double *output, PID *motorPID, uint8_t motor) 
{
    motorPID->Compute();
    if (*output > 0) //正轉CW
       motorAction(motor, *output, HIGH, LOW);
    else if (*output < 0) //逆轉CCW
      motorAction(motor, abs(*output), LOW, HIGH);
    else if (*output == 0)//停止
      motorAction(motor, 0, LOW, LOW);
}

void LeftInitialSetting()
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

void RightInitialSetting()
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

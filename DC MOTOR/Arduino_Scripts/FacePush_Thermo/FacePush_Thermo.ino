
#define SERIAL_BAUD 115200
#define DEBUG  1

//All Thermal Parameter
int AllThermo_Parameters[2];// L ColdorHot R ColdorHot
int HotPWM = -20;
int ColdPWM = 20;
int StopPWM = 0;

//Read String Parameter
String ReadString_Input = "";

// Left default setting(Thermal，Rotary Encoder，PID)
#define LeftThermal_A1_PIN 8
#define LeftThermal_B1_PIN 7
#define LeftThermal_PWM 5
#define LeftThermal_EnablePin A0
#define LeftThermal 1
int LeftThermal_Speed = 0;

//Right default setting(Thermal，Rotary Encoder，PID)
#define RightThermal_A2_PIN 4
#define RightThermal_B2_PIN 9
#define RightThermal_PWM 6
#define RightThermal_EnablePin A1
#define RightThermal 2
int RightThermal_Speed = 0;

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
    Serial.print(LeftThermal_Speed); Serial.print(" ");
  
    Serial.print("R: ");
    Serial.print(RightThermal_Speed); Serial.print(" ");

    Serial.print("Order: ");
    Serial.print(AllThermo_Parameters[0]); Serial.print(" ");
    Serial.print(AllThermo_Parameters[1]); Serial.println(" ");
  #else DEBUG == 0
//    Serial.print(LeftPID_Output); Serial.print(" ");
//    Serial.print(MotorCounter); Serial.println(" ");
    delay(10);
  #endif
    
  //Read Data and Handle Data
  int i = 0;
  while(Serial.available())
  {
    digitalWrite(LeftThermal_EnablePin, HIGH);
    digitalWrite(RightThermal_EnablePin, HIGH); 
    char c = Serial.read();
    if (c == ' ' || c == '\n')
    {
      AllThermo_Parameters[i] = ReadString_Input.toInt();
      i++;
      ReadString_Input = "";
    }
    else
    {
      ReadString_Input += c;
    }
    PID_Calculation( AllThermo_Parameters[0], LeftThermal);
    PID_Calculation( AllThermo_Parameters[1], RightThermal);
    Serial.flush();
  }

}

void ThermalAction(uint8_t Thermal, uint8_t pwm, int PinA_Value, int PinB_Value) 
{
  if(Thermal == LeftThermal)
  {
    digitalWrite(LeftThermal_A1_PIN, PinA_Value); 
    digitalWrite(LeftThermal_B1_PIN, PinB_Value);
    analogWrite(LeftThermal_PWM, pwm);
  }
  else if(Thermal == RightThermal)
  {
    digitalWrite(RightThermal_A2_PIN, PinA_Value);
    digitalWrite(RightThermal_B2_PIN, PinB_Value);
    analogWrite(RightThermal_PWM, pwm);
  }
}

void PID_Calculation(int output, uint8_t thermal) 
{
    if (output > 0) //正轉CW
       ThermalAction(thermal, output, HIGH, LOW);
    else if (output < 0) //逆轉CCW
      ThermalAction(thermal, abs(output), LOW, HIGH);
    else if (output == 0)//停止
      ThermalAction(thermal, 0, LOW, LOW);
}

void LeftInitialSetting()
{
  // Left setup (Motor，Rotary Encoder，PID)
  pinMode(LeftThermal_A1_PIN, OUTPUT);
  pinMode(LeftThermal_B1_PIN, OUTPUT);
  pinMode(LeftThermal_PWM, OUTPUT);
  pinMode(LeftThermal_EnablePin, OUTPUT);
}

void RightInitialSetting()
{
  // Right setup (Motor，Rotary Encoder，PID)
  pinMode(RightThermal_A2_PIN, OUTPUT);
  pinMode(RightThermal_B2_PIN, OUTPUT);
  pinMode(RightThermal_PWM, OUTPUT);
  pinMode(RightThermal_EnablePin, OUTPUT);
}

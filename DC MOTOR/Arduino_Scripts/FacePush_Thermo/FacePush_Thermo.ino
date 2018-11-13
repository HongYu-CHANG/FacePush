
#define SERIAL_BAUD 115200
#define DEBUG  0

//All Ｍotor Parameter

char AllMotor_Parameters;// ColdorHot left, speed left, ColdorHot right, speed right
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
    Serial.print(AllMotor_Parameters); Serial.println(" ");
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
    Serial.print(c); Serial.println(" ");
    if (c == 'C')
    {
      LeftThermal_Speed = 200;
      RightThermal_Speed = 200;
      AllMotor_Parameters = 'C';
    }
    else if (c == 'H')
    {
      LeftThermal_Speed = -200;
      RightThermal_Speed = -200;
      AllMotor_Parameters = 'H';
    }
    else if (c == 'S')
    {
      LeftThermal_Speed = 0;
      RightThermal_Speed = 0;
      AllMotor_Parameters = 'S';
    }
    else if (c == '\n' || c == ' ')
    {
      PID_Calculation( LeftThermal_Speed, LeftThermal);
      PID_Calculation( RightThermal_Speed, RightThermal);
    }
    Serial.flush();
  }
}

void motorAction(uint8_t motor, uint8_t pwm, int PinA_Value, int PinB_Value) 
{
  if(motor == LeftThermal)
  {
    Serial.print("LEFT"); Serial.println(" ");
    digitalWrite(LeftThermal_A1_PIN, PinA_Value); 
    digitalWrite(LeftThermal_B1_PIN, PinB_Value);
    analogWrite(LeftThermal_PWM, pwm);
  }
  else if(motor == RightThermal)
  {
    Serial.print("RIGHT"); Serial.println(" ");
    digitalWrite(RightThermal_A2_PIN, PinA_Value);
    digitalWrite(RightThermal_B2_PIN, PinB_Value);
    analogWrite(RightThermal_PWM, pwm);
  }
}

void PID_Calculation(int output, uint8_t thermal) 
{
    Serial.print(output); Serial.println(" ");
    if (output > 0) //正轉CW
       motorAction(thermal, output, HIGH, LOW);
    else if (output < 0) //逆轉CCW
      motorAction(thermal, abs(output), LOW, HIGH);
    else if (output == 0)//停止
      motorAction(thermal, 0, LOW, LOW);
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

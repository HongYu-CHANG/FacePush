
#include <Wire.h>
#include <Adafruit_MotorShield.h>
#define SERIAL_BAUD 115200

//All Thermal Parameter
int AllThermo_Parameters[4];// Down L R UP L R

//Read String Parameter
String ReadString_Input = "";

Adafruit_MotorShield AFMS = Adafruit_MotorShield(); 
Adafruit_DCMotor *DownLThermal = AFMS.getMotor(1);
Adafruit_DCMotor *DownRThermal = AFMS.getMotor(2);
Adafruit_DCMotor *UpLThermal = AFMS.getMotor(3);
Adafruit_DCMotor *UpRThermal = AFMS.getMotor(4);

// Thermal default setting(Thermal，Rotary Encoder，PID)
int DownLThermal_Speed = 0;
int DownRThermal_Speed = 0;
int UpLThermal_Speed = 0;
int UpRThermal_Speed = 0;

void setup()                         
{
  Serial.begin(SERIAL_BAUD);
  AFMS.begin();
}

void loop() 
{
  //Read Data and Handle Data
  int i = 0;
  while(Serial.available())
  {
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
    ThermalControl( AllThermo_Parameters[0], DownLThermal, "DownLThermal");
    ThermalControl( AllThermo_Parameters[1], DownRThermal, "DownRThermal");
    ThermalControl( AllThermo_Parameters[2], UpLThermal, "UpLThermal");
    ThermalControl( AllThermo_Parameters[3], UpRThermal, "UpRThermal");
    Serial.flush();
  }
}

void ThermalControl(int output, Adafruit_DCMotor *thermal, String temp)  //負責決定傳什麼訊號給制冷晶片
{
    
    if (output > 0) //正轉CW
    {
      thermal->setSpeed(abs(output));
      thermal->run(FORWARD);
    }
    else if (output < 0) //逆轉CCW
    {
      thermal->setSpeed(abs(output));
      thermal->run(BACKWARD);
    }
    else if (output == 0)//停止
    {
      thermal->setSpeed(abs(output));
      thermal->run(RELEASE);
    }
}

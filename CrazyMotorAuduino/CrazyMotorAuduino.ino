#include <Servo.h>

Servo rightServo; //create servo object
Servo leftServo; //create servo object
int lastRDegree = 0;
int lastLDegree = 150;

void setup()
{
  Serial.begin(115200);
  
  rightServo.attach(11,500,2500); //attach servo at pin 13
  rightServo.write(0);
  leftServo.attach(3,500,2500); //attach servo at pin 8
  leftServo.write(150);
}

void loop()
{
}

void serialEvent() //Called when data is available. Use Serial.read() to capture this data.
{
  String servoCmd = ""; 
  char inChar;
   while (Serial.available())
  {      
      inChar = (char)Serial.read();
      servoCmd += inChar;
      if (inChar == ' ')
      {
        if(servoCmd.toInt() != lastRDegree)
        {
          rightServo.write(servoCmd.toInt());
          lastRDegree = servoCmd.toInt();
        }
        servoCmd = "";
      }
      if (inChar == '\n')
      {
        if(servoCmd.toInt() != lastLDegree)
        {
          leftServo.write(servoCmd.toInt());
          lastLDegree = servoCmd.toInt();
        }
        servoCmd = "";
      }
      delay(10);      
  } 
}

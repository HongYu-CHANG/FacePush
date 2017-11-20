#include <Servo.h>

Servo rightServo; //create servo object
Servo leftServo; //create servo object

void setup()
{
  Serial.begin(115200);
  
  rightServo.attach(11,500,2500); //attach servo at pin 13
  rightServo.write(0);
  leftServo.attach(6,500,2500); //attach servo at pin 8
  leftServo.write(150);
  Serial.println("--- Start Serial Monitor SEND_RCVE ---");
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
      //Serial.println("servoCmd = "+servoCmd);
      if (servoCmd == "R\n")
      {
        Serial.println(String(rightServo.read()) + " " + String(leftServo.read()));
        servoCmd = "";
        inChar = "";
      }
      if (inChar == ' ')
      {
        rightServo.write(servoCmd.toInt());
        servoCmd = "";
      }
      if (inChar == '\n')
      {
        leftServo.write(servoCmd.toInt());
      }
      delay(5);      
  } 
}

/*
 * Easing
 * Tobias Toft <hello@tobiastoft.dk>
 * July 27, 2009
 *
 * Moves a servo motor back and forth between 0 and 140 degrees
 * when a button is pressed.
 *
 * This example is using the Servo.h library that comes
 * with the Arduino IDE.
 *
 * Easing functions based on Robert Penner's work,
 * for more info see Easing.h or Easing.cpp
 */
#include <Servo.h>

Servo rightServo; //create servo object
Servo leftServo; //create servo object

void setup()
{
  Serial.begin(115200);
  
  rightServo.attach(13); //attach servo at pin 13
  rightServo.write(180);  //put servo at 0 degrees
  leftServo.attach(7); //attach servo at pin 8
  leftServo.write(0);  //put servo at 140 degrees
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
      Serial.println("servoCmd = "+servoCmd);
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

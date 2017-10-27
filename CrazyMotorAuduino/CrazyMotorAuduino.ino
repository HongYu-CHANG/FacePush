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

#include <Easing.h>
#include <Servo.h>

Servo rightServo; //create servo object
Servo leftServo; //create servo object

void setup()
{
  rightServo.attach(13); //attach servo at pin 13
  rightServo.write(0);  //put servo at 0 degrees
  leftServo.attach(12); //attach servo at pin 12
  leftServo.write(140);  //put servo at 140 degrees
  Easing::initialSetup();
}

void loop()
{
  
}

void serialEvent() //Called when data is available. Use Serial.read() to capture this data.
{

}

void moveServo()
{
  int dur = 100; //duration is 100 loops
  for (int pos=0; pos<dur; pos++){
    //move servo from 0 and 140 degrees forward
    rightServo.write(Easing::degreeCal(callFuntion,pos, 0, 140, dur));
    delay(15); //wait for the servo to move
    leftServo.write(Easing::degreeCal(callFuntion,pos, 140, -140, dur));
    delay(15);
  }
  
  delay(1000); //wait a second, then move back using "bounce" easing
  //back to initial positon
  rightServo.write(0);  //put servo at 0 degrees
  leftServo.write(140);  //put servo at 140 degrees
}

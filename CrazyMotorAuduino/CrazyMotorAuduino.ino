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
#include <StandardCplusplus.h>
#include <string>
#include <map>
#include <Servo.h>
using namespace std;
Servo rightServo; //create servo object
Servo leftServo; //create servo object
typedef float(*FnPtr)(float, float, float, float);
std::map<string, FnPtr> funMap;
void setup()
{
  rightServo.attach(13); //attach servo at pin 13
  rightServo.write(0);  //put servo at 0 degrees
  leftServo.attach(12); //attach servo at pin 12
  leftServo.write(140);  //put servo at 140 degrees
  initialSetup();
}

void loop()
{
}

void serialEvent() //Called when data is available. Use Serial.read() to capture this data.
{
  //R L Duration
   while (Serial.available())
  {
      int i = 0;
      String servoCmd[3] = ""; 
      char inChar = (char)Serial.read();
      if (inChar == ' ') i++;
      else servoCmd[i] += inChar;
      if (inChar == '\n') moveServo(servoCmd[0], servoCmd[1], servoCmd[2]);
  }
}

void moveServo(String RservoCmd, String LservoCmd, String Duration)
{
  int dur = Duration.toInt();//Catch the duration
  for (int pos=0; pos<dur; pos++){
    //move servo from 0 and 140 degrees forward
    //rightServo.write(degreeCal(RservoCmd, pos, 0, 140, dur));
    //delay(15); //wait for the servo to move
    //leftServo.write(degreeCal(LservoCmd, pos, 140, -140, dur));
    delay(15);//wait for the servo to move
  }
  
  delay(1000); //wait a second, then move back using "bounce" easing
  //back to initial positon
  rightServo.write(0);  //put servo at 0 degrees
  leftServo.write(140);  //put servo at 140 degrees
}

void initialSetup()
{
  funMap["linearTween"] = linearTween;
  funMap["easeInQuad"] = easeInQuad; funMap["easeOutQuad"] = easeOutQuad; funMap["easeInOutQuad"] = easeInOutQuad; // Quad
  funMap["easeInCubic"] = easeInCubic; funMap["easeOutCubic"] = easeOutCubic; funMap["easeInOutCubic"] = easeInOutCubic; //Cubic
  funMap["easeInQuart"] = easeInQuart; funMap["easeOutQuart"] = easeOutQuart; funMap["easeInOutQuart"] = easeInOutQuart; //Quart
  funMap["easeInQuint"] = easeInQuint; funMap["easeOutQuint"] = easeOutQuint; funMap["easeInOutQuint"] = easeInOutQuint; //Quint
  funMap["easeInSine"] = easeInSine; funMap["easeOutSine"] = easeOutSine; funMap["easeInOutSine"] = easeInOutSine; //Sine
  funMap["easeInExpo"] = easeInExpo; funMap["easeOutExpo"] = easeOutExpo; funMap["easeInOutExpo"] = easeInOutExpo; //Expo
  funMap["easeInCirc"] = easeInCirc; funMap["easeOutCirc"] = easeOutCirc; funMap["easeInOutCirc"] = easeInOutCirc; //Circ
  funMap["easeInElastic"] = easeInElastic; funMap["easeOutElastic"] = easeOutElastic; funMap["easeInOutElastic"] = easeInOutElastic; //Elastic
  funMap["easeInBack"] = easeInBack; funMap["easeOutBack"] = easeOutBack; funMap["easeInOutBack"] = easeInOutBack; //Back
  funMap["easeInBounce"] = easeInBounce; funMap["easeOutBounce"] = easeOutBounce; funMap["easeInOutBounce"] = easeInOutBounce; //Bounce
}

float degreeCal(string funName, float t, float b, float c, float d)
{
  //std::string str(funName);
  return funMap[funName](t, b, c, d);
}

// simple linear tweening - no easing
// t: current time, b: beginning value, c: change in value, d: duration
float linearTween (float t, float b, float c, float d) {
  return c*t/d + b;
}

 ///////////// QUADRATIC EASING: t^2 ///////////////////

// quadratic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be in frames or seconds/milliseconds
float easeInQuad (float t, float b, float c, float d) {
  return c*(t/=d)*t + b;
}

// quadratic easing out - decelerating to zero velocity
float easeOutQuad (float t, float b, float c, float d) {
  return -c *(t/=d)*(t-2) + b;
}

// quadratic easing in/out - acceleration until halfway, then deceleration
float easeInOutQuad (float t, float b, float c, float d) {
  if ((t/=d/2) < 1) return c/2*t*t + b;
  return -c/2 * ((--t)*(t-2) - 1) + b;
}


 ///////////// CUBIC EASING: t^3 ///////////////////////

// cubic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
float easeInCubic (float t, float b, float c, float d) {
  return c*(t/=d)*t*t + b;
}

// cubic easing out - decelerating to zero velocity
float easeOutCubic (float t, float b, float c, float d) {
  return c*((t=t/d-1)*t*t + 1) + b;
}

// cubic easing in/out - acceleration until halfway, then deceleration
float easeInOutCubic (float t, float b, float c, float d) {
  if ((t/=d/2) < 1) return c/2*t*t*t + b;
  return c/2*((t-=2)*t*t + 2) + b;
}


 ///////////// QUARTIC EASING: t^4 /////////////////////

// quartic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
float easeInQuart (float t, float b, float c, float d) {
  return c*(t/=d)*t*t*t + b;
}

// quartic easing out - decelerating to zero velocity
float easeOutQuart (float t, float b, float c, float d) {
  return -c * ((t=t/d-1)*t*t*t - 1) + b;
}

// quartic easing in/out - acceleration until halfway, then deceleration
float easeInOutQuart (float t, float b, float c, float d) {
  if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
  return -c/2 * ((t-=2)*t*t*t - 2) + b;
}


 ///////////// QUINTIC EASING: t^5  ////////////////////

// quintic easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in value, d: duration
// t and d can be frames or seconds/milliseconds
float easeInQuint (float t, float b, float c, float d) {
  return c*(t/=d)*t*t*t*t + b;
}

// quintic easing out - decelerating to zero velocity
float easeOutQuint (float t, float b, float c, float d) {
  return c*((t=t/d-1)*t*t*t*t + 1) + b;
}

// quintic easing in/out - acceleration until halfway, then deceleration
float easeInOutQuint (float t, float b, float c, float d) {
  if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
  return c/2*((t-=2)*t*t*t*t + 2) + b;
}

 ///////////// SINUSOIDAL EASING: sin(t) ///////////////

// sinusoidal easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
float easeInSine (float t, float b, float c, float d) {
  return -c * cos(t/d * (M_PI/2)) + c + b;
}

// sinusoidal easing out - decelerating to zero velocity
float easeOutSine (float t, float b, float c, float d) {
  return c * sin(t/d * (M_PI/2)) + b;
}

// sinusoidal easing in/out - accelerating until halfway, then decelerating
float easeInOutSine (float t, float b, float c, float d) {
  return -c/2 * (cos(M_PI*t/d) - 1) + b;
}

 ///////////// EXPONENTIAL EASING: 2^t /////////////////

// exponential easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
float easeInExpo (float t, float b, float c, float d) {
  return (t==0) ? b : c * pow(2, 10 * (t/d - 1)) + b;
}

// exponential easing out - decelerating to zero velocity
float easeOutExpo (float t, float b, float c, float d) {
  return (t==d) ? b+c : c * (-pow(2, -10 * t/d) + 1) + b;
}

// exponential easing in/out - accelerating until halfway, then decelerating
float easeInOutExpo (float t, float b, float c, float d) {
  if (t==0) return b;
  if (t==d) return b+c;
  if ((t/=d/2) < 1) return c/2 * pow(2, 10 * (t - 1)) + b;
  return c/2 * (-pow(2, -10 * --t) + 2) + b;
}

 /////////// CIRCULAR EASING: sqrt(1-t^2) //////////////

// circular easing in - accelerating from zero velocity
// t: current time, b: beginning value, c: change in position, d: duration
float easeInCirc (float t, float b, float c, float d) {
  return -c * (sqrt(1 - (t/=d)*t) - 1) + b;
}

// circular easing out - decelerating to zero velocity
float easeOutCirc (float t, float b, float c, float d) {
  return c * sqrt(1 - (t=t/d-1)*t) + b;
}

// circular easing in/out - acceleration until halfway, then deceleration
float easeInOutCirc (float t, float b, float c, float d) {
  if ((t/=d/2) < 1) return -c/2 * (sqrt(1 - t*t) - 1) + b;
  return c/2 * (sqrt(1 - (t-=2)*t) + 1) + b;
}


 /////////// ELASTIC EASING: exponentially decaying sine wave  //////////////

// t: current time, b: beginning value, c: change in value, d: duration, a: amplitude (optional), p: period (optional)
// t and d can be in frames or seconds/milliseconds

float easeInElastic (float t, float b, float c, float d, float a, float p) {
  float s;
  if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  return -(a*pow(2,10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )) + b;
}

float easeOutElastic (float t, float b, float c, float d, float a, float p) {
  float s;
  if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  return a*pow(2,-10*t) * sin( (t*d-s)*(2*M_PI)/p ) + c + b;
}

float easeInOutElastic (float t, float b, float c, float d, float a, float p) {
  float s;
  if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (!p) p=d*(.3*1.5);
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  if (t < 1) return -.5*(a*pow(2,10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )) + b;
  return a*pow(2,-10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )*.5 + c + b;
}


//Four parameter versions
float easeInElastic (float t, float b, float c, float d) {
  float s;
  float a=0.0;
  float p=0.0;
  if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  return -(a*pow(2,10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )) + b;
}

float easeOutElastic (float t, float b, float c, float d) {
  float s;
  float a=0.0;
  float p=0.0;
  if (t==0) return b;  if ((t/=d)==1) return b+c;  if (!p) p=d*.3;
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  return a*pow(2,-10*t) * sin( (t*d-s)*(2*M_PI)/p ) + c + b;
}

float easeInOutElastic (float t, float b, float c, float d) {
  float s;
  float a=0.0;
  float p=0.0;
  if (t==0) return b;  if ((t/=d/2)==2) return b+c;  if (!p) p=d*(.3*1.5);
  if (a < fabs(c)) { a=c; s=p/4; }
  else s = p/(2*M_PI) * asin (c/a);
  if (t < 1) return -.5*(a*pow(2,10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )) + b;
  return a*pow(2,-10*(t-=1)) * sin( (t*d-s)*(2*M_PI)/p )*.5 + c + b;
}

 /////////// BACK EASING: overshooting cubic easing: (s+1)*t^3 - s*t^2  //////////////

// back easing in - backtracking slightly, then reversing direction and moving to target
// t: current time, b: beginning value, c: change in value, d: duration, s: overshoot amount (optional)
// t and d can be in frames or seconds/milliseconds
// s controls the amount of overshoot: higher s means greater overshoot
// s has a default value of 1.70158, which produces an overshoot of 10 percent
// s==0 produces cubic easing with no overshoot
float easeInBack (float t, float b, float c, float d, float s) {
  return c*(t/=d)*t*((s+1)*t - s) + b;
}

// back easing out - moving towards target, overshooting it slightly, then reversing and coming back to target
float easeOutBack (float t, float b, float c, float d, float s) {
  return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
}

// back easing in/out - backtracking slightly, then reversing direction and moving to target,
// then overshooting target, reversing, and finally coming back to target
float easeInOutBack (float t, float b, float c, float d, float s) {
  if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525))+1)*t - s)) + b;
  return c/2*((t-=2)*t*(((s*=(1.525))+1)*t + s) + 2) + b;
}


//Four parameter versions
float easeInBack (float t, float b, float c, float d) {
  float s=1.70158;
  return c*(t/=d)*t*((s+1)*t - s) + b;
}

float easeOutBack (float t, float b, float c, float d) {
  float s=1.70158;
  return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
}

float easeInOutBack (float t, float b, float c, float d) {
  float s=1.70158;
  if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525))+1)*t - s)) + b;
  return c/2*((t-=2)*t*(((s*=(1.525))+1)*t + s) + 2) + b;
}

 /////////// BOUNCE EASING: exponentially decaying parabolic bounce  //////////////

// bounce easing in
// t: current time, b: beginning value, c: change in position, d: duration
float easeInBounce (float t, float b, float c, float d) {
  return c - easeOutBounce (d-t, 0, c, d) + b;
}

// bounce easing out
float easeOutBounce (float t, float b, float c, float d) {
  if ((t/=d) < (1/2.75)) {
    return c*(7.5625*t*t) + b;
  } else if (t < (2/2.75)) {
    return c*(7.5625*(t-=(1.5/2.75))*t + .75) + b;
  } else if (t < (2.5/2.75)) {
    return c*(7.5625*(t-=(2.25/2.75))*t + .9375) + b;
  } else {
    return c*(7.5625*(t-=(2.625/2.75))*t + .984375) + b;
  }
}

// bounce easing in/out
float easeInOutBounce (float t, float b, float c, float d) {
  if (t < d/2) return easeInBounce (t*2, 0, c, d) * .5 + b;
  return easeOutBounce (t*2-d, 0, c, d) * .5 + c*.5 + b;
}

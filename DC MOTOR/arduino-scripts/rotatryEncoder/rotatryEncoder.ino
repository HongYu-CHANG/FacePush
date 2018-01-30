//From bildr article: http://bildr.org/2012/08/rotary-encoder-arduino/

//these pins can not be changed 2/3 are special pins

//arduino UNO
//int encoderPin1 = 2;
//int encoderPin2 = 3;

// Arduino Leonardo has 4 interrupt Pin, D3, D2, D0, D1
int encoderLeftPin1 = 3; // interrupt 0
int encoderLeftPin2 = 2; // interrupt 1
int encoderRightPin1 = 0;// interrupt 2
int encoderRightPin2 = 1;// interrupt 3

//int encoderPin1 = 2;//Yellow
//int encoderPin2 = 3;
    
//adafruit
//int encoderPin1 = 12; 
//int encoderPin2 = 13;

//volatile int lastEncoded = 0;
volatile int leftLastEncoded = 0;
volatile int rightLastEncoded = 0;

//volatile long encoderValue = 0;
volatile long encoderLeftValue = 0;
volatile long encoderRightValue = 0;

long lastencoderValue = 0;
int lastMSB = 0;
int lastLSB = 0;

void setup() {
  // put your setup code here, to run once:

  Serial.begin (9600);

//  pinMode(encoderPin1, INPUT); 
//  pinMode(encoderPin2, INPUT);
//
//  digitalWrite(encoderPin1, HIGH); //turn pullup resistor on
//  digitalWrite(encoderPin2, HIGH); //turn pullup resistor on
//  attachInterrupt(digitalPinToInterrupt(encoderPin1), updateEncoder, CHANGE); 
//  attachInterrupt(digitalPinToInterrupt(encoderPin2), updateEncoder, CHANGE);

  pinMode(encoderLeftPin1, INPUT); 
  pinMode(encoderLeftPin2, INPUT);
  pinMode(encoderRightPin1, INPUT); 
  pinMode(encoderRightPin2, INPUT);

  digitalWrite(encoderLeftPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderLeftPin2, HIGH); //turn pullup resistor on
  digitalWrite(encoderRightPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderRightPin2, HIGH); //turn pullup resistor on

  //call updateEncoder() when any high/low changed seen
  //on interrupt 0 (pin 2), or interrupt 1 (pin 3) 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin1), updateLeftEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderLeftPin2), updateLeftEncoder, CHANGE);
  attachInterrupt(digitalPinToInterrupt(encoderRightPin1), updateRightEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderRightPin2), updateRightEncoder, CHANGE);

}

void loop() {
  // put your main code here, to run repeatedly:

  // Serial.println(encoderValue);

  if (encoderLeftValue >= 0) {
    encoderLeftValue %= 1024;
  }
  else {
    encoderLeftValue %= 1024;
    encoderLeftValue += 1024;
  }
  
  encoderRightValue %= 1024;
  Serial.print(encoderLeftValue); Serial.print(" ");
  Serial.println(encoderRightValue);

  delay(10); //just here to slow down the output, and show it will work  even during a delay
}

//void updateEncoder(){
//  int MSB = digitalRead(encoderRightPin1); //MSB = most significant bit
//  int LSB = digitalRead(encoderRightPin2); //LSB = least significant bit
//
//  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
//  int sum  = (lastEncoded << 2) | encoded; //adding it to the previous encoded value
//
//  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderValue ++;
//  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderValue --;
//
//  lastEncoded = encoded; //store this value for next time
//}

void updateLeftEncoder(){
  int MSB = digitalRead(encoderLeftPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderLeftPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (leftLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderLeftValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderLeftValue --;

  leftLastEncoded = encoded; //store this value for next time
}

void updateRightEncoder(){
  int MSB = digitalRead(encoderRightPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderRightPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (rightLastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderRightValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderRightValue --;

  rightLastEncoded = encoded; //store this value for next time
}


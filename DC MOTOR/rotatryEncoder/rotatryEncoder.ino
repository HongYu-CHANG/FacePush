//From bildr article: http://bildr.org/2012/08/rotary-encoder-arduino/

//these pins can not be changed 2/3 are special pins

//arduino UNO
int encoderPin1 = 2;//Yellow
int encoderPin2 = 3;
    
//adafruit
//int encoderPin1 = 12; 
//int encoderPin2 = 13;

volatile int lastEncoded = 0;
volatile long encoderValue = 0;
long lastencoderValue = 0;
int lastMSB = 0;
int lastLSB = 0;

void setup() {
  // put your setup code here, to run once:

  Serial.begin (9600);

  pinMode(encoderPin1, INPUT); 
  pinMode(encoderPin2, INPUT);

  digitalWrite(encoderPin1, HIGH); //turn pullup resistor on
  digitalWrite(encoderPin2, HIGH); //turn pullup resistor on

  //call updateEncoder() when any high/low changed seen
  //on interrupt 0 (pin 2), or interrupt 1 (pin 3) 
  attachInterrupt(digitalPinToInterrupt(encoderPin1), updateEncoder, CHANGE); 
  attachInterrupt(digitalPinToInterrupt(encoderPin2), updateEncoder, CHANGE);

}

void loop() {
  // put your main code here, to run repeatedly:

  Serial.println(encoderValue);
  delay(10); //just here to slow down the output, and show it will work  even during a delay
}

void updateEncoder(){
  int MSB = digitalRead(encoderPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderPin2); //LSB = least significant bit

  int encoded = (MSB << 1) |LSB; //converting the 2 pin value to single number
  int sum  = (lastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderValue --;

  lastEncoded = encoded; //store this value for next time
}


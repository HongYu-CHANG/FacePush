#include <SLIPEncodedSerial.h>
#include <OSCData.h>
#include <OSCBoards.h>
#include <OSCTiming.h>
#include <OSCMatch.h>
#include <SLIPEncodedUSBSerial.h>
#include <SPI.h>
#include <WiFi101.h>
#include <WiFiUdp.h>
#include <OSCMessage.h>
#include <OSCBundle.h>
#include <Wire.h>
#include <Adafruit_MotorShield.h>
#include <PID_v1.h>
#define SLAVE_ADDRESS 0x12
#define SERIAL_BAUD 9600 

int status = WL_IDLE_STATUS;
char ssid[] = "NextInterfaces Lab"; // your network SSID (name)
char pass[] = "nextinterfaces"; // your network password (use for WPA, or use as key for WEP)

//IP setup
IPAddress sendToUnityPC_Ip(192, 168, 0, 154); // UnityPC's IP
unsigned int sendToUnityPC_Port = 8000; // UnityPC's listening port
unsigned int listenPort = 9000; // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
char ReplyBuffer[] = "acknowledged"; // a string to send back
WiFiUDP Udp_send;
WiFiUDP Udp_listen;

//DC Motor
Adafruit_MotorShield AFMS = Adafruit_MotorShield(); 
Adafruit_DCMotor *RMotor = AFMS.getMotor(2);// Select which 'port' M1, M2, M3 or M4. In this case, M1
Adafruit_DCMotor *LMotor = AFMS.getMotor(1);// Select which 'port' M1, M2, M3 or M4. In this case, M4

// rotary encoder
int encoderPin1 = 12;
int encoderPin2 = 13;
volatile int lastEncoded = 0;
volatile long encoderValue = 0;
// PID motor control
#include <PID_v1.h>
double kp = 10, ki = 0, kd = 0;
// input: current position (value of rotary encoder)
// output: result (where to go)
// setPoint: target position (position cmd from Feather)
double input = 0, output = 0, setPoint = 0;

PID myPID(&input, &output, &setPoint, kp, ki, kd, DIRECT); // DIRECT was defined in PID_v1.h
int Degree;
int speedNum;
char RorLMotor[1];

void setup() 
{
  WiFi.setPins(8, 7, 4, 2);//Configure pins for Adafruit ATWINC1500 Feather
  Serial.begin(SERIAL_BAUD);//Initialize serial and wait for port to open:
  AFMS.begin();

  // encoder and PID
  pinMode(encoderPin1, HIGH);
  pinMode(encoderPin2, HIGH);
  digitalWrite(encoderPin1, HIGH);
  digitalWrite(encoderPin2, HIGH);
  attachInterrupt(digitalPinToInterrupt(encoderPin1), updateEncoder, CHANGE);
  attachInterrupt(digitalPinToInterrupt(encoderPin2), updateEncoder, CHANGE);
  myPID.SetMode(AUTOMATIC);
  
  if (WiFi.status() == WL_NO_SHIELD) // check for the presence of the shields
  {
    Serial.println("WiFi shield not present");
    while (true); // don't continue:
  }
  while ( status != WL_CONNECTED) // attempt to connect to Wifi network:
  {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    status = WiFi.begin(ssid, pass); // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    delay(10000);
  }
  printWifiStatus();
  
  //OSC Start
  Udp_send.begin(sendToUnityPC_Port);
  Udp_listen.begin(listenPort);

  //I2C
  Wire.begin();
  Serial.println("I2C Master.02 started");
}

void printWifiStatus() 
{
  Serial.println("Connected to wifi");
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());
  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);
  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
  Serial.println("\nStarting connection to server...");
}

void loop() 
{
   // Write
   OSCMessage msg("/1/fader1");
   msg.add("Connected");
   Udp_send.beginPacket(sendToUnityPC_Ip, sendToUnityPC_Port);
   msg.send(Udp_send);
   Udp_send.endPacket();
   msg.empty();
//   delay(1000);  

  Serial.print("before update: ");
  Serial.print(encoderValue); Serial.print(" ");
  Serial.print(setPoint); Serial.print(" ");
//  Serial.print(input); Serial.print(" ");
  Serial.println(output);
  myPID.SetOutputLimits(-speedNum,speedNum);
  setPoint = Degree;
  input = encoderValue;
  myPID.Compute();


       
        if(RorLMotor[0] == 'R')
        {
           RMotor->setSpeed(output); 
           if (output > 0) {
            RMotor->run(FORWARD);
           }
           else {
            RMotor->run(BACKWARD);
           }
        }
  Serial.print("After update: ");
  Serial.print(encoderValue); Serial.print(" ");
  Serial.print(setPoint); Serial.print(" ");
//  Serial.print(input); Serial.print(" ");
  Serial.println(output);

  
  // Read Receive
  OSCMessage messageIn;
  int size;
  if ( (size = Udp_listen.parsePacket()) > 0)
  {
    while (size--)
      messageIn.fill(Udp_listen.read());
    if (!messageIn.hasError()) {
        RorLMotor[1];
        messageIn.getString(0, RorLMotor, 1);
        Degree = (int)messageIn.getInt(1);
        speedNum = (int)messageIn.getInt(2);
        
        // I2C message
        String temp = String(RorLMotor[0]) + " " + String(Degree) + " " + String(speedNum);
        char buffer[32];
        temp.toCharArray(buffer, 32);
        Serial.println(temp);
//        Wire.beginTransmission(SLAVE_ADDRESS);
//        Wire.write(buffer);
//        Wire.endTransmission();



  // test rotary encoder


//  pwmOut(output);

//        
//        if(RorLMotor[0] == 'R')
//        {
//           RMotor->setSpeed(output); 
//           if (output > 0) {
//            RMotor->run(FORWARD);
//           }
//           else if (output == 0) {
//            RMotor->run(RELEASE);
//           }
//           else {
//            RMotor->run(BACKWARD);
//           }

        

//           if(Direction == 1)  //CCW
//              RMotor->run(FORWARD);
//           else if(Direction == 2) //CW
//              RMotor->run(BACKWARD);
//           else if(Direction == 0)
//              RMotor->run(RELEASE);
//        } 
        
//        else
//        {
//           LMotor->setSpeed(speedNum);  
//            if(Direction == 1) 
//              LMotor->run(FORWARD);
//           else if(Direction == 2)
//              LMotor->run(BACKWARD);
//           else if(Direction == 0)
//              LMotor->run(RELEASE);
//        }
        
    }
  }
}

void updateEncoder(){
  int MSB = digitalRead(encoderPin1); //MSB = most significant bit
  int LSB = digitalRead(encoderPin2); //LSB = least significant bit

  int encoded = (MSB << 1) | LSB; //converting the 2 pin value to single number
  int sum  = (lastEncoded << 2) | encoded; //adding it to the previous encoded value

  if(sum == 0b1101 || sum == 0b0100 || sum == 0b0010 || sum == 0b1011) encoderValue ++;
  if(sum == 0b1110 || sum == 0b0111 || sum == 0b0001 || sum == 0b1000) encoderValue --;

  lastEncoded = encoded; //store this value for next time
}


//void pwmOut(int out) {                                // to H-Bridge board
//  if (out > 0) {
//    analogWrite(M1, out);                             // drive motor CW
//    analogWrite(M2, 0);
//  }
//  else {
//    analogWrite(M1, 0);
//    analogWrite(M2, abs(out));                        // drive motor CCW
//  }
//}

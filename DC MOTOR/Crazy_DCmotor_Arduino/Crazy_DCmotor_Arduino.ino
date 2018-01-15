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
//#include <Adafruit_MS_PWMServoDriver.h>
int sensorPin = A2;
int status = WL_IDLE_STATUS;
char ssid[] = "NextInterfaces Lab"; // your network SSID (name)
char pass[] = "nextinterfaces"; // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0; // your network key Index number (needed only for WEP)

//IP setup
IPAddress sendToUnityPC_Ip(192, 168, 0, 174); // UnityPC's IP
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
void setup() 
{
  WiFi.setPins(8, 7, 4, 2);//Configure pins for Adafruit ATWINC1500 Feather
  Serial.begin(9600);//Initialize serial and wait for port to open:
  AFMS.begin();
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
   /*// Write
   OSCMessage msg("/1/fader1");
   msg.add("C");
   Udp_send.beginPacket(sendToUnityPC_Ip, sendToUnityPC_Port);
   msg.send(Udp_send);
   Udp_send.endPacket();
   msg.empty();
   delay(3000);*/
  
  // Read Receive
  OSCMessage messageIn;
  int size;
  if ( (size = Udp_listen.parsePacket()) > 0)
  {
    while (size--)
      messageIn.fill(Udp_listen.read());
    if (!messageIn.hasError()) {
        char RorLMotor[1];
        messageIn.getString(0, RorLMotor, 1);
        uint8_t Direction = (uint8_t)messageIn.getInt(1);
        uint8_t speedNum = (uint8_t)messageIn.getInt(2);
        Serial.println(RorLMotor[0]);
        Serial.println(Direction);
        Serial.println(speedNum);
        if(RorLMotor[0] == 'R')
        {
           RMotor->setSpeed(speedNum); 
           if(Direction == 1) 
              RMotor->run(FORWARD);
           else if(Direction == 2)
              RMotor->run(BACKWARD);
           else if(Direction == 0)
              RMotor->run(RELEASE);
        } 
        
        else
        {
           LMotor->setSpeed(speedNum);  
            if(Direction == 1) 
              LMotor->run(FORWARD);
           else if(Direction == 2)
              LMotor->run(BACKWARD);
           else if(Direction == 0)
              LMotor->run(RELEASE);
        }
    }
  }
}


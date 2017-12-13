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

int status = WL_IDLE_STATUS;
char ssid[] = "NextInterfaces Lab"; // your network SSID (name)
char pass[] = "nextinterfaces"; // your network password (use for WPA, or use as key for WEP)
int keyIndex = 0; // your network key Index number (needed only for WEP)

//IP setup
IPAddress sendToUnityPC_Ip(192, 168, 0, 154); // UnityPC's IP
unsigned int sendToUnityPC_Port = 8000; // UnityPC's listening port
unsigned int listenPort = 9000; // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
char ReplyBuffer[] = "acknowledged"; // a string to send back
WiFiUDP Udp_send;
WiFiUDP Udp_listen;

void setup() 
{
  WiFi.setPins(8, 7, 4, 2);//Configure pins for Adafruit ATWINC1500 Feather
  Serial.begin(9600);//Initialize serial and wait for port to open:
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
  // Read Receive
  OSCMessage messageIn;
  int size;
  char str[255];
  if ( (size = Udp_listen.parsePacket()) > 0)
  {
    while (size--)
      messageIn.fill(Udp_listen.read());
    if (!messageIn.hasError()) {
        
        messageIn.getString(0, str, 255);
        Serial.println(str);
        messageIn.getString(1, str, 255);
        Serial.println(str);
        messageIn.getString(1, str, 255);
     
     
    }
  }
}


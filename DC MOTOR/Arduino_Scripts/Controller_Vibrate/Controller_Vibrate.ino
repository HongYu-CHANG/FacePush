#include <Wire.h>
#include "Adafruit_TPA2016.h"
#include <OSCBundle.h>
#include <OSCMessage.h>
#include <WiFi101.h>  
#include<WiFiUdp.h>

#define CPU_HZ 48000000
#define TIMER_PRESCALER_DIV 1024

void startTimer(int frequencyHz);
void setTimerFrequency(int frequencyHz);
void TC3_Handler();

bool toggle = false;

//global value
int timer = 0;// timer base 1ms
bool flag = true;

//for Uist'18 proj
int isVibrate = 0;
int vibrateFreq = 0;
int vibrateTime = 0;

//int pin_value[8] ={A2,A3,A4,A5,9,10,11,13};
int pin_value[8] ={9,10,11,13,A2,A3,A4,A5};
int vib_value[8];
int vibTime_value[8];
int vibDuration_value[8];


String vibrateString;
String vibrateTimeString;
String vibrateDurationString;

Adafruit_TPA2016 audioamp01 = Adafruit_TPA2016();

//wifi setting
//osc
int status = WL_IDLE_STATUS;
char ssid[] = "NextInterfaces Lab 2";
char pass[] = "nextinterfaces";
int keyIndex = 0;

IPAddress sendToUnityPC_Ip(10, 0, 1, 4);
unsigned int sendToUnityPC_Port = 8001; // the other is 8001
unsigned int listenPort = 9001; // the other is 9001

// 10.0.1.4
// 10.0.1.6, 8001, 9001
// 10.0.1.7, 8000, 9000

char packetBuffer[255];
char ReplyBuffer[] = "acknowledged";

WiFiUDP Udp_send;
WiFiUDP Udp_listen;

void setup() {
  //pinMode setting
  pinMode(12, OUTPUT);
    
  //interrupt setting
  startTimer(1000); //1ms  //1-> 1s //100->10ms
  
  //audioamp setting
  Serial.begin(9600);
  audioamp01.begin();
  audioamp01.enableChannel(true, true);
  audioamp01.setLimitLevelOff();
  audioamp01.setAGCCompression(TPA2016_AGC_OFF);
  
  //initial wifi
  initWifi();
}

void loop() {

  
  OSCMessage messageIn;
  int size;
  int stringLength;
  if((size = Udp_listen.parsePacket())>0)
  {
    while(size--)
      messageIn.fill(Udp_listen.read());

    if(!messageIn.hasError())
    {
      isVibrate = messageIn.getInt(0);
      vibrateFreq = messageIn.getInt(1);
      vibrateTime = messageIn.getInt(2);
      /*
      stringLength = messageIn.getDataLength(0);
      char str01[stringLength];
      messageIn.getString(0,str01,stringLength);
      vibrateString = String(str01);
      
      stringLength = messageIn.getDataLength(1);
      char str02[stringLength];
      messageIn.getString(1,str02,stringLength);
      vibrateTimeString = String(str02);

      stringLength = messageIn.getDataLength(2);
      char str03[stringLength];
      messageIn.getString(2,str03,stringLength);
      vibrateDurationString = String(str03);
      */
    }  
  }
  
  
  audioamp01.setGain(30);
  
  //vibrateString = "0000000000000000";
  //vibrateString = "05,00,00,00,00,00,00,04,";
  //getVibValue(vibrateString);
  vibrateVer2(12,vibrateFreq,vibrateTime);
  
  
}

void setTimerFrequency(int frequencyHz) {
  int compareValue = (CPU_HZ / (TIMER_PRESCALER_DIV * frequencyHz)) - 1;
  TcCount16* TC = (TcCount16*) TC3;
  // Make sure the count is in a proportional position to where it was
  // to prevent any jitter or disconnect when changing the compare value.
  TC->COUNT.reg = map(TC->COUNT.reg, 0, TC->CC[0].reg, 0, compareValue);
  TC->CC[0].reg = compareValue;
  Serial.println(TC->COUNT.reg);
  Serial.println(TC->CC[0].reg);
  while (TC->STATUS.bit.SYNCBUSY == 1);
}

void startTimer(int frequencyHz) {
  REG_GCLK_CLKCTRL = (uint16_t) (GCLK_CLKCTRL_CLKEN | GCLK_CLKCTRL_GEN_GCLK0 | GCLK_CLKCTRL_ID_TCC2_TC3) ;
  while ( GCLK->STATUS.bit.SYNCBUSY == 1 ); // wait for sync

  TcCount16* TC = (TcCount16*) TC3;

  TC->CTRLA.reg &= ~TC_CTRLA_ENABLE;
  while (TC->STATUS.bit.SYNCBUSY == 1); // wait for sync

  // Use the 16-bit timer
  TC->CTRLA.reg |= TC_CTRLA_MODE_COUNT16;
  while (TC->STATUS.bit.SYNCBUSY == 1); // wait for sync

  // Use match mode so that the timer counter resets when the count matches the compare register
  TC->CTRLA.reg |= TC_CTRLA_WAVEGEN_MFRQ;
  while (TC->STATUS.bit.SYNCBUSY == 1); // wait for sync

  // Set prescaler to 1024
  TC->CTRLA.reg |= TC_CTRLA_PRESCALER_DIV1024;
  while (TC->STATUS.bit.SYNCBUSY == 1); // wait for sync

  setTimerFrequency(frequencyHz);

  // Enable the compare interrupt
  TC->INTENSET.reg = 0;
  TC->INTENSET.bit.MC0 = 1;

  NVIC_EnableIRQ(TC3_IRQn);

  TC->CTRLA.reg |= TC_CTRLA_ENABLE;
  while (TC->STATUS.bit.SYNCBUSY == 1); // wait for sync
}




void TC3_Handler() {
  TcCount16* TC = (TcCount16*) TC3;
  // If this interrupt is due to the compare register matching the timer count
  // we toggle the LED.
  if (TC->INTFLAG.bit.MC0 == 1) {
    TC->INTFLAG.bit.MC0 = 1;
    // Write callback here!!!
    if(isVibrate == 1)
      timer ++;
    else
      timer =0;

    //if(timer>30000)
      //timer = 0;
  }
}

void vibrate(int pinNum, int half_wavelength,int vibrateTime, int vibrateDuration){//half_wavelength(ms)
  int cycleTime = vibrateTime + vibrateDuration;
  if((timer%cycleTime) <= vibrateTime){
    if((timer/half_wavelength)%2 ==0)
      digitalWrite(pinNum,HIGH);
    else 
      digitalWrite(pinNum,LOW);
  }
  else
    digitalWrite(pinNum,LOW);
}
void vibrateVer2(int pinNum, int half_wavelength,int vibrateTime){
  if(timer < vibrateTime && isVibrate == 1){
    if((timer/half_wavelength)%2 ==0)
      digitalWrite(pinNum,HIGH);
    else 
      digitalWrite(pinNum,LOW);
  }
  else{
    isVibrate = 0;
  }
}
void getVibAllValue() {
  getStringValue(vibrateString, vib_value);
  //getStringValue(vibrateTimeString, vibTime_value);
  //getStringValue(vibrateDurationString, vibDuration_value);
}

void getVibValue(String &a) {
  for(int i = 0; i < 8; i++){
    vib_value [i] = a.substring(2*i,2*i+2).toInt();
  }
}

void getStringValue(String &whichString, int *valueArray){
  int startValue = 0;
  int stringNum = 0;
  for (int i = 0; i < whichString.length(); i++) {
    if (whichString.substring(i, i+1) == ",") {
      valueArray[stringNum] = whichString.substring(startValue, i).toInt();
      startValue = i+1;
      stringNum++;
    }
  }
}


void printWifiStatus() 
{
  
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
}
void initWifi()
{ 
  //Configure pins for Adafruit ATWINC1500 Feather
  WiFi.setPins(8,7,4,2);
  //Initialize serial and wait for port to open:
  Serial.begin(9600);
  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) 
  {
    //Serial.println("WiFi shield not present");
    // don't continue:
    while (true);
  }
  // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) 
  {
    //Serial.print("Attempting to connect to SSID: ");
    //Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);
    // wait 10 seconds for connection:
    delay(10);
  }
  Serial.println("Connected to wifi");
  printWifiStatus();
  Serial.println("\nStarting connection to server...");
  // if you get a connection, report back via serial:
  Udp_send.begin(sendToUnityPC_Port);
  Udp_listen.begin(listenPort);
}

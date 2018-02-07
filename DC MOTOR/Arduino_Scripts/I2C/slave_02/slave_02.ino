#include <Wire.h>

#define SLAVE_ADDRESS 0x12
#define SERIAL_BAUD 9600 

void setup() {
  Serial.println("I2C Slave.02 started\n");

  Wire.begin(SLAVE_ADDRESS);    // join I2C bus as a slave with address 1
  Wire.onReceive(receiveEvent); // register event

  Serial.begin(SERIAL_BAUD);
}

void loop() {
}

void receiveEvent(int count) {
  Serial.println("Receive Data:");
  while(Wire.available()) {
    Serial.print((char) Wire.read());
  }
  Serial.println("\n");
}


#include<SoftwareSerial.h>
const int rxPin = 5;
const int txPin = 6;
SoftwareSerial hc = SoftwareSerial(rxPin, txPin);

#include <Wire.h>
#include <MechaQMC5883.h>

/* Assign a unique ID to this sensor at the same time */
// SDA=A4, SCL=A5
MechaQMC5883 qmc;

void setup() {
  // put your setup code here, to run once:
  hc.begin(9600); //init bluetooth
  Serial.begin(9600);
  Serial.println("Magnetometer Test"); Serial.println("");
  Wire.begin();
  qmc.init();
  Serial.println("ALL SET!");
  if (hc.isListening()) {
    Serial.println("Bluetooth is listening");
  }
}
void printHeading() {
  /* Get a new sensor event */
  int x, y, z;
  qmc.read(&x, &y, &z);

  float Pi = 3.14159;

  // Calculate the angle of the vector y,x
  float heading = (atan2(y, x) * 180) / Pi;

  // Normalize to 0-360
  if (heading < 0)
  {
    heading = 360 + heading;
  }
  int azimuth = heading;
  String data = "(" + String(azimuth) + ")";
  Serial.print("Compass Heading: ");
  Serial.println(data);
  hc.print(data);
  hc.flush();
  delay(500);
}
void readTurn() {
  if (hc.available()) {
    char c = (char)hc.read();
    Serial.println(c);
  }
}
void loop() {
  // put your main code here, to run repeatedly:
  //readColor();
  readTurn();
  printHeading();
}

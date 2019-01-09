#include<SoftwareSerial.h>
const int rxPin = 5;
const int txPin = 6;
SoftwareSerial hc = SoftwareSerial(rxPin, txPin);

#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_LSM303_U.h>
 
/* Assign a unique ID to this sensor at the same time */
// SDA=A4, SCL=A5
Adafruit_LSM303_Mag_Unified mag = Adafruit_LSM303_Mag_Unified(12345);
 
void setup() {
  // put your setup code here, to run once:
  hc.begin(9600); //init bluetooth

  
  Serial.begin(9600);
  Serial.println("Magnetometer Test"); Serial.println("");
  
  //Initialise the sensor 
  if(!mag.begin())
  {
    //There was a problem detecting the LSM303 ... check your connections 
    Serial.println("Ooops, no LSM303 detected ... Check your wiring!");
    while(1);
  }
  Serial.println("ALL SET!");
  
}

void readColor() {
  char c;
  c = (char)hc.read();

  Serial.println(c);
  
  if (c == 'Y') {
    hc.write('N');
  }
  if (c == 'N') {
    hc.write('Y');
  }
}

void printHeading() {
  /* Get a new sensor event */ 
  sensors_event_t event; 
  mag.getEvent(&event);
  
  float Pi = 3.14159;
  
  // Calculate the angle of the vector y,x
  float heading = (atan2(event.magnetic.y,event.magnetic.x) * 180) / Pi;
  
  // Normalize to 0-360
  if (heading < 0)
  {
    heading = 360 + heading;
  }
  int azimuth=heading;
  String data="("+String(azimuth)+")";
  Serial.print("Compass Heading: ");
  Serial.println(data);
  hc.print(data);
  hc.flush();
  delay(1000);
}

void loop() {
  // put your main code here, to run repeatedly:
  //readColor();
  printHeading();
}

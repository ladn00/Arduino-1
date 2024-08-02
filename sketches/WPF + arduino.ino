#include <DHT.h>   
DHT dht(3, DHT11);
String str;    
void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
  Serial.setTimeout(50);
  pinMode(11, OUTPUT);
}

void loop() {
  if(Serial.available()){
    str = Serial.readString();
    Serial.print(str);
  }

  if(str=="on"){
    tone(11, 500);
  }

  if(str == "off"){
    noTone(A0);
  }
}
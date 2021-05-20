//Comunicacion i2c esp32_2
// COM 6
#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>
#define DS3231_I2C_ADDRESS 0x68

const char* ssid = "LOPERAGALLO";
const char* password = "43204751";

WiFiUDP udpDevice;
uint16_t localUdpPort = 50006;
uint16_t UDPPort = 50002;
#define MAX_LEDSERVERS 3
const char* hosts[MAX_LEDSERVERS] = {"192.168.1.8", "?", "?.?.?.?"};
#define SERIALMESSAGESIZE 3
uint32_t previousMillis = 0;
#define ALIVE 1000
#define D0 17
uint8_t number;

byte decToBcd(byte val) {
  return ( (val / 10 * 16) + (val % 10) );
}

byte bcdToDec(byte val) {
  return ( (val / 16 * 10) + (val % 16) );
}

void setup() {
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  // Print the IP address
  Serial.println(WiFi.localIP());
  udpDevice.begin(localUdpPort);

  Wire.begin();
  setDS3231time(10, 53, 22, 1, 5, 4, 21);
}

void setDS3231time(byte second, byte minute, byte hour, byte dayOfWeek, byte
                   dayOfMonth, byte month, byte year) {
  // sets time and date data to DS3231
  Wire.beginTransmission(DS3231_I2C_ADDRESS);
  Wire.write(0);
  
  Wire.write(decToBcd(second));
  Wire.write(decToBcd(minute));
  Wire.write(decToBcd(hour));
  Wire.write(decToBcd(dayOfWeek));
  Wire.write(decToBcd(dayOfMonth));
  Wire.write(decToBcd(month));
  Wire.write(decToBcd(year));
  Wire.endTransmission();
}
void readDS3231time(byte *second, byte *minute, byte *hour, byte *dayOfWeek,
                    byte *dayOfMonth, byte *month, byte *year) {

  Wire.beginTransmission(DS3231_I2C_ADDRESS);
  Wire.write(0);
  Wire.endTransmission();
  Wire.requestFrom(DS3231_I2C_ADDRESS, 7);

  *second = bcdToDec(Wire.read() & 0x7f);
  *minute = bcdToDec(Wire.read());
  *hour = bcdToDec(Wire.read() & 0x3f);
  *dayOfWeek = bcdToDec(Wire.read());
  *dayOfMonth = bcdToDec(Wire.read());
  *month = bcdToDec(Wire.read());
  *year = bcdToDec(Wire.read());
}

void displayTime() {

  byte second, minute, hour, dayOfWeek, dayOfMonth, month, year;
  readDS3231time(&second, &minute, &hour, &dayOfWeek, &dayOfMonth, &month, &year);
  uint8_t command[] = {second, minute, hour, dayOfWeek, dayOfMonth, month, year};
  
  uint32_t currentMillis;
  currentMillis  = millis();

  if ((currentMillis - previousMillis) >= ALIVE) {
    previousMillis = currentMillis;
    Serial.println(command[0]);
    Serial.println(command[1]);
    udpDevice.beginPacket(hosts[0] , UDPPort);
    udpDevice.write(command, sizeof(command));
    Serial.println("enviado");
    udpDevice.endPacket();
  }
}
void loop() {
  displayTime();
}

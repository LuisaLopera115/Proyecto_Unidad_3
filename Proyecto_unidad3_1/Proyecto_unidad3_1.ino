//Comunicacion SPI esp32_1
// COM 3
#include <Adafruit_Sensor.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>

#define BME_SCK 18
#define BME_MISO 19
#define BME_MOSI 23
#define BME_CS 5

#define SEALEVELPRESSURE_HPA (1013.25)

//Adafruit_BME280 bme; // I2C
Adafruit_BME280 bme(BME_CS); // hardware SPI

//OutUdp

#include <WiFi.h>
#include <WiFiUdp.h>

const char* ssid = "LOPERAGALLO";
const char* password = "43204751";
WiFiUDP udpDevice;
uint16_t localUdpPort = 50005;
uint16_t UDPPort = 50001;
#define MAX_LEDSERVERS 3
const char* hosts[MAX_LEDSERVERS] = {"192.168.1.8", "192.168.1.13", "192.168.1.8"};
#define SERIALMESSAGESIZE 3
uint32_t previousMillis = 0;
#define ALIVE 1000
#define D0 17
uint8_t number;

void setup() {

  unsigned status;
  pinMode(D0, OUTPUT);     // Initialize the LED_BUILTIN pin as an output
  digitalWrite(D0, HIGH);
  Serial.begin(115200);
  Serial.println();
  Serial.println();
  Serial.print("Connecting to ");
  Serial.println(ssid);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  status = bme.begin();

  if (!status) {
    // mandar byte de fallo
    while (1) delay(10);
  }
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  // Print the IP address
  Serial.println(WiFi.localIP());
  udpDevice.begin(localUdpPort);
}

void networkTask() {

  float numTem = bme.readTemperature();
  static uint8_t arr[4] = {0};
  memcpy(arr, (uint8_t *)&numTem, 4);

  uint32_t currentMillis;
  currentMillis  = millis();

  if ((currentMillis - previousMillis) >= ALIVE) {
    previousMillis = currentMillis;
    Serial.println(numTem);
    udpDevice.beginPacket(hosts[0] , UDPPort);
    udpDevice.write(arr, 4);
    Serial.println("enviado");
    udpDevice.endPacket();
  }
  /*
    uint8_t sensor[] = {'s', 'e', 'n', 's', 'o', 'r', ' ', ' '};
    number = random(1, 3);
    if (number == 1 ) {
    sensor[7] = '1';
    } else if (number == 2 ) {
    sensor[7] = '2';
    }*/
}

void aliveTask() {
  uint32_t currentMillis;
  static uint8_t ledState = 0;
  currentMillis  = millis();
  if ((currentMillis - previousMillis) >= ALIVE) {
    previousMillis = currentMillis;
    if (ledState == 0) {
      digitalWrite(D0, HIGH);
      ledState = 1;
    }
    else {
      digitalWrite(D0, LOW);
      ledState = 0;
    }
  }
}

void loop() {
  networkTask();
  //aliveTask();
}

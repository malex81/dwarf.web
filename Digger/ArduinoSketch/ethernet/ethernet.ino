#include <SPI.h>
#include <Ethernet.h>

byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0xED };
IPAddress ip(192, 168, 1, 177);  // Статический IP для надежности
EthernetServer server(23);       // Сервер на порту 23 (TCP)

void setup() {
  Ethernet.begin(mac, ip);
  server.begin();  // Запускаем ожидание подключений

  Serial.begin(9600);
  while (!Serial) {
    ;  // wait for serial port to connect. Needed for native USB port only
  }
  Serial.println("Started ...");

  pinMode(LED_BUILTIN, OUTPUT);
}

unsigned long lastSend = 0;
const int interval = 100;  // 10 раз в секунду (100 мс)
EthernetClient activeClient;

void loop() {
  String inputString = "";
  // Проверяем, есть ли подключенный клиент
  // EthernetClient client = server.available();
  EthernetClient _client = server.accept();
  if (_client) {
    activeClient = _client;
    Serial.println("Client Connected!");
  }

  if (activeClient && activeClient.connected()) {
    while (activeClient.available() > 0) {
      char inChar = (char)activeClient.read();
      if (inChar == '\n') {
        processCommand(inputString);
        inputString = "";
      } else if (inChar != '\r') {
        inputString += inChar;
      }
    }

    if (millis() - lastSend >= interval) {
      String data = "STATE:";  // Маркер начала
      for (int i = 2; i < NUM_DIGITAL_PINS; i++) {
        data += "D" + String(i) + ":" + String(digitalRead(i)) + ";";
      }
      for (int i = 0; i < NUM_ANALOG_INPUTS; i++) {
        data += "A" + String(i) + ":" + String(analogRead(i)) + ";";
      }
      data += "\n";

      activeClient.print(data);
      lastSend = millis();
    }
  } else if (activeClient) {
    Serial.println("Client Disconnected");
    activeClient.stop();
  }

  // int val5 = analogRead(5);
  // if (val5 > 500) {
  //   server.println("A5: " + String(val5));
  // }
}

void processCommand(String cmd) {
  cmd.trim();  // Убираем лишние пробелы
  Serial.println("Command received: " + cmd);

  if (cmd == "LED_ON") {
    digitalWrite(LED_BUILTIN, HIGH);
    server.println("OK: LED IS ON");  // Отправка ответа в .NET
  } else if (cmd == "LED_OFF") {
    digitalWrite(LED_BUILTIN, LOW);
    server.println("OK: LED IS OFF");
  }
}

// Use fro test: telnet 192.168.1.177 23
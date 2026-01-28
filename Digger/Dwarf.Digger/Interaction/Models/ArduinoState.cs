namespace Dwarf.Digger.Interaction.Models;

record ArduinoPinState(string Name, int Value, bool IsAnalog = false);

record ArduinoState(ArduinoPinState[] PinStates);
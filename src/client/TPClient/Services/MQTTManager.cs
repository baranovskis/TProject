using Newtonsoft.Json;
using System;
using System.Text;
using TPClient.Model.Api;
using TPClient.Utilities;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace TPClient.Services
{
    public class MQTTManager : IDisposable
    {
        private MqttClient _mqttClient;
        private Action<Signal> _signalHandler;

        public MQTTManager(Action<Signal> signalHandler)
        {
            Log.Instance.Info("Init new signals listener...");

            _signalHandler = signalHandler;
        }

        public bool ListenSignalsTask()
        {
            Log.Instance.Info("Init MQTT subscriber & publisher...");

            try
            {
                _mqttClient = new MqttClient(Properties.Settings.Default.MQTT_BROKER_ADDRESS, Properties.Settings.Default.MQTT_BROKER_PORT, false, MqttSslProtocols.None, null, null);
                _mqttClient.MqttMsgPublishReceived += MqttMsgPublishReceived;

                string clientId = Guid.NewGuid().ToString();
                _mqttClient.Connect(clientId);

                _mqttClient.Publish(Properties.Settings.Default.MQTT_PUBLISH, new byte[] { });
                _mqttClient.Subscribe(new string[] { Properties.Settings.Default.MQTT_SUBSCRIBE }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error(e);
            }

            return false;
        }

        public void StopListeningForSignals()
        {
            Log.Instance.Debug("Stop listening signals");

            _mqttClient?.Disconnect();
            _mqttClient = null;
        }

        public bool IsListeningForSignals()
        {
            if (_mqttClient == null)
                return false;

            return _mqttClient.IsConnected;
        }

        private void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            try
            {
                if (e.Message == null)
                {
                    Log.Instance.Warn("Mqtt message is null, return and try get last dweet");
                    return;
                }

                var data = Encoding.UTF8.GetString(e.Message);
                var signal = JsonConvert.DeserializeObject<Signal>(data);

                if (signal == null)
                {
                    Log.Instance.Warn("Signal deserialize object failed! JSON: {0}", data);
                    return;
                }

                _signalHandler(signal);
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
            }
        }

        public void Dispose()
        {
            StopListeningForSignals();
        }
    }
}
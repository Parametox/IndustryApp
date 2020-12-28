using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mqtt;
using Prism.Services;
using IndustryApp.Model;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Xamarin.Forms;

namespace IndustryApp.ViewModels
{

    public class MainPageViewModel : ViewModelBase
    {
        private int ObjId;
        //private string mqttBroker = "mqtt.eclipse.org";
        private string mqttBroker = "test.mosquitto.org";
        private string clientId = Guid.NewGuid().ToString();
        //private List<ReceiverData> ReceiverDataList1;
        private MqttConfiguration config;
        private IMqttClient client;
        private Queue<ReciverData> queue;
        private ReciverData temp = new ReciverData();

        #region Properities
        private long fieldName;
        public long PropertyName
        {
            get { return fieldName; }
            set { SetProperty(ref fieldName, value); }
        }

        private string temperature;
        public string Temperature
        {
            get { return temperature; }
            set { SetProperty(ref temperature, value); }
        }


        private string date;
        public string Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

        private ReciverData reciverData;
        public ReciverData ReciverData
        {
            get { return reciverData; }
            set { SetProperty(ref reciverData, value); }
        }
        #endregion



        #region Methods
        private void InitValues()
        {
            base.Title = "Parametry układu";
            queue = new Queue<ReciverData>();
            this.InitBroker();
        }

        private async void InitBroker()
        {
            try
            {
                config = new MqttConfiguration();
                client = await MqttClient.CreateAsync(mqttBroker, config);
                var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: this.clientId.Replace("-", String.Empty)));
                await client.SubscribeAsync("pcipcipci", MqttQualityOfService.AtMostOnce);
            }
            catch (Exception ex)
            {
                await pageDialogService.DisplayAlertAsync("BŁĄD", "[MQTTBrokerException ]" + ex.Message, "OK");
            }

            client.MessageStream.Subscribe(msg => returnFormMqtt(msg.Topic, msg.Payload));// todo: optymalizacja, dedykowany wątek
        }
        private async void returnFormMqtt(string topic, byte[] text)
        {
            var txt = Encoding.UTF8.GetString(text);
            var txt1 = Encoding.UTF32.GetString(text);
            var txt2 = Encoding.UTF7.GetString(text);

            string callback = txt;
            int idx = 0;

            // pierwsza opcja
            ArrayList parameters = new ArrayList();
            parameters.Add(Temperature);
            parameters.Add(Address);

            if (!String.IsNullOrEmpty(callback))
            {
                string temp = String.Empty;

                for (int i = 0; i < parameters.Count; i++)
                {
                    idx = callback.IndexOf('-');
                    temp = callback.Substring(0, idx < 0 ? callback.Length : idx);
                    callback = callback.Remove(0, idx + 1);
                    parameters[i] = temp;
                }
            }
            temp = new ReciverData();
            temp.Address = parameters[0].ToString();
            temp.Temperature = parameters[1].ToString();
            this.queue.Enqueue(temp);
            Task.Run(() => this.InsertData());
            this.UpdateDisplay();
        }

        private void InsertData()
        {
            using (SqlConnection connection = new SqlConnection(sqlConnectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO TemperatureTable " +
                                                //$" VALUES('{temp.Temperature}', '{temp.Address}', '{temp.Date.ToString("yyyy-MM-dd HH:mm:ss")})'";
                                                " VALUES (@temp, @addr, @dt)";
                        command.Parameters.AddWithValue("@temp", temp.Temperature);
                        command.Parameters.AddWithValue("@addr", temp.Address);
                        command.Parameters.AddWithValue("@dt", temp.Date);

                        command.CommandType = System.Data.CommandType.Text;
                        int rows = command.ExecuteNonQuery();
                        //using (SqlDataReader reader = command.ExecuteReader())
                        //{
                        //    while (reader.Read())
                        //    {
                        //        Console.WriteLine(reader.GetInt64(0));
                        //    }
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Device.BeginInvokeOnMainThread(() => pageDialogService.DisplayAlertAsync("UWAGA",
                                                                   ex.Message,
                                                                   "OK"));
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void UpdateDisplay()
        {
            this.ReciverData = this.queue.Dequeue();
        }


        //private void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        //{
        //    string callback = Encoding.UTF32.GetString(e.Message);
        //    int idx = 0;

        //    // pierwsza opcja
        //    ArrayList parameters = new ArrayList();
        //    parameters.Add(Temperature);
        //    parameters.Add(Address);
        //    parameters.Add(Date);

        //    var x = new MqttConfiguration()

        //    // druga opcja
        //    ReceiverData receiverData = new ReceiverData();
        //    ArrayList parameters1 = new ArrayList();
        //    parameters1.Add(receiverData.Temperature);
        //    parameters1.Add(receiverData.SensorAddress);
        //    parameters1.Add(receiverData.Date);

        //    if (String.IsNullOrEmpty(callback))
        //    {
        //        string temp = String.Empty;

        //        for (int i = 0; i < parameters.Count; i++)
        //        {
        //            idx = callback.IndexOf('-');
        //            temp = callback.Substring(0, idx - 1);
        //            callback = callback.Remove(0, idx);
        //            parameters[i] = temp;
        //            parameters1[i] = temp;
        //        }
        //    }

        //    ReceiverDataList.Add(receiverData);
        //}

        //private void Client_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        //{

        //}
        #endregion



        #region Commands

        #endregion



        public MainPageViewModel(IPageDialogService p, INavigationService ns)
            : base(p, ns)
        {
            InitValues();

        }
    }
}

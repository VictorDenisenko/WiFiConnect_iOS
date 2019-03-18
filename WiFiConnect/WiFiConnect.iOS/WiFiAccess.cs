using Foundation;
using WiFiConnect.iOS;
using UIKit;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading;
using NetworkExtension;

[assembly: Dependency(typeof(WiFiAccess))]
namespace WiFiConnect.iOS
{
    delegate string FunctionTheSameAddress();

    public class WiFiAccess : IWiFiInterface
    {
        MainPage mp;
        BasicAlgorthm ba;
        
        string networkKey = "";
        string ssid = "";

        public WiFiAccess()
        {
            mp = MainPage.mainPage;
            ba = new BasicAlgorthm();
        }

        public async void ConnectToWifi(string _networkKey, string _ssid)
        {

            ssid = _ssid;
            networkKey = _networkKey;

            string result = await ba.IteratorAsync(CommonStruct.robotNetworkPassword, CommonStruct.robotNetworkAdminName, "", ba.LookingForInfo);
            if (result != "OK")
            {//Посылаю простейшую команду чтобы проверить наличие соединения
                mp.NotifyUser("Something is wrong. May be you need to reboot the robot.", MainPage.NotifyType.ErrorMessage);
            }
            else
            {
                string resultIpAddress = await ba.IteratorAsync(CommonStruct.robotNetworkPassword, CommonStruct.robotNetworkAdminName, "192.168.137.1:8080", ba.LookingForIPAddress);
                //Этот метод нужен, т.к. в нем находится Guid, без которого нельзя послать команду в Распберри
                if (resultIpAddress == "OK")
                {
                    mp.NotifyUser("Your robot is already connected to WiFi. IP address is " + CommonStruct.robotIpAddress, MainPage.NotifyType.StatusMessage);
                }
                else if (resultIpAddress == "NotConnected")
                {//Посылаем запрос к Windows Device Portal API, чтобы подключить Raspberry 
                    string resultConnect = await ba.ConnectRobotToWiFiAsync(ssid, networkKey, "");
                    resultIpAddress = await ba.IteratorAsync(CommonStruct.robotNetworkPassword, CommonStruct.robotNetworkAdminName, "192.168.137.1:8080", ba.LookingForIPAddress);
                    // Убеждаемся, что подключение есть.
                    if (resultIpAddress == "OK")
                    {
                        mp.NotifyUser("BotEyes is connected to WiFi successfully. IP address: " + CommonStruct.robotIpAddress, MainPage.NotifyType.StatusMessage);
                    }
                    else
                    {
                        mp.NotifyUser("Connection failed. Check SSID and password and try again.", MainPage.NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    mp.NotifyUser("I am not sure all is OK. See if robot green light diode is On. In other case try again." + CommonStruct.robotIpAddress, MainPage.NotifyType.ErrorMessage);
                }
            }
        }
    }
}
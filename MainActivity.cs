using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System;
using Android.Content;
using Android.Views;
using Android.Bluetooth;


using System.Linq;

namespace HC_06
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        BluetoothConnection myConnection = new BluetoothConnection();

        public BluetoothConnection MyConnection { get => myConnection; set => myConnection = value; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Button buttonConnect = FindViewById<Button>(Resource.Id.button1);
            Button buttonDisconnect = FindViewById<Button>(Resource.Id.button2);
            Button buttonStart = FindViewById<Button>(Resource.Id.button3);
            Button buttonStop = FindViewById<Button>(Resource.Id.button3);

            TextView connected = FindViewById<TextView>(Resource.Id.textView1);

                       // buttonDisconnect.Enabled = false;

            BluetoothSocket _connect;

            System.Threading.Thread listenThread = new System.Threading.Thread(Listener);
            listenThread.Abort();

            buttonConnect.Click += delegate {

                //DISCONNECT CLOSE
                //try {
                //    buttonDisconnect.Enabled = false;
                //    buttonConnect.Enabled = true;
                //    listenThread.Abort();

                //    myConnection.ThisDevice.Dispose();

                //    myConnection.ThisSocket.OutputStream.WriteByte(187);
                //    myConnection.ThisSocket.OutputStream.Close();

                //    myConnection.ThisSocket.Close();

                //    myConnection = new BluetoothConnection();
                //    _connect = null;

                //    connected.Text = "Disconnected!";
                //}
                //catch { }

                ////////////////////////////////////////////////
                listenThread.Start();

                MyConnection = new BluetoothConnection();
                //myConnection.ThisSocket = null;
                //_connect = null;

            MyConnection.GetAdapter();

                MyConnection.ThisAdapter.StartDiscovery();

            try
            { 
                    
                MyConnection.GetDevice();
                     MyConnection.ThisDevice.SetPairingConfirmation(false);
                  //   myConnection.ThisDevice.Dispose();
                    MyConnection.ThisDevice.SetPairingConfirmation(true); 
                    MyConnection.ThisDevice.CreateBond();


            }
                catch (Exception deviceEX)
                {
                }

                MyConnection.ThisAdapter.CancelDiscovery();


                _connect = myConnection.ThisDevice.CreateRfcommSocketToServiceRecord(Java.Util.UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                
                MyConnection.ThisSocket = _connect;

                //   System.Threading.Thread.Sleep(500);
                try
                {
                    MyConnection.ThisSocket.Connect();

                    connected.Text = "Connected!";
                    buttonDisconnect.Enabled = true;
                    buttonConnect.Enabled = false;

                    if (listenThread.IsAlive == false)
                    {
                        listenThread.Start();
                    }
                    //else
                    //{
                    //    listenThread.Abort();
                    //}

                }
                    catch (Exception CloseEX)
                {

                }


            };

            buttonDisconnect.Click += delegate {

                try {
                    //  buttonDisconnect.Enabled = false;
                    buttonConnect.Enabled = true;
                    listenThread.Abort();

                    MyConnection.ThisDevice.Dispose();

                    MyConnection.ThisSocket.OutputStream.WriteByte(187);
                    MyConnection.ThisSocket.OutputStream.Close();

                    MyConnection.ThisSocket.Close();

                    MyConnection = new BluetoothConnection();
                    _connect = null;

                    connected.Text = "Disconnected!";
                }
                catch  { }
            };

            buttonStart.Click += delegate
            {
                // byte[] Test = new byte [9];
                // Test [0] = 72; //H
                // Test [1] = 101; //e
                // Test [2] = 108; //l
                // Test [3] = 108; //l
                // Test [4] = 111; //o

                try
                {
                    // myConnection.ThisSocket.OutputStream,Write(Test, 0, Test.Length);
                    MyConnection.ThisSocket.OutputStream.WriteByte(1);
                    MyConnection.ThisSocket.OutputStream.WriteByte(1);
                    MyConnection.ThisSocket.OutputStream.WriteByte(1);
                    MyConnection.ThisSocket.OutputStream.Close();
                }
                catch (Exception outPutEX)
                {
                }
            };

            buttonStop.Click += delegate
            {
                // byte[] Test = new byte [9];
                // Test [0] = 72; //H
                // Test [1] = 101; //e
                // Test [2] = 108; //l
                // Test [3] = 108; //l
                // Test [4] = 111; //o

                try
                {
                    // myConnection.ThisSocket.OutputStream,Write(Test, 0, Test.Length);
                    MyConnection.ThisSocket.OutputStream.WriteByte(2);
                    MyConnection.ThisSocket.OutputStream.WriteByte(2);
                    MyConnection.ThisSocket.OutputStream.WriteByte(2);
                    MyConnection.ThisSocket.OutputStream.Close();
                }
                catch (Exception outPutEX)
                { }
            };
        }

        void Listener()
        {
            byte[] read = new byte[1];

            TextView readTextView = FindViewById<TextView>(Resource.Id.textView2);
            //DateTime onTime = DateTime.Now;
            //DateTime offTime = DateTime.Now;
            //DateTime ThisTime;
            //TimeSpan ThisSpan;

            //int timeSetOn = 0;
            //int timeSetOff = 0;

            TextView timeTextView = FindViewById<TextView>(Resource.Id.textView3);
            while (true)
            {

                //ThisTime = DateTime.Now;


                try
                {

                    MyConnection.ThisSocket.InputStream.Read(read, 0, 1);

                    MyConnection.ThisSocket.InputStream.Close();
                    RunOnUiThread(() =>
                    {

                        if (read[0] == 1)
                        {

                            readTextView.Text = "Relais AN";

                            //if (timeSetOn == 0)
                            //{
                            //    onTime = DateTime.Now;
                            //    timeSetOn = 1;

                            //}


                        }
                        else if (read[0] == 0)
                        {
                            readTextView.Text = "Relais AUS";
                            //timeSetOn = 0;

                            timeTextView.Text = "";
                        }

                        //if (timeSetOn == 1)
                        //{
                        //    thisSpan = thisTime-onTime;
                        //    timeTextView.Text = thisSpan.Minutes + ":" + thisSpan.Seconds;
                        //}

                    });
                }
                catch { }

            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public class BluetoothConnection
        {

            public void GetAdapter() { this.ThisAdapter = BluetoothAdapter.DefaultAdapter; }
            public void GetDevice() { this.ThisDevice = (from bd in this.ThisAdapter.BondedDevices where bd.Name == "EnviroMon" select bd).FirstOrDefault(); }

            public BluetoothAdapter ThisAdapter { get; set; }
            public BluetoothDevice ThisDevice { get; set; }

            public BluetoothSocket ThisSocket { get; set; }



        }
    }
}
using System;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Collections.ObjectModel;

using Microsoft.Devices;
using com.google.zxing;
using com.google.zxing.common;
using com.google.zxing.qrcode;
using System.Net;

namespace Phone2Web
{
    public partial class MainPage
    {
        const string relayurl = "http://192.168.1.70:31416/";

        private readonly DispatcherTimer _timer;
        private readonly ObservableCollection<string> _matches;

        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _reader;
        private PhotoCamera _photoCamera;
        
        public MainPage()
        {            
            InitializeComponent();

            _matches = new ObservableCollection<string>();
            _matchesList.ItemsSource = _matches;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _photoCamera = new PhotoCamera();
            _photoCamera.Initialized += OnPhotoCameraInitialized;            
            _previewVideo.SetSource(_photoCamera);

            CameraButtons.ShutterKeyHalfPressed += (o, arg) => _photoCamera.Focus();

            base.OnNavigatedTo(e);
        }

        private void OnPhotoCameraInitialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            
            _luminance = new PhotoCameraLuminanceSource(width, height);
            _reader = new QRCodeReader();

            Dispatcher.BeginInvoke(() => {
                _previewTransform.Rotation = _photoCamera.Orientation;
                _timer.Start();
            });
        }
 
        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
#if DEBUG
                var text = "http://localhost?h=foobarstore.com&p=255&id=1234";
#else
                var result = _reader.decode(binBitmap);
                var text = result.Text;                
#endif
                Dispatcher.BeginInvoke(() => DisplayResult(text));

            }
            catch (Exception e)
            {
            }            
        }

        private void DisplayResult(string text)
        {
            if (!_matches.Contains(text) && !_matches.Contains("Error parsing: " + text))
            {
                try
                {
                    this.SendDataToRelay(text);
                    _matches.Add(text);
                }
                catch
                {
                    _matches.Add("Error parsing: " + text);
                }
            }
        }

        void SendDataToRelay(string text)
        {
            string requestid;
            ulong parameters;

            this.ExtractData(text, out requestid, out parameters);

            Collection<string> identifiers = PersonalDataStore.GetIdentifiers(parameters);
            string json = PersonalDataStore.GetData(identifiers);
            WebClient client = new WebClient();
            client.UploadStringCompleted += new UploadStringCompletedEventHandler(client_UploadStringCompleted);
            client.UploadStringAsync(new Uri(relayurl + requestid), json);
        }

        void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                _matches.Add("Error sending data to relay.");
            }
            else
            {
                _matches.Add("Data sent to relay.");
            }
        }

        void ExtractData(string text, out string requestid, out ulong parameters)
        {
            requestid = null;
            parameters = 0;
            Uri uri = new Uri(text, UriKind.Absolute);
            string[] components = uri.Query.Split('&');
            foreach (string component in components)
            {
                if (component.StartsWith("p="))
                {
                    parameters = ulong.Parse(component.Substring(2));
                }
                else if (component.StartsWith("id="))
                {
                    requestid = component.Substring(3);
                }
            }

            if (parameters == 0 || requestid == null)
            {
                throw new InvalidOperationException("Unable to extract data from the URI.");
            }
        }
    }
}
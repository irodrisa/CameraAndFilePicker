using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.UI.Xaml.Controls;

namespace MediaApp.Common
{
    public class UniversalCamera
    {
        private static UniversalCamera _Default;
        public CaptureElement CameraCaptureElement;
        public MediaCapture CameraMediaCapture;

        public static UniversalCamera GetDefault()
        {
            if (_Default == null)
            {
                _Default = new UniversalCamera();
            }
            return _Default;
        }

        public async Task InitPreview(CaptureElement captureElement)
        {
            this.CameraCaptureElement = captureElement;
            this.CameraMediaCapture = new MediaCapture();

            // Initialize capture settings for Video mode
            MediaCaptureInitializationSettings settings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
            settings.PhotoCaptureSource = PhotoCaptureSource.VideoPreview;

            // Select proper Video device to match back camera (depending on whether or not the device has front+back cameras or only back camera)
            DeviceInformationCollection Videodevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            if (Videodevices.Count == 0)
            {
                // No camera available
                captureElement.Tag = "NoCamera";
                return;
            }
            else
            {
                this.CameraCaptureElement.Tag = "CameraFound";
                if (Videodevices.Count == 1)
                {
                    settings.VideoDeviceId = Videodevices[0].Id; // Videodevices[0].Id; -> back camera
                }
                else
                {
                    settings.VideoDeviceId = Videodevices[1].Id; // Videodevices[0].Id; -> front camera, Videodevices[1].Id; -> back camera
                }               
            }            
          
            // Set settings and source on Camera
            await this.CameraMediaCapture.InitializeAsync(settings);
            SetResolution();

            // Adjust camera preview orientation
            DisplayInformation displayInfo = DisplayInformation.GetForCurrentView();
            displayInfo.OrientationChanged += DisplayInfo_OrientationChanged;
            DisplayInfo_OrientationChanged(displayInfo, null);

            this.CameraCaptureElement.Source = this.CameraMediaCapture;            

            // Start Camera preview
            await this.CameraMediaCapture.StartPreviewAsync();
        }

        public async void SetResolution()
        {
            System.Collections.Generic.IReadOnlyList<IMediaEncodingProperties> res;
            res = CameraMediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview);
            uint maxResolution = 0;
            int indexMaxResolution = 0;

            if (res.Count >= 1)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    VideoEncodingProperties vp = (VideoEncodingProperties)res[i];

                    if (vp.Width > maxResolution)
                    {
                        indexMaxResolution = i;
                        maxResolution = vp.Width;
                    }
                }
                await CameraMediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, res[indexMaxResolution]);
            }
        }

        private void DisplayInfo_OrientationChanged(DisplayInformation sender, object args)
        {
            if (this.CameraMediaCapture != null)
            {
                this.CameraMediaCapture.SetPreviewRotation(VideoRotationLookup(sender.CurrentOrientation, false));
            }
        }

        private VideoRotation VideoRotationLookup(DisplayOrientations displayOrientation, bool counterclockwise)
        {
            switch (displayOrientation)
            {
                case DisplayOrientations.Landscape:
                    return VideoRotation.None;

                case DisplayOrientations.Portrait:
                    return (counterclockwise) ? VideoRotation.Clockwise270Degrees : VideoRotation.Clockwise90Degrees;

                case DisplayOrientations.LandscapeFlipped:
                    return VideoRotation.Clockwise180Degrees;

                case DisplayOrientations.PortraitFlipped:
                    return (counterclockwise) ? VideoRotation.Clockwise90Degrees :
                    VideoRotation.Clockwise270Degrees;

                default:
                    return VideoRotation.None;
            }
        }

        public async Task StopPreview()
        {
            // No camera available anymore
            this.CameraCaptureElement.Tag = "NoCamera";

            // Stop Camera preview
            await this.CameraMediaCapture.StopPreviewAsync();
            this.CameraMediaCapture.Dispose();
            this.CameraMediaCapture = null;

            // Reset Camera
            this.CameraCaptureElement.Source = null;
            _Default = null;
        }
    }
}

using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;


namespace ColorToDepth_KinectV1._8
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor sensor;

        private int showValidColorPixels = 0;
        /// <summary>
        /// Bitmap that will hold color information
        /// </summary>
        private WriteableBitmap colorBitmap;

        private WriteableBitmap depthBitmap;
        //   private WriteableBitmap depthBitmap;
        /// <summary>
        /// Intermediate storage for the color data received from the camera
        /// </summary>
        private byte[] colorPixels;

        private byte[] depthPixelsArray;

        /// <summary>
        /// Color image format variable
        /// </summary>
        private ColorImageFormat colorFormat = ColorImageFormat.RgbResolution640x480Fps30;

        private DepthImageFormat depthFormat = DepthImageFormat.Resolution320x240Fps30;
        private DepthImagePixel[] depthPixels;

        /// <summary>
        /// To hold depth image points that have been mapped.
        /// </summary>
        private DepthImagePoint[] depthPoints;

        /// <summary>
        /// Intermediate storage for the depth to color mapping
        /// </summary>
        private ColorImagePoint[] colorCoordinates;

        // private int colorToDepthDivisor;

        private int depthWidth;
        private int depthHeight;
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Execute startup tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            // Look through all sensors and start the first connected one.
            // This requires that a Kinect is connected at the time of app startup.
            // To make your app robust against plug/unplug, 
            // it is recommended to use KinectSensorChooser provided in Microsoft.Kinect.Toolkit (See components in Toolkit Browser).
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                // Turn on the color stream to receive color frames
                this.sensor.ColorStream.Enable(this.colorFormat);

                int colorWidth = this.sensor.ColorStream.FrameWidth;
                int colorHeight = this.sensor.ColorStream.FrameHeight;

                //Turn on the depth stream to recieve depth frames
                this.sensor.DepthStream.Enable(this.depthFormat);

                this.depthWidth = this.sensor.DepthStream.FrameWidth;
                this.depthHeight = this.sensor.DepthStream.FrameHeight;

                //Allocate space for depth pixels
                this.depthPixelsArray = new byte[this.sensor.DepthStream.FramePixelDataLength];

                this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];

                this.depthPoints = new DepthImagePoint[colorWidth * colorHeight];

                // Allocate space to put the pixels we'll receive
                this.colorPixels = new byte[this.sensor.ColorStream.FramePixelDataLength];

                this.colorCoordinates = new ColorImagePoint[this.sensor.DepthStream.FramePixelDataLength];



                // This is the bitmap we'll display on-screen
                this.colorBitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                this.depthBitmap = new WriteableBitmap(depthWidth, depthHeight, 96.0, 96.0, PixelFormats.Bgr32, null);

                // this.depthBitmap = new WriteableBitmap(depthWidth, depthHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
                // Set the image we display to point to the bitmap where we'll put the image data
                this.camera.Source = this.colorBitmap;
                this.depth.Source = this.depthBitmap;


                //this.depth.Source = this.depthBitmap;

                // Add an event handler to be called whenever there is new color frame data
                this.sensor.AllFramesReady += this.SensorAllFramesReady;

                // Start the sensor!
                try
                {
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    this.sensor = null;
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.Stop();
            }
        }

        /// <summary>
        /// Event handler for Kinect sensor's ColorFrameReady event
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void SensorAllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame != null)
                {


                    // Copy the pixel data from the image to a temporary array
                    colorFrame.CopyPixelDataTo(this.colorPixels);

                    //Changes alpha value to 0 if the pixel does not have a known depth
                    if (showValidColorPixels == 0)
                    {
                        for (int i = 0; i < (colorPixels.Length) / 4; i++)
                        {
                            if (depthPoints.ElementAt(i).Depth == 0)
                            {
                                colorPixels[i * 4] = 0;
                            }
                        }
                    }

                        // Write the color pixel data into our bitmap

                        this.colorBitmap.WritePixels(
                            new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                            this.colorPixels,
                            this.colorBitmap.PixelWidth * sizeof(int),
                            0);

                }
            }

            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame != null)
                {

                    // Copy the pixel data from the image to a temporary array
                    depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                    // Get the min and max reliable depth for the current frame
                    int minDepth = depthFrame.MinDepth;
                    int maxDepth = depthFrame.MaxDepth;

                    this.sensor.CoordinateMapper.MapColorFrameToDepthFrame(colorFormat, depthFormat, this.depthPixels, this.depthPoints); //depth points contains mapped points

                    this.sensor.CoordinateMapper.MapDepthFrameToColorFrame(depthFormat, this.depthPixels, colorFormat, this.colorCoordinates);




                    // Write the color pixel data into our bitmap
                    this.depthBitmap.WritePixels(
                        new Int32Rect(0, 0, this.depthWidth, this.depthHeight),
                        this.depthPixels,
                        this.depthBitmap.PixelWidth * sizeof(int),
                        0);


                    /**
                    // Write the depth pixel data into our bitmap
                    this.depthBitmap.WritePixels(
                        new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                        this.depthPixels,
                        this.depthBitmap.PixelWidth * sizeof(int),
                        0);
                    //Map color image to depth image
                   */



                }
            }


        }

        private void TextBlock_LeftMouseDown(Object sender, MouseEventArgs e)
        {
            var block = sender as TextBlock;
            if (block != null)
            {
                block.Text = " millimeters";
            }
        }

        private void depth_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (sender != null)
            {
                System.Windows.Point point = e.GetPosition(depth);

                ColorImagePoint cpoint = colorCoordinates.ElementAt((int)(point.X + (point.Y) * this.depthWidth));

                DepthImagePixel dpix = depthPixels.ElementAt((int)(point.X + (point.Y) * this.depthWidth));
                if (dpix.IsKnownDepth)
                {
                    distanceData.Text = " " + ((double)(dpix.Depth) / 1000) + " meters";
                    coordinatesText.Text = "x: " + cpoint.X + " y: " + cpoint.Y;
                }
                else
                    distanceData.Text = " unkown depth";
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 5;
                ellipse.Height = 5;
                ellipse.Fill = System.Windows.Media.Brushes.Orange;
                Canvas.SetLeft(ellipse, point.X);
                Canvas.SetTop(ellipse, point.Y);

                depthCanvas.Children.Add(ellipse);

            }
        }
        private void color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {


            if (sender != null)
            {
                System.Windows.Point point = e.GetPosition(camera);
                DepthImagePoint dpoint = this.depthPoints.ElementAt((int)((point.X) + (point.Y) * this.sensor.ColorStream.FrameWidth));
                if (dpoint.Depth > 0)
                {
                    distanceData.Text = " " + ((double)(dpoint.Depth) / 1000) + " meters";
                }
                else distanceData.Text = " Unknown Depth";

                colorPixelCoordinates.Text = "x: " + point.X + "y: " + point.Y;
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 5;
                ellipse.Height = 5;
                ellipse.Fill = System.Windows.Media.Brushes.Orange;
                Canvas.SetLeft(ellipse, point.X);
                Canvas.SetTop(ellipse, point.Y);

                colorCanvas.Children.Add(ellipse);

            }
        }
        private void color_toggle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                showValidColorPixels++;
                showValidColorPixels = showValidColorPixels % 2;
            }
        }
        private void clear_circles_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != null)
            {
                colorCanvas.Children.Clear();
                depthCanvas.Children.Clear();
            }
        }
    }
}
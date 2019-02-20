using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Tools;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class Sensordata
{

    private static int hand;
    private static int maxfrequence = 7000;

    public static List<ulong> bodyIdList;//get set
    public static List<int> frequences; //get set
    public static List<double> Zaxis; //get set

    public static bool trackdata { get; set; }
    public static KinectSensor kinectSensor = null;
    public static BodyFrameReader BodyFrameReader;
    public static MultiSourceFrameReader MultiSourceFrameReader;
    public static ColorFrameReader ColorFrameReader;
    public static Body[] bodies = null; //get

    private static string data = null;

    private static bool datawritten = false;

    private static ColorSpacePoint M; // neck

    private static ColorSpacePoint RE; //right elbow
    private static ColorSpacePoint RH; //right hand
    private static ColorSpacePoint RS; //right shoulder

    private static ColorSpacePoint LE; //left elbow
    private static ColorSpacePoint LH; //left hand
    private static ColorSpacePoint LS; //left shoulder

    public static WriteableBitmap bitmap = null; //get
    public static DrawingImage Bodysource = null; //get
    private static DrawingGroup drawingGroup;


    private static Point pointRH;
    private static Point pointRE;
    private static Point pointRS;
    private static Point pointLH;
    private static Point pointLE;
    private static Point pointLS;
    private static Point pointMid;
    private static SolidColorBrush yellowbrush = new SolidColorBrush(Colors.Yellow);
    private static SolidColorBrush orangebrush = new SolidColorBrush(Colors.Orange);
    private static SolidColorBrush orangebrushT = new SolidColorBrush(Colors.Orange);
    private static Pen bodypen = new Pen(orangebrushT, 30);

    //getset
    public static WriteableBitmap getBitmap() { return bitmap; }
    public static DrawingImage getBodysource() { return Bodysource; }


    public static string getData()
    {
        if (datawritten == true)
            return data;    // Data string von Sensor
        else return "None"; // kein vom Sensor körper erkannt
    }


    public static void StartData() //initiallisiert KinectBodyFrameReader
    {
        kinectSensor = KinectSensor.GetDefault();
        FrameDescription colorFrameDescription = kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
        bitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
        drawingGroup = new DrawingGroup();
        Bodysource = new DrawingImage(drawingGroup);



        if (kinectSensor != null)
        {
            kinectSensor.Open();
        }

        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectSensor.Open();
        }

        yellowbrush.Opacity = 0.3;
        orangebrushT.Opacity = 0.5;
        BodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
        ColorFrameReader = kinectSensor.ColorFrameSource.OpenReader();


        if (ColorFrameReader != null)
        {
            ColorFrameReader.FrameArrived += Reader_ColorFrameArrived;
        }

        if (BodyFrameReader != null)
        {
            BodyFrameReader.FrameArrived += Reader_FrameArrived;
        }

    }


    public static void DisposeSensors() {
        if (BodyFrameReader != null)
        {
            // BodyFrameReader is IDisposable
            BodyFrameReader.Dispose();
            BodyFrameReader = null;
        }

        if (kinectSensor != null)
        {
            ColorFrameReader.Dispose();
            kinectSensor = null;
        }
        if (kinectSensor != null)
        {
            kinectSensor.Close();
            kinectSensor = null;
        }
    }

    public static void StartPausedSensors() //initiallisiert KinectBodyFrameReader
    {
        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectSensor.Open();
        }

        kinectSensor = KinectSensor.GetDefault();

        if (kinectSensor != null)
        {
            kinectSensor.Open();
        }

        BodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
        ColorFrameReader = kinectSensor.ColorFrameSource.OpenReader();


        if (ColorFrameReader != null)
        {
            ColorFrameReader.FrameArrived += Reader_ColorFrameArrived;
        }

        if (BodyFrameReader != null)
        {
            BodyFrameReader.FrameArrived += Reader_FrameArrived;
        }


    }


    


    public static void Reader_ColorFrameArrived(Object sender, ColorFrameArrivedEventArgs e)
    {
            // ColorFrame is IDisposable
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        bitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == bitmap.PixelWidth) && (colorFrameDescription.Height == bitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                bitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
                        }

                        bitmap.Unlock();
                    }

                    if (datawritten == true)
                    {
                        using (DrawingContext DrawingContext = drawingGroup.Open())
                        {
                            //reset for next input
                            Zaxis = new List<double>();
                            frequences = new List<int>();
                            bodyIdList = new List<ulong>();
                            foreach (Body body in bodies)
                            {
                                if (body != null)
                                {
                                    if (body.IsTracked)
                                    {
                                        // Find the joints
                                        Joint handRight = body.Joints[JointType.HandRight];
                                        Joint elbowRight = body.Joints[JointType.ElbowRight];
                                        Joint ShoulderRight = body.Joints[JointType.ShoulderRight];
                                        Joint handLeft = body.Joints[JointType.HandLeft];
                                        Joint elbowLeft = body.Joints[JointType.ElbowLeft];
                                        Joint ShoulderLeft = body.Joints[JointType.ShoulderLeft];
                                        Joint mid = body.Joints[JointType.SpineMid];

                                        RH = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(handRight.Position);
                                        RE = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(elbowRight.Position);
                                        RS = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(ShoulderRight.Position);
                                        LH = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(handLeft.Position);
                                        LE = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(elbowLeft.Position);
                                        LS = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(ShoulderLeft.Position);
                                        M = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(mid.Position);

                                        pointRH = new Point((int)RH.X, (int)RH.Y);
                                        pointRE = new Point((int)RE.X, (int)RE.Y);
                                        pointRS = new Point((int)RS.X, (int)RS.Y);
                                        pointLH = new Point((int)LH.X, (int)LH.Y);
                                        pointLE = new Point((int)LE.X, (int)LE.Y);
                                        pointLS = new Point((int)LS.X, (int)LS.Y);
                                        pointMid = new Point((int)M.X, (int)M.Y);


                                        DrawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, bitmap.PixelWidth, bitmap.PixelHeight));
                                        DrawingContext.DrawEllipse(yellowbrush, null, pointRH, 90, 90);
                                        DrawingContext.DrawEllipse(yellowbrush, null, pointLH, 90, 90);

                                        DrawingContext.DrawEllipse(orangebrush, null, pointRH, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointRE, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointRS, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointLH, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointLE, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointLS, 15, 15);
                                        DrawingContext.DrawEllipse(orangebrush, null, pointMid, 15, 15);

                                        DrawingContext.DrawLine(bodypen, pointRH, pointRE);
                                        DrawingContext.DrawLine(bodypen, pointRE, pointRS);
                                        DrawingContext.DrawLine(bodypen, pointRS, pointMid);
                                        DrawingContext.DrawLine(bodypen, pointMid, pointLS);
                                        DrawingContext.DrawLine(bodypen, pointLS, pointLE);
                                        DrawingContext.DrawLine(bodypen, pointLE, pointLH);


                                        // get Y for each body
                                        Zaxis.Add(handRight.Position.Z);

                                        Joint Head = body.Joints[JointType.Head];
                                        Joint KneeRight = body.Joints[JointType.KneeRight];
                                        ColorSpacePoint min = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(Head.Position);
                                        ColorSpacePoint max = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(KneeRight.Position);


                                        if ((max.Y > pointRH.Y) && (min.Y < pointRH.Y))
                                        {
                                            hand = (int)(maxfrequence * (((max.Y - pointRH.Y) / (max.Y - min.Y))));
                                            frequences.Add(hand);
                                        }
                                        else if (min.Y > pointRH.Y)
                                        {
                                            frequences.Add((int)maxfrequence);
                                        }
                                        else frequences.Add(0);
                                    bodyIdList.Add(body.TrackingId);

                                }

                                }
                            }

                        drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, bitmap.PixelWidth, bitmap.PixelHeight));
                            datawritten = false;
                        }
                    }
                }
            }
        

    }








    public static void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e) //KinectBodyFrameReader
    {

            bool dataReceived = false;
            datawritten = false;
            data = null;
            data += Environment.NewLine + "/" + Environment.NewLine + Environment.NewLine;
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }
            if (dataReceived == true)
            {
                foreach (Body body in bodies)
                {
                    if (body != null)
                    {
                        if (body.IsTracked)
                        {
                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints; // Dictionary mit joints in bodies
                            data += body.TrackingId.ToString() + Environment.NewLine;
                            foreach (JointType jointType in joints.Keys) // für jeden joint im Körper
                            {
                                CameraSpacePoint position = joints[jointType].Position;
                                ColorSpacePoint colorpoint = kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(position);
                                data += ("" + colorpoint.X + '|' + colorpoint.Y + '|' + position.Z + Environment.NewLine); //name|x|y|z -newline
                            }
                            data += Environment.NewLine + "-" + Environment.NewLine + Environment.NewLine; //trennung von bodies
                        }
                    }
                }
                datawritten = true;
            
        }
    }
}

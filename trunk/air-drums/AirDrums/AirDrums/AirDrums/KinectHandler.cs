using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Research.Kinect.Nui; 

namespace AirDrums
{
    class KinectHandler 
    {
        Runtime nui; 
        public KinectHandler()
        {

           
        }

        private void SetupKinect()
        {
            if (Runtime.Kinects.Count == 0)
            {
               
            }
            else
            {
                //use first Kinect
                nui = Runtime.Kinects[0];

                //UseDepthAndPlayerIndex and UseSkeletalTracking
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking);

                //register for event
                nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);

                //DepthAndPlayerIndex ImageType
                nui.DepthStream.Open(ImageStreamType.Depth, 2, ImageResolution.Resolution320x240,
                    ImageType.DepthAndPlayerIndex);
            }
        }
        void nui_DepthFrameReady(object sender, ImageFrameReadyEventArgs e)
        {
            //Convert depth information for a pixel into color information
            byte[] ColoredBytes = GenerateColoredBytes(e.ImageFrame);

            //create an image based on returned colors

            PlanarImage image = e.ImageFrame.Image;
            image1.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null,
                ColoredBytes, image.Width * PixelFormats.Bgr32.BitsPerPixel / 8);
        }

        private byte[] GenerateColoredBytes(ImageFrame imageFrame)
        {

            int height = imageFrame.Image.Height;
            int width = imageFrame.Image.Width;

            //Depth data for each pixel
            Byte[] depthData = imageFrame.Image.Bits;


            //colorFrame contains color information for all pixels in image
            //Height x Width x 4 (Red, Green, Blue, empty byte)
            Byte[] colorFrame = new byte[imageFrame.Image.Height * imageFrame.Image.Width * 4];

            //Bgr32  - Blue, Green, Red, empty byte
            //Bgra32 - Blue, Green, Red, transparency 
            //You must set transparency for Bgra as .NET defaults a byte to 0 = fully transparent

            //hardcoded locations to Blue, Green, Red (BGR) index positions       
            const int BlueIndex = 0;
            const int GreenIndex = 1;
            const int RedIndex = 2;


            var depthIndex = 0;
            for (var y = 0; y < height; y++)
            {

                var heightOffset = y * width;

                for (var x = 0; x < width; x++)
                {

                    var index = ((width - x - 1) + heightOffset) * 4;

                    //var distance = GetDistance(depthData[depthIndex], depthData[depthIndex + 1]);
                    var distance = GetDistanceWithPlayerIndex(depthData[depthIndex], depthData[depthIndex + 1]);

                    if (distance > 2000)
                        distance = 2000;

                    var color = ((double)distance / 2000.0) * 255;

                    colorFrame[index + BlueIndex] = (byte)color;
                    colorFrame[index + GreenIndex] = (byte)color;
                    colorFrame[index + RedIndex] = (byte)color;

                    if (distance > 800 && distance < 900)
                    {
                        if (x < 100 && x > 50 && y < 100 && y > 50)
                        {
                            colorFrame[index + BlueIndex] = 0;
                            colorFrame[index + GreenIndex] = 0;
                            colorFrame[index + RedIndex] = 255;

                            //Console.Out.Write("Toquei vermelho");
                        }
                        else if (x < 200 && x > 150 && y < 100 && y > 50)
                        {
                            colorFrame[index + BlueIndex] = 0;
                            colorFrame[index + GreenIndex] = 255;
                            colorFrame[index + RedIndex] = 0;

                            //Console.Out.Write("Toquei verde");
                        }
                        else if (x < 300 && x > 250 && y < 100 && y > 50)
                        {
                            colorFrame[index + BlueIndex] = 255;
                            colorFrame[index + GreenIndex] = 0;
                            colorFrame[index + RedIndex] = 0;

                            // Console.Out.Write("Toquei blue");
                        }

                    }


                    /*if (distance <= 900)
                    {
                        //we are very close
                        colorFrame[index + BlueIndex] = 255;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 0;
       
                    }
                    else if (distance > 900 && distance < 2000)
                    {
                        //we are a bit further away
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 0;
                    }
                    else if (distance > 2000)
                    {
                        //we are the farthest
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 0;
                        colorFrame[index + RedIndex] = 255;
                    }
                    

                    ////equal coloring for monochromatic histogram
                    //var intensity = CalculateIntensityFromDepth(distance);
                    //colorFrame[index + BlueIndex] = intensity;
                    //colorFrame[index + GreenIndex] = intensity;
                    //colorFrame[index + RedIndex] = intensity;

                    ////Color a player
                    if (GetPlayerIndex(depthData[depthIndex]) > 0)
                    {
                        //we are the farthest
                        colorFrame[index + BlueIndex] = 0;
                        colorFrame[index + GreenIndex] = 255;
                        colorFrame[index + RedIndex] = 255;
                    }
                    */
                    //jump two bytes at a time
                    depthIndex += 2;
                }
            }

            return colorFrame;
        }


    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using BlogEngine.Core.Providers;
using System.Drawing.Imaging;
using System.Reflection;

namespace BlogEngine.Core.FileSystem
{
    /// <summary>
    /// image specific class, will contain the extra methods and properties of an image
    /// </summary>
    public partial class Image : File
    {
        #region Fields , Constants & Enums
        /// <summary>
        /// the current image bitmap. call the accessor directly
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// enum for the file modification methods
        /// </summary>
        public enum ModificationType
        {
            FlipHorizontal,
            FlipVertical,
            RotateLeft,
            RotateRight
        }
        #endregion

        #region Ctor
        /// <summary>
        /// ctor, copys all the properties from the file object into the new object.
        /// </summary>
        public Image(File obj)
        {
            if (obj != null)
            {
                var qry = typeof(File).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
                foreach (var pi in qry)
                    this.GetType().GetProperty(pi.Name).SetValue(this, pi.GetValue(obj, null), null);
            }
            if (!this.IsImage)
                throw new Exception("File not of an image type");
            
        }

        /// <summary>
        /// ctor, property free constructor, initiates the this.Image(file) passing in a null file reference
        /// </summary>
        public Image()
            : this(null) { }

        #endregion

        #region Properties
        /// <summary>
        /// gets the bitmap of the current image Item
        /// </summary>
        /// <remarks>
        /// set accessor set to internal
        /// </remarks>
        public Bitmap bitMap
        {
            get
            {
                if (this.bitmap == null)
                    bitMap = ArrayToBmp(this.FileContents);
                return this.bitmap;
            }
            internal set
            {
                this.bitmap = value;
            }
        }

        /// <summary>
        /// gets the full download path to the file, using the file handler
        /// </summary>
        public string ImageUrl
        {
            get
            {
                return string.Format("{0}IMAGES{1}.jpgx", Utils.RelativeWebRoot, this.SafeFilePath);
            }
        }

        #endregion

        #region Methods
        /// <summary>
        /// resizes the image to a new size. The image will be replaced
        /// </summary>
        /// <param name="newSize">the new Size for the image</param>
        /// <returns>the new image</returns>
        public Image ResizeImage(Size newSize)
        {
            this.bitMap = (Bitmap)resizeImage((System.Drawing.Image)this.bitMap, newSize);
            var nfile = BlogService.UploadFile(BmpToArray(this.bitMap), this.Name, this.ParentDirectory, true);
            this.FileContents = nfile.FileContents;
            nfile.Dispose();
            return this;
        }

        public static byte[] ResizeImageThumbnail(int MaxSize, byte[] originalContents)
        {
            var bmp = ArrayToBmp(originalContents);
            bmp = (Bitmap)resizeImage((System.Drawing.Image)bmp, new Size(MaxSize, MaxSize));
            originalContents = BmpToArray(bmp);
            bmp.Dispose();
            return originalContents;
        }

        /// <summary>
        /// resizes the image to a new size based on a percentage
        /// </summary>
        /// <param name="newSize">the percentage to resize</param>
        /// <returns>the new image</returns>
        /// <remarks>
        /// This may not be exactly to the percent as we must maintain aspect ratios.
        /// </remarks>
        public Image ResizeImage(decimal Percent)
        {
            int sizeX = (int)Math.Ceiling((decimal)this.bitMap.Width * (decimal)((decimal)Percent / 100));
            int sizeY = (int)Math.Ceiling((decimal)this.bitMap.Height * (decimal)((decimal)Percent / 100));
            var newSize = new Size(sizeX, sizeY);
            this.bitMap = (Bitmap)resizeImage((System.Drawing.Image)this.bitMap, newSize);
            var nfile = BlogService.UploadFile(BmpToArray(this.bitMap), this.Name, this.ParentDirectory, true);
            this.FileContents = nfile.FileContents;
            nfile.Dispose();
            return this;
        }

        /// <summary>
        /// accepts change requests for an image for rotate & flip
        /// </summary>
        /// <param name="modifications">a list of changes, will be executed in the order recieved</param>
        /// <returns>the original image</returns>
        public Image ModifyImage(params ModificationType[] modifications)
        {
            foreach (var change in modifications)
            {
                switch (change)
                {
                    case ModificationType.RotateLeft:
                        this.bitMap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                    case ModificationType.RotateRight:
                        this.bitMap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case ModificationType.FlipHorizontal:
                        this.bitMap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        break;
                    case ModificationType.FlipVertical:
                        this.bitMap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        break;
                }
            }
            var nfile = BlogService.UploadFile(BmpToArray(this.bitMap), this.Name, this.ParentDirectory, true);
            this.FileContents = nfile.FileContents;
            nfile.Dispose();
            return this;
        }

        /// <summary>
        /// Crops an image by giving the crop start x & y co-ordinates and then the height & width to crop to. Making a perfect rectangle of the crop area.
        /// </summary>
        /// <param name="x">the x co-ordinate</param>
        /// <param name="y">the y co-ordinate</param>
        /// <param name="width">the width to crop</param>
        /// <param name="height">the height to crop</param>
        /// <returns>the new object, on exception the image will not be modified.</returns>
        public Image CropImage(int x, int y, int width, int height)
        {
            try
            {
                Rectangle cropArea = new Rectangle(x, y, width, height);
                Bitmap target = new Bitmap(cropArea.Width, cropArea.Height);

                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(this.bitMap, new Rectangle(0, 0, width, height),
                                     new Rectangle(x, y, target.Width, target.Height),
                                     GraphicsUnit.Pixel);

                }
                //Bitmap bmpCrop = this.bitMap.Clone(cropArea, PixelFormat.DontCare);
                //this.bitMap = bmpCrop;
                //var nfile = BlogService.UploadFile(BmpToArray(this.bitMap), this.Name, this.ParentDirectory, true);
                //this.FileContents = nfile.FileContents;
                //nfile.Dispose();
                
                var nfile = BlogService.UploadFile(BmpToArray(target), this.Name, this.ParentDirectory, true);
                this.FileContents = nfile.FileContents;
                this.bitMap.Dispose();
                this.bitMap = null;
                return this;
            }
            catch
            {
                return this;
            }
        }

        /// <summary>
        /// converts a byte array to a bitmap
        /// </summary>
        /// <param name="inArray">the in array</param>
        /// <returns>the new bitmap</returns>
        private static Bitmap ArrayToBmp(byte[] inArray)
        {
            using (MemoryStream ms = new MemoryStream(inArray))
            {
                Bitmap bmp = (Bitmap)System.Drawing.Image.FromStream(ms);
                return bmp;
            }
        }

        /// <summary>
        /// converts a bitmap to an array
        /// </summary>
        /// <param name="bmp">the bitmap to convert</param>
        /// <returns>the byte array object</returns>
        private static byte[] BmpToArray(Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Jpeg);
            byte[] bmpBytes = ms.GetBuffer();
            bmp.Dispose();
            ms.Close();
            return bmpBytes;
        }

        /// <summary>
        /// resizes an image to  specific size
        /// </summary>
        /// <param name="imgToResize">the image to resize</param>
        /// <param name="size">the new size</param>
        /// <returns>new resized image</returns>
        private static System.Drawing.Image resizeImage(System.Drawing.Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (System.Drawing.Image)b;
        }
        #endregion
    }
}

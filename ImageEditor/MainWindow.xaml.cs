using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor
{
    public partial class MainWindow : Window
    {
        private BitmapImage originalImage;
        private Bitmap editedBitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InvertColors_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                Bitmap bitmap = BitmapImageToBitmap(originalImage);
                Bitmap invertedBitmap = InvertColors(bitmap);
                EditedImage.Source = BitmapToImageSource(invertedBitmap);
                editedBitmap = invertedBitmap; // Store the edited bitmap
            }
        }

        private void ShowHistogram_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                Bitmap bitmap = BitmapImageToBitmap(originalImage);
                int[] histogram = CalculateHistogram(bitmap);

                // Open a new window to display the histogram
                // HistogramWindow histogramWindow = new HistogramWindow(histogram);
                // histogramWindow.Show();
            }
        }

        private void Sepia_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                Bitmap bitmap = BitmapImageToBitmap(originalImage);
                Bitmap sepiaBitmap = ApplySepia(bitmap);
                EditedImage.Source = BitmapToImageSource(sepiaBitmap);
                editedBitmap = sepiaBitmap; // Store the edited bitmap
            }
        }

        private Bitmap InvertColors(Bitmap original)
        {
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(originalColor.A, 255 - originalColor.R, 255 - originalColor.G, 255 - originalColor.B);
                    original.SetPixel(x, y, invertedColor);
                }
            }
            return original;
        }

        private int[] CalculateHistogram(Bitmap bitmap)
        {
            int[] histogram = new int[256];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    int gray = (color.R + color.G + color.B) / 3;
                    histogram[gray]++;
                }
            }
            return histogram;
        }

        private Bitmap ApplySepia(Bitmap original)
        {
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color originalColor = original.GetPixel(x, y);
                    int tr = (int)(0.393 * originalColor.R + 0.769 * originalColor.G + 0.189 * originalColor.B);
                    int tg = (int)(0.349 * originalColor.R + 0.686 * originalColor.G + 0.168 * originalColor.B);
                    int tb = (int)(0.272 * originalColor.R + 0.534 * originalColor.G + 0.131 * originalColor.B);

                    int r = Math.Min(255, tr);
                    int g = Math.Min(255, tg);
                    int b = Math.Min(255, tb);

                    original.SetPixel(x, y, Color.FromArgb(originalColor.A, r, g, b));
                }
            }
            return original;
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                originalImage = new BitmapImage(new Uri(openFileDialog.FileName));
                OriginalImage.Source = originalImage;
                EditedImage.Source = originalImage; // Initially show the same image
            }
        }

        private void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                Bitmap bitmap = BitmapImageToBitmap(originalImage);
                Bitmap grayBitmap = MakeGrayscale(bitmap);
                EditedImage.Source = BitmapToImageSource(grayBitmap);
            }
        }

        private void Original_Click(object sender, RoutedEventArgs e)
        {
            if (originalImage != null)
            {
                EditedImage.Source = originalImage;
            }
        }

        private Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        private Bitmap MakeGrayscale(Bitmap original)
        {
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    System.Drawing.Color originalColor = original.GetPixel(x, y);
                    int grayScale = (int)((originalColor.R * 0.3) + (originalColor.G * 0.59) + (originalColor.B * 0.11));
                    System.Drawing.Color grayColor = System.Drawing.Color.FromArgb(originalColor.A, grayScale, grayScale, grayScale);
                    original.SetPixel(x, y, grayColor);
                }
            }
            return original;
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            if (editedBitmap != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*"
                };
                if (saveFileDialog.ShowDialog() == true)
                {
                    editedBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Image saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No edited image to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}

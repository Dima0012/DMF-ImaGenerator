namespace Trace.Cameras;


/// <summary>
/// Trace an image by shooting light Rays through each of its pixels
/// </summary>
public class ImageTracer 
{
    public HdrImage Image { get; set; }
    public ICamera Camera { get; set; }

    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Camera = camera;
    }

    /// <summary>
    /// Shoot one Ray through a pixel of the Image. 
    /// </summary>
    public Ray fire_ray(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        //There is an error in this formula, professor said to implement it as it is
        float u = (col + uPixel) / (Image.Width - 1);
        float v = (row + vPixel) / (Image.Height - 1);
        return Camera.fire_ray(u, v);
    }

    /// <summary>
    /// Shoots several Rays crossing each of the pixel in the Image.  
    /// </summary>
    public void fire_all_rays(Func<Ray, Color> renderer)
    {
        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                var ray = fire_ray(col, row);
                var color = renderer(ray);
                Image.set_pixel(col, row, color);
            }
        }
    }
}
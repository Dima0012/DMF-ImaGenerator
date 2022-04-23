using Trace.Geometry;

namespace Trace.Cameras;

public interface IImageTracer
{
    public void fire_all_rays();
}

public class ImageTracer : IImageTracer
{
    public HdrImage Image { get; set; }
    public ICamera Camera { get; set; }

    public ImageTracer(HdrImage image, ICamera camera)
    {
        Image = image;
        Camera = camera;
    }

    public Ray fire_ray(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        //There is an error in this formula, professor said to implement it as it is
        float u = (col + uPixel) / (Image.Width - 1);
        float v = (row + vPixel) / (Image.Height - 1);
        return Camera.fire_ray(u, v);
    }

    public void fire_all_rays()
    {
        for (int row = 0; row < Image.Height; row++)
        {
            for (int col = 0; col < Image.Width; col++)
            {
                var ray = fire_ray(col, row);
                var color = new Color(1.0f, 2.0f, 3.0f);
                Image.set_pixel(col, row, color);
            }
        }
    }
}
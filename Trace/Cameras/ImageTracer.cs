namespace Trace.Cameras;

/// <summary>
/// Trace an image by shooting light Rays through each of its pixels
/// </summary>
public class ImageTracer
{
    public HdrImage Image { get; set; }
    public ICamera Camera { get; set; }
    public Pcg Pcg { get; set; }
    public int SamplesPerSide { get; set; }

    public ImageTracer(HdrImage image, ICamera camera, int samplesPerSide)
    {
        Image = image;
        Camera = camera;
        SamplesPerSide = samplesPerSide;
        Pcg = new Pcg();
    }

    public ImageTracer(HdrImage image, ICamera iCamera, int samplesPerSide, Pcg pcg)
    {
        Image = image;
        Camera = iCamera;
        SamplesPerSide = samplesPerSide;
        Pcg = pcg;
    }

    /// <summary>
    /// Shoot one Ray through a pixel of the Image. 
    /// </summary>
    public Ray fire_ray(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
    {
        var u = (col + uPixel) / Image.Width;
        var v = 1.0f - (row + vPixel) / Image.Height;
        return Camera.fire_ray(u, v);
    }

    /// <summary>
    /// Shoots several Rays crossing each of the pixel in the Image.  
    /// </summary>
    public void fire_all_rays(Renderer renderer)
    {
        for (var row = 0; row < Image.Height; row++)
        {
            for (var col = 0; col < Image.Width; col++)
            {
                var cumColor = new Color(0, 0, 0);

                if (SamplesPerSide > 0)
                {
                    // Run stratified sampling for anti-aliasing
                    for (var interPixelRow = 0; interPixelRow < SamplesPerSide; interPixelRow++)
                    {
                        for (var interPixelCol = 0; interPixelCol < SamplesPerSide; interPixelCol++)
                        {
                            var uPixel = (interPixelCol + Pcg.random_float()) / SamplesPerSide;
                            var vPixel = (interPixelRow + Pcg.random_float()) / SamplesPerSide;
                            var ray = fire_ray(col, row, uPixel, vPixel);
                            cumColor += renderer.Render(ray);
                        }
                    }

                    Image.set_pixel(col, row, cumColor * (1.0f / (SamplesPerSide * SamplesPerSide)));
                }
                else
                {
                    var ray = fire_ray(col, row);
                    var color = renderer.Render(ray);
                    Image.set_pixel(col, row, color);
                }
            }
        }
    }
}
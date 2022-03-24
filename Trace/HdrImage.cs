using System.Text;

namespace Trace;

/// <summary>
/// This class represents a HDR image as a matrix of Color elements, each one representing a pixel.
/// </summary>
public class HdrImage
{
    public int Height { get; set; }
    public int Width { get; set; }

    public Color[] Pixels;

    public HdrImage(int width, int height)
    {
        Height = height;
        Width = width;
        Pixels = new Color[Height * Width];

        for (int i = 0; i < Height * Width; i++)
        {
            Pixels[i] = new Color();
        }
    }

    public bool valid_coordinates(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public int pixel_offset(int x, int y)
    {
        return y * Width + x;
    }
    
    public Color get_pixel(int x, int y)
    {
        System.Diagnostics.Trace.Assert(valid_coordinates(x, y));
        return Pixels[pixel_offset(x, y)];
    }

    public void set_pixel(int x, int y, Color color)
    {
        System.Diagnostics.Trace.Assert(valid_coordinates(x, y));
        Pixels[pixel_offset(x, y)] = color;
    }

    /// <summary>
    /// Write a float number in a stream as a sequence of bytes.
    /// </summary>
    /// <param name="outputStream"> The output Stream to write the number onto.</param>
    /// <param name="value"> The number to be written.</param>
    /// <param name="endianness"> The endianness for writing the bytes; -1.0 for little endian, +1.0 for big endian</param>
    public static void write_float(Stream outputStream, float value, double endianness)
    {
        var seq = BitConverter.GetBytes(value);
        if ((endianness < 0 && !BitConverter.IsLittleEndian) || (endianness > 0 && BitConverter.IsLittleEndian))
        {
            Array.Reverse(seq);
        }
        
        outputStream.Write(seq, 0, seq.Length);
    }

    /// <summary>
    /// Save an HDR image as a .pfm file.
    /// </summary>
    /// <param name="outputStream"> The output Stream where to save the file to.</param>
    /// <param name="endianness"> The endianness for writing the bytes; -1.0 for little endian, +1.0 for big endian. </param>
    public void write_pfm(Stream outputStream, double endianness)
    {
        string endianness_str;
        if (endianness < 0)
        {
            endianness_str = "-1.0";
        }
        else
        {
            endianness_str = "1.0";
        }
        // Define header
        var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{endianness_str}\n");
        outputStream.Write(header);

        //Write image (from bottom left corner)
        for (int y = Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < Width; x++)
            {
                var color = get_pixel(x, y);
                write_float(outputStream, color.R, endianness);
                write_float(outputStream, color.G, endianness);
                write_float(outputStream, color.B, endianness);
            }
        }   
    }

    public string read_line(Stream inputStream)
    {
        string result = ""; 

        BinaryReader binReader = new BinaryReader(inputStream);

        while (true)
        {
            byte[] cur_byte = binReader.ReadBytes(1);
            string cur_string = Encoding.ASCII.GetString(cur_byte);

            if (cur_string == "\n" || cur_string=="")
            {
                return result;
            }
            result += cur_string;
        }
    }

    // public float read_float(Stream inputStream, bool endiannessBool)
    // {
    //     BinaryReader binReader = new BinaryReader(inputStream);
    //     byte[] cur_bytes = binReader.ReadBytes(4);
    //     float value = BitConverter.ToSingle( cur_bytes, 0 );
    // }

    /// <summary>
    /// Reads endianness from a PFM file and returns true if endianness matches computer endianness,
    ///  otherwise returns false. 
    /// </summary>
    public bool _parse_endianness(string line)
    {
        double endianness;
        try
        {
            endianness = Convert.ToDouble(line);
        }
        catch ( FormatException  err)
        {
            throw new InvalidPfmFileFormat("Missing endianness specification", err);
        }

        if ((endianness < 0 && !BitConverter.IsLittleEndian) || (endianness > 0 && BitConverter.IsLittleEndian) )
        {
            return false;
        }

        if (endianness == 0)
        {
            throw new InvalidPfmFileFormat("Invalid endianness specification, it cannot be zero");
        }
        
        return true;
        
    }
    
    public double average_luminosity(double delta = 1e-10)
    {
        double cumsum = 0;
        for (int i = 0; i < Height * Width; i++)
        {
            cumsum += Math.Log10(Pixels[i].luminosity() + delta);
        }
        
        return Math.Pow(10, cumsum / Pixels.Length);
    }
    
    public bool double_is_close(double a, double b, double epsilon = 1e-5)
    {
        return Math.Abs(a - b) < epsilon;
    }

    public void normalize_image(float factor, float? luminosity = null)
    {
        var lum = luminosity ?? average_luminosity();
        for (int i = 0; i < Pixels.Length; ++i)
        {
            float alpha = (float) (factor / lum);
            Pixels[i] *= alpha;
        }
    }

    private float clamp(float x)
    {
        return x / (1 + x);
    }

    public void clamp_image()
    {
        for (int i = 0; i < Pixels.Length; ++i)
        {
            Pixels[i].R = clamp(Pixels[i].R);
            Pixels[i].G = clamp(Pixels[i].G);
            Pixels[i].B = clamp(Pixels[i].B);
        }
    }
}

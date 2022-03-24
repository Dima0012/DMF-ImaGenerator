﻿using System.Text;

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
    
    /// <summary>
    /// Set the Color in Pixel with argument color, at coordinates (x,y). 
    /// </summary>
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
        string endiannessStr;
        if (endianness < 0)
        {
            endiannessStr = "-1.0";
        }
        else
        {
            endiannessStr = "1.0";
        }

        // Define header
        var header = Encoding.ASCII.GetBytes($"PF\n{Width} {Height}\n{endiannessStr}\n");
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
            byte[] curByte = binReader.ReadBytes(1);
            string curString = Encoding.ASCII.GetString(curByte);

            if (curString == "\n" || curString == "")
            {
                return result;
            }

            result += curString;
        }
    }

    /// <summary>
    /// Reads a float from an input stream.
    /// The parameter endiannessBool checks if reversing the bytes is needed.
    /// </summary>
    public float read_float(Stream inputStream, bool endiannessBool)
    {
        var binReader = new BinaryReader(inputStream);

        var curBytes = binReader.ReadBytes(4);
        if (!endiannessBool)
        {
            Array.Reverse(curBytes);
        }

        var value = BitConverter.ToSingle(curBytes, 0);

        return value;
    }

    /// <summary>
    /// Reads endianness from a PFM file and returns true if endianness matches computer endianness,
    ///  otherwise returns false. 
    /// </summary>
    public bool parse_endianness(string line)
    {
        double endianness;
        try
        {
            endianness = Convert.ToDouble(line);
        }
        catch (FormatException err)
        {
            throw new InvalidPfmFileFormat("Missing endianness specification.", err);
        }

        if ((endianness < 0 && !BitConverter.IsLittleEndian) || (endianness > 0 && BitConverter.IsLittleEndian))
        {
            return false;
        }

        if (endianness == 0)
        {
            throw new InvalidPfmFileFormat("Invalid endianness specification, it cannot be zero.");
        }

        return true;
        
}
    /// <summary>
    /// Reads a string and parses from it the width and height of the image,
    /// returning it as a tuple of int.
    /// </summary>
    public (int, int) parse_img_size(string line)
    {
        var elements = line.Split(" ");

        if (elements.Length != 2)
        {
            throw new InvalidPfmFileFormat("Invalid image size specification.");
        }

        int width, height;

        try
        {
            width = int.Parse(elements[0]);
            height = int.Parse(elements[1]);

            if (width <= 0 || height <= 0)
            {
                throw new InvalidPfmFileFormat("width and height must be positive.");
            }
        }
        catch (FormatException err)
        {
            throw new InvalidPfmFileFormat("Invalid width/height.", err);
        }

        return (width, height);
    }

    /// <summary>
    /// Read a PFM file from an input stream, and load image on the HdrImage.
    /// </summary>
    public void read_pfm_image(Stream inputStream)
    {
        // Read magic
        var magic = read_line(inputStream);
        if (magic != "PF")
        {
            throw new InvalidPfmFileFormat("Invalid magic in PFM file.");
        }

        // Read img size
        var imgSize = read_line(inputStream);
        var (width, height) = parse_img_size(imgSize);

        Width = width;
        Height = height;

        // Check endianness
        var endiannessValue = read_line(inputStream);
        var endiannessConsistency = parse_endianness(endiannessValue);

        // Reads colors and set pixels
        for (int y = Height - 1; y >= 0; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var r = read_float(inputStream, endiannessConsistency);
                var g = read_float(inputStream, endiannessConsistency);
                var b = read_float(inputStream, endiannessConsistency);

                var col = new Color(r, g, b);
                
                set_pixel(x,y, col);
            }
        }
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
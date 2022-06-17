# DMF-ImaGenerator
Project for RayTracing Course @ Physics UniMi AY 21/22

This application renders simple photo-realistic images, by reading a scene from a text file, and it saves them in HDR format (PFM) and LDR format (PNG). It has now reached the (course) final version 1.0.0. It is available for MacOS and Linux.

To install the application, simply download the .zip file ``DMF-ImaGenerator`` from the latest release. You then need to add executable rights with the Unix command: ``chmod +x DMF-ImaGenerator`` via Terminal. Then the application is ready to run via command line as ``./DMF-ImaGenerator`` . You can also run the program. via ``dotnet run``if you have [.NET6](https://dotnet.microsoft.com/en-us/) and the required libraries installed.

We noted that there is an erroneous parsing of numbers if you use a Terminal without different languages setting than English; in fact, it may interpret the wrong numbers and not generate the correct image. Consider switching to English as the Terminal usage language, or similar solutions.


# Features available

Command ``pfm2png`` : convert a PFM image into a PNG file.

Options are: 
1. ``-g , --gamma`` The gamma correction for the image.
2. ``-f , --factor`` The factor for luminosity normalization.


Command ``demo`` : generates a simple demo image. 

Options are:
1. ``-w, --width`` Width in pixels of the demo image.
2. ``-h , --height`` Height in pixels of the demo image.
3. ``-a , --angle-deg``Angle from which to observe the image.
4. ``-o`` Enables the Ortoghonal Camera instead of the Perspective one.



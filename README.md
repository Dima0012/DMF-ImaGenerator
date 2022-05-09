# DMF-ImaGenerator
Project for RayTracing Course @ Physics UniMi AY 21/22

This application renders photo-realistic images, saving them in HDR format (PFM) and LDR format (PNG). It is currently in development.

To install the application, simply download the .exe file ``DMF-ImaGenerator`` from the latest release. You then need to add executable rights with the Unix command: ``chmod +x DMF-ImaGenerator`` . Then the application is ready to run via command line as ``./DMF-ImaGenerator`` . 

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



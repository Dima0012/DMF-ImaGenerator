# HEAD 
- New version released on 06/17/22
- Implemented a lexer and a parser to read scene files written using a simple textual syntax.
- New command ``renderer`` which reads a scene from a file and generates an image.
- Merged the two demo commands into one single ``demo`` command. Now it generates different images based on the chosen renderer.
- New and improved error displaying for erroneus scenes' decleration and bad application's usage. 
- New option ``--declare-float`` to declare a floating point variable from the command line.


# Version 0.3.0

- New version released on 05/25/22
- Improved ``demo``, now can render the image with a ``flat``renderer; it generates colored and checkered spheres.
- More parameters to personalize the image generated: luminosity, correction for PNG saving.
- Implemented ``antialisin.``.
- A new parser option ``samples-per-pixel``can be choose to reduce the granularity of the image generated with any method.
- Added two new renderning methods: ``path-tracing``and ``point-light``.
- New ``demo-full``command. It can be run with the above new rendering methods. It display a much more detailed image.
- Added parameters to personalize the rendering of the image when using ``path-tracing``.
- Now the program displays the progress time of the rendering and the total time elapsed.
- New BRDF added, Specular and Diffusive.
- Pigments added.
- New materials added.

# Version 0.2.0

- New version released on 05/9/22
- Now the application is more user friendly, using CommandLineParser to parse argument and commands. See README.
- New feature ``demo`` now generates a simple demo image, with the possibility to choose the size of the image and the angle for the observer.
- The old feature is now named ``pfm2png``.
- Internally added new geometry class, with shapes Sphere and Plane.
- Fixed bug in issue https://github.com/Dima0012/DMF-ImaGenerator/issues/3


# Version 0.1.0

- First release of the code. 

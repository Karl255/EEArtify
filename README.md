# EEArtify
A tool that makes images drawn with EEditor look better.

Download it [here](https://github.com/Karl255/EEArtify/releases).

# Tutorial
The AllBlocks image is the minimap screenshot that contains all your blocks or only the blocks you want to use for the image. It is important that you take this screenshot with the ingame key combination (shift + V).  
The input image is the image you want to convert and the output image is where the converted image should be saved.  
The algorithm is the method used to compare colors and find the best fitting one from the AllBlocks filter image. They're sorted by the complexity, accuracy and speed (deltaE76 being simple, inaccurate and fast and deltaE2000 being complex, accurate and slow). DeltaE 76 is often good enough, but for images with a lot of dark detail, you should use somethig else.  
To start the process, just click on the Start button. A message box will pop up and play a sound when the conversion is finished.

# Built With
- [ColorMine](http://colormine.org/) - A library used to compare the appearance of colors

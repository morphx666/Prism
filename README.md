# Prism
A brave attempt to create a full featured subtitler for live programming

Implemented so far:
- Multiple tracks
- Multiple layers
- Layers support elements such as audio, clock, image, image sequence, marquee, three types of shapes, text, timer and video
- All objects' properties are keyable, and support linear transitions
- Live Mode. In this mode, changes can be made without affceting the main project until changes are applied

The project includes a custom version of [AForge's Video.DirectShow library](http://aforgenet.com/) which allows us to consume the audio information from any video or audio file so that it can be plotted in the program's timeline.

![Prism](https://xfx.net/stackoverflow/Prism/prism01.png)

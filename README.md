## PixelIt.web

This is the web instance for the [PixelIt](https://github.com/o0shojo0o/PixelIt) project.  
PixelIt.web provides the API, Pixel Gallery and Pixel Creator.  
  
[Docu](https://docs.bastelbunker.de/pixelit/) |
[Forum](https://github.com/o0shojo0o/PixelIt/discussions) |
[Blog](https://www.bastelbunker.de/pixel-it/) |
[PixelIt Web](https://pixelit.bastelbunker.de/PixelGallery) |
[![](https://img.shields.io/endpoint?style=flat-square&url=https%3A%2F%2Frunkit.io%2Fdamiankrawczyk%2Ftelegram-badge%2Fbranches%2Fmaster%3Furl%3Dhttps%3A%2F%2Ft.me%2Fpixelitdisplay)](https://t.me/pixelitdisplay) |
[![](https://img.shields.io/discord/558849582377861122?logo=discord)](https://discord.gg/JHE9P9zczW)

## Installation

Use docker-compose

```yml
pixelitweb:
    restart: unless-stopped
    container_name: pixelitweb
    image: ghcr.io/o0shojo0o/pixelit.web:latest
    volumes:
        - /etc/localtime:/etc/localtime:ro
        - ./pixelitweb/images:/app/wwwroot/images
        - ./pixelitweb/temp:/app/wwwroot/temp
        - ./pixelitweb/logs:/app/logs
    environment:
        MYSQL_HOST: host
        MYSQL_DATABASE: database
        MYSQL_USER: user
        MYSQL_PASSWORD: password
```

## License

MIT License

Copyright (c) 2021 Dennis Rathjen <info@bastelbunker.de>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

function getImageWidth() {
    var image = document.getElementById('carrierImage');
    return image.width;
}

function getImageHeight() {
    var image = document.getElementById('carrierImage');
    return image.height;
}

async function getImageData() {
    var img = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    canvas.width = img.width;
    canvas.height = img.height;
    var ctx = canvas.getContext('2d');
    ctx.drawImage(img, 0, 0);
    const data = await ctx.getImageData(0, 0, img.width, img.height).data;
    return data;
}

function getImageURL(data, type) {
    var img = document.getElementById('carrierImage');
    var canvas = document.getElementById('canvas');
    var ctx = canvas.getContext('2d');

    let imageData = new ImageData(Uint8ClampedArray.from(data), img.width, img.height);

    for (let i = 0; i < imageData.data.length; i += 4) {
        imageData.data[i + 3] = 255;
    }

    ctx.putImageData(imageData, 0, 0);

    var dataUrl = canvas.toDataURL(type, 1.0);
    return dataUrl;
}

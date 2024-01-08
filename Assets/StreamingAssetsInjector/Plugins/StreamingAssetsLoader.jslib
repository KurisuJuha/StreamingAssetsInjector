mergeInto(LibraryManager.library, {
    LoadStreamingAssetsData: function() {
        var returnStr = GetStreamingAssetsData();
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
})
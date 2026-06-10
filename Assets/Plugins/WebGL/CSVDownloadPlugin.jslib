mergeInto(LibraryManager.library, {
  WebGLDownloadTextFile: function (fileNamePtr, textPtr) {
    var fileName = UTF8ToString(fileNamePtr);
    var text = UTF8ToString(textPtr);

    var blob = new Blob([text], { type: "text/csv;charset=utf-8;" });
    var url = URL.createObjectURL(blob);

    var link = document.createElement("a");
    link.href = url;
    link.download = fileName;

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url);
  }
});
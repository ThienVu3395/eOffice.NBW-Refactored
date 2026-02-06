angular
    .module('aims.shared.utils')
    .factory('FileUtil', FileUtil);

function FileUtil() {
    return {
        downloadBase64File: function (
            base64,
            fileName,
            mimeType = 'application/pdf')
        {
            if (!base64) return;

            const linkSource = `data:${mimeType};base64,${base64}`;
            const downloadLink = document.createElement('a');

            downloadLink.href = linkSource;
            downloadLink.download = fileName || 'download';

            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);
        }
    };
}

angular
    .module('aims.shared.utils')
    .factory('StringUtil', StringUtil);

function StringUtil() {
    // ====== PRIVATE ======
    function decodeHTMLEntities(input) {
        if (!input) return '';
        const doc = new DOMParser().parseFromString(input, 'text/html');
        return doc.documentElement.textContent || '';
    }

    // VD: "P. CNTT" -> "P.CNTT"
    function formatDonViName(name) {
        if (!name) return '';
        return name.replace(/\b([A-ZĐ])\.\s+/g, '$1.');
    }

    /**
     * Loại bỏ HTML + giữ xuống dòng theo <p>
     * Dùng cho: nội dung bút phê, mô tả rich text
     */
    function removeHTMLTagsAndNewlines(input) {
        if (!input) return '';

        const decodedText = decodeHTMLEntities(input);

        // <p>abc</p><p>xyz</p> -> abc\nxyz
        return decodedText
            .replace(/<p>(.*?)<\/p>/gi, '$1\n')
            .replace(/<\/?[^>]+>/g, '')   // fallback remove tag
            .trim();
    }

    return {
        formatDonViName,
        decodeHTMLEntities,
        removeHTMLTagsAndNewlines
    };
}

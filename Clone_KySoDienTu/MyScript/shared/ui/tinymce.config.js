angular
    .module('aims.shared.ui')
    .constant('TINYMCE_CONFIG', {
        onChange: function () { },
        menubar: false,
        resize: false,
        toolbar: 'undo redo | bold italic underline | forecolor bullist',
        plugins: 'textcolor advlist',
        skin: 'lightgray',
        theme: 'modern',
        content_css: '/Scripts/tinymce/tinyMod.css'
    });
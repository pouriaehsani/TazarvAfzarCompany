document.addEventListener("DOMContentLoaded", function () {

    tinymce.init({
        selector: "#articleContent",

        height: 600,

        menubar: true,

        directionality: "rtl",

        plugins: [
            "advlist",
            "autolink",
            "lists",
            "link",
            "image",
            "charmap",
            "preview",
            "anchor",
            "searchreplace",
            "visualblocks",
            "code",
            "fullscreen",
            "media",
            "table",
            "help",
            "wordcount"
        ],

        toolbar:
            "undo redo | " +
            "blocks | " +
            "bold italic underline strikethrough | " +
            "alignleft aligncenter alignright alignjustify | " +
            "bullist numlist | " +
            "link image media table | " +
            "forecolor backcolor | " +
            "removeformat | " +
            "code fullscreen preview",

        images_upload_url:
            window.articleContentImageUploadUrl,

        automatic_uploads: true,

        content_style:
            "body {" +
            "font-family: Tahoma, Arial, sans-serif;" +
            "font-size: 16px;" +
            "direction: rtl;" +
            "text-align: right;" +
            "}"
    });

});
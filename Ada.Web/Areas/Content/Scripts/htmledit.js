function initSummernote() {
    $.ajax({
        url: 'https://api.github.com/emojis',
        async: false
    }).then(function (data) {
        window.emojis = Object.keys(data);
        window.emojiUrls = data;
    });
    $('.summernote').summernote({
        lang: 'zh-CN',
        height: 500,
        hint: {
            match: /:([\-+\w]+)$/,
            search: function (keyword, callback) {
                callback($.grep(emojis, function (item) {
                    return item.indexOf(keyword) === 0;
                }));
            },
            template: function (item) {
                var content = emojiUrls[item];
                return '<img src="' + content + '" width="20" /> :' + item + ':';
            },
            content: function (item) {
                var url = emojiUrls[item];
                if (url) {
                    return $('<img />').attr('src', url).css('width', 20)[0];
                }
                return '';
            }
        },
        callbacks: {
            onImageUpload: function (files, editor, $editable) {
                var data = new FormData(), filename = 'summernoteFile';
                data.append(filename, files[0]);
                $.ajax({
                    data: data,
                    type: "POST",
                    url: "/admin/common/uploadimage?filename=" + filename, //图片上传出来的url，返回的是图片上传后的路径，http格式
                    cache: false,
                    contentType: false,
                    processData: false,
                    dataType: "json",
                    success: function (data) {//data是返回的hash,key之类的值，key是定义的文件名
                        if (data.State == 1) {
                            $('.summernote').summernote('insertImage', data.Msg);
                        } else {
                            swal("操作提醒", data.Msg, "error");
                        }
                    },
                    error: function () {
                        swal("操作提醒", "图片上传失败", "error");
                    }
                });
            }
        }
    });
}
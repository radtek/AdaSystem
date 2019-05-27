$(function () {
    //初始化绑定默认的属性
    $.upLoadDefaults = $.upLoadDefaults || {};
    $.upLoadDefaults.property = {
        multiple: false, //是否多文件
        sendurl: null, //发送地址
        filetypes: "jpg,jpge,png,gif", //文件类型
        filesize: "2048", //文件大小
        btntext: "浏览...", //上传按钮的文字
        swf: null //SWF上传控件相对地址
    };
    //初始化上传控件
    $.fn.InitUploader = function(b) {
        var fun = function(parentObj) {
            var p = $.extend({}, $.upLoadDefaults.property, b || {});
            var btnObj = $('<div class="upload-btn">' + p.btntext + '</div>').appendTo(parentObj);
            //初始化WebUploader
            var uploader = WebUploader.create({
                compress: false, //不压缩
                auto: true, //自动上传
                swf: p.swf, //SWF路径
                server: p.sendurl, //上传地址
                pick: {
                    id: btnObj,
                    multiple: p.multiple
                },
                accept: {
                    /*title: 'Images',*/
                    extensions: p.filetypes
                    /*mimeTypes: 'image/*'*/
                },
                formData: p.postdata || {},
                fileVal: p.postdata.input || 'upfile', //上传域的名称
                //sendAsBinary: true, //二进制流上传
                fileSingleSizeLimit: p.filesize * 1024 //文件大小
            });

            //当validate不通过时，会以派送错误事件的形式通知
            uploader.on('error',
                function(type) {
                    switch (type) {
                    case 'Q_EXCEED_NUM_LIMIT':
                        alert("错误：上传文件数量过多！");
                        break;
                    case 'Q_EXCEED_SIZE_LIMIT':
                        alert("错误：文件总大小超出限制！");
                        break;
                    case 'F_EXCEED_SIZE':
                        alert("错误：文件大小超出限制！");
                        break;
                    case 'Q_TYPE_DENIED':
                        alert("错误：禁止上传该类型文件！");
                        break;
                    case 'F_DUPLICATE':
                        alert("错误：请勿重复上传该文件！");
                        break;
                    default:
                        alert('错误代码：' + type);
                        break;
                    }
                });

            //当有文件添加进来的时候
            uploader.on('fileQueued',
                function(file) {
                    //如果是单文件上传，把旧的文件地址传过去
                    if (!p.multiple) {
                        uploader.options.formData.DelFilePath = parentObj.siblings(".upload-path").val();
                    }
                    //防止重复创建
                    if (parentObj.children(".upload-progress").length == 0) {
                        //创建进度条
                        var fileProgressObj = $('<div class="upload-progress"></div>').appendTo(parentObj);
                        var progressText = $('<span class="txt">正在上传，请稍候...</span>').appendTo(fileProgressObj);
                        var progressBar = $('<span class="bar"><b></b></span>').appendTo(fileProgressObj);
                        var progressCancel = $('<a class="close" title="取消上传"><i class="iconfont icon-remove"></a>')
                            .appendTo(fileProgressObj);
                        //绑定点击事件
                        progressCancel.click(function() {
                            uploader.cancelFile(file);
                            fileProgressObj.remove();
                        });
                    }
                });

            //文件上传过程中创建进度条实时显示
            uploader.on('uploadProgress',
                function(file, percentage) {
                    var progressObj = parentObj.children(".upload-progress");
                    progressObj.children(".txt").html(file.name);
                    progressObj.find(".bar b").width(percentage * 100 + "%");
                });

            //当文件上传出错时触发
            uploader.on('uploadError',
                function(file, reason) {
                    uploader.removeFile(file); //从队列中移除
                    alert(file.name + "上传失败，错误代码：" + reason);
                });

            //当文件上传成功时触发
            uploader.on('uploadSuccess',
                function(file, data) {
                    if (data.State == 0) {
                        var progressObj = parentObj.children(".upload-progress");
                        progressObj.children(".txt").html(data.Msg);
                    } else {
                        //如果是单文件上传，则赋值相应的表单
                        if (!p.multiple) {
                            parentObj.siblings(".upload-name").val(data.Data.FileName);
                            parentObj.siblings(".upload-path").val(data.Data.FilePath);
                            parentObj.siblings(".upload-size").val(data.Data.FileSize);
                        } else {
                            addImage(parentObj, data.Data);
                        }
                        var progressObj = parentObj.children(".upload-progress");
                        progressObj.children(".txt").html("上传成功：" + file.name);
                    }
                    uploader.removeFile(file); //从队列中移除
                });

            //不管成功或者失败，文件上传完成时触发
            uploader.on('uploadComplete',
                function(file) {
                    var progressObj = parentObj.children(".upload-progress");
                    progressObj.children(".txt").html("上传完成");
                    //如果队列为空，则移除进度条
                    if (uploader.getStats().queueNum == 0) {
                        progressObj.remove();
                    }
                });
        };
        return $(this).each(function() {
            fun($(this));
        });
    };
    //设置图片描述
    $('#imgRemark').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);
        var modal = $(this);
        var hidRemarkObj = button.parent().parent().prevAll("input[name='MaterialRemark']"); //取得隐藏值
        var $remark = modal.find('.modal-body input');
        $remark.val(hidRemarkObj.val());
        modal.find('.modal-footer button[name=batch]').on("click", function () {
            if ($remark.val()) {
                $(".file-list").find("input[name='MaterialRemark']").val($remark.val());
                $(".file-list").find(".file-name").html($remark.val());
            }
            modal.modal('hide');
        });
        modal.find('.modal-footer button[name=single]').on("click", function () {
            if ($remark.val()) {
                hidRemarkObj.val($remark.val());
                button.parent().parent().find(".file-name").html($remark.val());
            }
            modal.modal('hide');
        });
    }).on('hidden.bs.modal', function () {
        $(this).find('.modal-footer button').off("click");
    });
});

/*图片相册处理事件
=====================================================*/
//添加图片相册
function addImage(targetObj, imgInfo) {
    //插入到相册UL里面
    var file = $('<div class="file-box">'
        + '<input type="hidden" name="MaterialImage" value="' + imgInfo.FilePath + '" />'
        + '<input type="hidden" name="MaterialThumbImage" value="' + imgInfo.ThumbnailPath  + '" />'
        + '<input type="hidden" name="MaterialRemark" value="" />'
        + '<input type="hidden" name="MaterialName" value="' + imgInfo.FileName  + '" />'
        + '<input type="hidden" name="MaterialSize" value="' + imgInfo.FileSize + '" />'
        + '<input type="hidden" name="MaterialExt" value="' + imgInfo.FileExt + '" />'
        + '<div class="file">'
        + '<div class="file-image">'
        + '<img class="img-responsive" src="' + imgInfo.ThumbnailPath  + '" />'
        + '</div>'
        + '<div class="file-name text-center">暂无描述</div>'
        + '<div class="file-btn">'
        + '<button type="button" class="btn btn-sm btn-outline btn-primary" data-toggle="modal" data-target="#imgRemark">描述</button>'
        + '<button type="button" onclick="delImg(this);" class="btn btn-sm btn-outline btn-danger pull-right">删除</button>'
        + '</div>'
        + '</div>'
        + '</div>');
    file.appendTo(targetObj.siblings(".file-list"));

    //默认第一个为相册封面
    //var focusPhotoObj = targetObj.siblings(".focus-photo");
    //if (focusPhotoObj.val() == "") {
    //    focusPhotoObj.val(thumbSrc);
    //    newLi.children(".img-box").addClass("selected");
    //}
}
//设置相册封面
function setFocusImg(obj) {
    var focusPhotoObj = $(obj).parents(".photo-list").siblings(".focus-photo");
    focusPhotoObj.val($(obj).children("img").eq(0).attr("src"));
    $(obj).parent().siblings().children(".img-box").removeClass("selected");
    $(obj).addClass("selected");
}


//删除图片LI节点
function delImg(obj) {

    //var parentObj = $(obj).parent().parent();
    //var focusPhotoObj = parentObj.parent().siblings(".focus-photo");
    //var smallImg = $(obj).siblings(".img-box").children("img").attr("src");
    $(obj).parent().parent().parent().remove(); //删除的LI节点
    //检查是否为封面
    //if (focusPhotoObj.val() == smallImg) {
    //    focusPhotoObj.val("");
    //    var firtImgBox = parentObj.find("li .img-box").eq(0); //取第一张做为封面
    //    firtImgBox.addClass("selected");
    //    focusPhotoObj.val(firtImgBox.children("img").attr("src")); //重新给封面的隐藏域赋值
    //}
}
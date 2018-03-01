$(function() {
    $('#uploadModal').on('shown.bs.modal',
        function () {

        }).on('hidden.bs.modal',
        function () {
            $('.fileinput').fileinput("clear");
        });
});
function groupDetail(id) {
    $("#modalView").load("/Resource/MediaGroup/Detail/" + id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                $('[data-toggle="tooltip"]').tooltip();
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });

}
function formatterPrice(value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<span class='label label-info'>" + v.AdPositionName + "：" + (v.PurchasePrice == 0 ? "不接单" : v.PurchasePrice) + "　　有效期至：" + (v.InvalidDate ? moment(v.InvalidDate).format("YYYY-MM-DD") : "") + "</span><br/>";
        });
    return result;
}

function formatterGroup(value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<button class='btn btn-warning btn-xs' onclick=\"groupDetail('" + v.Id + "');\"><i class='fa fa-object-group'></i> " + v.GroupName + "</button> ";
        });
    return result;
}
//撤销
function cancle(id) {
    swal({
        title: "您确定吗?",
        text: "确认要删除此数据吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,

    }, function () {
        $.ajax({
            type: "post",
            headers: {
                '__RequestVerificationToken': $("input[name='__RequestVerificationToken']").val()
            },
            url: "/Resource/Media/Delete",
            data: { "id": id },
            success: function (data) {
                if (data.State == 1) {
                    $("#table").bootstrapTable('refresh');
                    swal({
                        title: "操作成功",
                        text: data.Msg,
                        timer: 2000,
                        type: "success",
                        showConfirmButton: false
                    });

                } else {
                    swal("操作提醒", data.Msg, "warning");
                }
            },
            error: function () {
                swal("操作失败", "系统错误", "error");
            },
            complete: function () {

            }
        });
    });
}

function upLoadFile() {

    var fileObj = document.getElementById("upfile").files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        swal("操作失败", "请选择文件", "warning");
        return;
    }
    if (!checkFileExt(fileObj.name)) {
        swal("操作失败", "只支持xlsx后缀文件", "warning");
        return;
    }
    var formFile = new FormData();
    formFile.append("upfile", fileObj); //加入文件对象
    var data = formFile;
    var subBtn = $('.ladda-button').ladda();
    $.ajax({
        url: "/Resource/Media/Import",
        data: data,
        type: "Post",
        dataType: "json",
        cache: false, //上传文件无需缓存
        processData: false, //用于对data参数进行序列化处理 这里必须false
        contentType: false, //必须
        success: function (result) {
            if (result.State == 1) {
                swal({
                    title: "操作成功",
                    text: data.Msg,
                    timer: 2000,
                    type: "success",
                    showConfirmButton: false
                });
            } else {
                swal("操作失败", result.Msg, "error");
            }
        },
        error: function () {
            swal("操作失败", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}

function checkFileExt(filename) {
    var flag = false; //状态
    var arr = ["xlsx"];
    //取出上传文件的扩展名
    var index = filename.lastIndexOf(".");
    var ext = filename.substr(index + 1);
    //循环比较
    for (var i = 0; i < arr.length; i++) {
        if (ext == arr[i]) {
            flag = true; //一旦找到合适的，立即退出循环
            break;
        }
    }
    return flag;
}


function collectArticle(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length > 1) {
        swal("操作提醒", "只能选择一行数据", "warning");
        return;
    }
    if (arrselections.length <= 0) {
        swal("操作提醒", "请选择有效数据", "warning");
        return;
    }
    $("#modalView").load(url+"/" + arrselections[0].Id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                collection();
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });

}
function details(url) {

    $("#modalView").load(url,
        function () {
            //$('#modalView .modal').on('shown.bs.modal', function () {
            //    collection();
            //}).on('hidden.bs.modal', function () {

            //});
            $('#modalView .modal').modal('show');
        });

}
function collection() {
    $("#modalView form").validate({
        submitHandler: function (form) {
            var subBtn = $('.ladda-button').ladda();
            var $form = $(form),
                data = $form.serialize();
            $.ajax({
                type: "post",
                url: form.action,
                data: data,
                success: function (res) {
                    if (res.State == 1) {
                        $("#table").bootstrapTable('refresh');
                        swal({
                            title: "操作成功",
                            text: res.Msg,

                            type: "success"

                        });

                    } else {
                        swal("操作提醒", res.Msg, "warning");
                    }
                },
                error: function () {
                    swal("操作失败", "系统错误", "error");
                },
                beforeSend: function () {
                    subBtn.ladda('start');
                },
                complete: function () {
                    subBtn.ladda('stop');
                }
            });

        }
    });

}
function collectWeiXinFree(url) {
    $.ajax({
        type: "post",
        url: url,
        success: function (res) {
            if (res.State == 1) {
                $("#table").bootstrapTable('refresh');
            } else {
                swal("操作提醒", res.Msg, "warning");
            }
        },
        error: function () {
            swal("操作失败", "系统错误", "error");
        },
        beforeSend: function () {
        },
        complete: function () {
        }
    });
}
function collectWeiXin(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length <= 0) {
        swal("操作提醒", "请选择需采集的微信号", "warning");
        return;
    }
    var ids = [];
    $.each(arrselections,
        function (k, v) {
            ids.push(v.MediaID);
        });
    swal({
        title: "您确定吗?",
        text: "确认要采集这" + arrselections.length+"个微信号吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true

    }, function () {
        $.ajax({
            type: "post",
            headers: {
                '__RequestVerificationToken': $("input[name='__RequestVerificationToken']").val()
            },
            url: url,
            data: { CallIndex: "weixin", UID: ids.join(',') },
            success: function (data) {
                if (data.State == 1) {
                    $("#table").bootstrapTable('refresh');
                    swal({
                        title: "操作成功",
                        text: data.Msg,
                        timer: 2000,
                        type: "success",
                        showConfirmButton: false
                    });
                } else {
                    swal("操作提醒", data.Msg, "warning");
                }
            },
            error: function () {
                swal("操作失败", "系统错误", "error");
            },
            complete: function () {

            }
        });
    });
}


//扩展表单提交成JSON格式数据
$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || null);
        } else {
            o[this.name] = this.value || null;
        }
    });
    return o;
};

Math.toFixMoney = function (value) {
    var money = value || 0;
    return Math.round(money * 100) / 100;
}


var searchFrm = {};
searchFrm.search = function (form, evt) {
    var query = $(form).serializeObject();
    $("#table").bootstrapTable("refresh", { query: query });
    evt.preventDefault();
}
searchFrm.reset = function (form) {
    form[0].reset();
    $("#table").bootstrapTable("refresh");
}

var formatter = {};
formatter.userStatus= function(value,row,index) {
    if (value==1) {
        return "<span class='label label-primary'>是</span>";
    } else {
        return "<span class='label label-danger'>否</span>";
    }
}
formatter.normalStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>正常</span>";
    } else {
        return "<span class='label label-danger'>关闭</span>";
    }
}
formatter.businessStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>正常</span>";
    } else if (value == -1) {
        return "<span class='label label-danger'>作废</span>";
    }else {
        return "<span class='label label-warning'>待审</span>";
    }
}
function initChangePwd(url,token) {
    $("#changepwd").click(function() {
        $("#modalView").load(url,
            function () {
                $('#modalView .modal').modal('show');
                $("#modalView form").validate({
                    submitHandler: function (form) {
                        var $form = $(form),
                            data = $form.serialize(); //序列化表单数据
                        $.ajax({
                            type: "post",
                            headers: {
                                '__RequestVerificationToken': token
                            },
                            url: form.action,
                            data: data,
                            success: function (res) {
                                if (res.State == 1) {
                                    $('#modalView .modal').modal('hide');
                                    swal({
                                        title: "操作成功",
                                        text: res.Msg,
                                        timer: 2000,
                                        type: "success",
                                        showConfirmButton: false
                                    });

                                } else {
                                    swal("操作提醒", res.Msg, "warning");
                                }
                            },
                            error: function () {
                                swal("操作失败", res.Msg, "error");
                            }
                        });
                    }
                });
            });
    });
    
}
function initCropper(apiurl) {
    $("#changeimage").click(function() {
        $("#modalView").load(apiurl,
            function () {
                var image = document.getElementById('image');
                var cropBoxData;
                var canvasData;
                var cropper;
                var isload = false;
                var options = {
                    autoCropArea: 0.5,
                    preview: ".img-preview",
                    ready: function (e) {
                        //console.log(e.type);
                        // Strict mode: set crop box data first
                        cropper.setCropBoxData(cropBoxData).setCanvasData(canvasData);
                    },
                    cropstart: function (e) {
                        //console.log(e.type, e.detail.action);
                    },
                    cropmove: function (e) {
                        //console.log(e.type, e.detail.action);
                    },
                    cropend: function (e) {
                        //console.log(e.type, e.detail.action);
                    },
                    crop: function (e) {
                        //var data = e.detail;
                        //console.log(e.type);
                    },
                    zoom: function (e) {
                        //console.log(e.type, e.detail.ratio);
                    }
                };
                $('#modalView .modal').on('shown.bs.modal', function () {
                    cropper = new Cropper(image, options);
                    
                }).on('hidden.bs.modal', function () {
                    cropBoxData = cropper.getCropBoxData();
                    canvasData = cropper.getCanvasData();
                    cropper.destroy();
                    isload = false;
                });
                $("#zoomIn").click(function () {
                    if (isload) {
                        cropper.zoom(0.1);
                    }
                });
                $("#zoomOut").click(function () {
                    if (isload) {
                        cropper.zoom(-0.1);
                    }

                });
                $("#rotateLeft").click(function () {
                    if (isload) {
                        cropper.rotate(-45);
                    }
                });
                $("#rotateRight").click(function () {
                    if (isload) {
                        cropper.rotate(45);
                    }
                });
                $("#cropReset").click(function () {
                    if (isload) {
                        cropper.reset();
                    }
                });
                $("#savePic").click(function () {
                    var result = cropper.getCroppedCanvas({ width: 320, height: 180 });
                    if (result) {
                        var href = result.toDataURL();
                        $.ajax({
                            type: "post",
                            url: apiurl,
                            data: { upfile: href },
                            success: function (res) {
                                if (res.State == 1) {
                                    $('#modalView .modal').modal('hide');
                                    image.src = "";
                                    swal({
                                        title: "操作成功",
                                        text: "修改相片成功",
                                        type: "success",
                                        showCancelButton: false,
                                        confirmButtonText: "确定",
                                        closeOnConfirm: false
                                    },
                                        function () {
                                            window.location.reload();
                                        });

                                } else {
                                    swal("操作提醒", res.Msg, "warning");
                                }
                            },
                            error: function () {
                                swal("操作失败", res.Msg, "error");
                            }
                        });
                    }
                });
                var $inputImage = $("#inputImage");
                if (window.FileReader) {
                    $inputImage.change(function () {
                        var fileReader = new FileReader(),
                            files = this.files;
                        if (!files.length) {
                            return;
                        }
                        var file = files[0];
                        if (/^image\/\w+$/.test(file.type)) {
                            fileReader.readAsDataURL(file);
                            fileReader.onload = function () {
                                image.src = this.result;
                                cropper.destroy();
                                cropper = new Cropper(image, options);
                                $inputImage.val("");
                                isload = true;
                            };
                        } else {
                            swal("操作提醒", "请选择图片类型文件", "warning");
                        }
                    });
                } else {
                    $inputImage.addClass("hide");
                }

                $('#modalView .modal').modal('show');
            });
    });
    
}


//初始化ichecks
function initiChecks() {
    $('.i-checks').iCheck({
        checkboxClass: 'icheckbox_square-green',
        radioClass: 'iradio_square-green'
    });
}

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
    //return Math.round(money * 100) / 100;
    return Math.round(money);
}


var searchFrm = {};
searchFrm.search = function (form, evt) {
    $("#table").bootstrapTable("refresh");
    evt.preventDefault();
}
searchFrm.reset = function (form) {
    form[0].reset();
    $("#table").bootstrapTable("refresh");
}
searchFrm.queryParams = function (parameters) {
    var query = $("#searchFrm").serializeObject();
    $.each(query,
        function (k, v) {
            if (isArray(v)) {
                $.each(v,
                    function (a, b) {
                        parameters[k + "[" + a + "]"] = b;
                    });
            } else {
                parameters[k] = v;
            }

        });
    return parameters;
}

var formatter = {};
formatter.userStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>是</span>";
    } else {
        return "<span class='label label-warning'>否</span>";
    }
};
formatter.verification = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-success'>已核销</span>";
    } else {
        return "<span class='label'>未核销</span>";
    }
};
formatter.yesorno = function (value, row, index) {
    if (value == true) {
        return '<a href="javascript:;"><i class="fa fa-check text-navy"></i></a>';
    } else {
        return '<a href="javascript:;"><i class="fa fa-times text-danger"></i></a>';
    }
};
formatter.isOpen = function (value, row, index) {
    if (value == 1) {
        return '<span><i class="fa fa-toggle-on text-navy"></i></span>';
    } else {
        return '<span><i class="fa fa-toggle-off text-warning"></i></span>';
    }
};
formatter.islock = function (value, row, index) {
    if (value == 1) {
        return '<span><i class="fa fa-lock text-navy"></i></span>';
    } else {
        return '<span><i class="fa fa-unlock text-warning"></i></span>';
    }
};
formatter.inout = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>收入</span>";
    } else {
        return "<span class='label label-warning'>支出</span>";
    }
};
formatter.normalStatus = function (value, row, index) {
    if (value == 1 || value == true) {
        return "<span class='label label-primary'>正常</span>";
    } else {
        return "<span class='label'>关闭</span>";
    }
};
formatter.businessStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>已下单</span>";
    } else if (value < 0) {
        return "<span class='label label-warning'>待审批</span>";
    } else if (value == 2) {
        return "<span class='label label-success'>已完成</span>";
    } else if (value == 3) {
        return "<span class='label label-danger'>订单失败</span>";
    } else {
        return "<span class='label'>待转单</span>";
    }
};
formatter.auditStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>同意</span>";
    } else if (value == -1) {
        return "<span class='label label-danger'>拒绝</span>";
    } else {
        return "<span class='label'>待审</span>";
    }
};
formatter.payStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>已付</span>";
    } else {
        return "<span class='label'>待付</span>";
    }
};
formatter.invoiceStatus = function (value, row, index) {
    if (value == 1) {
        return "<span class='label label-primary'>已开</span>";
    } else {
        return "<span class='label'>待开</span>";
    }
};
formatter.purchaseStatus = function (value, row, index) {
    if (value == 2) {
        return "<span class='label label-primary'>已确认</span>";
    } else if (value == 1) {
        return "<span class='label label-warning'>正处理</span>";
    } else if (value == 0) {
        return "<span class='label'>待响应</span>";
    } else if (value == 3) {
        return "<span class='label label-success'>已完成</span>";
    } else if (value == -1) {
        return "<span class='label label-danger'>采购失败</span>";
    }
    return "";
};

formatter.url = function (value, row, index) {
    if (value) {
        return "<a class='label' href='" + value + "' target='_blank'><i class='fa fa-link'></i> 浏览</a>";
    }
};
formatter.tooltip = function (value, row, index) {
    if (value) {
        return '<span class="label label-info" data-toggle="tooltip" data-placement="bottom" title="' +
            value +
            '"><i class="fa fa-info-circle"></i> 查看</span>';
    }
};
formatter.pie = function (value, row, index) {
    return '<span class="pie">' + value + '</span>';
};
formatter.image = function (value, row, index) {
    if (value) {
        return '<div class="lightBoxGallery"><a href="' + value + '" title="预览图片" data-gallery=""><i class="fa fa-picture-o"></i></a></div>';
    }
};
formatter.linkman = function (value, row, index) {
    return "<a class='label label-info' href=\"javascript:linkmanDetail('" + row.LinkManId + "')\"><i class='fa fa-address-book-o'></i> " + value + "</a>";
}
formatter.ip = function (value, row, index) {
    if (isIP(value)) {
        return "<a class='label label-info' href=\"javascript:showIP('" + value + "')\">" + value + "</a>";
    } else {
        return value;
    }
};

function showIP(ip) {
    $('#ipmodal').on('shown.bs.modal', function () {
        $("#ipmodal iframe").attr("src", "http://whois.pconline.com.cn/ip.jsp?ip=" + ip);
    }).on('hidden.bs.modal', function () {

    });
    $('#ipmodal').modal('show');
}
function isArray(o) {
    return Object.prototype.toString.call(o) == '[object Array]';
}
function linkmanDetail(id) {
    $("#modalView").load("/Customer/LinkMan/Detail/" + id,
        function () {
            $('#modalView .modal').modal('show');
        });
}
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}
function isIP(ip) {
    var re = /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/;//正则表达式     
    if (re.test(ip)) {
        if (RegExp.$1 < 256 && RegExp.$2 < 256 && RegExp.$3 < 256 && RegExp.$4 < 256)
            return true;
    }
    return false;
}
function initPie() {
    $("span.pie").peity("pie",
        {
            fill: ['#1ab394', '#d7d7d7', '#ffffff']
        });
}
function initChangePwd(url, token) {
    $("#changepwd").click(function () {
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
                                swal("操作失败", "系统错误", "error");
                            }
                        });
                    }
                });
            });
    });

}
function initCropper(apiurl) {
    $("#changeimage").click(function () {
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
                                swal("操作失败", "系统错误", "error");
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
//初始化select2
function initSelect(id, opt, $obj) {
    var ajaxOption = {
        url: opt.url,
        dataType: "json",
        delay: 250,
        data: opt.paramsData,
        processResults: opt.processResults,
        cache: true
    };
    if ($obj) {
        ajaxOption.dropdownParent = $obj;
    }
    $("#" + id).select2({
        placeholder: "请输入关键字",
        language: "zh-CN",
        ajax: ajaxOption,
        escapeMarkup: function (markup) { return markup; }, // 字符转义处理
        minimumInputLength: 1,
        templateResult: opt.formatRepo, //返回结果回调function formatRepo(repo){return repo.text},这样就可以将返回结果的的text显示到下拉框里，当然你可以return repo.text+"1";等
        templateSelection: opt.formatRepoSelection //选中项回调function formatRepoSelection(repo) { return repo.text }

    });
}

//注册选中事件
function checkOn(table, obj, fun) {
    var mark;
    var union = function (array, ids) {
        $.each(ids, function (i, id) {
            if ($.inArray(id, array) == -1) {
                array[array.length] = id;
            }
        });
        return array;
    };
    var difference = function (array, ids) {
        $.each(ids, function (i, id) {
            var index = $.inArray(id, array);
            mark = index;
            if (index != -1) {
                array.splice(index, 1);
            }
        });
        return array;
    };
    var unions = function (arrays, rowa) {
        $.each(rowa, function (i, row) {
            if ($.inArray(row, arrays) == -1) {
                arrays[arrays.length] = row;
            }
        });
        return arrays;
    };
    var differences = function (arrays, rowa) {
        $.each(rowa, function (i, row) {

            if (mark != -1) {
                arrays.splice(mark, 1);
            }
        });

        return arrays;
    };
    table.on('check.bs.table check-all.bs.table ' +
        'uncheck.bs.table uncheck-all.bs.table', function (e, rows) {

            var d = { "union": union, "difference": difference };
            var r = { "unions": unions, "differences": differences };
            var ids = $.map(!$.isArray(rows) ? [rows] : rows, function (row) {
                return row.Id;
            }),
                rowarry = $.map(!$.isArray(rows) ? [rows] : rows, function (row) {
                    return row;
                }),
                func = $.inArray(e.type, ['check', 'check-all']) > -1 ? 'union' : 'difference',
                funcs = $.inArray(e.type, ['check', 'check-all']) > -1 ? 'unions' : 'differences';
            obj.ids = d[func](obj.ids, ids);
            obj.rows = r[funcs](obj.rows, rowarry);
            if (fun) {
                fun();
            }
        });
}
function goBackOrClose() {
    if (window.history.length > 2) {
        window.history.go(-1);
    } else {
        window.opener = null; window.close();
    }
}

function initDateRange($datepicker) {
    $datepicker.daterangepicker({
        autoUpdateInput: false,
        opens: "center",
        linkedCalendars: false,
        ranges: {
            '今日': [moment(), moment()],
            '昨日': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            '最近7日': [moment().subtract(6, 'days'), moment()],
            '最近30日': [moment().subtract(29, 'days'), moment()],
            '本月': [moment().startOf('month'), moment().endOf('month')],
            '上月': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: {
            format: 'YYYY-MM-DD',
            separator: "至",
            applyLabel: "确认",
            cancelLabel: "取消",
            fromLabel: "从",
            toLabel: "到",
            customRangeLabel: "自定义",
            weekLabel: "周",
            daysOfWeek: [
                "日",
                "一",
                "二",
                "三",
                "四",
                "五",
                "六"
            ],
            monthNames: [
                "一月",
                "二月",
                "三月",
                "四月",
                "五月",
                "六月",
                "七月",
                "八月",
                "九月",
                "十月",
                "十一月",
                "十二月"
            ],
            firstDay: 1
        }
    });

    $datepicker.on('apply.daterangepicker', function (ev, picker) {
        $(this).val(picker.startDate.format('YYYY-MM-DD') + '至' + picker.endDate.format('YYYY-MM-DD'));
    });
    $datepicker.on('cancel.daterangepicker', function (ev, picker) {
        $(this).val('');
    });
}

function exportDate() {
    swal({
        title: "您确定吗?",
        text: "确认要导出数据吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true
    }, function () {
        $('#searchFrm')[0].submit();
        swal({
            title: "数据正在导出中...",
            text: "请耐心等待",
            timer: 2000,
            showConfirmButton: false
        });
    });
}
function isJSON(str) {
    if (typeof str == 'string') {
        try {
            var obj = JSON.parse(str);
            if (typeof obj == 'object' && obj) {
                return true;
            } else {
                return false;
            }

        } catch (e) {
            return false;
        }
    }
    return false;
}
function syntaxHighlight(json) {
    if (typeof json != 'string') {
        json = JSON.stringify(json, undefined, 2);
    }
    json = json.replace(/&/g, '&').replace(/</g, '<').replace(/>/g, '>');
    return json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {
        var cls = 'number';
        if (/^"/.test(match)) {
            if (/:$/.test(match)) {
                cls = 'key';
            } else {
                cls = 'string';
            }
        } else if (/true|false/.test(match)) {
            cls = 'boolean';
        } else if (/null/.test(match)) {
            cls = 'null';
        }
        return '<span class="' + cls + '">' + match + '</span>';
    });
}
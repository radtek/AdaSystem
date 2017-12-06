window.operateEvents = {
    'click .remove': function (e, value, row, index) {
        $table.bootstrapTable('remove', {
            field: 'Id',
            values: [row.Id]
        });
    }
};
Dropzone.autoDiscover = false;
var myDropzone = new Dropzone("#myDropzone", {
    url: uploadapi,
    paramName: "upfile",
    addRemoveLinks: true,
    dictRemoveFile: '删除',
    dictCancelUpload: "取消",
    maxFiles: 1,
    maxFilesize: 2,
    dictFileTooBig: "图片文件太大，当前为{{filesize}}M，最大只能{{maxFilesize}}M",
    acceptedFiles: ".jpg,.png,.jpeg",
    dictInvalidFileType: "只允许上传图片类型文件（*.jpg,*.jpge,*.png）",
    dictMaxFilesExceeded: "只允许上传一张图片",
    dictResponseError: "上传失败，{{statusCode}}",
    //sending: function (file, xhr, formData) {//如果你想给上传的文件添加额外（多个文件时会具体到每个文件）,您可以注册发送事件
    //    formData.append("filesize", file.size);
    //},
    success: function (file, response, e) {
        if (response.State == 1) {
            if (file.previewElement) {
                $(file.previewTemplate).children('.dz-success-mark').css('opacity', '1');
            }
            $("#Image").val(response.Msg);
        } else {
            $(file.previewTemplate).children('.dz-error-mark').css('opacity', '1');
        }
    },
    removedfile: function (file) {
        if (file.previewElement != null && file.previewElement.parentNode != null) {
            file.previewElement.parentNode.removeChild(file.previewElement);
        }
        if (file.status == "success") {//清空上传成功的文件
            $("#Image").val("");
            //TODO 删除服务器上的文件
        }
        return this._updateMaxFilesReachedClass();
    }
});
$(function () {
    initData();
    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            setTableData();
            $('.wrapper.wrapper-content .ibox').children('.ibox-content').toggleClass('sk-loading');
            form.submit();
        }
    });
});
function initData() {
    $('.input-group.date').datetimepicker({
        language: 'zh-CN',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: "yyyy年mm月dd日"
    });
    $table.bootstrapTable({
        data: payData,
        toolbar: '#toolbar',
        classes: "table table-no-bordered",
        columns: [
            {
                field: 'IncomeExpendId',
                title: '收支项目',
                align: "center", valign: "middle",
                footerFormatter: function () {
                    return "合计";
                },
                editable: {
                    mode: "inline",
                    emptytext: '请选择',
                    type: 'select',
                    source: incomeExpends
                }
            },
            {
                field: 'SettleAccountId',
                title: '结算账户',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请选择',
                    type: 'select',
                    source: settleAccounts
                }
            },
            {
                field: 'Money',
                title: '金额',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入',
                    validate: function (value) {
                        if (isNaN(value)) {
                            return '请输入有效的金额';
                        }
                    }
                },
                footerFormatter: sumFormatter
            }
            ,
            {
                field: 'operate',
                title: '操作',
                align: "center", valign: "middle",
                events: operateEvents,
                formatter: operateFormatter

            }

        ],
        formatNoMatches: function () {  //没有匹配的结果
            return '请先添加结算账户';
        },
        showFooter: true
    });
    $("#btn_add").click(function () {
        insertRow();
    });
    //初始化增加一个结算账户
    if (payData.length == 0) {
        insertRow(paymoney);
    }

}

function insertRow(money) {
    var randomId = 100 + ~~(Math.random() * 100);
    $table.bootstrapTable('insertRow', {
        index: 0,
        row: {
            Id: randomId,
            IncomeExpendId: "",
            SettleAccountId: "",
            Money: money || 0
        }
    });
}
//移除结算账户
function operateFormatter(value, row, index) {
    return [
        '<a class="remove" href="javascript:void(0)" title="Remove">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}
//合计
function sumFormatter(data) {
    var field = this.field;
    var total_sum = data.reduce(function (sum, row) {
        return (sum) * 1 + (row[field] || 0) * 1;
    }, 0);
    return Math.toFixMoney(total_sum);
}
function setTableData() {
    var tableData = $table.bootstrapTable('getData');
    $("#PayDetails").val(JSON.stringify(tableData));
}
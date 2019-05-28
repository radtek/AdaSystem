jQuery.validator.addMethod("isMediaId", function (value, element) {
    var reg = /^[A-Za-z0-9_\-\.]+$/ig;
    return this.optional(element) || (reg.test(value));
}, "请正确填写媒体ID");
window.operateEvents = {
    'click .remove': function (e, value, row, index) {
        $table.bootstrapTable('remove', {
            field: 'Id',
            values: [row.Id]
        });
    }
};
var $table = $("#tablePrice");
$(function () {
    $("#LinkManId").select2({
        placeholder: "请选择",
        language: "zh-CN",
        //allowClear: true,
        ajax: {
            url: linkmanurl,
            dataType: 'json',
            delay: 250,
            data: function (params) {
                return {
                    search: params.term, // search term
                    IsBusiness: true
                };
            },
            processResults: function (data, params) {
                var result = $.map(data.rows,
                    function (v, k) {
                        return { id: v.Id, text: v.Name, commpany: v.CommpanyName };
                    });
                return {
                    results: result
                };
            },
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // 字符转义处理
        minimumInputLength: 1,
        templateResult:
        formatRepo, //返回结果回调function formatRepo(repo){return repo.text},这样就可以将返回结果的的text显示到下拉框里，当然你可以return repo.text+"1";等
        templateSelection: formatRepoSelection //选中项回调function formatRepoSelection(repo) { return repo.text }

    });
    $('.input-group.date').datetimepicker({
        language: 'zh-CN',
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        format: "yyyy-mm-dd"
    });
    $table.bootstrapTable({
        data: data,
        //classes: "table table-no-bordered",
        columns: [
            {
                field: 'Platform',
                title: '平台',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请选择',
                    type: 'select',
                    source: platformData
                }
            },
            {
                field: 'PriceName',
                title: '价格名称',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入'
                }
            },
            {
                field: 'Offer',
                title: '报价',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入'
                }
            },
            {
                field: 'OfferDate',
                title: '报价日期',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline", emptytext: '请输入', validate: function (value) {
                        var reg = /^(\d{4})-(0\d{1}|1[0-2])-(0\d{1}|[12]\d{1}|3[01])$/;
                        if (!reg.test(value)) {
                            return '请输入有效的日期格式（如：2018-01-01）';
                        }

                    }
                }, formatter: function (value) {
                    if (value) {
                        return moment(value).format("YYYY-MM-DD");
                    } else {
                        return moment().format("YYYY-MM-DD");
                    }
                    
                }
            },
            {
                field: 'operate',
                title: '操作',
                align: "center", valign: "middle",
                events: operateEvents,
                formatter: operateFormatter
            }

        ],
        formatNoMatches: function () {  //没有匹配的结果
            return '请添加平台报价';
        }
    });
    $("#btn_add").click(function () {
        insertRow();
    });
    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            var tableData = $table.bootstrapTable('getData');
            if (tableData.length>0) {
                $("#MediaReferencePriceJson").val(JSON.stringify(tableData));
            } else {
                $("#MediaReferencePriceJson").val("");
            }
            
            form.submit();
        },
        rules: {
            //MediaLink: {
            //    url: true
            //},
            MediaID: {
                isMediaId: true
            }
        }
    });
});

function formatRepo(repo) {
    if (repo.loading) {
        return repo.text;
    }
    return "<p>" + repo.commpany + " 【 " + repo.text + " 】 <p>";
}

function formatRepoSelection(repo) {
    $("#LinkManName").val(repo.text);
    return repo.text;
}
//删除操作
function operateFormatter(value, row, index) {
    return [
        '<a class="remove" href="javascript:;" title="Remove">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}

function insertRow() {
    var randomId = 100 + ~~(Math.random() * 100);
    $table.bootstrapTable('insertRow', {
        index: 0,
        row: {
            Id: randomId,
            Platform: "",
            PriceName: "",
            Offer: 0,
            OfferDate: moment().format('YYYY-MM-DD')
        }
    });
}
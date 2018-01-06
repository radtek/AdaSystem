var $mediatable,
    $table = $('#table'),
    key,
    selections = {},
    linkmanSelect = {},
    transactorSelect = {};

selections.ids = [];
selections.rows = [];

linkmanSelect.url = linkmanUrl;
linkmanSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        IsBusiness: false
    };
};
linkmanSelect.processResults = function (data, params) {
    var result = $.map(data.rows,
        function (v, k) {
            return { id: v.Id, text: v.Name, commpany: v.CommpanyName };
        });
    return {
        results: result
    };
};
linkmanSelect.formatRepo = function (repo) {
    if (repo.loading) {
        return repo.text;
    }
    return "<p>" + repo.commpany + " 【 " + repo.text + " 】 <p>";
};
linkmanSelect.formatRepoSelection = function (repo) {
    $("#LinkManName").val(repo.text);
    return repo.text;
};

transactorSelect.url = managerUrl;
transactorSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        Status: 1
    };
};
transactorSelect.processResults = function (data, params) {
    var result = $.map(data.rows,
        function (v, k) {
            return { id: v.Id, text: v.UserName };
        });
    return {
        results: result
    };
};
transactorSelect.formatRepo = function (repo) {
    if (repo.loading) {
        return repo.text;
    }
    return repo.text;
};
transactorSelect.formatRepoSelection = function (repo) {
    $("#Transactor").val(repo.text);
    return repo.text;
};
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
    
    
    var isvisible = $("#Status").val() == 0 ? true : false;
   
    if (!isReadonly) {
        initSelect2("LinkManId", linkmanSelect);
        initSelect2("TransactorId", transactorSelect);
    }
    $table.bootstrapTable({
        data: orderData,
        toolbar: '#toolbar',
        classes: "table table-no-bordered",
        clickToSelect: true,
        columns: [
            {
                checkbox: true,
                formatter: function (value, row, index) {
                    if (row.Status === 1) {
                        return {
                            disabled: true
                        };
                    }
                }
            },
            {
                field: 'MediaTypeName',
                title: '媒体类型',
                align: "center", valign: "middle",
                footerFormatter: function () {
                    return "合计";
                }
            },
            {
                field: 'MediaName',
                title: '媒体名称',
                align: "center", valign: "middle"

            },
            {
                field: 'AdPositionName',
                title: '广告位',
                align: "center", valign: "middle"
            },
            {
                field: 'CostMoney',
                title: '参考成本',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter
            },
            {
                field: 'MediaByPurchase',
                title: '经办媒介',
                align: "center", valign: "middle"

            },
            {
                field: 'MediaTitle',
                title: '稿件标题',
                align: "center", valign: "middle",
                editable: { mode: "inline", emptytext: '请输入' }
            },
            {
                field: 'PrePublishDate',
                title: '预出刊日期',
                align: "center", valign: "middle",
                editable: {
                    type: 'combodate',
                    mode: "inline",
                    emptytext: '请输入',
                    format: 'YYYY-MM-DD',
                    viewformat: 'YYYY-MM-DD',
                    template: 'YYYY 年 MM 月 DD 日',
                    combodate: {
                        minYear: moment().format('YYYY'),
                        maxYear: moment().add(1, "years").format('YYYY'),
                        minuteStep: 1
                    }
                }
            },
            {
                field: 'SellMoney',
                title: '无税金额',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline", emptytext: '请输入', validate: function (value) {
                        if (isNaN(value)) {
                            return '请输入有效的金额';
                        }
                    }
                },
                footerFormatter: sumFormatter
            },
            {
                field: 'Money',
                title: '销售金额',
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
            },
            {
                field: 'Tax',
                title: '税率%',
                align: "center", valign: "middle",
                //editable: { mode: "inline", emptytext: '请输入' }
            },
            {
                field: 'TaxMoney',
                title: '税额',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter
            },
            {
                field: 'Remark',
                title: '备注',
                align: "center", valign: "middle",
                editable: { mode: "inline", emptytext: '请输入' }
            }
            ,
            {
                field: 'operate',
                title: '操作',
                align: "center", valign: "middle",
                events: operateEvents,
                formatter: operateFormatter,
                visible: isvisible

            }

        ],
        formatNoMatches: function () {  //没有匹配的结果
            return '请先添加销售明细';
        },
        onEditableSave: function (field, row, oldValue, $el) {
            var index = $($el).parents('tr[data-index]').data('index');
            if (row.SellMoney && field == "SellMoney") {
                row.Money = Math.toFixMoney(row.SellMoney * (1 + row.Tax / 100));//乘以税率
            }
            if (row.Money && field == "Money") {
                row.SellMoney = Math.toFixMoney(row.Money / (1 + row.Tax / 100));
            }
            row.TaxMoney = Math.toFixMoney(row.SellMoney * (row.Tax / 100));
            $table.bootstrapTable('updateRow', { index: index, row: row });
        },
        showFooter: true
    });
    $('#Tax').on('input propertychange', function () {
        var tax = $(this).val();
        //更新表格数据
        if (tax.trim() != "" && !isNaN(tax)) {
            var temp = $table.bootstrapTable('getData');
            if (temp.length > 0) {
                $.each(temp,
                    function (k, v) {
                        if (v.SellMoney) {
                            v.Money = Math.toFixMoney(v.SellMoney * (1 + tax / 100));//乘以税率
                        } else if (v.Money) {
                            v.SellMoney = Math.toFixMoney(v.Money / (1 + tax / 100));
                        }
                        v.TaxMoney = Math.toFixMoney(v.SellMoney * (tax / 100));
                        v.Tax = tax;
                    });
                $table.bootstrapTable('load', temp);
            }

        }

    });
}
function showMedia(url) {
    $("#modalView").load(url,
        function () {
            //初始化
            initMediaData($(this).find(".nav-tabs .active"));
            $('#modalView .modal').on('shown.bs.modal', function () {
                $mediatable = $('#mediaTable');
                $mediatable.bootstrapTable({
                    url: mediapriceUrl,
                    toolbar: '#mediatoolbar',
                    classes: "table table-no-bordered",
                    cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                    pagination: true,                   //是否显示分页（*）
                    sortable: true,                     //是否启用排序
                    sortOrder: "desc",                   //排序方式
                    sortName: "Id",
                    sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
                    pageNumber: 1,                       //初始化加载第一页，默认第一页
                    pageSize: 10,                       //每页的记录行数（*）
                    // search: true,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
                    //strictSearch: true,                 //设置为 true启用 全匹配搜索，否则为模糊搜索
                    //showColumns: true,                  //是否显示所有的列
                    //showRefresh: true,                  //是否显示刷新按钮
                    clickToSelect: true,                //是否启用点击选中行
                    uniqueId: "Id",                     //每一行的唯一标识，一般为主键列
                    responseHandler: responseHandler,
                    mobileResponsive: true,
                    queryParams: function (parameters) {
                        parameters.MediaTypeIndex = key;
                        parameters.AdPositionName = $("#AdPositionName").val();
                        parameters.MediaNames = $("#MediaName").val();
                        return parameters;
                    },
                    columns: [
                        {
                            field: 'state',
                            checkbox: true
                        },
                        {
                            field: 'MediaName',
                            title: '媒体信息',
                            align: "center",
                            //formatter: function (v, r, i) {
                            //    var str = v;
                            //    switch (key) {
                            //        case "website":
                            //            str = v + "-" + r.Client + "-" + r.Channel;
                            //            break;
                            //        case "weixin":
                            //            str = v + " [ " + r.MediaID + " ]";
                            //            break;
                            //        case "headline":
                            //        case "webcast":
                            //            str = "[ " + r.Platform + " ] " + v;
                            //            break;
                            //    }
                            //    return str;
                            //}
                        },
                        //{
                        //    field: 'MediaID',
                        //    title: '媒体ID',
                        //    align: "center"
                        //},
                        {
                            field: 'MediaTagStr',
                            title: '媒体分类',
                            align: "center"
                        },
                        {
                            field: 'AdPositionName',
                            title: '媒体价格',
                            align: "center",
                            formatter: function (v, r, i) {
                                return "<span class='label label-warning'>" + v + "：" + (r.PurchasePrice == 0 ? "不接单" : r.PurchasePrice) + "</span>";
                            }

                        }
                    ]
                });
                //注册选中事件
                $mediatable.on('check.bs.table check-all.bs.table ' +
                    'uncheck.bs.table uncheck-all.bs.table', function (e, rows) {
                        var ids = $.map(!$.isArray(rows) ? [rows] : rows, function (row) {
                            return row.Id;
                        }),
                            rowarry = $.map(!$.isArray(rows) ? [rows] : rows, function (row) {
                                return row;
                            }),
                            func = $.inArray(e.type, ['check', 'check-all']) > -1 ? 'union' : 'difference';
                        selections.ids = _[func](selections.ids, ids);
                        selections.rows = _[func](selections.rows, rowarry);
                    });
            }).on('hidden.bs.modal', function () {
                //重置数据
                selections.ids = [];
                selections.rows = [];
                $mediatable.bootstrapTable('destroy');
            });
            $('#modalView .modal').modal('show');

        });
}
//初始化媒体选择参数
function initMediaData($obj, isrefresh) {
    key = $obj.attr("data-key");
    var arrposition = $obj.attr("data-value");
    var options = JSON.parse(arrposition);
    var $positionSelect = $("#AdPositionName");
    $positionSelect.empty();
    $.each(options,
        function (k, v) {
            $positionSelect.append('<option value="' + v + '">' + v + '</option>');
        });
    if (isrefresh) {
        $mediatable.bootstrapTable('refresh');
    }
}

//保留选中结果
function responseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.Id, selections.ids) !== -1;
    });
    return res;
}
//切换类型，筛选数据
function mediasByType(obj, key) {
    $(obj).addClass("active").siblings().removeClass("active");
    $mediatable.bootstrapTable('refresh', { url: mediapriceUrl + '?MediaTypeIndex=' + key });
}
function initSelect2(id, opt) {
    $("#" + id).select2({
        placeholder: "请输入关键字",
        language: "zh-CN",
        ajax: {
            url: opt.url,
            dataType: 'json',
            delay: 250,
            data: opt.paramsData,
            processResults: opt.processResults,
            cache: true
        },
        escapeMarkup: function (markup) { return markup; }, // 字符转义处理
        minimumInputLength: 1,
        templateResult: opt.formatRepo, //返回结果回调function formatRepo(repo){return repo.text},这样就可以将返回结果的的text显示到下拉框里，当然你可以return repo.text+"1";等
        templateSelection: opt.formatRepoSelection //选中项回调function formatRepoSelection(repo) { return repo.text }

    });
}
window.operateEvents = {
    'click .remove': function (e, value, row, index) {
        $table.bootstrapTable('remove', {
            field: 'MediaPriceId',
            values: [row.MediaPriceId]
        });
    }
};

function deleteRow() {
    var rows = $table.bootstrapTable("getSelections");
    if (rows.length > 0) {
        var arr = [];
        $.each(rows,
            function(k, v) {
                arr.push(v.MediaPriceId);
            });
        $table.bootstrapTable('remove', {
            field: 'MediaPriceId',
            values: arr
        });
    } else {
        swal("操作提醒", "请选择要删除的数据", "warning");
    }
}
function operateFormatter(value, row, index) {
    return [
        '<a class="remove" href="javascript:void(0)" title="Remove">',
        '<i class="glyphicon glyphicon-remove"></i>',
        '</a>'
    ].join('');
}
//获取选中数据，插入
function confirmData() {
    var temp = getData();
    if (temp.length > 0) {
        $table.bootstrapTable('append', temp);
    }
    $('#modalView .modal').modal('hide');
}
//过滤重复数据
function getData() {
    var temp = [], tax = $("#Tax").val(), now = moment().add(10, "days").format('YYYY-MM-DD');
    var tableData = $table.bootstrapTable('getData');
    $.each(selections.rows,
        function (k, v) {
            var index = _.findIndex(tableData, { 'MediaPriceId': v.Id });
            if (index < 0) {
                temp.push({
                    MediaTypeName: v.TypeName,
                    MediaPriceId: v.Id,
                    MediaName: v.MediaName,
                    AdPositionName: v.AdPositionName,
                    TaxMoney: 0,
                    Tax: tax,
                    Money: 0,
                    SellMoney: 0,
                    MediaTitle: "",
                    PrePublishDate: now,
                    Remark: "",
                    MediaByPurchase: v.Transactor,
                    CostMoney: v.PurchasePrice
                });
            }

        });
    return temp;
}
function sumFormatter(data) {
    var field = this.field;
    var total_sum = data.reduce(function (sum, row) {
        return (sum) * 1 + (row[field] || 0) * 1;
    }, 0);
    return Math.toFixMoney(total_sum);
}

function setTableData() {
    var tableData = $table.bootstrapTable('getData');
    $("#OrderDetails").val(JSON.stringify(tableData));
}
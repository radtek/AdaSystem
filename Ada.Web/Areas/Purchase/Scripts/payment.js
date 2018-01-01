var $ordertable, $table = $('#table'), $paytable = $('#paytable'),
    selections = {},
    linkmanSelect = {},
    transactorSelect = {};
selections.ids = [];
selections.rows = [];
linkmanSelect.url = linkmanUrl;
linkmanSelect.paramsData = function (params) {
    return {
        search: params.term, // search term
        IsBusiness: true
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
    $table.add($paytable).bootstrapTable('removeAll');
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
window.operateEvents = {
    'click .remove': function (e, value, row, index) {
        $table.add($paytable).bootstrapTable('remove', {
            field: 'Id',
            values: [row.Id]
        });
    }
};
$(function () {
    initData();
    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            setTableData();
            $('.wrapper.wrapper-content .ibox').children('.ibox-content').toggleClass('sk-loading');
            form.submit();
        },
        rules: {
            PayMoney: {
                min: 0.01
            }
        },
        messages: {
            PayMoney: {
                min: "申请金额须大于0"
            }
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
    if (!isReadonly) {
        initSelect2("LinkManId", linkmanSelect);
        initSelect2("TransactorId", transactorSelect);
    }
    $table.bootstrapTable({
        data: orders,
        classes: "table table-no-bordered",
        columns: [
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
                title: '成本价格',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter
            },
            {
                field: 'PurchaseMoney',
                title: '无税金额',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline", emptytext: '请输入', validate: function (value) {
                        if (isNaN(value)) {
                            return '请输入有效的金额';
                        }
                        var index = $(this).parents('tr[data-index]').data('index'),
                            data = $table.bootstrapTable('getData'),
                            row = data[index];
                        if (value > row.CostMoney) {
                            return '不能超出成本价格';
                        }
                    }
                },
                footerFormatter: sumFormatter
            },
            {
                field: 'Money',
                title: '采购金额',
                align: "center", valign: "middle",
                //editable: {
                //    mode: "inline",
                //    emptytext: '请输入',
                //    validate: function (value) {
                //        if (isNaN(value)) {
                //            return '请输入有效的金额';
                //        }
                //    }
                //},
                footerFormatter: sumFormatter
            },
            {
                field: 'Tax',
                title: '税率%',
                align: "center", valign: "middle"
            },
            {
                field: 'TaxMoney',
                title: '税额',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter
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
            return '请先添加订单';
        },
        onEditableSave: function (field, row, oldValue, $el) {
            var index = $($el).parents('tr[data-index]').data('index');
            if (row.PurchaseMoney && field == "PurchaseMoney") {
                row.Money = Math.toFixMoney(row.PurchaseMoney * (1 + row.Tax / 100));//乘以税率
            }
            if (row.Money && field == "Money") {
                row.PurchaseMoney = Math.toFixMoney(row.Money / (1 + row.Tax / 100));
            }
            row.TaxMoney = Math.toFixMoney(row.PurchaseMoney * (row.Tax / 100));
            $table.bootstrapTable('updateRow', { index: index, row: row });
        },
        showFooter: true
    });
    $("#btn_add").click(function () {
        showOrder();
    });
    //付款明细
    $paytable.bootstrapTable({
        data: pays,
        classes: "table table-no-bordered",
        columns: [
            {
                field: 'PaymentType',
                title: '付款性质',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请选择',
                    type: 'select',
                    source: paymentTypes
                },
                footerFormatter: function () {
                    return "合计";
                }
            },
            {
                field: 'AccountBank',
                title: '开户行',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入'
                }

            },
            {
                field: 'AccountName',
                title: '开户名',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入'
                }
            },
            {
                field: 'AccountNum',
                title: '开户号',
                align: "center", valign: "middle",
                editable: {
                    mode: "inline",
                    emptytext: '请输入'
                }
            },
            {
                field: 'PayMoney',
                title: '申请金额',
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
            return '请先付款信息';
        },
        showFooter: true
    });
    $("#paybtn_add").click(function () {
        var randomId = 100 + ~~(Math.random() * 100);
        $paytable.bootstrapTable('insertRow', {
            index: 0,
            row: {
                Id: randomId,
                PaymentType: "",
                AccountBank: "",
                AccountName: "",
                AccountNum: "",
                PayMoney: 0
            }
        });
    });
    //税率变动事件
    $('#Tax').on('input propertychange', function () {
        var tax = $(this).val();
        //更新表格数据
        if (tax.trim() != "" && !isNaN(tax)) {
            var temp = $table.bootstrapTable('getData');
            if (temp.length > 0) {
                $.each(temp,
                    function (k, v) {
                        if (v.PurchaseMoney) {
                            v.Money = Math.toFixMoney(v.PurchaseMoney * (1 + tax / 100));//乘以税率
                        } else if (v.Money) {
                            v.PurchaseMoney = Math.toFixMoney(v.Money / (1 + tax / 100));
                        }
                        v.TaxMoney = Math.toFixMoney(v.PurchaseMoney * (tax / 100));
                        v.Tax = tax;
                    });
                $table.bootstrapTable('load', temp);
            }
        }
    });

}
//保留选中结果
function responseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.Id, selections.ids) !== -1;
    });
    return res;
}
//初始化下拉
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
//删除操作
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
    var tableData = $table.bootstrapTable('getData'), temp = [];
    $.each(selections.rows,
        function (k, v) {
            var index = _.findIndex(tableData, { 'Id': v.Id });
            if (index < 0) {
                v.Tax = $("#Tax").val() || 0;
                v.PurchaseMoney = v.CostMoney;
                v.Money = Math.toFixMoney(v.CostMoney * (1 + v.Tax / 100));//乘以税率
                v.TaxMoney = Math.toFixMoney(v.CostMoney * (v.Tax  / 100));
                temp.push(v);
            }

        });
    return temp;
}
//合计算法
function sumFormatter(data) {
    var field = this.field;
    var total_sum = data.reduce(function (sum, row) {
        return (sum) * 1 + (row[field] || 0) * 1;
    }, 0);
    var result = Math.toFixMoney(total_sum);
    if (field =="Money") {
        $("#PayMoney").val(result);
    }
    return result;
}
//订单数据加载到表单
function setTableData() {
    var tableData = $table.bootstrapTable('getData'), paytableData = $paytable.bootstrapTable('getData');
    $("#OrderDetails").val(JSON.stringify(tableData));
    $("#PayDetails").val(JSON.stringify(paytableData));
}
//显示采购订单模态窗口
function showOrder() {
    var linkman = $("#LinkManId").val();
    if (linkman) {
        $("#modalView").load(orderdetail,
            function () {
                //初始化
                $('#modalView .modal').on('shown.bs.modal', function () {
                    $ordertable = $('#orderTable');
                    $ordertable.bootstrapTable({
                        classes: "table table-no-bordered",
                        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                        url: orderapi,
                        pagination: true,                   //是否显示分页（*）
                        sortable: true,                     //是否启用排序
                        sortOrder: "desc",                   //排序方式
                        sortName: "Id",
                        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
                        pageNumber: 1,                       //初始化加载第一页，默认第一页
                        pageSize: 10,                       //每页的记录行数（*）
                        search: true,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
                        strictSearch: true,                 //设置为 true启用 全匹配搜索，否则为模糊搜索
                        //showColumns: true,                  //是否显示所有的列
                        showRefresh: true,                  //是否显示刷新按钮
                        clickToSelect: true,                //是否启用点击选中行
                        uniqueId: "Id",                     //每一行的唯一标识，一般为主键列
                        responseHandler: responseHandler,
                        queryParams: function (parameters) {
                            parameters.LinkManId = linkman;
                            parameters.IsPayment = false;
                            return parameters;
                        },
                        columns: [
                            {
                                field: 'state',
                                checkbox: true
                            },
                            {
                                field: 'MediaTypeName',
                                title: '媒体类型',
                                align: "center", valign: "middle"

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
                                field: 'PublishDate',
                                title: '出刊日期',
                                align: "center", valign: "middle",
                                formatter: function (value) {
                                    if (value) {
                                        return moment(value).format("YYYY-MM-DD");
                                    }
                                }
                            },
                            {
                                field: 'CostMoney',
                                title: '采购成本',
                                align: "center", valign: "middle"
                            }
                        ]
                    });
                    //注册选中事件
                    $ordertable.on('check.bs.table check-all.bs.table ' +
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
                    $ordertable.bootstrapTable('destroy');
                });
                $('#modalView .modal').modal('show');

            });
    } else {
        swal("操作提醒", "请先选择供应商", "warning");
    }

}
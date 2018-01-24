var $ordertable, $table = $('#table'),
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
    $table.bootstrapTable("removeAll");
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
        $table.bootstrapTable('remove', {
            field: 'BusinessOrderId',
            values: [row.BusinessOrderId]
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
            TotalMoney: {
                min: 0.01
            }
        },
        messages: {
            TotalMoney: {
                min: "开票金额须大于0"
            }
        }
    });
});

function initData() {
    if (!isReadonly) {
        initSelect2("LinkManId", linkmanSelect);
        initSelect2("TransactorId", transactorSelect);
    }
    $table.bootstrapTable({
        data: orders,
        classes: "table table-no-bordered",
        columns: [
            {
                field: 'OrderNum',
                title: '订单编号',
                align: "center", valign: "middle",
                footerFormatter: function () {
                    return "合计";
                }
            },
            {
                field: 'Remark',
                title: '项目摘要',
                align: "center", valign: "middle"

            },
            
            {
                field: 'OrderMoney',
                title: '可开票金额',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter
            },
            {
                field: 'InvoiceMoney',
                title: '本次开票金额',
                align: "center", valign: "middle",
                footerFormatter: sumFormatter,
                editable: {
                    mode: "inline",
                    emptytext: '请输入',
                    validate: function (value) {
                        if (isNaN(value)) {
                            return '请输入有效的金额';
                        }
                    }
                }
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
            return '请先添加开票订单';
        },
        showFooter: true
    });
    $("#btn_add").click(function () {
        showOrder();
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
        '<a class="remove" href="javascript:void(0)" title="移除">',
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
            var index = _.findIndex(tableData, { 'BusinessOrderId': v.Id });
            if (index < 0) {
                var invoiceDetail = {
                    BusinessOrderId: v.Id,
                    OrderMoney: v.TotalMoney,
                    InvoiceMoney: v.TotalMoney,
                    Remark: v.Remark,
                    OrderNum: v.OrderNum
                };
                temp.push(invoiceDetail);
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
    //赋值到发票金额
    if (field =="InvoiceMoney") {
        $("#TotalMoney").val(total_sum);
    }
    return Math.toFixMoney(total_sum);
}
//订单数据加载到表单
function setTableData() {
    var tableData = $table.bootstrapTable('getData');
    $("#OrderDetails").val(JSON.stringify(tableData));
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
                            //parameters.IsInvoice = false;
                            return parameters;
                        },
                        columns: [
                            {
                                field: 'state',
                                checkbox: true
                            },
                            {
                                field: 'Remark',
                                title: '项目摘要',
                                align: "center", valign: "middle"

                            },
                            {
                                field: 'OrderNum',
                                title: '订单编号',
                                align: "center", valign: "middle"

                            },
                            {
                                field: 'TotalMoney',
                                title: '销售金额',
                                align: "center", valign: "middle"
                            },
                            {
                                field: 'OrderDate',
                                title: '单据时间',
                                align: "center", valign: "middle",
                                formatter: function (value) {
                                    return moment(value).format("YYYY-MM-DD");
                                }
                            }
                        ]
                    });
                    //注册选中事件
                    checkOn($ordertable, selections);
                }).on('hidden.bs.modal', function () {
                    //重置数据
                    selections.ids = [];
                    selections.rows = [];
                    $ordertable.bootstrapTable('destroy');
                });
                $('#modalView .modal').modal('show');

            });
    } else {
        swal("操作提醒", "请先选择客户", "warning");
    }

}
var linkmanSelect = {},
    transactorSelect = {},
    isPayeeSelect = false,
    isOrderSelect = false,
    payeeSelections = {},
    orderSelections = {},
    $payeeTable,
    $orderTable;
payeeSelections.ids = [];
payeeSelections.rows = [];
orderSelections.ids = [];
orderSelections.rows = [];
linkmanSelect.url = linkmanapi;
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
    isPayeeSelect = true;
    isOrderSelect = true;
    payeeSelections.ids = [];
    payeeSelections.rows = [];
    orderSelections.ids = [];
    orderSelections.rows = [];

    return repo.text;
};
transactorSelect.url = transactorapi;
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
    $(".wrapper.wrapper-content form").steps({
        bodyTag: "fieldset",
        labels: {
            cancel: "取消",
            finish: "确认核销",
            next: "下一步",
            previous: "上一步",
            loading: "核销中 ..."
        },
        onInit: function (event, currentIndex) {
            initSelect2("LinkManId", linkmanSelect);
            $payeeTable = $('#payeetable');
            $orderTable = $('#ordertable');
            initPayee();
            initOrder();
        },
        onStepChanging: function (event, currentIndex, newIndex) {
            // Always allow going backward even if the current step contains invalid fields!
            if (currentIndex > newIndex) {
                return true;
            }

            //// Forbid suppressing "Warning" step if the user is to young
            //if (newIndex === 3 && Number($("#age").val()) < 18) {
            //    return false;
            //}

            var form = $(this);

            // Clean up if user went backward before
            if (currentIndex < newIndex) {
                // To remove error styles
                $(".body:eq(" + newIndex + ") label.error", form).remove();
                $(".body:eq(" + newIndex + ") .error", form).removeClass("error");
            }

            // Disable validation on fields that are disabled or hidden.
            form.validate().settings.ignore = ":disabled,:hidden";

            // Start validation; Prevent going forward if false
            return form.valid();
        },
        onStepChanged: function (event, currentIndex, priorIndex) {
            //// Suppress (skip) "Warning" step if the user is old enough.
            if (currentIndex === 3) {
                initSelect2("TransactorId", transactorSelect);
                //计算选取总额
                var orderMoney = 0, payeeMoney = 0;
                $.each(orderSelections.rows,
                    function (k, v) {
                        orderMoney += v.VerificationMoney;
                    });
                $.each(payeeSelections.rows,
                    function (k, v) {
                        payeeMoney += v.VerificationMoney;
                    });
                $("#OrderMoney").val(orderMoney);
                $("#PayeeMoney").val(payeeMoney);
                $("#Payees").val(payeeSelections.ids.join(","));
                $("#Orders").val(orderSelections.ids.join(","));
                
                //$(this).steps("next");
            }
            if (currentIndex === 1 && isPayeeSelect) {
                $("#payeetable").bootstrapTable('refresh');
                isPayeeSelect = false;
            }
            if (currentIndex === 2 && isOrderSelect) {
                $("#ordertable").bootstrapTable('refresh');
                isOrderSelect = false;
            }
            //// Suppress (skip) "Warning" step if the user is old enough and wants to the previous step.
            //if (currentIndex === 2 && priorIndex === 3) {
            //    $(this).steps("previous");
            //}

        },
        onFinishing: function (event, currentIndex) {
            var form = $(this);

            // Disable validation on fields that are disabled.
            // At this point it's recommended to do an overall check (mean ignoring only disabled fields)
            form.validate().settings.ignore = ":disabled";

            // Start validation; Prevent form submission if false
            return form.valid();
        },
        onFinished: function (event, currentIndex) {
            var form = $(this);
            $('.wrapper.wrapper-content .ibox').children('.ibox-content').toggleClass('sk-loading');
            // Submit form input
            form.submit();
        }
    }).validate({
        errorPlacement: function (error, element) {
            element.before(error);
        },
        rules: {
            PayeeMoney: {
                equalTo: "#OrderMoney",
                min: 0.01
            }
        },
        messages: {
            PayeeMoney: {
                equalTo: "核销金额不一致",
                min: "核销金额须大于0"
            }
        }

    });
});
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
//领款记录
function initPayee() {
    $payeeTable.bootstrapTable({
        classes: "table table-no-bordered",
        url: payeeapi,         //请求后台的URL（*）
        height: 360,
        striped: true,                      //是否显示行间隔色
        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
        sortable: true,                     //是否启用排序
        sortOrder: "desc",                   //排序方式
        sortName: "Id",
        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
        pagination: true,
        pageNumber: 1,                       //初始化加载第一页，默认第一页
        pageSize: 7,                       //每页的记录行数（*）
        pageList: [7, 15, 50],
        clickToSelect: true,                //是否启用点击选中行
        //singleSelect: true,                  //设置True 将禁止多选
        uniqueId: "Id",                     //每一行的唯一标识，一般为主键列
        mobileResponsive: true,
        queryParams: function (parameters) {
            parameters.LinkManId = $("#LinkManId").val();
            parameters.VerificationStatus = 0;
            return parameters;
        },
        responseHandler: payeeResponseHandler,
        columns: [
            {
                field: 'state',
                checkbox: true
            },
            {
                field: 'LinkManName',
                title: '客户名称',
                align: "center", valign: "middle"
            },
            {
                field: 'VerificationMoney',
                title: '未核销金额',
                align: "center", valign: "middle"
            },
            {
                field: 'ClaimDate',
                title: '领款日期',
                align: "center", valign: "middle",
                formatter: function (value) {
                    return moment(value).format("YYYY-MM-DD");
                }
            }
        ]
    });
    checkOn($payeeTable, payeeSelections);
}
//订单列表
function initOrder() {
    $orderTable.bootstrapTable({
        classes: "table table-no-bordered",
        url: orderapi,         //请求后台的URL（*）
        height: 360,
        striped: true,                      //是否显示行间隔色
        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
        sortable: true,                     //是否启用排序
        sortOrder: "desc",                   //排序方式
        sortName: "Id",
        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
        pagination: true,
        pageNumber: 1,                       //初始化加载第一页，默认第一页
        pageSize: 7,                       //每页的记录行数（*）
        pageList: [7, 15, 50],
        clickToSelect: true,                //是否启用点击选中行
        //singleSelect: true,                  //设置True 将禁止多选
        uniqueId: "Id",                     //每一行的唯一标识，一般为主键列
        mobileResponsive: true,
        queryParams: function (parameters) {
            parameters.OrderNum = $("#OrderNum").val();
            parameters.VerificationStatus = 0;
            parameters.Status = 1;
            return parameters;
        },
        responseHandler: orderResponseHandler,
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
                field: 'Money',
                title: '销售金额',
                align: "center", valign: "middle"
            }
            ,
            {
                field: 'VerificationMoney',
                title: '未核销金额',
                align: "center", valign: "middle"
            }
        ]
    });
    checkOn($orderTable, orderSelections);
}
//保留选中结果
function payeeResponseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.Id, payeeSelections.ids) !== -1;
    });
    return res;
}
//保留选中结果
function orderResponseHandler(res) {
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.Id, orderSelections.ids) !== -1;
    });
    return res;
}

//注册选中事件
function checkOn($table, selections) {
    $table.on('check.bs.table check-all.bs.table ' +
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
}
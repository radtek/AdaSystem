
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
function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
}
var formatter = {};

formatter.mediaLogo = function (value, row) {
    if (!value) {
        value = "/Images/medialogo/" + row.MediaTypeIndex + ".jpg";
    }
    var logo = '<img alt="' +
        row.MediaName +
        '" class="img-circle" src="' +
        value +
        '" style="width: 60px; height: 60px;"/>';

    var arr = [];
    //if (row.IsTop) {
    //    arr.push('<span class="badge badge-success" data-toggle="tooltip" data-placement="bottom" title="Tooltip on bottom">置顶</span>');
    //}
    if (row.IsHot) {
        arr.push('<span class="badge badge-warning" data-toggle="tooltip" data-placement="bottom" title="对接广告主，在沟通，速度，效果方面表现优异">优质</span>');
    }
    if (row.IsRecommend) {
        arr.push('<span class="badge badge-danger" data-toggle="tooltip" data-placement="bottom" title="广告主的人气和关注度高">推荐</span>');
    }
    var vg = '';
    if (arr.length > 0) {
        vg = "<div class='p-xxs text-center'>" + arr.join(' ') + "</div>";
    }
    return '<div class="p-xxs text-center">' + logo + '</div>' + vg;
};
formatter.mediaInfo = function (value, row) {
    var sex = "";
    if (row.Sex == '男') {
        sex = '<img alt="男" class="img-circle" src="/Images/man.png" style="width: 25px; height: 25px;"/> ';
    }
    if (row.Sex == '女') {
        sex = '<img alt="女" class="img-circle" src="/Images/lady.png" style="width: 25px; height: 25px;"/> ';
    }
    var area = "";
    if (row.Areas) {
        area = " <span class='btn btn-info btn-outline btn-xs'><i class='fa fa-map-marker'></i> " + row.Areas + "</span>";
    }
    var fans = "<span class='btn btn-danger btn-xs btn-outline'><i class='fa fa-users'></i> 粉丝：" + row.FansNum.toFixed(1) + " 万</span>";
    var tags = "";
    if (row.MediaTags) {
        var arr = [];
        $.each(row.MediaTags,
            function (k, v) {
                arr.push("<span class='btn btn-success btn-xs btn-outline'><i class='fa fa-tag'></i> " + v.TagName + "</span>");
            });
        tags = arr.join(' ');
    }
    var weixinid = "";
    if (row.MediaTypeIndex == "weixin") {
        weixinid = "<span class='btn btn-warning btn-xs btn-outline'><i class='fa fa-weixin'></i> 微信号：" + row.MediaID + " </span>";
    }
    var line1 = "<div class='p-xxs'>" + sex + value + "</div>";
    var line2 = weixinid ? "<div class='p-xxs'>" + weixinid + "</div>" : "";
    var line3 = area ? "<div class='p-xxs'>" + area + "</div>" : "";
    var line4 = tags ? "<div class='p-xxs'>" + tags + "</div>" : "";
    var line5 = "<div class='p-xxs'>" + fans + "</div>";
    return line1 + line2 + line3 + line4 + line5;
};
formatter.mediaPrice = function (value, row) {
    var arr = [];
    $.each(value,
        function (k, v) {
            var price = v.PurchasePrice == 0 ? "不接单" : "<i class='fa fa-jpy'></i> " + v.PurchasePrice;
            arr.push("<li class='list-group-item p-xxs'><span class='badge badge-success'>" + price + "</span> " + v.AdPositionName + "</li>");
        });
    return "<ul class='list-group m-b-none'>" + arr.join('') + "</ul>";
};
formatter.mediaData = function (value, row) {

    var read = "";
    if (row.AvgReadNum) {
        read = " <span class='label label-info'>平均浏览数：" + row.AvgReadNum + "</span>";
    }
    var lastread = "";
    if (row.LastReadNum) {
        lastread = " <span class='label label-info'>最近浏览数：" + row.LastReadNum + "</span>";
    }
    var comment = "";
    if (row.CommentNum) {
        comment = " <span class='label label-info'>平均评论数：" + row.CommentNum + "</span>";
    }
    var like = "";
    if (row.LikesNum) {
        like = " <span class='label label-info'>平均点赞数：" + row.LikesNum + "</span>";
    }
    var avgpost = "";
    if (row.MonthPostNum) {
        avgpost = " <span class='label label-info'>平均发文数：" + row.MonthPostNum + "</span>";
    }
    var post = "";
    if (row.PostNum) {
        post = " <span class='label label-info'>发布总数：" + row.PostNum + "</span>";
    }
    var friend = "";
    if (row.FriendNum) {
        friend = " <span class='label label-info'>关注数：" + row.FriendNum + "</span>";
    }
    var frequency = "";
    if (row.PublishFrequency) {
        frequency = " <span class='label label-info'>月发布频次：" + row.PublishFrequency + "</span>";
    }
    var share = "";
    if (row.TransmitNum) {
        share = " <span class='label label-info'>平均分享数：" + row.TransmitNum + "</span>";
    }
    var lastdate = "";
    if (row.LastPushDate) {
        var date = moment(row.LastPushDate).format("YYYY-MM-DD HH:mm");
        lastdate = " <span class='label label-info'>最近更新日期：" + date + "</span>";
    }
    var line1 = read ? "<div class='p-xxs'>" + read + "</div>" : "";
    var line2 = like ? "<div class='p-xxs'>" + like + "</div>" : "";
    var line3 = comment ? "<div class='p-xxs'>" + comment + "</div>" : "";
    var line4 = avgpost ? "<div class='p-xxs'>" + avgpost + "</div>" : "";
    var line5 = post ? "<div class='p-xxs'>" + post + "</div>" : "";
    var line6 = lastread ? "<div class='p-xxs'>" + lastread + "</div>" : "";
    var line7 = lastdate ? "<div class='p-xxs'>" + lastdate + "</div>" : "";
    var line8 = friend ? "<div class='p-xxs'>" + friend + "</div>" : "";
    var line9 = frequency ? "<div class='p-xxs'>" + frequency + "</div>" : "";
    var line10 = share ? "<div class='p-xxs'>" + share + "</div>" : "";
    var line = "";
    if (row.MediaTypeIndex == "weixin") {
        line = line1 + line6 + line4 + line9+line7;
    }
    if (row.MediaTypeIndex == "sinablog") {
        line = line2 + line3 + line10 + line5 + line7;
    }
    if (row.MediaTypeIndex == "douyin") {
        line = line1 + line2 + line3 + line10 + line8 + line5;
    }
    return line || "<span class='label label-warning'>抱歉！暂无相关数据</span>";
};

formatter.mediaOperation = function(value, row) {

    return "<div class='p-xxs'><div class='btn-group'>" +
        "<a class='btn btn-danger btn-outline btn-sm' href='/Resource/Media/Update/" +
        value +
        "');' target='_blank'><i class='fa fa-info-circle'></i> 查看详情</a> " +
        "<button class='btn btn-danger btn-outline btn-sm' onclick=\"operation.cancle('" +
        value +
        "');\"><i class='fa fa-cart-arrow-down'></i> 加入分组</button>" +
        "</div></div>";
};













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
    if (value == 1) {
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
function isArray(o) {
    return Object.prototype.toString.call(o) == '[object Array]';
}

function initTooltip() {
    $('[data-toggle="tooltip"]').tooltip();
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
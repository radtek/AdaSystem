var timeoutId = 0;
var filterFormat = {};


filterFormat.mediaLogo = function (value, row) {
    if (!value) {
        value = "/Images/medialogo/" + row.MediaTypeIndex + ".jpg";
    }
    var logo = '<img alt="' +
        row.MediaName +
        '" class="img-circle" src="' +
        value +
        '" style="width: 60px; height: 60px;"/>';

    var arr = [];
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
    var detail = "";
    if (row.MediaTypeIndex == "weixin" || row.MediaTypeIndex == "douyin" || row.MediaTypeIndex == "sinablog" || row.MediaTypeIndex == "redbook") {
        detail = "<a class='btn btn-warning btn-outline btn-xs' href='/Resource/Media/Detail/" + row.Id + "' target='_blank'><i class='fa fa-info-circle'></i> 详情</a>";
    }
    var line = detail ? "<div class='p-xxs text-center'>" + detail + "</div>" : "";
    var comment = "<div class='p-xxs text-center'><a class='btn btn-info btn-outline btn-xs' href='/Resource/Media/Comment/" + row.Id + "' target='_blank'><i class='fa fa-comment'></i> 评价</a></div>";
    return '<div class="p-xxs text-center">' + logo + '</div>' + vg + line + comment;
};
filterFormat.mediaInfo = function (value, row) {
    var sex = "";
    if (row.Sex == '男') {
        sex = '<img alt="男" class="img-circle" src="/Images/man.png" style="width: 25px; height: 25px;"/> ';
    }
    if (row.Sex == '女') {
        sex = '<img alt="女" class="img-circle" src="/Images/lady.png" style="width: 25px; height: 25px;"/> ';
    }
    var isAuth = "";
    if (row.IsAuthenticate == true) {
        isAuth = " <span class='label label-primary'>证</span> ";
    }
    var aType = "";
    if (row.AuthenticateType == "黄V") {
        aType = " <span class='label label-warning'> " + row.AuthenticateType + "</span> ";
    } else if (row.AuthenticateType == "蓝V") {
        aType = " <span class='label label-success'> " + row.AuthenticateType + "</span> ";
    } else if (row.AuthenticateType == "达人") {
        aType = " <span class='label label-danger'><i class='fa fa-star-o'></i> " + row.AuthenticateType + "</span> ";
    } else {
        if (row.AuthenticateType) {
            aType = " <span class='label label-info'> " + row.AuthenticateType + "</span> ";
        }

    }
    var area = "";
    if (row.Areas) {
        area = " <span class='btn btn-info btn-outline btn-xs'><i class='fa fa-map-marker'></i> " + row.Areas + "</span>";
    }
    var platform = "";
    if (row.Platform) {
        platform = " <span class='btn btn-info btn-outline btn-xs'><i class='fa fa-star-o'></i> 平台：" + row.Platform + "</span>";
    }
    var fans = "<span class='btn btn-danger btn-xs btn-outline'><i class='fa fa-users'></i> 粉丝：" + (row.FansNum ? row.FansNum.toFixed(1) + " 万" : "不详") + "</span>";
    var tags = "";
    if (row.MediaTags) {
        var arr = [];
        $.each(row.MediaTags,
            function (k, v) {
                if (((k + 1) % 3) == 0) {
                    arr.push("<span class='btn btn-success btn-xs btn-outline'><i class='fa fa-tag'></i> " + v.TagName + "</span><br />");
                } else {
                    arr.push("<span class='btn btn-success btn-xs btn-outline'><i class='fa fa-tag'></i> " + v.TagName + "</span>");
                }

            });
        tags = arr.join(' ');
    }
    var weixinid = "";
    if (row.MediaTypeIndex == "weixin") {
        var copywxId =
            "<button type='button' class='btn btn-white btn-xs copyid' data-toggle='tooltip' title='复制到剪切板' data-clipboard-target='#" +
            row.MediaID +
                "'><i class='fa fa-copy'></i></button>";
        weixinid = "<span class='label label-warning'><i class='fa fa-weixin'></i> 微信号：<span id='" + row.MediaID + "'>" + row.MediaID + "</span></span> " + copywxId;
    }
    var copyName =
        " <button type='button' class='btn btn-white btn-xs copyid m-l-xs' data-toggle='tooltip' title='复制到剪切板' data-clipboard-target='#" +
            row.Id +
        "'><i class='fa fa-copy'></i></button>";
    var tempValue = "<span id='" + row.Id + "'>" + value + "</span> ";
    var mediaName = "";
    if (row.MediaTypeIndex == "weixin") {
        mediaName = "<a class='label' href='http://weixin.sogou.com/weixin?type=1&query=" + row.MediaID + "' target='_blank'><i class='fa fa-link'></i> " + tempValue + "</a> " + copyName;
    } else {
        if (row.MediaLink) {
            mediaName = " <a class='label' href='" + row.MediaLink + "' target='_blank'><i class='fa fa-link'></i> " + tempValue + "</a> " + copyName;
        } else {
            mediaName = tempValue + copyName;
        }
    }

    var line1 = "<div class='p-xxs'>" + sex + isAuth + mediaName + "</div>";
    var line2 = weixinid ? "<div class='p-xxs'>" + weixinid + "</div>" : "";
    var line3 = area ? "<div class='p-xxs'>" + area + "</div>" : "";
    var line33 = platform ? "<div class='p-xxs'>" + platform + "</div>" : "";
    var line0 = aType ? "<div class='p-xxs'>" + aType + "</div>" : "";
    var line4 = tags ? "<div class='p-xxs'>" + tags + "</div>" : "";
    var line5 = "<div class='p-xxs'>" + fans + "</div>";
    var line6 = row.RetentionTime ? "<div class='p-xxs'><span class='btn btn-info btn-outline btn-xs'><i class='fa fa-clock-o'></i> 保留时长：" + row.RetentionTime + "</span></div>" : "";

    return line1 + line2 + line3 + line33 + line0 + line4 + line5 + line6;
};
filterFormat.mediaPrice = function (value, row) {
    var arr = [];
    $.each(value,
        function (k, v) {
            arr.push("<li class='list-group-item p-xxs'>" +
                "<span class='badge badge-success'>成本 : <i class='fa fa-jpy'></i> " + v.PurchasePrice+"</span> " +
                "<span class='badge badge-danger'>销售 : <i class='fa fa-jpy'></i> " + v.MarketPrice +"</span> " +
                "<span class='badge badge-warning'>零售 : <i class='fa fa-jpy'></i> " + v.SellPrice +"</span> "+
                 v.AdPositionName +
                "</li>");
        });
    arr.push("<li class='list-group-item p-xxs'><span class='badge'>" + moment(value[0].InvalidDate).format("YYYY-MM-DD") + "</span>价格有效期</li>");
    return "<ul class='list-group m-b-none'>" + arr.join('') + "</ul>";
};
filterFormat.priceProtection = function (value, row) {
    if (value == 0 || !value) {
        return "不保价";
    }
    var str = "";
    switch (value) {
        case 1:
            str = "一个月";
            break;
        case 2:
            str = "两个月";
            break;
        case 3:
            str = "三个月";
            break;
        case 6:
            str = "半年";
            break;
    }
    var line = "<li class='list-group-item p-xxs'><span class='badge badge-success'>" +
        str +
        "</span> 保价期</li>";
    line += "<li class='list-group-item p-xxs'><span class='badge badge-success'>" +
        (row.PriceProtectionIsPrePay == true ? "是" : "否") +
        "</span> 是否预付</li>";
    line += "<li class='list-group-item p-xxs'><span class='badge badge-success'>" +
        (row.PriceProtectionIsBrand == true ? "是" : "否") +
        "</span> 品牌提供</li>";
    return "<ul class='list-group'>" + line + "</ul>";
}

filterFormat.mediaData = function (value, row) {
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
        avgpost = " <span class='label label-info'>月发文数：" + row.MonthPostNum + "</span>";
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
        share = " <span class='label label-info'>平均转发数：" + row.TransmitNum + "</span>";
    }
    var lastdate = "";
    if (row.LastPushDate) {
        var date = moment(row.LastPushDate).format("YYYY-MM-DD HH:mm");
        lastdate = " <span class='label label-info'>最近发布日期：" + date + "</span>";
    }
    var dz = "";
    if (row.AvgReadNum) {
        dz = " <span class='label label-info'>平均点赞数：" + row.AvgReadNum + "</span>";
    }
    var sc = "";
    if (row.TransmitNum) {
        sc = " <span class='label label-info'>平均收藏数：" + row.TransmitNum + "</span>";
    }
    var bj = "";
    if (row.PostNum) {
        bj = " <span class='label label-info'>笔记总数：" + row.PostNum + "</span>";
    }
    var zc = "";
    if (row.LikesNum) {
        zc = " <span class='label label-info'>赞与收藏：" + row.LikesNum + "</span>";
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
    var linedz = dz ? "<div class='p-xxs'>" + dz + "</div>" : "";
    var linesc = sc ? "<div class='p-xxs'>" + sc + "</div>" : "";
    var linebj = bj ? "<div class='p-xxs'>" + bj + "</div>" : "";
    var linezc = zc ? "<div class='p-xxs'>" + zc + "</div>" : "";
    var line = "";
    if (row.MediaTypeIndex == "weixin") {
        line = line1 + line6 + line4 + line9 + line7;
    }
    if (row.MediaTypeIndex == "sinablog") {
        line = line2 + line3 + line10 + line5 + line7;
    }
    if (row.MediaTypeIndex == "douyin") {
        line = line2 + line3 + line10 + line5;
    }
    if (row.MediaTypeIndex == "redbook") {
        line = linedz + line3 + linesc + line8 + linezc + linebj;
    }
    return line || "<span class='label label-warning'>抱歉！暂无相关数据</span>";
};



filterFormat.mediaGroup = function (value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<button class='btn btn-warning btn-outline btn-sm' onclick=\"groupDetail('" + v.Id + "');\"><i class='fa fa-object-group'></i> " + v.GroupName + "</button> ";
        });
    return result;
}

function initFilter() {
    $("select").add("input[name='MediaTagIds']").change(function () {
        if ($(this).attr("name") != "MediaTypeId") {
            searchTable();
        }

    });
    $('#searchModal').on('shown.bs.modal', function () {
        $('#resultModal .modal-body').empty();
    }).on('hidden.bs.modal', function () {

    });
}

function resetFilter() {
    $("form")[0].reset();
    $("#MediaBatch").val("");
    $("#table").bootstrapTable("refresh", { query: { search: '' } });

}
function searchTable() {
    timeoutId = setTimeout(function () {
        $("#table").bootstrapTable("refresh");
    }, 500);
}

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

function onLoadSuccess(data) {
    if ($("#MediaBatch").val()) {
        if (data.no.length > 0) {
            //$.each(data.no,
            //    function (k, v) {
            //        $('#resultModal .modal-body').append('<div class="checkbox checkbox-success checkbox-inline">' +
            //            '<input type = "checkbox" id = "inlineCheckbox' + k + '" name = "Develop" value = "' + v + '" checked = "" >' +
            //            '<label for="inlineCheckbox' + k + '"> ' + v + ' </label>' +
            //            '</div>');
                    
            //    });
            $('#resultModal .modal-body').html(data.no.join(','));
            $('#resultModal').modal('show');
        }
    }
    new Clipboard('.copyid');
    $('[data-toggle="tooltip"]').tooltip();
}

function serachFilter(parameters) {
    var mediaBatch = $("#MediaBatch").val();
    if (mediaBatch) {
        parameters.MediaTypeId = $("#MediaTypeId").val();
        parameters.MediaBatch = mediaBatch;
        return parameters;
    }
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
    $('#toolbar').find('input[name],select[name]').each(function () {
        parameters[$(this).attr('name')] = $(this).val();
    });
    return parameters;
}
function exportFile() {
    if (!$("#MediaTypeId").val()) {
        swal("提醒", "请选择媒体类型！", "warning");
        $('#exportModal').modal('hide');
        return;
    }
    var chkValue = [];
    $('#exportModal input:checkbox:checked').each(function () {
        chkValue.push($(this).val());
    });
    var parameters = {};
    parameters.MediaTypeId = $("#MediaTypeId").val();
    parameters.PriceType = chkValue.join(',');
    parameters.search = $("#table").bootstrapTable('getOptions').searchText;
    parameters = serachFilter(parameters);
    var subBtn = $('.ladda-button').ladda();
    $.ajax({
        type: "post",
        headers: {
            '__RequestVerificationToken': $("input[name='__RequestVerificationToken']").val()
        },
        url: "/Resource/Media/Exports",
        data: parameters,
        success: function (data) {
            if (data.State == 1) {
                $('#exportModal').modal('hide');
                swal({
                    title: "导出成功",
                    text: "正在下载中...",
                    timer: 2000,
                    showConfirmButton: false
                });
                window.location.href = "/Resource/Media/Download?file=" + data.Msg;
            } else {
                swal("消息", data.Msg, "warning");
            }
        },
        error: function () {
            swal("错误", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}
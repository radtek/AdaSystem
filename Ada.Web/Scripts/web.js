﻿var timeoutId = 0;
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
}
$.fn.labelauty = function (options) {
    /*
     * Our default settings
     * Hope you don't need to change anything, with these settings
     */
    var settings = $.extend(
        {
            // Development Mode
            // This will activate console debug messages
            development: false,

            // Trigger Class
            // This class will be used to apply styles
            class: "labelauty",

            // Use text label ?
            // If false, then only an icon represents the input
            label: true,

            // Separator between labels' messages
            // If you use this separator for anything, choose a new one
            separator: "|",

            // Default Checked Message
            // This message will be visible when input is checked
            checked_label: "Checked",

            // Default UnChecked Message
            // This message will be visible when input is unchecked
            unchecked_label: "Unchecked",

            // Minimum Label Width
            // This value will be used to apply a minimum width to the text labels
            minimum_width: false,

            // Use the greatest width between two text labels ?
            // If this has a true value, then label width will be the greatest between labels
            same_width: true
        }, options);

    /*
     * Let's create the core function
     * It will try to cover all settings and mistakes of using
     */
    return this.each(function () {
        var $object = $(this);
        var use_labels = true;
        var labels;
        var labels_object;
        var input_id;

        // Test if object is a check input
        // Don't mess me up, come on
        if ($object.is(":checkbox") === false && $object.is(":radio") === false)
            return this;

        // Add "labelauty" class to all checkboxes
        // So you can apply some custom styles
        $object.addClass(settings.class);

        // Get the value of "data-labelauty" attribute
        // Then, we have the labels for each case (or not, as we will see)
        labels = $object.attr("data-labelauty");

        use_labels = settings.label;

        // It's time to check if it's going to the right way
        // Null values, more labels than expected or no labels will be handled here
        if (use_labels === true) {
            if (labels == null || labels.length === 0) {
                // If attribute has no label and we want to use, then use the default labels
                labels_object = new Array();
                labels_object[0] = settings.unchecked_label;
                labels_object[1] = settings.checked_label;
            }
            else {
                // Ok, ok, it's time to split Checked and Unchecked labels
                // We split, by the "settings.separator" option
                labels_object = labels.split(settings.separator);

                // Now, let's check if exist _only_ two labels
                // If there's more than two, then we do not use labels :(
                // Else, do some additional tests
                if (labels_object.length > 2) {
                    use_labels = false;
                    debug(settings.development, "There's more than two labels. LABELAUTY will not use labels.");
                }
                else {
                    // If there's just one label (no split by "settings.separator"), it will be used for both cases
                    // Here, we have the possibility of use the same label for both cases
                    if (labels_object.length === 1)
                        debug(settings.development, "There's just one label. LABELAUTY will use this one for both cases.");
                }
            }
        }

        /*
         * Let's begin the beauty
         */

        // Start hiding ugly checkboxes
        // Obviously, we don't need native checkboxes :O
        $object.css({ display: "none" });

        // We don't need more data-labelauty attributes!
        // Ok, ok, it's just for beauty improvement
        $object.removeAttr("data-labelauty");

        // Now, grab checkbox ID Attribute for "label" tag use
        // If there's no ID Attribute, then generate a new one
        input_id = $object.attr("id");

        if (input_id == null) {
            var input_id_number = 1 + Math.floor(Math.random() * 1024000);
            input_id = "labelauty-" + input_id_number;

            // Is there any element with this random ID ?
            // If exists, then increment until get an unused ID
            while ($(input_id).length !== 0) {
                input_id_number++;
                input_id = "labelauty-" + input_id_number;
                debug(settings.development, "Holy crap, between 1024 thousand numbers, one raised a conflict. Trying again.");
            }

            $object.attr("id", input_id);
        }

        // Now, add necessary tags to make this work
        // Here, we're going to test some control variables and act properly
        $object.after(create(input_id, labels_object, use_labels));

        // Now, add "min-width" to label
        // Let's say the truth, a fixed width is more beautiful than a variable width
        if (settings.minimum_width !== false)
            $object.next("label[for=" + input_id + "]").css({ "min-width": settings.minimum_width });

        // Now, add "min-width" to label
        // Let's say the truth, a fixed width is more beautiful than a variable width
        if (settings.same_width != false && settings.label == true) {
            var label_object = $object.next("label[for=" + input_id + "]");
            var unchecked_width = getRealWidth(label_object.find("span.labelauty-unchecked"));
            var checked_width = getRealWidth(label_object.find("span.labelauty-checked"));

            if (unchecked_width > checked_width)
                label_object.find("span.labelauty-checked").width(unchecked_width);
            else
                label_object.find("span.labelauty-unchecked").width(checked_width);
        }
    });
}

/*
 * Tricky code to work with hidden elements, like tabs.
 * Note: This code is based on jquery.actual plugin.
 * https://github.com/dreamerslab/jquery.actual
 */
function getRealWidth(element) {
    var width = 0;
    var $target = element;
    var style = 'position: absolute !important; top: -1000 !important; ';

    $target = $target.clone().attr('style', style).appendTo('body');
    width = $target.width(true);
    $target.remove();

    return width;
}

function debug(debug, message) {
    if (debug && window.console && window.console.log)
        window.console.log("jQuery-LABELAUTY: " + message);
}

function create(input_id, messages_object, label) {
    var block;
    var unchecked_message;
    var checked_message;

    if (messages_object == null)
        unchecked_message = checked_message = "";
    else {
        unchecked_message = messages_object[0];

        // If checked message is null, then put the same text of unchecked message
        if (messages_object[1] == null)
            checked_message = unchecked_message;
        else
            checked_message = messages_object[1];
    }

    if (label == true) {
        block = '<label for="' + input_id + '">' +
            '<span class="labelauty-unchecked-image"></span>' +
            '<span class="labelauty-unchecked">' + unchecked_message + '</span>' +
            '<span class="labelauty-checked-image"></span>' +
            '<span class="labelauty-checked">' + checked_message + '</span>' +
            '</label>';
    }
    else {
        block = '<label for="' + input_id + '">' +
            '<span class="labelauty-unchecked-image"></span>' +
            '<span class="labelauty-checked-image"></span>' +
            '</label>';
    }

    return block;
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
    var mediaBatch = $("#MediaBatch").val();
    if (mediaBatch) {
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
function onLoadSuccess(data) {
    if ($("#MediaBatch").val()) {
        if (data.no.length > 0) {
            $.each(data.no,
                function (k, v) {
                    $('#resultModal .modal-body').append('<div class="checkbox checkbox-success checkbox-inline">' +
                        '<input type = "checkbox" id = "inlineCheckbox' + k + '" name = "Develop" value = "' + v + '" checked = "" >' +
                        '<label for="inlineCheckbox' + k + '"> ' + v + ' </label>' +
                        '</div>');
                });

            $('#resultModal').modal('show');
        }
    }
    initTooltip();
}

function initFilter() {
    $('#searchModal').on('shown.bs.modal', function () {
        $("#MediaBatch").val("");
        $('#resultModal .modal-body').empty();
    }).on('hidden.bs.modal', function () {

    });
    $(':input:radio').labelauty();
    $("input[name='radio']").change(function () {
        var arr = $(this).val().split('-');
        $("#FansNumStart").val(arr[0]);
        $("#FansNumEnd").val(arr[1]);
        //addFilter("fans", "粉丝数", $(this).data("str"));
        searchTable();
    });
    $("input[name='priceradio']").change(function () {
        var arr = $(this).val().split('-');
        $("#PriceStart").val(arr[0]);
        $("#PriceEnd").val(arr[1]);
        searchTable();
    });
    $("select").add("input[name='MediaTagIds']").add("input[name='Platform']").change(function () {
        searchTable();
    });
}

function addFilter(id, name, value) {
    $.each($(".filters .btn"),
        function (k, v) {
            var btnData = $(v).data("name");
            if (btnData == name) {
                $(v).remove();
            }
        });
    $(".filters").append('<button type="button" data-parent="' + id + '" data-name="' + name + '" class="btn btn-warning btn-sm btn-outline" onclick="resetCurrent(this);">' + name + "：" + value +
        ' <i class="fa fa-close"></i>' +
        '</button >');

}

function resetFilter() {
    $("form")[0].reset();
    $("#MediaBatch").val("");
    $("#table").bootstrapTable("refresh");

}
function resetCurrent(o) {
    var parent = $(o).data("parent");
    $('#' + parent + ' input').val("");
    $('#' + parent + ' input:radio').attr('checked', false);
    $(o).remove();
}

function searchTable() {
    timeoutId = setTimeout(function () {
        $("#table").bootstrapTable("refresh");
    }, 500);
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
    var detail = "";
    if (row.MediaTypeIndex == "weixin" || row.MediaTypeIndex == "douyin" || row.MediaTypeIndex == "sinablog" || row.MediaTypeIndex == "redbook") {
        detail = "<a class='btn btn-warning btn-outline btn-xs' href='/Media/Detail/" + row.Id + "' target='_blank'><i class='fa fa-info-circle'></i> 查看详情</a>";
    }
    var line = detail ? "<div class='p-xxs text-center'>" + detail + "</div>" : "";
    
    var comment = "";
    if (row.IsComment) {
        comment = "<div class='p-xxs text-center'><a class='btn btn-danger btn-outline btn-xs' href='/Media/CommentDetail/" + row.Id + "' target='_blank'><i class='fa fa-comment'></i> 评价详情</a></div>";
    }
    return '<div class="p-xxs text-center">' + logo + '</div>' + vg + line + comment;
};
formatter.mediaInfo = function (value, row) {
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
    var fans = "";
    if (row.FansNum) {
        fans = "<span class='btn btn-danger btn-xs btn-outline'><i class='fa fa-users'></i> 粉丝：" + row.FansNum.toFixed(1) + " 万</span>";
    }
        
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
        weixinid = "<span class='label label-warning'><i class='fa fa-weixin'></i> 微信号：" + row.MediaID + " </span>";
    }
    if (row.MediaTypeIndex == "weixin") {
        value = " <a class='label' href='http://weixin.sogou.com/weixin?type=1&query=" + row.MediaID + "' target='_blank'><i class='fa fa-link'></i> " + value + "</a>";
    } else if (row.MediaTypeIndex == "taobao") {
        value = " <a class='label' href='https://market.m.taobao.com/apps/abs/9/41/index?accountId=" + row.MediaID + "' target='_blank'><i class='fa fa-link'></i> " + value + "</a>";
    } else {
        if (row.MediaLink) {
            value = " <a class='label' href='" + row.MediaLink + "' target='_blank'><i class='fa fa-link'></i> " + value + "</a>";
        }
    }
    var remark = "";
    if (row.MediaTypeIndex == "writer") {
        var temp = row.Content ? row.Content : "暂无相关简介";
        remark = '<span class="btn btn-info btn-xs btn-outline" data-toggle="tooltip" data-placement="bottom" title="' + temp+'">' +
            '<i class="fa fa-address-card-o"></i> 文案简介</span>';
    }
    var article = "";
    if (row.MediaTypeIndex == "writer") {
        article = "<a class='btn btn-danger btn-xs btn-outline' href='javascript:;' onclick=\"articleDetails('" +
            row.Id +
            "');\");'><i class='fa fa-newspaper-o'></i> 查看案例</a> ";
    }
    var line1 = "<div class='p-xxs'>" + sex + isAuth + value + "</div>";
    var line2 = weixinid ? "<div class='p-xxs'>" + weixinid + "</div>" : "";
    var line3 = area ? "<div class='p-xxs'>" + area + "</div>" : "";
    var line0 = aType ? "<div class='p-xxs'>" + aType + "</div>" : "";
    var line4 = tags ? "<div class='p-xxs'>" + tags + "</div>" : "";
    var line5 = fans ? "<div class='p-xxs'>" + fans + "</div>" : "";
    var line6 = remark ? "<div class='p-xxs'>" + remark + "</div>" : "";
    var line7 = article ? "<div class='p-xxs'>" + article + "</div>" : "";
    return line1 + line2 + line3 + line0 + line4 + line5 + line6 + line7;
};
formatter.mediaPrice = function (value, row) {
    var arr = [];
    $.each(value,
        function (k, v) {
            if (v.SellPrice) {
                var price =  "<span class='badge badge-success'>税前 : <i class='fa fa-jpy'></i> " + v.SellPrice + "</span> ";
                var tax ="<span class='badge badge-warning'>税后 : <i class='fa fa-jpy'></i> " + v.SellPrice * 1.06 + "</span> ";
                arr.push("<li class='list-group-item p-xxs'>" + tax + price + v.AdPositionName + "</li>");
            }
            
        });
    arr.push("<li class='list-group-item p-xxs'><span class='badge'>" + moment(value[0].InvalidDate).format("YYYY-MM-DD") + "</span>价格有效期</li>");
    arr.push("<li class='list-group-item p-xxs'><span class='badge badge-info' onclick=\"mediaPrice('" + row.Id + "');\">点击查看</span>价格走势</li>");
    return "<ul class='list-group m-b-none'>" + arr.join('') + "</ul>";
};
formatter.priceProtection = function (value, row) {
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

formatter.mediaData = function (value, row) {

    var read = "", tbzh = "",bread="";
    if (row.AvgReadNum) {
        read = " <span class='label label-info'>平均浏览数：" + row.AvgReadNum + "</span>";
        tbzh = " <span class='label label-info'>综合能力指数：" + row.AvgReadNum + "</span>";
        bread = " <span class='label label-info'>阅读数：" + row.AvgReadNum + "</span>";
    }

    var lastread = "";
    if (row.LastReadNum) {
        lastread = " <span class='label label-info'>最近浏览数：" + row.LastReadNum + "</span>";
    }
    var comment = "";
    if (row.CommentNum) {
        comment = " <span class='label label-info'>平均评论数：" + row.CommentNum + "</span>";
    }
    var like = "",blike="";
    if (row.LikesNum) {
        like = " <span class='label label-info'>平均点赞数：" + row.LikesNum + "</span>";
        blike = " <span class='label label-info'>播放数：" + row.LikesNum + "</span>";
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
    var sclx = "";
    if (row.ResourceType) {
        sclx = " <span class='label label-info'>擅长类型：" + row.ResourceType + "</span>";
    }
    var cgsd = "";
    if (row.Efficiency) {
        cgsd = " <span class='label label-info'>出稿速度：" + row.Efficiency + "</span>";
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
    var linesclx = sclx ? "<div class='p-xxs'>" + sclx + "</div>" : "";
    var linecgsd = cgsd ? "<div class='p-xxs'>" + cgsd + "</div>" : "";
    var linetbzh = tbzh ? "<div class='p-xxs'>" + tbzh + "</div>" : "";
    var linetbread = bread ? "<div class='p-xxs'>" + bread + "</div>" : "";
    var linetblike = blike ? "<div class='p-xxs'>" + blike + "</div>" : "";

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
    if (row.MediaTypeIndex == "writer") {
        line = linesclx + linecgsd;
    }
    if (row.MediaTypeIndex == "taobao") {
        line = linetbzh;
    }
    if (row.MediaTypeIndex == "bilibili") {
        line = line8 + linetblike + linetbread;
    }
    if (row.MediaTypeIndex == "toutiao") {
        line = line8 + line1 + line3 + line7;
    }
    return line || "<span class='label label-warning'>抱歉！暂无相关数据</span>";
};

formatter.mediaOperation = function (value, row) {
    var title = '加入分组', css = 'btn-outline';
    if (row.IsGroup) {
        title = '重新分组', css = "";
    }
    return "<div class='p-xxs'><div class='btn-group'>" +
        "<button class='btn btn-danger btn-sm " + css + "' onclick=\"showGroup('" + row.MediaName + "','" + row.Id + "');\"><i class='fa fa-star-o'></i> " + title + "</button>" +
        "</div></div>";
};

formatter.mediaGroup = function (value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<button class='btn btn-warning btn-outline btn-sm' onclick=\"groupDetail('" + v.Id + "');\"><i class='fa fa-object-group'></i> " + v.GroupName + "</button> ";
        });
    return result;
}

formatter.groupOperation = function (value, row) {
    return "<div class='p-xxs'><div class='btn-group'>" +
        "<button class='btn btn-danger btn-outline btn-sm' onclick=\"showGroup('" + row.MediaName + "','" + row.Id + "');\"><i class='fa fa-star-o'></i> 重新分组</button>" +
        "<button class='btn btn-danger btn-outline btn-sm' onclick=\"removeMedia('" + row.MediaName + "','" + row.Id + "');\"><i class='fa fa-trash-o'></i> 移出分组</button>" +
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
    var parameters = {};
    var arr = [], checkeds = $("[name='ExcelField']:checked");
    $.each(checkeds, function () {
        arr.push($(this).val());
    });
    parameters.MediaTypeId = $("#MediaTypeId").val();
    parameters.MediaTypeIndex = $("#MediaTypeIndex").val();
    parameters.ExcelField = arr.join(',');
    parameters.search = $("#table").bootstrapTable('getOptions').searchText;
    parameters = searchFrm.queryParams(parameters);
    var subBtn = $('.ladda-button').ladda();
    $.ajax({
        type: "post",
        url: "/Media/Export",
        data: addAntiForgeryToken(parameters),
        success: function (data) {
            if (data.State == 1) {
                $('#exportModal').modal('hide');
                swal({
                    title: "导出成功",
                    text: "正在下载中...",
                    timer: 2000,
                    showConfirmButton: false
                });
                window.location.href = "/Media/Download?file=" + data.Msg;
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

function todo() {
    swal("消息", "敬请期待！", "warning");
}
function initScroll(el, callback) {
    var contentListHeight = $("#" + el + " .feed-activity-list").height();
    var divHeight = $(this).height();
    if (contentListHeight < divHeight) {
        $("#" + el + " .loading").html("没有更多评论了");
    } else {
        var timeoutId = 0;
        $("#" + el).scroll(
            function () {
                var currentScrollTop = $(this).scrollTop();
                contentListHeight = $("#" + el + " .feed-activity-list").height();
                divHeight = $(this).height();
                if (currentScrollTop >= contentListHeight - divHeight) {
                    clearTimeout(timeoutId); // doesn't matter if it's 0
                    timeoutId = setTimeout(function () {
                        callback();
                    }, 600);
                }
            });
    }

}
function nextPage(el, url, offset) {
    var json = { MediaId: $("#MediaId").val(), offset: offset, limit: $("#PageSize").val() };
    $.ajax({
        type: "post",
        url: url,
        data: addAntiForgeryToken(json),
        success: function (data) {
            $.each(data.rows,
                function (k, v) {
                    $("#" + el + " .feed-activity-list").append('<div class="feed-element">' +
                        '<div class="media-body">' +
                        '<small class="pull-right">' + moment(v.CommentDate).locale('zh-cn').startOf('hour').fromNow() +
                        '</small>' +
                        '<strong>' + v.Transactor +
                        '</strong>' +
                        '<br>' +
                        '<small>' + starHtml(v.Score) +
                        '</small>' +
                        '<div class="well">' + v.Content +
                        '</div>' +
                        '</div>' +
                        '</div>');
                });

        },
        error: function () {

        },
        beforeSend: function () {

        },
        complete: function () {

        }
    });
}

function starHtml(score) {
    var html = "";
    for (var i = 0; i < score; i++) {
        html += '<i class="fa fa-star text-danger"></i>';
    }
    for (var j = 0; j < 5 - score; j++) {
        html += '<i class="fa fa-star text-muted"></i>';
    }
    return html;
}

function develop() {
    var arr = [], checkeds = $("[name='Develop']:checked");
    $.each(checkeds, function () {
        arr.push($(this).val());
    });

    if (arr.length <= 0) {
        swal("消息", "请选择要申请开发的媒体", "warning");
        return;
    }
    var subBtn = $('.ladda-button').ladda();
    var json = {
        MediaName: arr.join(','),
        MediaTypeId: $("#MediaTypeId").val(),
        MediaTypeName: $("#MediaTypeName").val()
    };
    $.ajax({
        type: "post",
        url: "/Media/Develop",
        data: addAntiForgeryToken(json),
        success: function (data) {
            $('#resultModal').modal('hide');
            swal("消息", data.Msg, "success");
        },
        error: function () {
            swal("消息", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}
function mediaPrice(id) {
    $("#modalView").load("/Media/Price?id=" + id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                initPriceCharts();
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });

}
function groupDetail(id) {
    $("#modalView").load("/Media/GroupDetail/" + id,
        function () {
            $('#modalView .modal').modal('show');
        });

}

function showGroup(name, id) {
    $("#modalView").load("/Media/AddGroup?name=" + name + "&id=" + id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                $("#MediaId").val(id);
                $("#errorMsg").text("");
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });
}
function addGroup() {
    var groupName = $("#groupName").val();
    if (groupName) {
        if (groupName.length > 10) {
            $("#errorMsg").text("分组名称不能超过10个字符");
            return;
        } else {
            var subBtn = $('.btn-warning.ladda-button').ladda();
            var json = { GroupName: groupName };
            $.ajax({
                type: "post",
                url: "/Media/AddGroup",
                data: addAntiForgeryToken(json),
                success: function (data) {
                    if (data.State == 1) {
                        var id = data.Msg;
                        $("#groups").append('<div class="col-md-4 col-xs-6">' +
                            '<div class="checkbox checkbox-success checkbox-inline">' +
                            '<input type="checkbox" id="' + id + '" name="Group" value="' + id + '" checked="">' +
                            '<label for="' + id + '"> ' + groupName + ' </label>' +
                            '</div>' +
                            '</div>');
                        $("#errorMsg").text("");
                        $("#groupName").val("");
                    } else {
                        $("#errorMsg").text(data.Msg);
                    }
                },
                error: function () {
                    $("#errorMsg").text("系统错误");
                },
                beforeSend: function () {
                    subBtn.ladda('start');
                },
                complete: function () {
                    subBtn.ladda('stop');
                }
            });
        }
    } else {
        $("#errorMsg").text("请输入分组名称");
        return;
    }
}

function joinGroup() {
    var arr = [], checkeds = $("[name='Group']:checked");
    $.each(checkeds, function () {
        arr.push($(this).val());
    });
    if (arr.length <= 0) {
        $("#errorMsg").text("请选择要加入的分组，如果没有，请先添加分组");
        return;
    }
    var subBtn = $('.btn-primary.ladda-button').ladda();
    var json = { gIds: arr.join(','), mId: $("#MediaId").val() };
    $.ajax({
        type: "post",
        url: "/Media/JoinGroup",
        data: addAntiForgeryToken(json),
        success: function (data) {
            $('#modalView .modal').modal('hide');
            var gid = $("#GroupId").val();
            if (gid) {
                $('#table').bootstrapTable('refresh', { query: { GroupId: gid } });
            } else {
                $("#table").bootstrapTable("refresh");
            }

            swal("消息", data.Msg, "success");
        },
        error: function () {
            swal("消息", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}

function removeMedia(name, id) {
    swal({
        title: "提醒",
        text: "确认要将 [" + name + "] 移除分组吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true

    }, function () {
        var json = { mId: id, gId: $("#GroupId").val() };
        $.ajax({
            type: "post",
            url: "/Media/RemoveGroup",
            data: addAntiForgeryToken(json),
            success: function (data) {
                if (data.State == 1) {
                    $("#table").bootstrapTable('refresh');
                    swal({
                        title: "操作成功",
                        text: data.Msg,
                        timer: 1000,
                        type: "success",
                        showConfirmButton: false
                    });

                } else {
                    swal("操作提醒", data.Msg, "warning");
                }
            },
            error: function () {
                swal("操作失败", "系统错误", "error");
            },
            complete: function () {

            }
        });
    });
}

function delGroup() {
    swal({
        title: "提醒",
        text: "确认要将删除此分组吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true

    }, function () {
        var json = { id: $("#GroupId").val() };
        $.ajax({
            type: "post",
            url: "/Media/DeleteGroup",
            data: addAntiForgeryToken(json),
            success: function (data) {
                if (data.State == 1) {

                    swal({
                        title: "操作成功",
                        text: data.Msg,
                        timer: 1000,
                        type: "success"

                    },
                        function () {
                            window.location.reload();
                        });

                } else {
                    swal("操作提醒", data.Msg, "warning");
                }
            },
            error: function () {
                swal("操作失败", "系统错误", "error");
            },
            complete: function () {

            }
        });
    });
}

function exportGroup() {
    var subBtn = $('.ladda-button').ladda();
    var json = { id: $("#GroupId").val(), isData: $("input[name='IsData']:checked").val() };
    $.ajax({
        type: "post",
        url: "/Media/ExportGroup",
        data: addAntiForgeryToken(json),
        success: function (data) {
            if (data.State == 1) {
                $('#exportGroupModal').modal('hide');
                swal({
                    title: "导出成功",
                    text: "正在下载中...",
                    timer: 2000,
                    showConfirmButton: false
                });
                window.location.href = "/Media/Download?file=" + data.Msg;
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

function articleDetails(id) {
    $("#modalView").load("/Resource/Media/ArticleDetails?id=" + id,
        function () {
            $('#modalView .modal').modal('show');
        });

}
// CSRF (XSRF) security
function addAntiForgeryToken(data) {
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
}
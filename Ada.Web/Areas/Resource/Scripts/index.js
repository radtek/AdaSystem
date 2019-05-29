var operation = {};
//撤销
operation.cancle = function (id) {
    swal({
        title: "您确定吗?",
        text: "确认要删除此数据吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,

    }, function () {
        ajaxFun("/Resource/Media/Delete/" + id);
    });
}
//置顶
operation.top = function (id) {
    ajaxFun("/Resource/Media/Top/" + id);
}
//优质
operation.hot = function (id) {
    ajaxFun("/Resource/Media/Hot/" + id);
}
//推荐
operation.recommend = function (id) {
    ajaxFun("/Resource/Media/Recommend/" + id);
}

$(function () {
    $('#uploadModal').on('shown.bs.modal',
        function () {

        }).on('hidden.bs.modal',
            function () {
                $('.fileinput').fileinput("clear");
            });
    $('.input-group.date').datepicker({
        autoclose: true,
        language: "zh-CN",
        todayHighlight: true
    });
});
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
function formatterPrice(value, row, index) {
    var arr = [];
    $.each(value,
        function (k, v) {
            if (v.PurchasePrice) {
                var price = "<i class='fa fa-jpy'></i> " + v.PurchasePrice;
                arr.push("<li class='list-group-item p-xxs'><span class='badge badge-success'>" + price + "</span> " + v.AdPositionName + "</li>");
            }
            
        });
    arr.push("<li class='list-group-item p-xxs'><span class='badge'>" + moment(value[0].InvalidDate).format("YYYY-MM-DD") + "</span>价格有效期</li>");
    return "<ul class='list-group m-b-none'>" + arr.join('') + "</ul>" ;
}
function formatterPriceProtection(value, row, index) {
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
    line += "<li class='list-group-item p-xxs'><span class='badge badge-success' data-toggle='tooltip' data-placement='bottom' title='" + row.PriceProtectionRemark + "'>查看</span> 保价备注</li>";
    return "<ul class='list-group'>" + line + "</ul>";
}
function formatterTag(value) {
    var arr = [];
    $.each(value,
        function (k, v) {
            arr.push("<span class='btn btn-success btn-xs btn-outline'><i class='fa fa-tag'></i> " + v.TagName + "</span>");
        });
    return "<div class='p-xxs'>" + arr.join(' ') + "</div>";
}

function formatterFans(value) {
    return "<span class='btn btn-danger btn-xs btn-outline'><i class='fa fa-users'></i> 粉丝：" + value + " 万</span>";
}
function formatterGroup(value, row, index) {
    var result = "";
    $.each(value,
        function (k, v) {
            result += "<button class='btn btn-warning btn-xs' onclick=\"groupDetail('" + v.Id + "');\"><i class='fa fa-object-group'></i> " + v.GroupName + "</button> ";
        });
    if (result) {
        return "<div class='p-xxs'>" + result + "</div>";
    }
    return result;
}

function formatterLogo(value, row) {

    var logo = "<i class='fa " + row.MediaTypeLogo + " fa-4x'></i>";
    if (value) {
        logo = '<img alt="' +
            row.MediaName +
            '" class="img-circle" src="' +
            value +
            '" style="width: 60px; height: 60px;"/>';
    }
    var arr = [];
    if (row.IsTop) {
        arr.push('<span class="badge badge-success">顶</span>');
    }
    if (row.IsHot) {
        arr.push('<span class="badge badge-warning">优</span>');
    }
    if (row.IsRecommend) {
        arr.push('<span class="badge badge-danger">荐</span>');
    }
    var vg = '';
    if (arr.length > 0) {
        vg = "<div class='p-xxs text-center'>" + arr.join(' ') + "</div>";
    }
    var info = "";
    if (row.IsComment) {
        info =
            "<div class='p-xxs text-center'><a class='btn btn-warning  btn-outline btn-xs' data-toggle='tooltip' data-placement='bottom' title='查看详情' href='/Resource/Media/Detail/" +
            row.Id +
            "' target='_blank'>查看详情</a></div>";
    }

    var state = "<div class='p-xxs text-center'>" + vg + "</div>";//+ formatter.normalStatus(row.Status)
    return '<div class="p-xxs text-center">' + logo + '</div>' + vg + info;
}
function formatterWeiXin(value, row) {
    var url =
        "<div class='p-xxs'><a class='label' href='http://weixin.sogou.com/weixin?type=1&query=" + row.MediaID + "' target='_blank'><i class='fa fa-link'></i> <span id='" + row.Id + "'>" + value + " - " + row.MediaID + "</span></a></div>";
    var tags = formatterTag(row.MediaTags);
    var fans = "<div class='p-xxs'>" + formatterFans(row.FansNum) + "</div>";
    var retentionTime = row.RetentionTime ? "<div class='p-xxs'><span class='btn btn-info btn-outline btn-xs'><i class='fa fa-clock-o'></i> 保留时长：" + row.RetentionTime+"</div>" : "";
    return url +
        "<div class='p-xxs'>" +
        "<button class='btn btn-white btn-xs' data-toggle='tooltip' title='媒体名称复制到剪切板' data-clipboard-target='#" + row.Id + "'><i class='fa fa-copy'></i></button>" +
        " <img rel='drevil' data-content='<div id=\"popOverBox\"><img src=\"" + (row.MediaQR || '/Images/nopic.png') + "\" width=\"100\" height=\"100\" /></div>'  src='/Images/ewm.png' style='width: 20px; height: 20px;'/>" +
        "</div>" +
        tags + fans + formatterGroup(row.MediaGroups) + retentionTime;
}

function formatterBlog(value, row) {
    var sex = "";
    if (row.Sex == '男') {
        sex = '<img alt="男" class="img-circle" src="/Images/man.png" style="width: 25px; height: 25px;"/>';
    }
    if (row.Sex == '女') {
        sex = '<img alt="女" class="img-circle" src="/Images/lady.png" style="width: 25px; height: 25px;"/>';
    }
    var url =
        "<div class='p-xxs'>" + sex + "<a class='label' href='https://weibo.com/u/" + row.MediaID + "' target='_blank' title='" + (row.Abstract || '暂无摘要') + "'><i class='fa fa-link'></i> <span id='" + row.Id + "'>" + value + "</span></a>" +
        " <button class='btn btn-white btn-xs' data-toggle='tooltip' title='媒体名称复制到剪切板' data-clipboard-target='#" + row.Id + "'><i class='fa fa-copy'></i></button>" +
        "</div>";;
    var tags = "";
    if (row.MediaTags) {
        tags = formatterTag(row.MediaTags);
    }

    var fans = "<div class='p-xxs'>" + formatterFans(row.FansNum) + "</div>";
    var level = formatterblogLevel(row.AuthenticateType);
    var area = "";
    if (row.Areas) {
        area = " <span class='btn btn-info btn-outline btn-xs'><i class='fa fa-map-marker'></i> " + row.Areas + "</span>";
    }
    var retentionTime = row.RetentionTime ? "<div class='p-xxs'><span class='btn btn-info btn-outline btn-xs'><i class='fa fa-clock-o'></i> 保留时长：" + row.RetentionTime + "</div>" : "";
    return url +
        "<div class='p-xxs'>" + level + area +
        "</div>" + tags + fans + formatterGroup(row.MediaGroups) + retentionTime;
}

function formatterMediaName(value, row) {
    var sex = "";
    if (row.Sex == '男') {
        sex = '<img alt="男" class="img-circle" src="/Images/man.png" style="width: 25px; height: 25px;"/>';
    }
    if (row.Sex == '女') {
        sex = '<img alt="女" class="img-circle" src="/Images/lady.png" style="width: 25px; height: 25px;"/>';
    }
    var name = "<span id='" + row.Id + "'>" + value + "</span>";
    switch (row.MediaTypeIndex) {
        case "website":
            name = "<span id='" + row.Id + "'>" + value + " - " + row.Client + " - " + row.Channel + "</span>";
            break;
        case "headline":
        case "webcast":
        case "brush":
            name = "<span id='" + row.Id + "'>" + row.Platform + " - " + value + "</span>";
            break;
    }
    if (row.MediaLink) {
        name = "<a class='label' href='" + row.MediaLink + "' target='_blank'><i class='fa fa-link'></i> " + name + "</a>";
    }
    var url =
        "<div class='p-xxs'>" + sex + name +
        " <button class='btn btn-white btn-xs' data-toggle='tooltip' title='媒体名称复制到剪切板' data-clipboard-target='#" + row.Id + "'><i class='fa fa-copy'></i></button>" +
        "</div>";;
    var tags = "";
    if (row.MediaTags) {
        tags = formatterTag(row.MediaTags);
    }
    var fans = "";
    if (row.FansNum) {
        fans = "<div class='p-xxs'>" + formatterFans(row.FansNum) + "</div>";
    }
    var area = "";
    if (row.Areas) {
        area = "<div class='p-xxs'><span class='btn btn-info  btn-outline btn-xs'><i class='fa fa-map-marker'></i> " + row.Areas + "</span></div>";
    }
    var grad = "";
    if (row.AuthenticateType) {
        grad = "<div class='p-xxs'><span class='btn btn-warning  btn-outline btn-xs'><i class='fa fa-trophy'></i> " + row.AuthenticateType + "</span></div>";
    }
    var retentionTime = row.RetentionTime ? "<div class='p-xxs'><span class='btn btn-info btn-outline btn-xs'><i class='fa fa-clock-o'></i> 保留时长：" + row.RetentionTime + "</div>" : "";
    
    return url + area + grad + tags + fans + formatterGroup(row.MediaGroups) + retentionTime;
}




function formatterWeiXinData(value, row) {
    var date = "--";
    if (row.LastPushDate) {
        date = moment(row.LastPushDate).format("YYYY-MM-DD HH:mm");
    }
    return "<div class='p-xxs'><span class='label label-info'>平均头条阅读数：" +
        (row.AvgReadNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>最近头条阅读数：" +
        (row.LastReadNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>月发文频次：" +
        (row.PublishFrequency || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>月发文篇数：" +
        (row.MonthPostNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>最近发文日期：" +
        date + "</div>";
}
function formatterBlogData(value, row) {
    var date = "--";
    if (row.LastPushDate) {
        date = moment(row.LastPushDate).format("YYYY-MM-DD HH:mm");
    }
    return "<div class='p-xxs'><span class='label label-info'>平均点赞数：" +
        (row.LikesNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均评论数：" +
        (row.CommentNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>一周博文数：" +
        (row.MonthPostNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>微博总数：" +
        (row.PostNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>最近博文日期：" +
        date + "</div>";
}
function formatterDouYinData(value, row) {
    return "<div class='p-xxs'><span class='label label-info'>平均浏览数：" +
        (row.AvgReadNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均评论数：" +
        (row.CommentNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均点赞数：" +
        (row.LikesNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均分享数：" +
        (row.TransmitNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>关注数：" +
        (row.FriendNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>总视频数：" +
        (row.PostNum || 0) +
        "</div>";
}
function formatterRedBookData(value, row) {
    return "<div class='p-xxs'><span class='label label-info'>平均评论数：" +
        (row.CommentNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均点赞数：" +
        (row.AvgReadNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均收藏数：" +
        (row.TransmitNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>关注数：" +
        (row.FriendNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>赞与收藏：" +
        (row.LikesNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>笔记数：" +
        (row.PostNum || 0) +
        "</div>";
}
function formatterBilibiliData(value, row) {
    return "<div class='p-xxs'><span class='label label-info'>关注数：" +
        (row.FriendNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>播放数：" +
        (row.LikesNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>阅读数：" +
        (row.AvgReadNum || 0) +
        "</div>";
}
function formatterToutiaoData(value, row) {
    var date = "--";
    if (row.LastPushDate) {
        date = moment(row.LastPushDate).format("YYYY-MM-DD HH:mm");
    }
    return "<div class='p-xxs'><span class='label label-info'>关注数：" +
        (row.FriendNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>转发数：" +
        (row.TransmitNum || 0) +
        "</div>" +"<div class='p-xxs'><span class='label label-info'>平均阅读数：" +
        (row.AvgReadNum || 0) +
        "</div>" +
        "<div class='p-xxs'><span class='label label-info'>平均评论数：" +
        (row.CommentNum || 0) +
        "</div>" +
        
        "<div class='p-xxs'><span class='label label-info'>最近文章日期：" +
        date + "</div>";
}
function formatterblogLevel(value) {
    if (value == "黄V") {
        return " <span class='label label-warning'>V</span>";
    } else if (value == "蓝V") {
        return " <span class='label label-success'>V</span>";
    } else if (value == "金V") {
        return " <span class='label label-danger'>V</span>";
    } else if (value == "达人") {
        return " <span class='label label-danger'><i class='fa fa-star-o'></i></span>";
    }
    return "";
};
function formatterRemark(value, row) {
    return "<div class='p-xxs'><span class='label label-info' data-toggle='tooltip' data-placement='bottom' title='" + (row.Content || '暂无信息') + "'>媒体说明</div>" +
        "<div class='p-xxs'><span class='label label-info'  data-toggle='tooltip' data-placement='bottom' title='" + (row.Remark || '暂无信息') + "'>媒介备注</div>";
}

function formatterOperation(value, row) {
    var tj = row.IsRecommend ? 'warning' : 'default', yz = row.IsHot ? 'warning' : 'default', zd = row.IsTop ? 'warning' : 'default';
    return "<div class='p-xxs'><div class='btn-group'>" +
        "<button class='btn btn-" + tj + " btn-outline btn-xs' onclick=\"operation.recommend('" + value + "');\"><i class='fa fa-thumbs-up'></i> 推荐</button>" +
        "<button class='btn btn-" + yz + " btn-outline btn-xs' onclick=\"operation.hot('" + value + "');\"><i class='fa fa-diamond'></i> 优质</button>" +
        "<button class='btn btn-" + zd + " btn-outline btn-xs' onclick=\"operation.top('" + value + "');\"><i class='fa fa-trophy'></i> 置顶</button>" +
        "</div></div>" +
        "<div class='p-xxs'><div class='btn-group'>" +
        "<a class='btn btn-success btn-outline btn-xs' href='/Resource/Media/Update/" + value + "');' target='_blank'><i class='fa fa-pencil'></i> 编辑</a> " +
        "<a class='btn btn-success btn-outline btn-xs' href='/Resource/Media/Comment/" + value + "');' target='_blank'><i class='fa fa-commenting'></i> 评价</a> " +
        "<button class='btn btn-success btn-outline btn-xs' onclick=\"operation.cancle('" + value + "');\"><i class='fa fa-trash-o'></i> 删除</button>" +
        "</div></div>";
}


function ajaxFun(url, data) {
    var jsondata = data || {};
    $.ajax({
        type: "post",
        url: url,
        data: addAntiForgeryToken(jsondata),
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
}

function upLoadFile() {

    var fileObj = document.getElementById("upfile").files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        swal("操作失败", "请选择文件", "warning");
        return;
    }
    if (!checkFileExt(fileObj.name)) {
        swal("操作失败", "只支持xlsx后缀文件", "warning");
        return;
    }
    var formFile = new FormData();
    formFile.append("upfile", fileObj); //加入文件对象
    formFile.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]').val());
    var data = formFile;
    var subBtn = $('.ladda-button').ladda();
    $.ajax({
        url: "/Resource/Media/Import",
        data: data,
        type: "Post",
        dataType: "json",
        cache: false, //上传文件无需缓存
        processData: false, //用于对data参数进行序列化处理 这里必须false
        contentType: false, //必须
        success: function (result) {
            if (result.State == 1) {
                swal({
                    title: "操作成功",
                    text: result.Msg,
                    timer: 2000,
                    type: "success",
                    showConfirmButton: false
                });
            } else {
                swal("操作失败", result.Msg, "error");
            }
        },
        error: function () {
            swal("操作失败", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}
function priceUpdate(is) {
    var platform = $("#pricePlatform").val();
    if (!platform) {
        swal("操作失败", "请选择报价平台", "warning");
        return;
    }
    //导入
    if (is) {
        var fileObj = document.getElementById("pricefile").files[0]; // js 获取文件对象
        if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
            swal("操作失败", "请选择文件", "warning");
            return;
        }
        if (!checkFileExt(fileObj.name)) {
            swal("操作失败", "只支持xlsx后缀文件", "warning");
            return;
        }
        var formFile = new FormData();
        formFile.append("platform", platform);
        formFile.append("upfile", fileObj); //加入文件对象
        formFile.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]').val());
        var data = formFile;
        var subBtn = $('.ladda-button').ladda();
        $.ajax({
            url: "/Resource/Media/Import",
            data: data,
            type: "Post",
            dataType: "json",
            cache: false, //上传文件无需缓存
            processData: false, //用于对data参数进行序列化处理 这里必须false
            contentType: false, //必须
            success: function (result) {
                if (result.State == 1) {
                    swal({
                        title: "操作成功",
                        text: result.Msg,
                        timer: 2000,
                        type: "success",
                        showConfirmButton: false
                    });
                } else {
                    swal("操作失败", result.Msg, "error");
                }
            },
            error: function () {
                swal("操作失败", "系统错误", "error");
            },
            beforeSend: function () {
                subBtn.ladda('start');
            },
            complete: function () {
                subBtn.ladda('stop');
            }
        }); 
    } else {
        //导出
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
            $("#MediaReferencePricePlatform").val(platform);
            $('#searchFrm')[0].submit();
            swal({
                title: "数据正在导出中...",
                text: "请耐心等待",
                timer: 2000,
                showConfirmButton: false
            });
            $("#MediaReferencePricePlatform").val("");
        });
    }
    
}
function confirmFile() {

    var fileObj = document.getElementById("confirmfile").files[0]; // js 获取文件对象
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        swal("操作失败", "请选择文件", "warning");
        return;
    }
    if (!checkFileExt(fileObj.name)) {
        swal("操作失败", "只支持xlsx后缀文件", "warning");
        return;
    }
    var formFile = new FormData();
    formFile.append("upfile", fileObj); //加入文件对象
    formFile.append("__RequestVerificationToken", $('input[name=__RequestVerificationToken]').val());
    var data = formFile;
    var subBtn = $('.ladda-button').ladda();
    $.ajax({
        url: "/Resource/Media/Confirm",
        data: data,
        type: "Post",
        dataType: "json",
        cache: false, //上传文件无需缓存
        processData: false, //用于对data参数进行序列化处理 这里必须false
        contentType: false, //必须
        success: function (result) {
            if (result.State == 1) {
                swal({
                    title: "验证成功",
                    text: result.Msg,
                    type: "success"
                });
            } else {
                swal("操作失败", result.Msg, "error");
            }
        },
        error: function () {
            swal("操作失败", "系统错误", "error");
        },
        beforeSend: function () {
            subBtn.ladda('start');
        },
        complete: function () {
            subBtn.ladda('stop');
        }
    });
}

function checkFileExt(filename) {
    var flag = false; //状态
    var arr = ["xlsx"];
    //取出上传文件的扩展名
    var index = filename.lastIndexOf(".");
    var ext = filename.substr(index + 1);
    //循环比较
    for (var i = 0; i < arr.length; i++) {
        if (ext == arr[i]) {
            flag = true; //一旦找到合适的，立即退出循环
            break;
        }
    }
    return flag;
}


function collectArticle(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length > 1) {
        swal("操作提醒", "只能选择一行数据", "warning");
        return;
    }
    if (arrselections.length <= 0) {
        swal("操作提醒", "请选择有效数据", "warning");
        return;
    }
    $("#modalView").load(url + "/" + arrselections[0].Id,
        function () {
            $('#modalView .modal').on('shown.bs.modal', function () {
                collection();
            }).on('hidden.bs.modal', function () {

            });
            $('#modalView .modal').modal('show');
        });

}
function details(url) {
    $("#modalView").load(url,
        function () {
            //$('#modalView .modal').on('shown.bs.modal', function () {
            //    collection();
            //}).on('hidden.bs.modal', function () {

            //});
            $('#modalView .modal').modal('show');
        });
}

function collection() {
    $("#modalView form").validate({
        submitHandler: function (form) {
            var subBtn = $('.ladda-button').ladda();
            var $form = $(form),
                data = $form.serialize();
            $.ajax({
                type: "post",
                url: form.action,
                data: data,
                success: function (res) {
                    if (res.State == 1) {
                        $("#table").bootstrapTable('refresh');
                        swal({
                            title: "操作成功",
                            text: res.Msg,

                            type: "success"

                        });

                    } else {
                        swal("操作提醒", res.Msg, "warning");
                    }
                },
                error: function () {
                    swal("操作失败", "系统错误", "error");
                },
                beforeSend: function () {
                    subBtn.ladda('start');
                },
                complete: function () {
                    subBtn.ladda('stop');
                }
            });

        }
    });

}

function collectWeiXin(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length <= 0) {
        swal("操作提醒", "请选择需采集的微信号", "warning");
        return;
    }
    var ids = [];
    $.each(arrselections,
        function (k, v) {
            ids.push(v.MediaID);
        });
    swal({
        title: "您确定吗?",
        text: "确认要采集这" + arrselections.length + "个微信号吗?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确定",
        cancelButtonText: "取消",
        closeOnConfirm: false,
        showLoaderOnConfirm: true

    }, function () {
        var json = { CallIndex: "weixin", CallIndexWeiXinInfo: "weixinpro", UID: ids.join(',') };
        $.ajax({
            type: "post",
            url: url,
            data: addAntiForgeryToken(json),
            success: function (data) {
                if (data.State == 1) {
                    $("#table").bootstrapTable('refresh');
                    swal({
                        title: "操作成功",
                        text: data.Msg,
                        timer: 2000,
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

function collectDouYin(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length == 1) {
        swal({
            title: "您确定吗?",
            text: "确认要采集抖音用户信息吗?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            closeOnConfirm: false,
            showLoaderOnConfirm: true

        }, function () {
            var kw = arrselections[0].Abstract || arrselections[0].MediaName;
            var json = { CallIndex: "douyininfo", UID: arrselections[0].MediaID, KeyWord: kw };
            $.ajax({
                type: "post",
                url: url,
                data: addAntiForgeryToken(json),
                success: function (data) {
                    if (data.State == 1) {
                        $("#table").bootstrapTable('refresh');
                        swal({
                            title: "操作成功",
                            text: data.Msg,
                            type: "success"
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

    } else {
        swal("操作提醒", "请选择一条抖音用户记录", "warning");
        return;
    }


}

function collectInfo(url) {
    var arrselections = $("#table").bootstrapTable('getSelections');
    if (arrselections.length == 1) {
        swal({
            title: "采集数据",
            text: "确认要采集用户信息吗?",
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#DD6B55",
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            closeOnConfirm: false,
            showLoaderOnConfirm: true

        }, function () {
            $.ajax({
                type: "get",
                url: url,
                data: { id: arrselections[0].Id },
                success: function (data) {
                    if (data.State == 1) {
                        swal({
                            title: "操作成功",
                            text: data.Msg,
                            type: "success"
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

    } else {
        swal("操作提醒", "请选择一条数据", "warning");
        return;
    }


}

$(document).ready(function () {
    $('#jstree').jstree({
        'core': {
            'check_callback': true
        },
        'plugins': ['types', 'checkbox'],
        'types': {
            'default': {
                'icon': 'fa fa-folder'
            }
        },
        "checkbox": {
            "keep_selected_style": false,
            //"tie_selection": false
            "three_state": true
        }
    });
    $('#jstree_action').jstree({
        'core': {
            'check_callback': true
        },
        'plugins': ['types'],
        'types': {
            'default': {
                'icon': 'fa fa-folder'
            }
        }
    });
    //机构选中
    if ($("#OrganizationIds").val()) {
        var ids = $("#OrganizationIds").val().split(",");
        $('#jstree').jstree("check_node", ids);
    }
    //权限选中
    if ($("#ActionIds").val()) {
        resetAction($("#ActionIds").val());
    }
    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            var arry = $('#jstree').jstree("get_top_checked");
            $("#OrganizationIds").val(arry.join(','));
            
            //特殊权限处理
            var actions = $(".action");
            var actionArry = [];
            $.each(actions,
                function (k, v) {
                    var ispass = $(v).attr("ispass");
                    if (ispass) {
                        var id = $(v).attr("data-value");
                        actionArry.push(id + "^" + ispass);
                    }
                });
            $("#ActionIds").val(actionArry.join(','));
            form.submit();
        }
    });


});

function seletAction(id, text) {
    var classname = "btn-success";
    var ispass = "";
    if (text == "允许") {
        classname = "btn-primary";
        ispass = "true";
    }
    if (text == "拒绝") {
        classname = "btn-danger";
        ispass = "false";
    }
    $("." + id).text(text).attr("ispass", ispass).parent().removeClass('btn-success btn-danger btn-primary').addClass(classname);
}
function resetAction(ids) {
    var arry = ids.split(',');
    $.each(arry,
        function (k, v) {
            var temp = v.split('^');
            var text = "拒绝";
            var classname = "btn-danger";
            if (temp[1] == "true") {
                text = "允许";
                classname = "btn-primary";
            }
            $("." + temp[0]).text(text).attr("ispass", temp[1]).parent().removeClass('btn-success btn-danger btn-primary').addClass(classname);
        });
}
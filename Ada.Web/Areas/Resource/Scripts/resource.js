jQuery.validator.addMethod("isMediaId", function (value, element) {
    var reg = /^[A-Za-z0-9_\-]+$/ig;
    return this.optional(element) || (reg.test(value));
}, "请正确填写媒体ID");
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
    $('.input-group.date').datepicker({
        todayBtn: "linked",
        keyboardNavigation: false,
        forceParse: false,
        autoclose: true,
        language: "zh-CN",
        orientation: "bottom right",
        todayHighlight: true
    });

    $(".wrapper.wrapper-content form").validate({
        submitHandler: function (form) {
            //$('.wrapper.wrapper-content .tabs-container').children('.tab-content').toggleClass('sk-loading');
            form.submit();
        },
        rules: {
            MediaLink: {
                url: true
            },
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